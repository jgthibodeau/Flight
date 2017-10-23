Shader "Hidden/radialBlur" {
Properties {
    _MainTex ("Input", RECT) = "white" {}
    _BlurStrength ("", Float) = 0.5
    _BlurWidth ("", Float) = 0.5
}
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }
       
    CGPROGRAM
   
    #pragma vertex vert_img
    #pragma fragment frag
    #pragma fragmentoption ARB_precision_hint_fastest
 
    #include "UnityCG.cginc"
 
    uniform samplerRECT _MainTex;
    uniform half _BlurStrength;
    uniform half _BlurWidth;
    uniform half _iWidth;
    uniform half _iHeight;
 
    half4 frag (v2f_img i) : COLOR {
        half4 color = texRECT(_MainTex, i.uv);
       
        // some sample positions
        half samples[10] = half[](-0.08,-0.05,-0.03,-0.02,-0.01,0.01,0.02,0.03,0.05,0.08);
       
        //vector to the middle of the screen
        half2 dir = 0.5 * half2(_iHeight,_iWidth) - i.uv;
       
        //distance to center
        half dist = sqrt(dir.x*dir.x + dir.y*dir.y);
       
        //normalize direction
        dir = dir/dist;
       
        //additional samples towards center of screen
        half4 sum = color;
        for(int n = 0; n < 10; n++)
        {
            sum += texRECT(_MainTex, i.uv + dir * samples[n] * _BlurWidth * _iWidth);
        }
       
        //eleven samples...
        sum *= 1.0/11.0;
       
        //weighten blur depending on distance to screen center
        half t = dist * _BlurStrength / _iWidth;
        t = clamp(t, 0.0, 1.0);
       
        //blend original with blur
        return lerp(color, sum, t);
    }
    ENDCG
        }
    }
}