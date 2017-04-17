Shader "Custom/ToonWater"
{
    Properties
    {
       _Color ("Main Color", Color) = (1,1,1,1)
       _MainTex ("Base", 2D) = "" {}
       _BlendTex ("Under Tone", 2D) = ""
       _BlendAlpha ("Blend Alpha", float) = -3.0 //-3 as default
    }
    SubShader
    {
       Tags { "Queue"="Geometry-9" "IgnoreProjector"="True" "RenderType"="Transparent" }
       Lighting Off
       LOD 200
       Blend SrcAlpha OneMinusSrcAlpha
 
       CGPROGRAM
       #pragma surface surf Lambert
 
       fixed4 _Color;
       sampler2D _MainTex;
       sampler2D _BlendTex;
       float _BlendAlpha;
 
       struct Input {
         float2 uv_MainTex;
         float4 _Time;
       };
 
       void surf (Input IN, inout SurfaceOutput o) {
         fixed4 c = ( ( 1 - _BlendAlpha ) *  tex2D( _MainTex, IN.uv_MainTex + _Time.xx) + _BlendAlpha * tex2D( _BlendTex, IN.uv_MainTex - _Time.xx) ) * _Color;
         o.Albedo = c.rgb;
         o.Alpha = c.a;
       }
       ENDCG
    }
 
    Fallback "Transparent/VertexLit"
}