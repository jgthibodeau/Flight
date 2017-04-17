Shader "Hidden/TerrainEngine/Splatmap/DiffuseUVMixing-AddPass" {
	Properties {
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "black" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}

		_BaseModifier("Base mixing modifier", Range(0, 8)) = 2
		_BaseUVCoordinate("Base UV coordinate multiplier", Range(-10, 10)) = -0.25

		_MixingStartRange("Mixing start distance", Range(0, 1000)) = 10
		_MixingTransitionLength("Mixing transition length", Range(0, 500)) = 20

		_Octaves("Octaves", Float) = 8.0
		_Frequency("Frequency", Float) = 1.5
		_Amplitude("Amplitude", Float) = 0.75
		_Lacunarity("Lacunarity", Float) = 1.8
		_Persistence("Persistence", Float) = 0.7
		_Offset("Offset", Vector) = (0.0, 0.0, 0.0, 0.0)
		_NoiseValueOffset("Noise value offset", Range(-1, 1)) = 0.7
		_NoiseCutoffDistance("Noise cutoff distance", Range(0, 100000)) = 100000

		_Brightness("Brightness", Range(-1, 1)) = -0.1
		_Contrast("Contrast", Range(0, 2)) = 0.6
		_Saturation("Saturation", Range(0, 2)) = 0.8

		_WorldScale("World Scale", Vector) = (0.002, 0.002, 0.002, 0)
		_WorldTranslation("World Translation", Vector) = (0, 0, 0, 0)
	}

	CGINCLUDE
		#pragma surface surf Lambert decal:add vertex:SplatmapVert finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer
		#pragma multi_compile_fog

		#pragma multi_compile MIXXER_USE_TRIPLANAR_ON MIXXER_USE_TRIPLANAR_OFF
		#pragma multi_compile MIXXER_USE_NOISE_ON MIXXER_USE_NOISE_OFF
		#pragma multi_compile MIXXER_USE_COLOR_CORRECTION_OFF MIXXER_USE_COLOR_CORRECTION_ON

		#define TERRAIN_SPLAT_ADDPASS

		struct Input
		{
#if MIXXER_USE_TRIPLANAR_ON
			float3 vertNormal;
#else
			float2 uv_Splat0 : TEXCOORD0;
			float2 uv_Splat1 : TEXCOORD1;
			float2 uv_Splat2 : TEXCOORD2;
			float2 uv_Splat3 : TEXCOORD3;
#endif
			float2 tc_Control : TEXCOORD4;
			float3 worldPos;
			UNITY_FOG_COORDS(5)
		};

		#include "NoiseCommon.cginc"
		#include "TerrainSplatmapCommon_MixUV.cginc"		
		#include "TerrainSplatmapCommon.cginc"

		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 splat_control;
			half weight;
			fixed4 mixedDiffuse;
			SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
		}
	ENDCG

	Category {
		Tags {
			"Queue" = "Geometry-99"
			"IgnoreProjector"="True"
			"RenderType" = "Opaque"
		}
		// TODO: Seems like "#pragma target 3.0 _TERRAIN_NORMAL_MAP" can't fallback correctly on less capable devices?
		// Use two sub-shaders to simulate different features for different targets and still fallback correctly.
		SubShader { // for sm3.0+ targets
			CGPROGRAM
				#pragma target 3.0
				#pragma multi_compile __ _TERRAIN_NORMAL_MAP
			ENDCG
		}
		SubShader { // for sm2.0 targets
			CGPROGRAM
			ENDCG
		}
	}

	Fallback off

	CustomEditor "TerrainUVMixingEditor"
}
