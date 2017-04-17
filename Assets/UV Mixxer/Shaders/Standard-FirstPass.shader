Shader "Nature/Terrain/Standard (UV Mixing)" {
	Properties {
		// set by terrain engine
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		[HideInInspector] [Gamma] _Metallic0 ("Metallic 0", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic1 ("Metallic 1", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic2 ("Metallic 2", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic3 ("Metallic 3", Range(0.0, 1.0)) = 0.0
		[HideInInspector] _Smoothness0 ("Smoothness 0", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness1 ("Smoothness 1", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness2 ("Smoothness 2", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness3 ("Smoothness 3", Range(0.0, 1.0)) = 1.0

		// used in fallback on old cards & base map
		[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)

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

	SubShader {
		Tags {
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}

		CGPROGRAM
		#pragma surface surf Standard vertex:SplatmapVert finalcolor:SplatmapFinalColor finalgbuffer:SplatmapFinalGBuffer fullforwardshadows
		#pragma multi_compile_fog
		#pragma target 3.0
		// needs more than 8 texcoords
		#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"

		#pragma multi_compile __ _TERRAIN_NORMAL_MAP

		#pragma multi_compile MIXXER_USE_TRIPLANAR_ON MIXXER_USE_TRIPLANAR_OFF
		#pragma multi_compile MIXXER_USE_NOISE_ON MIXXER_USE_NOISE_OFF
		#pragma multi_compile MIXXER_USE_COLOR_CORRECTION_OFF MIXXER_USE_COLOR_CORRECTION_ON

		#define TERRAIN_STANDARD_SHADER
		#define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard

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
		 
		half _Metallic0;
		half _Metallic1;
		half _Metallic2;
		half _Metallic3;
		
		half _Smoothness0;
		half _Smoothness1;
		half _Smoothness2;
		half _Smoothness3;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 splat_control;
			half weight;
			fixed4 mixedDiffuse;
			half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
			SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
			o.Smoothness = mixedDiffuse.a;
			o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
		}
		ENDCG
	}

	Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/StandardUVMixing-AddPass"
	Dependency "BaseMapShader" = "Hidden/TerrainEngine/Splatmap/StandardUVMixing-Base"

	Fallback "Nature/Terrain/Diffuse"

	CustomEditor "TerrainUVMixingEditor"
}
