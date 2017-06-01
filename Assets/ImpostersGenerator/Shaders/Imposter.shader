// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Imposter" {
   Properties {
      _MainTex ("Texture Image", 2D) = "white" {}
	  _Slices ("Texture slices", float) = 8.0
	  _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	  [HideInInspector]_OffsetX ("OffsetX", float) = 0
	  [HideInInspector]_OffsetY ("OffsetY", float) = 0
   }

   SubShader {
      Tags {"Queue"="AlphaTest" "IgnoreProjector"="True"  "RenderType"="TransparentCutout"}
	  		Alphatest Greater [_Cutoff]
		AlphaToMask True
		ColorMask RGB
      Pass {   
         CGPROGRAM

         #pragma vertex vert  
         #pragma fragment frag 

		 #define TWO_PI 6.2831854
         
         uniform sampler2D _MainTex; 
		 uniform fixed _Cutoff;  
		 uniform half _Slices; 
		 uniform half _OffsetX;
		 uniform half _OffsetY;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
         };

         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
			half2 offset : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;

			output.pos = UnityObjectToClipPos(input.vertex);
			output.offset = float2(_OffsetX,_OffsetY);
            output.tex = input.texcoord;
 
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
			float2 _uv = float2(input.tex.x, input.tex.y ) * (1/_Slices);
			_uv += input.offset ;

            fixed4 texcol = tex2D(_MainTex, float2(_uv ) );   
			clip( texcol.a - _Cutoff );
			return texcol;
         }
 
         ENDCG
      }
   }
}
