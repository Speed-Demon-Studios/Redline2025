Shader "Hidden/TinyMotion/MotionVectors"
{
	SubShader
	{
		Tags{ 
			"LightMode" = "MotionVectors"
		}
		
		Pass
		{
			Name "Tiny Motion Vectors"

			HLSLPROGRAM
			#pragma target 5.0

			#pragma multi_compile_instancing
			#pragma vertex Vert
			#pragma fragment Frag

			// -------------------------------------
			// Includes
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
			float _FrameTimeScale;
			float _Intensity;

			// -------------------------------------
			// Structs
			struct Attributes
			{
				float4 position : POSITION;
				float3 positionOLD : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionHCS : SV_POSITION;
				float4 positionVPM : TEXCOORD0;
				float4 positionVPMPrev : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			// -------------------------------------
			// Vertex
			Varyings Vert(Attributes input)
			{
				Varyings OUT = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, OUT);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.positionHCS = TransformObjectToHClip(input.position.xyz);

				// OpenGL
				#if !UNITY_REVERSED_Z
					OUT.positionHCS.z -= unity_MotionVectorsParams.z * OUT.positionHCS.w;
				#else
					OUT.positionHCS.z += unity_MotionVectorsParams.z * OUT.positionHCS.w;
				#endif

				OUT.positionVPM = mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, input.position));
				OUT.positionVPMPrev = mul(_PrevViewProjMatrix, mul(unity_MatrixPreviousM, unity_MotionVectorsParams.x == 1 ? float4(input.positionOLD, 1) : input.position));
				return OUT;
			}

			// -------------------------------------
			// Fragment
			half4 Frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				
				// Force no motion output
				if (unity_MotionVectorsParams.y == 0.0)
				{
					return half4(0.5, 0.5, 0.5, 0.0);
				}

				input.positionVPM.xyz = input.positionVPM.xyz / input.positionVPM.w;
				input.positionVPMPrev.xyz = input.positionVPMPrev.xyz / input.positionVPMPrev.w;
				float3 velocity = (input.positionVPM.xyz - input.positionVPMPrev.xyz) * _FrameTimeScale * _Intensity;

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
