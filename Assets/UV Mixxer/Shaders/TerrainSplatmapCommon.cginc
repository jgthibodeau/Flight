// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef TERRAIN_SPLATMAP_COMMON_CGINC_INCLUDED
#define TERRAIN_SPLATMAP_COMMON_CGINC_INCLUDED

sampler2D _Control;
float4 _Control_ST;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float4 _WorldScale;
float4 _WorldTranslation;

#if MIXXER_USE_TRIPLANAR_ON
	float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
#endif

#ifdef _TERRAIN_NORMAL_MAP
	sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
#endif

void SplatmapVert(inout appdata_full v, out Input data)
{
	UNITY_INITIALIZE_OUTPUT(Input, data);
	data.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);	// Need to manually transform uv here, as we choose not to use 'uv' prefix for this texcoord.
	float4 pos = UnityObjectToClipPos (v.vertex);
	UNITY_TRANSFER_FOG(data, pos);

#if MIXXER_USE_TRIPLANAR_ON	
	data.vertNormal = v.normal;
#endif

#ifdef _TERRAIN_NORMAL_MAP
	v.tangent.xyz = cross(v.normal, float3(0,0,1));
	v.tangent.w = -1;
#endif
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
		clip(weight - 0.0039 /*1/255*/);
	#endif

	// Normalize weights before lighting and restore weights in final modifier functions so that the overal
	// lighting result can be correctly weighted.
	splat_control /= (weight + 1e-3f);

	float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
	mixedDiffuse = 0.0f;
	
	#if MIXXER_USE_TRIPLANAR_ON	
		float3 worldPos = IN.worldPos * _WorldScale.xyz + _WorldTranslation.xyz;
		float3 normal = abs(IN.vertNormal);
		normal /= normal.x + normal.y + normal.z + 1e-3f;

		#ifdef TERRAIN_STANDARD_SHADER
			mixedDiffuse += splat_control.r * MixUVTriplanar(_Splat0, worldPos, normal, _Splat0_ST, dist) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
			mixedDiffuse += splat_control.g * MixUVTriplanar(_Splat1, worldPos, normal, _Splat1_ST, dist) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
			mixedDiffuse += splat_control.b * MixUVTriplanar(_Splat2, worldPos, normal, _Splat2_ST, dist) * half4(1.0, 1.0, 1.0, defaultAlpha.b);
			mixedDiffuse += splat_control.a * MixUVTriplanar(_Splat3, worldPos, normal, _Splat3_ST, dist) * half4(1.0, 1.0, 1.0, defaultAlpha.a);
		#else
			mixedDiffuse += splat_control.r * MixUVTriplanar(_Splat0, worldPos, normal, _Splat0_ST, dist);
			mixedDiffuse += splat_control.g * MixUVTriplanar(_Splat1, worldPos, normal, _Splat1_ST, dist);
			mixedDiffuse += splat_control.b * MixUVTriplanar(_Splat2, worldPos, normal, _Splat2_ST, dist);
			mixedDiffuse += splat_control.a * MixUVTriplanar(_Splat3, worldPos, normal, _Splat3_ST, dist);
		#endif
	#else
		#ifdef TERRAIN_STANDARD_SHADER
			mixedDiffuse += splat_control.r * MixUV(_Splat0, IN.uv_Splat0, dist) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
			mixedDiffuse += splat_control.g * MixUV(_Splat1, IN.uv_Splat1, dist) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
			mixedDiffuse += splat_control.b * MixUV(_Splat2, IN.uv_Splat2, dist) * half4(1.0, 1.0, 1.0, defaultAlpha.b);
			mixedDiffuse += splat_control.a * MixUV(_Splat3, IN.uv_Splat3, dist) * half4(1.0, 1.0, 1.0, defaultAlpha.a);
		#else
			mixedDiffuse += splat_control.r * MixUV(_Splat0, IN.uv_Splat0, dist);
			mixedDiffuse += splat_control.g * MixUV(_Splat1, IN.uv_Splat1, dist);
			mixedDiffuse += splat_control.b * MixUV(_Splat2, IN.uv_Splat2, dist);
			mixedDiffuse += splat_control.a * MixUV(_Splat3, IN.uv_Splat3, dist);
		#endif
	#endif
	
	#ifdef _TERRAIN_NORMAL_MAP
		fixed4 nrm = 0.0f;
		#if MIXXER_USE_TRIPLANAR_ON	
			nrm += splat_control.r * TriplanarSimple(_Normal0, worldPos, normal, _Splat0_ST);
			nrm += splat_control.g * TriplanarSimple(_Normal1, worldPos, normal, _Splat1_ST);
			nrm += splat_control.b * TriplanarSimple(_Normal2, worldPos, normal, _Splat2_ST);
			nrm += splat_control.a * TriplanarSimple(_Normal3, worldPos, normal, _Splat3_ST);
		#else
			nrm += splat_control.r * tex2D(_Normal0, IN.uv_Splat0);
			nrm += splat_control.g * tex2D(_Normal1, IN.uv_Splat1);
			nrm += splat_control.b * tex2D(_Normal2, IN.uv_Splat2);
			nrm += splat_control.a * tex2D(_Normal3, IN.uv_Splat3);
		#endif
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

#endif // TERRAIN_SPLATMAP_COMMON_CGINC_INCLUDED
