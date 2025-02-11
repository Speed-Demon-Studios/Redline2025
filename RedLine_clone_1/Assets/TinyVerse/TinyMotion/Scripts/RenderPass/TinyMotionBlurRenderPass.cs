using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TinyVerse.TinyMotion
{
	sealed class TinyMotionBlurRenderPass : ScriptableRenderPass
	{
		TinyMotionComponent _TinyMotionComponent;
		RenderTexture _renderTexture;
		Material _MotionBlurMaterial;

		int _depthBufferBits;
		bool _skipSecondPass;

		internal TinyMotionBlurRenderPass()
		{
			renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
		}

		internal void Setup(TinyMotionComponent tinyMotionComponent, int depthBufferBits)
		{
			_TinyMotionComponent = tinyMotionComponent;
			_depthBufferBits = depthBufferBits;
			_skipSecondPass = _TinyMotionComponent.skipSecondPass.value;

			if (_MotionBlurMaterial == null)
				_MotionBlurMaterial = new Material(Shader.Find("Hidden/TinyMotion/MotionBlur"));
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			Camera camera = renderingData.cameraData.camera;
			CommandBuffer cmd = CommandBufferPool.Get("TinyMotion Blur");
			ExecutePass(context, ref renderingData, ref cmd, camera);
			CommandBufferPool.Release(cmd);
		}

		void ExecutePass(ScriptableRenderContext context, ref RenderingData renderingData, ref CommandBuffer cmd, Camera camera)
		{
			int debugMode = (int)_TinyMotionComponent.debugMode.value;
			int blurMode = (int)_TinyMotionComponent.blurMode.value;

			// Simple Options - Simplified Shader Pass
			// _MotionBlurMaterial.SetFloat("_Intensity", _TinyMotionComponent.intensity.value * 0.5f);
			_MotionBlurMaterial.SetFloat("_Threshold", _TinyMotionComponent.threshold.value * 0.01f);
			_MotionBlurMaterial.SetInteger("_NoiseMode", (int)_TinyMotionComponent.noiseMode.value);
			_MotionBlurMaterial.SetInteger("_AnimateNoise", _TinyMotionComponent.animateNoise.value ? 1 : 0);

			// Complex Options - Complex Shader Pass
			if (blurMode >= 1)
			{
				_MotionBlurMaterial.SetInteger("_SampleCount", (int)_TinyMotionComponent.samples.value);
				_MotionBlurMaterial.SetInteger("_DynamicSampleCount", (int)_TinyMotionComponent.sampleCountMode.value);
				_MotionBlurMaterial.SetInteger("_MinSampleCount", (int)_TinyMotionComponent.minSampleCount.value);
				_MotionBlurMaterial.SetInteger("_MaxSampleCount", (int)_TinyMotionComponent.maxSampleCount.value);
				_MotionBlurMaterial.SetFloat("_ClampVelocity", _TinyMotionComponent.clampVelocity.value * 0.1f);
				// DepthSeparation
				if (blurMode >= 2)
				{
					_MotionBlurMaterial.SetInteger("_DepthSeparationBothWays", _TinyMotionComponent.depthSeparationBothWays.value ? 1 : 0);
					_MotionBlurMaterial.SetInteger("_DepthSeparationVelocity", _TinyMotionComponent.depthSeparationVelocity.value ? 1 : 0);
					_MotionBlurMaterial.SetFloat("_DepthSeparationTreshold", _TinyMotionComponent.depthSeparationTreshold.value * 0.01f);
					_MotionBlurMaterial.SetFloat("_DepthSeparationVelocityTreshold", _TinyMotionComponent.depthSeparationVelocityTreshold.value * 0.1f);
				}
			}
			else
			{
				_MotionBlurMaterial.SetInteger("_DynamicSampleCount", 0);
				_MotionBlurMaterial.SetInteger("_DepthSeparationVelocity", 0);
				_MotionBlurMaterial.SetFloat("_DepthSeparationVelocityTreshold", 0);
				_MotionBlurMaterial.SetFloat("_ClampVelocity", 0);
			}

			_MotionBlurMaterial.SetInteger("_DebugMode", debugMode);

			int shaderPassIndex = debugMode != 0 ? 3 : blurMode;


#if UNITY_2023
			RenderTargetIdentifier renderTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
#else
			RenderTargetIdentifier renderTarget = renderingData.cameraData.renderer.cameraColorTarget;
#endif
			RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
			descriptor.colorFormat = RenderTextureFormat.DefaultHDR;
			descriptor.depthBufferBits = _depthBufferBits;

			_renderTexture = RenderTexture.GetTemporary(descriptor);
			cmd.Blit(renderTarget, _renderTexture, _MotionBlurMaterial, shaderPassIndex);

			if (_skipSecondPass)
			{
				cmd.Blit(_renderTexture, renderTarget);
			}
			else
			{
				cmd.Blit(_renderTexture, renderTarget, _MotionBlurMaterial, shaderPassIndex);
			}

			RenderTexture.ReleaseTemporary(_renderTexture);

			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}
	}
}
