using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace TinyVerse.TinyMotion
{
	sealed class TinyMotionRendererFeature : ScriptableRendererFeature
	{
		[Tooltip("Select Layers that should be affected by motion blur")]
		[FormerlySerializedAs("_includedLayers")]
		public LayerMask includedLayers = ~0;

		[Tooltip("At which step should motion vectors be calculated.\nAfterRenderingSkybox recommended.")]
		[FormerlySerializedAs("_motionVectorsRenderPassEvent")]
		public RenderPassEvent motionVectorsRenderPassEvent = RenderPassEvent.AfterRenderingSkybox;

		[Tooltip("At which step should motion blur be applied.\nAfterRenderingSkybox recommended.")]
		[FormerlySerializedAs("_motionBlurRenderPassEvent")]
		public RenderPassEvent motionBlurRenderPassEvent = RenderPassEvent.AfterRenderingSkybox;

		public enum DepthBufferBits
		{
			_16 = 16,
			_24 = 24,
			_32 = 32,
		}

		[Tooltip("The minimum number of bits used for depth in the Depth/Stencil buffer format.\nHigher values result in better quality at the cost of performance")]
		[FormerlySerializedAs("_depthBufferBits")]
		public DepthBufferBits depthBufferBits = DepthBufferBits._24;

		[Header("Shader References")]
		[Tooltip("Reference to shader for compilation")]
		public Shader motionBlurShader;

		[Tooltip("Reference to shader for compilation")]
		public Shader motionVectorsShader;

		[Tooltip("Reference to shader for compilation")]
		public Shader cameraMotionVectorsShader;

		static TinyMotionVectorRenderPass _TinyMotionVectorRenderPass;
		static TinyMotionBlurRenderPass _TinyMotionBlurRenderPass;
		Dictionary<Camera, TinyMotionData> _TinyMotionDataDictionary;
		TinyMotionComponent _TinyMotionComponent;

		public override void Create()
		{
			name = "TinyMotion";
			_TinyMotionVectorRenderPass = new TinyMotionVectorRenderPass();
			_TinyMotionBlurRenderPass = new TinyMotionBlurRenderPass();
			_TinyMotionDataDictionary = new Dictionary<Camera, TinyMotionData>();
			_TinyMotionComponent = VolumeManager.instance.stack.GetComponent<TinyMotionComponent>();
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (_TinyMotionComponent.IsActive())
			{
				Camera camera = renderingData.cameraData.camera;

				if (camera.cameraType == CameraType.Preview || camera.cameraType == CameraType.Reflection)
					return;

				UniversalAdditionalCameraData cameraData = camera.GetComponent<UniversalAdditionalCameraData>();
				if (cameraData != null && cameraData.renderPostProcessing)
				{
					if (Application.isPlaying && (_TinyMotionComponent.viewInEditor.value || !renderingData.cameraData.isSceneViewCamera))
					{
						if (!_TinyMotionDataDictionary.TryGetValue(camera, out TinyMotionData _TinyMotionData))
						{
							_TinyMotionData = new TinyMotionData();
							_TinyMotionDataDictionary.Add(camera, _TinyMotionData);
						}

						SetProjectionMatrices(camera, ref _TinyMotionData);

						_TinyMotionVectorRenderPass.renderPassEvent = motionVectorsRenderPassEvent;
						_TinyMotionVectorRenderPass.Setup(_TinyMotionData, _TinyMotionComponent, includedLayers.value);
						renderer.EnqueuePass(_TinyMotionVectorRenderPass);

						_TinyMotionBlurRenderPass.renderPassEvent = motionBlurRenderPassEvent;
						_TinyMotionBlurRenderPass.Setup(_TinyMotionComponent, (int)depthBufferBits);
						renderer.EnqueuePass(_TinyMotionBlurRenderPass);
					}
				}
			}
		}

		void SetProjectionMatrices(Camera camera, ref TinyMotionData _TinyMotionData)
		{
			Matrix4x4 viewMatrix = camera.worldToCameraMatrix;
			Matrix4x4 gpuViewMatrix = GL.GetGPUProjectionMatrix(camera.nonJitteredProjectionMatrix, true);
			Matrix4x4 newViewProjectionMatrix = gpuViewMatrix * viewMatrix;
			if (_TinyMotionData.lastFrameCount != Time.frameCount)
			{
				_TinyMotionData.previousViewProjectionMatrix = _TinyMotionData.firstFrame ? newViewProjectionMatrix : _TinyMotionData.currentViewProjectionMatrix;
				_TinyMotionData.firstFrame = false;
			}
			_TinyMotionData.currentViewProjectionMatrix = newViewProjectionMatrix;
			_TinyMotionData.lastFrameCount = Time.frameCount;
		}

		void OnValidate()
		{
			if (motionBlurShader == null)
			{
				motionBlurShader = Shader.Find("Hidden/TinyMotion/MotionBlur");
			}
			if (motionVectorsShader == null)
			{
				motionVectorsShader = Shader.Find("Hidden/TinyMotion/MotionVectors");
			}
			if (cameraMotionVectorsShader == null)
			{
				cameraMotionVectorsShader = Shader.Find("Hidden/TinyMotion/CameraMotionVectors");
			}
		}
	}
}
