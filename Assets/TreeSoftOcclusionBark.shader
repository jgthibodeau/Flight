// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Nature/Tree Soft Occlusion Bark" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,0)
		_MainTex ("Main Texture", 2D) = "white" {}
		_BaseLight ("Base Light", Range(0, 1)) = 0.35
		_AO ("Amb. Occlusion", Range(0, 10)) = 2.4
		_Outline ("Outline width", Range (0.0, 0.03)) = .01
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		
		// These are here only to provide default values
		[HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
		[HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
		[HideInInspector] _SquashAmount ("Squash", Float) = 1
	}

	SubShader {
		Tags {
			"IgnoreProjector"="True"
			"RenderType" = "TreeOpaque"
			"DisableBatching"="True"
		}
		// ----------------------
        // Start of Outline adding
   
        CGINCLUDE
        #include "UnityCG.cginc"
		#include "UnityBuiltin2xTreeLibrary.cginc"
     
        struct appdata2 {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };
     
        struct v2f2 {
            float4 pos : SV_POSITION;
//            float4 scale : SV_SCALE;
            UNITY_FOG_COORDS(0)
            fixed4 color : COLOR;
        };
     
        uniform float _Outline;
        uniform float4 _OutlineColor;

         float randomNum(in float2 uv) {
		     float2 noise = (frac(sin(dot(uv ,float2(12.9898,78.233)*2.0)) * 43758.5453));
		     return abs(noise.x + noise.y) * 0.5;
		 }

//		 float random(float2 p){return fract(cos(dot(p,float2(23.14069263277926,2.665144142690225)))*123456.);}


        v2f2 vert(appdata_full v) {
//            // just make a copy of incoming vertex data but scaled according to normal direction
//            v2f2 o;
//
//            float3 scale = UnityObjectToClipPos(v.vertex*_TreeInstanceScale);
//			if (_SquashAmount == 1) {
////				v.vertex.x *= 1.1;
////				v.vertex.y *= 1.1;
////				v.vertex.z *= 1.1;
////				v.vertex.z *= _TreeInstanceScale.z * 1.1;
////				v.vertex.y -= 1.1/2;
////				v.vertex.x *= 1.01;
////				v.vertex.y *= 1.01;
////				v.vertex.z *= 1.01;
//			}
//			else {
//				v.vertex *= 0;
//			}
//
//            o.pos = UnityObjectToClipPos(v.vertex);
//
//            v.normal.x = randomNum(v.normal.x);
//            v.normal.y = randomNum(v.normal.y);
//            v.normal.z = randomNum(v.normal.z);
//
//            float3 norm = normalize(mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
//            float3 offset = TransformViewToProjection(norm.xyz);
//             o.pos.xyz += offset  * _Outline * 10;
//	         o.color = _OutlineColor;
//
//            UNITY_TRANSFER_FOG(o,o.pos);
//            return o;

			    v2f2 o;
			    o.pos = UnityObjectToClipPos(v.vertex);
			    float3 norm = mul ((float3x3)UNITY_MATRIX_MV, v.normal);
			    norm.x *= UNITY_MATRIX_P[0][0];
			    norm.y *= UNITY_MATRIX_P[1][1];

//			    float rand = v.vertex.x - v.vertex.y + v.vertex.z - v.normal.x + v.normal.y - v.normal.z % 1.0f;
//
//			    norm.x = rand;
//			    norm.y = rand;
//			    norm.z = rand;

//            norm.x = randomNum(v.normal.x);
//            norm.y = randomNum(v.normal.y);
//            v.normal.z = randomNum(v.normal.z);

//			    o.pos.xy += norm.xy * _Outline*5;
			    o.color = _OutlineColor;
			    return o;
        }
        ENDCG      
 
//        Pass {
//            Name "OUTLINE"
//            Tags { "LightMode" = "Always" }
////            Cull Front
//            ZWrite On
//            ColorMask RGB
//            Blend SrcAlpha OneMinusSrcAlpha
// 
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #pragma multi_compile_fog
//            fixed4 frag(v2f2 i) : SV_Target
//            {
//                UNITY_APPLY_FOG(i.fogCoord, i.color);
//                return i.color;
//            }
//            ENDCG
//        }
 
        // End of Outline adding
        // ----------------------

		Pass {
			Lighting On
		
			CGPROGRAM
			#pragma vertex bark
			#pragma fragment frag
			#pragma multi_compile_fog
//			#include "UnityBuiltin2xTreeLibrary.cginc"
			
			sampler2D _MainTex;
			
			fixed4 frag(v2f input) : SV_Target
			{
				fixed4 col = input.color;
				col.rgb *= tex2D( _MainTex, input.uv.xy).rgb;
				UNITY_APPLY_FOG(input.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
			ENDCG
		}
		
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			
//			struct v2f { 
//				V2F_SHADOW_CASTER;
//				UNITY_VERTEX_OUTPUT_STEREO
//			};
			
			struct appdata {
			    float4 vertex : POSITION;
				float3 normal : NORMAL;
			    fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				TerrainAnimateTree(v.vertex, v.color.w);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}
			
			float4 frag( v2f i ) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG	
		}
	}
	
	Dependency "BillboardShader" = "Hidden/Nature/Tree Soft Occlusion Bark Rendertex"
	Fallback Off
}
