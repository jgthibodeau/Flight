#ifndef TERRAIN_SPLATMAP_COMMON_MIXUV_CGINC_INCLUDED
#define TERRAIN_SPLATMAP_COMMON_MIXUV_CGINC_INCLUDED

float _BaseModifier;
float _BaseUVCoordinate;

float _UseNoise;
fixed _Octaves;
float _Frequency;
float _Amplitude;
float2 _Offset;
float _Lacunarity;
float _Persistence;
float _NoiseValueOffset;
float _NoiseCutoffDistance;

float _UseColorCorrection;
float _Brightness;
float _Contrast;
float _Saturation;

float _MixingStartRange;
float _MixingTransitionLength;

inline fixed4 TriplanarSimple(sampler2D s, float3 wp, float3 norm, float4 st)
{
	return norm.x * tex2D(s, st.xy * wp.zy + st.zw) + norm.y * tex2D(s, st.xy * wp.xz + st.zw) + norm.z * tex2D(s, st.xy * wp.xy + st.zw);
}

fixed4 MixUVTriplanar(sampler2D tex, float3 wp, float3 norm, float4 st, float dist)
{
	float value = saturate((dist - _MixingStartRange) / _MixingTransitionLength);

	float2 uv_a = st.xy * wp.zy + st.zw;
	float2 uv_b = st.xy * wp.xz + st.zw;
	float2 uv_c = st.xy * wp.xy + st.zw;

	half4 original = norm.x * tex2D(tex, uv_a) + norm.y * tex2D(tex, uv_b) + norm.z * tex2D(tex, uv_c);
	
	// Noise
	float consistency = 1;

#if MIXXER_USE_NOISE_ON
	if (dist < _NoiseCutoffDistance)
	{
		consistency = PerlinNormal(uv_b, _Octaves, _Offset, _Frequency, _Amplitude, _Lacunarity, _Persistence);
		consistency = (consistency * 0.5 + 0.5) + _NoiseValueOffset;
	}
#endif

	float2x2 rotationMatrix = float2x2(0.70710678118, -0.70710678118, 0.70710678118, 0.70710678118);
	float2 mixingUV_a = mul(uv_a, rotationMatrix) * _BaseUVCoordinate;
	float2 mixingUV_b = mul(uv_b, rotationMatrix) * _BaseUVCoordinate;
	float2 mixingUV_c = mul(uv_c, rotationMatrix) * _BaseUVCoordinate;

	half4 mixedBase = norm.x * tex2D(tex, mixingUV_a) + norm.y * tex2D(tex, mixingUV_b) + norm.z * tex2D(tex, mixingUV_c);
	half4 mixed = consistency * original * mixedBase * _BaseModifier;

#if MIXXER_USE_COLOR_CORRECTION_ON
	mixed.rgb = (mixed.rgb - 0.5f) * _Contrast + 0.5f;
	mixed.rgb = mixed.rgb + _Brightness;
	float3 intensity = dot(mixed.rgb, float3(0.299, 0.587, 0.114));
	mixed.rgb = lerp(intensity, mixed.rgb, _Saturation);
#endif

	half4 result = lerp(original, mixed, value);

	return result;
}

fixed4 MixUV(sampler2D tex, float2 uv, float dist)
{
	float value = saturate((dist - _MixingStartRange) / _MixingTransitionLength);

	// Noise
	float consistency = 1;

#if MIXXER_USE_NOISE_ON
	if (dist < _NoiseCutoffDistance)
	{
		consistency = PerlinNormal(uv, _Octaves, _Offset, _Frequency, _Amplitude, _Lacunarity, _Persistence);
		consistency = (consistency * 0.5 + 0.5) + _NoiseValueOffset;
	}
#endif

	float2 mixingUV = uv;

	float2x2 rotationMatrix = float2x2(0.70710678118, -0.70710678118, 0.70710678118, 0.70710678118);
	mixingUV = mul(uv, rotationMatrix);

	half4 original = tex2D(tex, uv);
	half4 mixed = consistency * original * tex2D(tex, mixingUV * _BaseUVCoordinate) * _BaseModifier;

#if MIXXER_USE_COLOR_CORRECTION_ON
	mixed.rgb = (mixed.rgb - 0.5f) * _Contrast + 0.5f;
	mixed.rgb = mixed.rgb + _Brightness;
	float3 intensity = dot(mixed.rgb, float3(0.299, 0.587, 0.114));
	mixed.rgb = lerp(intensity, mixed.rgb, _Saturation);
#endif

	half4 result = lerp(original, mixed, value);

	return result;
}

#endif // TERRAIN_SPLATMAP_COMMON_MIXUV_CGINC_INCLUDED
