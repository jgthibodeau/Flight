Shader "Hidden/TerrainEngine/Splatmap/DiffuseUVMixing-Base" {
	Properties {
		_MainTex ("Base (RGB) Smoothness (A)", 2D) = "white" {}

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

		// used in fallback on old cards
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags {
			"RenderType" = "Opaque"
			"Queue" = "Geometry-100"
		}
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows
		#pragma target 3.0

		#pragma multi_compile MIXXER_USE_TRIPLANAR_ON MIXXER_USE_TRIPLANAR_OFF
		#pragma multi_compile MIXXER_USE_NOISE_ON MIXXER_USE_NOISE_OFF
		#pragma multi_compile MIXXER_USE_COLOR_CORRECTION_OFF MIXXER_USE_COLOR_CORRECTION_ON

		// needs more than 8 texcoords
		#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"
		#include "NoiseCommon.cginc"
		#include "TerrainSplatmapCommon_MixUV.cginc"

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
			half4 c = MixUV (_MainTex, IN.uv_MainTex * 255, dist);
			o.Albedo = c.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}

	FallBack "Diffuse"

	CustomEditor "TerrainUVMixingEditor"
}
