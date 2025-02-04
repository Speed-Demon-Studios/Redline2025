using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TinyVerse.TinyMotion
{
	sealed class TinyMotionVectorRenderPass : ScriptableRenderPass
	{
		static string[] _ShaderTags = new string[4]
		{
			"UniversalForward", // Lit
			"UniversalForwardOnly", // Complex Lit
			"UniversalGBuffer", // Deffered Rendering
			"DepthOnly", // BakedLit, Unlit
		};

		RenderTextureFormat[] RenderTextureFormats = new RenderTextureFormat[3] { RenderTextureFormat.RGFloat, RenderTextureFormat.RGHalf, RenderTextureFormat.RG16 };

#if UNITY_2023
		RTHandle _MotionVectorHandle;
		private int _DestinationID { get; set; }
#else
		RenderTargetHandle _MotionVectorHandle;
#endif

		TinyMotionComponent _TinyMotionComponent;
		TinyMotionData _TinyMotionData;
		Material _Material;
		Material _CameraMaterial;

		int _includedLayers;
		int _downSample;
		float _shutterSpeed;
		float _frameTimeScale;
		float _frameTimeScaleVelocity;
		float _intensity;
		float _intensityCamera;

		internal TinyMotionVectorRenderPass()
		{
			renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
		}

		internal void Setup(TinyMotionData tinyMotionData, TinyMotionComponent tinyMotionComponent, int includedLayers)
		{
			_TinyMotionData = tinyMotionData;
			_TinyMotionComponent = tinyMotionComponent;
			_includedLayers = includedLayers;
			_downSample = (int)_TinyMotionComponent.downsample.value;
			_shutterSpeed = (float)_TinyMotionComponent.shutterSpeed.value;
			_intensity = (float)_TinyMotionComponent.intensity.value * 0.5f;
			_intensityCamera = (float)_TinyMotionComponent.intensityCamera.value;

			if (_Material == null)
				_Material = new Material(Shader.Find("Hidden/TinyMotion/MotionVectors"));
			if (_CameraMaterial == null)
				_CameraMaterial = new Material(Shader.Find("Hidden/TinyMotion/CameraMotionVectors"));
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
#if UNITY_2023
			_MotionVectorHandle = RTHandles.Alloc("_TinyMotionVectors", name: "_TinyMotionVectors");
			_DestinationID = Shader.PropertyToID(_MotionVectorHandle.name);
#else
			_MotionVectorHandle.Init("_TinyMotionVectors");
			RenderTargetIdentifier identifier = _MotionVectorHandle.Identifier();
#endif

			RenderTextureDescriptor descriptor = cameraTextureDescriptor;
			descriptor.colorFormat = RenderTextureFormats[(int)_TinyMotionComponent.motionTextureQuality.value];

			if (_downSample != 1)
			{
				descriptor.width /= _downSample;
				descriptor.height /= _downSample;
			}

			CalculateFrameTimeScale();

#if UNITY_2023
			cmd.GetTemporaryRT(_DestinationID, descriptor, FilterMode.Point);
			ConfigureTarget(_MotionVectorHandle, _MotionVectorHandle);
			cmd.SetRenderTarget(_MotionVectorHandle, _MotionVectorHandle);
#else
			cmd.GetTemporaryRT(_MotionVectorHandle.id, descriptor, FilterMode.Point);
			ConfigureTarget(identifier, identifier);
			cmd.SetRenderTarget(identifier, identifier);
#endif

			cmd.ClearRenderTarget(true, true, Color.gray, 1.0f);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			Camera camera = renderingData.cameraData.camera;
			CommandBuffer cmd = CommandBufferPool.Get("TinyMotion Vectors");
			ExecutePass(context, renderingData, ref cmd, camera);
			CommandBufferPool.Release(cmd);
		}

		void CalculateFrameTimeScale()
		{
#if UNITY_EDITOR
			if (Time.deltaTime != 0)
			{
				_frameTimeScale = Mathf.SmoothDamp(_frameTimeScale, (1 / _shutterSpeed) / Time.deltaTime, ref _frameTimeScaleVelocity, 0.5f);
			}
#else
			_frameTimeScale = Mathf.SmoothDamp(_frameTimeScale, (1 / _shutterSpeed) / Time.deltaTime, ref _frameTimeScaleVelocity, 0.5f);
#endif
		}

		void ExecutePass(ScriptableRenderContext context, RenderingData renderingData, ref CommandBuffer cmd, Camera camera)
		{
			Shader.SetGlobalMatrix("_PrevViewProjMatrix", _TinyMotionData.previousViewProjectionMatrix);
			camera.depthTextureMode |= (DepthTextureMode.MotionVectors | DepthTextureMode.Depth);

			if (_intensityCamera != 0)
			{
				_CameraMaterial.SetFloat("_FrameTimeScale", _frameTimeScale);
				_CameraMaterial.SetFloat("_Intensity", _intensityCamera);
				DrawCameraMotionVectors(context, cmd, camera);
			}

			_Material.SetFloat("_FrameTimeScale", _frameTimeScale);
			_Material.SetFloat("_Intensity", _intensity);
			DrawObjectMotionVectors(context, ref renderingData, cmd, camera);

			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}

		DrawingSettings GetDrawingSettings(ref RenderingData renderingData)
		{
			SortingSettings sortingSettings = new SortingSettings(renderingData.cameraData.camera) { criteria = SortingCriteria.CommonOpaque };
			DrawingSettings drawingSettings = new DrawingSettings(ShaderTagId.none, sortingSettings) { perObjectData = PerObjectData.MotionVectors, enableInstancing = true, };

			for (int i = 0; i < _ShaderTags.Length; i++)
			{
				drawingSettings.SetShaderPassName(i, new ShaderTagId(_ShaderTags[i]));
			}
			drawingSettings.SetShaderPassName(_ShaderTags.Length + 1, new ShaderTagId());

			drawingSettings.overrideMaterial = _Material;
			drawingSettings.overrideMaterialPassIndex = 0;

			return drawingSettings;
		}

		void DrawCameraMotionVectors(ScriptableRenderContext context, CommandBuffer cmd, Camera camera)
		{
			cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _CameraMaterial, 0, 0);
			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}

		void DrawObjectMotionVectors(ScriptableRenderContext context, ref RenderingData renderingData, CommandBuffer cmd, Camera camera)
		{
			DrawingSettings drawingSettings = GetDrawingSettings(ref renderingData);
			FilteringSettings filteringSettings = new FilteringSettings(_TinyMotionComponent.includeTransparentObjects.value ? RenderQueueRange.all : RenderQueueRange.opaque, camera.cullingMask & _includedLayers);
			RenderStateBlock renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
			context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException("cmd");

#if UNITY_2023
			if (_MotionVectorHandle.rt == null && _DestinationID != -1)
			{
				cmd.ReleaseTemporaryRT(_DestinationID);
				_MotionVectorHandle.Release();
				_MotionVectorHandle = null;
			}
#else
			if (_MotionVectorHandle != RenderTargetHandle.CameraTarget)
			{
				cmd.ReleaseTemporaryRT(_MotionVectorHandle.id);
				_MotionVectorHandle = RenderTargetHandle.CameraTarget;
			}
#endif
		}
	}
}
