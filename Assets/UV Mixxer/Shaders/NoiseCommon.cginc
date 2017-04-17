#ifndef NOISE_COMMON_CGINC_INCLUDED
#define NOISE_COMMON_CGINC_INCLUDED

void FAST32_hash_2D(float2 gridcell, out float4 hash_0, out float4 hash_1)	//	generates 2 random numbers for each of the 4 cell corners
{
	//    gridcell is assumed to be an integer coordinate
	const float2 OFFSET = float2(26.0, 161.0);
	const float DOMAIN = 71.0;
	const float2 SOMELARGEFLOATS = float2(951.135664, 642.949883);
	float4 P = float4(gridcell.xy, gridcell.xy + 1.0);
	P = P - floor(P * (1.0 / DOMAIN)) * DOMAIN;
	P += OFFSET.xyxy;
	P *= P;
	P = P.xzxz * P.yyww;
	hash_0 = frac(P * (1.0 / SOMELARGEFLOATS.x));
	hash_1 = frac(P * (1.0 / SOMELARGEFLOATS.y));
}

//
//	Interpolation functions
//	( smoothly increase from 0.0 to 1.0 as x increases linearly from 0.0 to 1.0 )
//	http://briansharpe.wordpress.com/2011/11/14/two-useful-interpolation-functions-for-noise-development/
//
float2 Interpolation_C2(float2 x) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }
//
//	Perlin Noise 2D  ( gradient noise )
//	Return value range of -1.0->1.0
//	http://briansharpe.files.wordpress.com/2011/11/perlinsample.jpg
//
float Perlin2D(float2 P)
{
	//	establish our grid cell and unit position
	float2 Pi = floor(P);
	float4 Pf_Pfmin1 = P.xyxy - float4(Pi, Pi + 1.0);

	//	calculate the hash.
	float4 hash_x, hash_y;
	FAST32_hash_2D(Pi, hash_x, hash_y);

	//	calculate the gradient results
	float4 grad_x = hash_x - 0.49999;
	float4 grad_y = hash_y - 0.49999;
	float4 grad_results = rsqrt(grad_x * grad_x + grad_y * grad_y) * (grad_x * Pf_Pfmin1.xzxz + grad_y * Pf_Pfmin1.yyww);

#if 1
	//	Classic Perlin Interpolation
	grad_results *= 1.4142135623730950488016887242097;		//	(optionally) scale things to a strict -1.0->1.0 range    *= 1.0/sqrt(0.5)
	float2 blend = Interpolation_C2(Pf_Pfmin1.xy);
	float2 res0 = lerp(grad_results.xy, grad_results.zw, blend.y);
	return lerp(res0.x, res0.y, blend.x);
#else
	//	Classic Perlin Surflet
	//	http://briansharpe.wordpress.com/2012/03/09/modifications-to-classic-perlin-noise/
	grad_results *= 2.3703703703703703703703703703704;		//	(optionally) scale things to a strict -1.0->1.0 range    *= 1.0/cube(0.75)
	float4 vecs_len_sq = Pf_Pfmin1 * Pf_Pfmin1;
	vecs_len_sq = vecs_len_sq.xzxz + vecs_len_sq.yyww;
	return dot(Falloff_Xsq_C2(min(float4(1.0, 1.0, 1.0, 1.0), vecs_len_sq)), grad_results);
#endif
}

float PerlinNormal(float2 p, int octaves, float2 offset, float frequency, float amplitude, float lacunarity, float persistence)
{
	float sum = 0;
	for (int i = 0; i < octaves; i++)
	{
		float h = 0;
		h = Perlin2D((p + offset) * frequency);
		sum += h*amplitude;
		frequency *= lacunarity;
		amplitude *= persistence;
	}
	return sum;
}

#endif