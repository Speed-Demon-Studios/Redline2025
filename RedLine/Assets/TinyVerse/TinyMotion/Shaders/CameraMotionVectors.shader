Shader "Hidden/TinyMotion/CameraMotionVectors"
{
	SubShader
	{
		Pass
		{
			Name "Tiny Motion Camera Vectors"

			Cull Off 
			ZWrite Off 
			ZTest Always

			HLSLPROGRAM
			#pragma target 3.5

			#pragma multi_compile_instancing
			#pragma vertex vert
			#pragma fragment frag

			// -------------------------------------
			// Includes
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			float _FrameTimeScale;
			float _Intensity;

			// -------------------------------------
			// Inputs
			TEXTURE2D_X(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);

			// -------------------------------------
			// Structs
			struct Attributes
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			// -------------------------------------
			// Vertex
			Varyings vert(Attributes input)
			{
				Varyings OUT = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, OUT);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				
				OUT.position = float4(input.position.xyz, 1);
				#if SHADER_API_VULKAN || UNITY_UV_STARTS_AT_TOP
					OUT.position.y = -OUT.position.y;
				#endif
				
				OUT.uv = input.uv;
				return OUT;
			}

			// -------------------------------------
			// Fragment
			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				half depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv.xy).x;
				half2 screenSize = half2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
				PositionInputs positionInputs = GetPositionInput(input.position.xy, screenSize, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				float4 positionVP = mul(UNITY_MATRIX_VP, float4(positionInputs.positionWS, 1.0));
				float4 positionVPPrev = mul(_PrevViewProjMatrix, float4(positionInputs.positionWS, 1.0));

				positionVP.xyz = positionVP.xyz / positionVP.w;
				positionVPPrev.xyz = positionVPPrev.xyz / positionVPPrev.w;
				float3 velocity = (positionVP.xyz - positionVPPrev.xyz) * _FrameTimeScale * _Intensity;

				#if UNITY_UV_STARTS_AT_TOP
					velocity.y = -velocity.y;
				#endif

				velocity = pow(abs(velocity), 1/3.0) * sign(velocity) * 0.5 + 0.5;

				return half4(velocity, 1);
			}

			ENDHLSL
		}
	}
}