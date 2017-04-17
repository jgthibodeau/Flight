// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef TRIPLANAR_SPLATMAP_CGINC_INCLUDED
#define TRIPLANAR_SPLATMAP_CGINC_INCLUDED

struct Input
{
	float2 tc_Control : TEXCOORD4;	// Not prefixing '_Contorl' with 'uv' allows a tighter packing of interpolators, which is necessary to support directional lightmap.
	
	float3 worldPos;
	float3 vertNormal;
	
	UNITY_FOG_COORDS(5)
};

sampler2D _Control;
float4 _Control_ST;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
float4 _WorldS, _WorldT;

#ifdef _TERRAIN_NORMAL_MAP
	sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
#endif

void SplatmapVert(inout appdata_full v, out Input data)
{
	UNITY_INITIALIZE_OUTPUT(Input, data);
	data.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);	// Need to manually transform uv here, as we choose not to use 'uv' prefix for this texcoord.
	float4 pos = UnityObjectToClipPos(v.vertex);
	UNITY_TRANSFER_FOG(data, pos);
		
	data.vertNormal = v.normal;
		
#ifdef _TERRAIN_NORMAL_MAP
	v.tangent.xyz = cross(v.normal, float3(0,0,1));
	v.tangent.w = -1;
#endif	
}

inline fixed4 Triplanar(float3 wp, float3 n, float4 st, sampler2D s)
{
	return 	n.x * tex2D(s, st.xy * wp.zy + st.zw) + 
			n.y * tex2D(s, st.xy * wp.xz + st.zw) + 
			n.z * tex2D(s, st.xy * wp.xy + st.zw);
}

#ifdef TERRAIN_STANDARD_SHADER
void SplatmapMix(Input IN, half4 defaultAlpha, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
#else
void SplatmapMix(Input IN, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
#endif
{
	splat_control = tex2D(_Control, IN.tc_Control);
	weight = dot(splat_control, half4(1,1,1,1));

	#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
		clip(weight == 0.0f ? -1 : 1);
	#endif

	// Normalize weights before lighting and restore weights in final modifier functions so that the overal
	// lighting result can be correctly weighted.
	splat_control /= (weight + 1e-3f);

	float3 worldPos = IN.worldPos * _WorldS.xyz + _WorldT.xyz;
	float3 normal = abs(IN.vertNormal);
	normal /= normal.x + normal.y + normal.z + 1e-3f;
	
	mixedDiffuse = 0.0f;
	#ifdef TERRAIN_STANDARD_SHADER
		mixedDiffuse += splat_control.r * Triplanar(worldPos, normal, _Splat0_ST, _Splat0) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
		mixedDiffuse += splat_control.g * Triplanar(worldPos, normal, _Splat1_ST, _Splat1) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
		mixedDiffuse += splat_control.b * Triplanar(worldPos, normal, _Splat2_ST, _Splat2) * half4(1.0, 1.0, 1.0, defaultAlpha.b);
		mixedDiffuse += splat_control.a * Triplanar(worldPos, normal, _Splat3_ST, _Splat3) * half4(1.0, 1.0, 1.0, defaultAlpha.a);
	#else                                 
		mixedDiffuse += splat_control.r * Triplanar(worldPos, normal, _Splat0_ST, _Splat0);
		mixedDiffuse += splat_control.g * Triplanar(worldPos, normal, _Splat1_ST, _Splat1);
		mixedDiffuse += splat_control.b * Triplanar(worldPos, normal, _Splat2_ST, _Splat2);
		mixedDiffuse += splat_control.a * Triplanar(worldPos, normal, _Splat3_ST, _Splat3);
	#endif

	#ifdef _TERRAIN_NORMAL_MAP
		fixed4 nrm = 0.0f;
		nrm += splat_control.r * Triplanar(worldPos, normal, _Splat0_ST, _Normal0);
		nrm += splat_control.g * Triplanar(worldPos, normal, _Splat1_ST, _Normal1);
		nrm += splat_control.b * Triplanar(worldPos, normal, _Splat2_ST, _Normal2);
		nrm += splat_control.a * Triplanar(worldPos, normal, _Splat3_ST, _Normal3);
		mixedNormal = UnpackNormal(nrm);
	#endif
}

#ifndef TERRAIN_SURFACE_OUTPUT
	#define TERRAIN_SURFACE_OUTPUT SurfaceOutput
#endif

void SplatmapFinalColor(Input IN, TERRAIN_SURFACE_OUTPUT o, inout fixed4 color)
{
	color *= o.Alpha;
	#ifdef TERRAIN_SPLAT_ADDPASS
		UNITY_APPLY_FOG_COLOR(IN.fogCoord, color, fixed4(0,0,0,0));
	#else
		UNITY_APPLY_FOG(IN.fogCoord, color);
	#endif
}

void SplatmapFinalPrepass(Input IN, TERRAIN_SURFACE_OUTPUT o, inout fixed4 normalSpec)
{
	normalSpec *= o.Alpha;
}

void SplatmapFinalGBuffer(Input IN, TERRAIN_SURFACE_OUTPUT o, inout half4 diffuse, inout half4 specSmoothness, inout half4 normal, inout half4 emission)
{
	diffuse.rgb *= o.Alpha;
	specSmoothness *= o.Alpha;
	normal.rgb *= o.Alpha;
	emission *= o.Alpha;
}

#endif // TRIPLANAR_SPLATMAP_CGINC_INCLUDED
