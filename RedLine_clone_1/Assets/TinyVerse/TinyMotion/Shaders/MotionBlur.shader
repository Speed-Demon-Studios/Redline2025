Shader "Hidden/TinyMotion/MotionBlur"
{
	Properties
	{
		_MainTex("Source", 2D) = "white" {}
	}

	HLSLINCLUDE

	// -------------------------------------
	// Includes
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Random.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

	TEXTURE2D_X(_MainTex); SAMPLER(sampler_PointClamp);
	TEXTURE2D_X(_TinyMotionVectors); SAMPLER(sampler_TinyMotionVectors);

	// -------------------------------------
	// Inputs
	float4 _MainTex_TexelSize;

	float _Threshold;
	float _ClampVelocity;
	int _SampleCount;
	int _DynamicSampleCount;
	int _MinSampleCount;
	int _MaxSampleCount;
	int _NoiseMode;
	int _AnimateNoise;
	int _DebugMode;
	
	int _DepthSeparation;
	int _DepthSeparationBothWays;
	float _DepthSeparationTreshold;
	int _DepthSeparationVelocity;
	float _DepthSeparationVelocityTreshold;

	// -------------------------------------
	// Structs
	struct Attributes
	{
		float4 positionOS   : POSITION;
		float4 uv           : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct VaryingsMB
	{
		float4 positionCSH   : SV_POSITION;
		float4 uv            : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};
	
	// -------------------------------------
	// Vertex
	VaryingsMB VertMB(Attributes input)
	{
		VaryingsMB OUT;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
		OUT.positionCSH = TransformObjectToHClip(input.positionOS.xyz);
		float4 projPos = OUT.positionCSH * 0.5;
		OUT.uv.xy = input.uv;
		OUT.uv.zw = projPos.xy + projPos.w;
		return OUT;
	}
	
	float SampleDepthTexture(float2 uv)
	{
		float z = SampleSceneDepth(uv);
		#if defined(UNITY_REVERSED_Z)
			z = 1.0f - z;
		#endif
		return z;
	}

	float2 SampleVelocityTexture(float2 _uv)
	{
		return pow(SAMPLE_TEXTURE2D_X(_TinyMotionVectors, sampler_TinyMotionVectors, _uv).rg * 2 - 1, 3.0);
	}
	float2 NormalizeVelocitySample(float2 _velocity)
	{
		if( _ClampVelocity != 0 )
		{
			if(length(_velocity) > _ClampVelocity){
				_velocity = _velocity * ( _ClampVelocity / length(_velocity) );
			}
		}

		return _velocity;
	}
	float2 GetVelocitySample(float2 _uv){
		return NormalizeVelocitySample(SampleVelocityTexture(_uv));
	}

	half GetNoiseSample(float2 _uv)
	{
		// _NoiseMode == 2
		half returnSample = 0.5;

		if( _NoiseMode == 1 )
		{
			if( _AnimateNoise == 1 ){
				_uv *= _Time.x;
			}
			returnSample = frac(sin(dot(_uv.xy, float2(1123.581321,1161.803398))) * 3142.592653);
		}
		else if( _NoiseMode == 0 )
		{
			if( _AnimateNoise == 1 ){
				returnSample = InterleavedGradientNoise(_uv * _MainTex_TexelSize.zw, _Time.y * 60);
			}
			else
			{
				returnSample = InterleavedGradientNoise(_uv * _MainTex_TexelSize.zw, 0);
			}
		}
		
		return returnSample;
	}
	int GetSampleCount(float2 velocity){
		return _DynamicSampleCount == 1 ? clamp(int(length(velocity * 2 / _MainTex_TexelSize.xy)), _MinSampleCount, _MaxSampleCount) : _SampleCount;
	}

	half4 DepthSeparation(float2 centerUV, float2 sampleUV, float depth)
	{
		float depthSample = SampleDepthTexture(sampleUV);
		float2 _uv = abs(depthSample - depth) > _DepthSeparationTreshold && (depthSample < depth || _DepthSeparationBothWays == 1) ? centerUV : sampleUV;
		return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, _uv);
	}
	half4 DepthSeparationWithVelocity(float2 centerUV, float2 sampleUV, float2 velocity, float depth)
	{
		float depthSample = SampleDepthTexture(sampleUV);
		float2 _uv = sampleUV;

		if( abs(depthSample - depth) > _DepthSeparationTreshold && (depthSample < depth || _DepthSeparationBothWays == 1) )
		{
			float2 sampleVelocity = NormalizeVelocitySample(SampleVelocityTexture(sampleUV));
			if( abs(length(sampleVelocity - velocity)) > _DepthSeparationVelocityTreshold * 0.25)
			{
				_uv = centerUV;
			}
		}
		
		return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, _uv);
	}

	half4 GetColorSample(float sampleNumber, float2 velocity, float rcpSampleCount, float2 centerUV, half noiseSample, half velocitySign)
	{
		float offsetLength = (sampleNumber + 0.5) + ((noiseSample - 0.5) * velocitySign);
		float2 sampleUV = centerUV + (offsetLength * rcpSampleCount) * velocity * velocitySign;
		return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, sampleUV);
	}
	half4 GetColorSampleWithDepthSeparation(float sampleNumber, float2 velocity, float rcpSampleCount, float2 centerUV, half noiseSample, half depth, half velocitySign)
	{
		float offsetLength = (sampleNumber + 0.5) + ((noiseSample - 0.5) * velocitySign);
		float2 sampleUV = centerUV + (offsetLength * rcpSampleCount) * velocity * velocitySign;
		return _DepthSeparationVelocity == 1 ? DepthSeparationWithVelocity(centerUV, sampleUV, velocity, depth) : DepthSeparation(centerUV, sampleUV, depth);
	}

	// SIMPLE MODE
	half4 FragSimple(VaryingsMB input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 uv = UnityStereoTransformScreenSpaceTex(input.uv.xy);
		float2 velocity = GetVelocitySample(uv);

		if( _Threshold != 0 )
		{
			if(abs(length(velocity)) < _Threshold)
			{
				return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
			}
			velocity.x -= _Threshold * sign(velocity.x);
			velocity.y -= _Threshold * sign(velocity.y);
		}

		half noiseSample = GetNoiseSample(uv);
		half4 resultColor = 0.0;

		// 0.25 = rcp(4);
		UNITY_UNROLL
		for (int i = 0; i < 4; i++)
		{
			resultColor += GetColorSample(i, velocity, 0.25, uv, noiseSample, -1.0);
			resultColor += GetColorSample(i, velocity, 0.25, uv, noiseSample, 1.0);
		}

		// 0.125 = rcp(8);
		return resultColor * 0.125;
	}

	// ADVANCED MODE
	half4 FragAdvanced(VaryingsMB input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 uv = UnityStereoTransformScreenSpaceTex(input.uv.xy);
		float2 velocity = GetVelocitySample(uv);

		if( _Threshold != 0 )
		{
			if(abs(length(velocity)) < _Threshold)
			{
				return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
			}
			velocity.x -= _Threshold * sign(velocity.x);
			velocity.y -= _Threshold * sign(velocity.y);
		}

		int sampleCount = GetSampleCount(velocity);

		if( sampleCount == 0 ){
			return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
		}

		half noiseSample = GetNoiseSample(uv);
		float rcpSampleCount = rcp(sampleCount);
		half4 resultColor = 0.0;

		UNITY_LOOP
		for (int i = 0; i < sampleCount; i++)
		{
			resultColor += GetColorSample(i, velocity, rcpSampleCount, uv, noiseSample, -1.0);
			resultColor += GetColorSample(i, velocity, rcpSampleCount, uv, noiseSample, 1.0);
		}

		return resultColor * rcp(sampleCount * 2);
	}
	
	// ADVANCED MODE WITH DEPTH SEPARATION
	half4 FragAdvancedDepth(VaryingsMB input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 uv = UnityStereoTransformScreenSpaceTex(input.uv.xy);
		float2 velocity = GetVelocitySample(uv);

		if( _Threshold != 0 )
		{
			if(abs(length(velocity)) < _Threshold)
			{
				return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
			}
			velocity.x -= _Threshold * sign(velocity.x);
			velocity.y -= _Threshold * sign(velocity.y);
		}

		int sampleCount = GetSampleCount(velocity);

		if( sampleCount == 0 ){
			return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
		}
		
		float depthSample = SampleDepthTexture(uv);
		half noiseSample = GetNoiseSample(uv);
		float rcpSampleCount = rcp(sampleCount);
		half4 resultColor = 0.0;

		UNITY_LOOP
		for (int i = 0; i < sampleCount; i++)
		{
			resultColor += GetColorSampleWithDepthSeparation(i, velocity, rcpSampleCount, uv, noiseSample, depthSample, -1.0);
			resultColor += GetColorSampleWithDepthSeparation(i, velocity, rcpSampleCount, uv, noiseSample, depthSample, 1.0);
		}

		return resultColor * rcp(sampleCount * 2);
	}

	ENDHLSL

	SubShader
	{
		Tags { 
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}
		LOD 100 
		ZTest Always
		ZWrite Off
		Cull Off
		
		Pass
		{
			Name "Tiny Motion Blur - Simple"
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex VertMB
			#pragma fragment FragSimple
			ENDHLSL
		}

		Pass
		{
			Name "Tiny Motion Blur - Advanced"
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex VertMB
			#pragma fragment FragAdvanced
			ENDHLSL
		}

		Pass
		{
			Name "Tiny Motion Blur - Advanced With Depth Separation"
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex VertMB
			#pragma fragment FragAdvancedDepth
			ENDHLSL
		}

		Pass
		{
			Name "Tiny Motion Blur - Debug Mode"
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex VertMB
			#pragma fragment Frag
			half4 Frag(VaryingsMB input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float2 uv = UnityStereoTransformScreenSpaceTex(input.uv.xy);
				float2 velocity = GetVelocitySample(uv);
				half4 originalSample = SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
				float depthSample = SampleDepthTexture(uv);
				half noiseSample = GetNoiseSample(uv);
				int sampleCount = GetSampleCount(velocity);
				
				// DYNAMIC SAMPLE COUNT
				if( _DebugMode == 6 ){
					half scRatio = (half)sampleCount / (half)_MaxSampleCount;
					half3 rgb = half3(0,0,0);
					half rTr = 0.8;
					half gTr = 0.5;
					half bTr = 0.2;
					half lw = 0.2;
					if( sampleCount > 0 )
					{
						if( scRatio < bTr )
						{
							rgb.x = 0.5;
							rgb.y = 0.5;
							rgb.z = 0.5;
						}
						else
						{
							if( scRatio >= rTr - lw )
							{
								rgb.x = (scRatio - rTr) / 0.2;
							}
							if( scRatio >= gTr - lw && scRatio < rTr + lw )
							{
								rgb.y = (scRatio - gTr) / (rTr - gTr);
							}
							if( scRatio >= bTr - lw && scRatio < gTr + lw )
							{
								rgb.z = (scRatio - bTr) / (gTr - bTr);
							}
						}
						return (half4(rgb,1) + originalSample) / 2;
					}
					else
					{
						return originalSample;
					}
				}

				// DEPTH
				if( _DebugMode == 5 )
				{
					return half4(depthSample,depthSample,depthSample,1);
				}
				
				// NOISE
				if( _DebugMode == 4 )
				{
					return half4(noiseSample,noiseSample,noiseSample,1);
				}

				// MOTION VECTORS
				if( _DebugMode == 3 )
				{
					return SAMPLE_TEXTURE2D_X(_TinyMotionVectors, sampler_TinyMotionVectors, uv);
				}

				// VELOCITY BW
				if( _DebugMode == 2 )
				{
					float velLen = length(velocity);
					return half4( velLen, velLen, velLen, 1);
				}
				
				// VELOCITY RG
				if  ( velocity.x < 0 ) velocity.x *= -1;
				if  ( velocity.y < 0 ) velocity.y *= -1;
				return half4( velocity, 0, 1);

			}
			ENDHLSL
		}
	}
}
