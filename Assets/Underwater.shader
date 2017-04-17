Shader "Custom/Underwater"
{
  Properties 
  {
    _WaterColor ("Water Color", Color)  = (1, 1, 1, .5) //Base Color of the Water
    _ObjectColor ("Color of Objects Underwater", Color) = (0, 0, 0, 1) //Color of Objects in Water
    _NoiseTexture ("Noise Texture", 2D) = "white" //Texture for Noise
    _NoiseRefractMultiplier ("Noise Refraction Multiplier", Float) = .5 //How much to offset the screen-space uv coordinates by the noise
    _NoiseScale("Noise Texture Scale", Float) = .02 //How often the noise texture tiles   
    _FogStart("Fog Start", Float) = 2 //When to start fading to _WaterColor
    _FogEnd("Fog End", Float) = 2 //When to completely fade to _WaterColor
  }
    SubShader 
  {
    Tags { "Queue" = "Transparent-1"}
     
    GrabPass {}
     
    Pass 
    { 
      ZWrite Off
         
      CGPROGRAM
      #pragma target 3.0
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
   
      uniform sampler2D _CameraDepthTexture;
      uniform sampler2D _NoiseTexture;
      uniform float4 _WaterColor;
      uniform float4 _ObjectColor;
      uniform float _NoiseRefractMultiplier;
      uniform float _NoiseScale;
      uniform float _FogStart;
      uniform float _FogEnd;
       
      struct v2f 
      {
        float4 pos : SV_POSITION;
        float3 worldPos : TEXCOORD0; //World position of pos
        float4 projPos : TEXCOORD1; //Screen position of pos
      };
       
      v2f vert(appdata_base v)
      {         
        v2f o;
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
         
        o.worldPos = v.vertex.xyz;
         
        o.projPos = ComputeScreenPos(o.pos);
        COMPUTE_EYEDEPTH(o.projPos.z);
         
        return o;
      }
       
      half4 frag(v2f i) : COLOR 
      {            
        //Get the projected coordinates, then modify them by noise
        float3 originalTexCoord = UNITY_PROJ_COORD(i.projPos);      
        float3 modifiedTexCoord = originalTexCoord;
         
        //Modify by noise
        float time = _Time.x;
         
        float2 noiseUV = float2(i.worldPos.x * _NoiseScale,
                                i.worldPos.z * _NoiseScale);
        noiseUV += time;
         
        float3 noiseAmount = tex2D(_NoiseTexture, noiseUV);
        noiseAmount -= .5;
        noiseAmount *= _NoiseRefractMultiplier;
         
        modifiedTexCoord.x += noiseAmount.g;
        modifiedTexCoord.y += noiseAmount.b;
         
        //Get the distance to the camera from the depth buffer for this point
        float modifiedSceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture,
                              modifiedTexCoord).r);
        float sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, 
                              originalTexCoord).r);
                   
 
        //Actual distance to the camera
                float partZ = i.projPos.z;
                 
                if(modifiedSceneZ >= partZ)
          sceneZ = modifiedSceneZ;
           
                float diff = sceneZ - partZ;
                 
                if(diff < 0) 
                {
                  diff = 0;
            }
                else if(diff > _FogStart)
                {
                  diff = lerp(_WaterColor.a, 
                        1, 
                        (diff - _FogStart) / (_FogEnd - _FogStart));
                }
                else
                {
                  diff = _WaterColor.a;
                }                      
         
        half4 c;
        c.r = (half)lerp(_ObjectColor.r, _WaterColor.r, diff);
        c.g = (half)lerp(_ObjectColor.g, _WaterColor.g, diff);
        c.b = (half)lerp(_ObjectColor.b, _WaterColor.b, diff);
        c.a = 1;
         
        return c;
      }
       
      ENDCG
    }
  }
  FallBack "VertexLit"
}