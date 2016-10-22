using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Basic Time Of Day/Scripts/Time Of Day Manager")]
public class TimeOfDayManager : MonoBehaviour 
{

	#region resources.
	// Autoassign sky material?.
	[SerializeField]
	private bool m_AutoAssignSky = true;

	// Sky material.
	[SerializeField]
	private Material m_SkyMaterial = null;

	// Sun transform.
	[SerializeField]
	private Transform m_Sun = null;

	// Sun light.
	[SerializeField]
	private Light m_SunLight = null;

	// Moon transform.
	[SerializeField]
	private Transform m_Moon = null;

	// Moon light.
	[SerializeField]
	private Light m_MoonLight = null;

	// Moon texture.
	public Texture2D moonTexture = null;

	// Stars cubemap.
	public  Cubemap starsCubemap = null;

	// Stars noise cubemap.
	public  Cubemap starsNoiseCubemap = null;

	//=============================================================================================
	#endregion




	#region Time Of Day
	[Range(-180f,180f)]
	public float longitude = 25f;

	// Play time?
	public bool  playTime      = true;

	// Day in seconds.
	public float dayInSeconds = 60f;

	// Current time.
	public float currentTime   = 10f; 

	// Day duration in the earth.
	private const float k_DayDuration = 24f;

	// Time counter.
	private float m_TimeCounter = 0;

	// Used to evaluate the curves and gradients.
	public float CGCycle{get{return currentTime  / k_DayDuration;}}

	// Use for sun.
	public float SunCycle{get{return  (currentTime * (360 / k_DayDuration)) -90;}}
	//=============================================================================================
	#endregion


	#region Atmosphere
	public Gradient skyTint = new Gradient()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(.45f, .45f, .45f, 1f), .25f),
			new GradientColorKey(new Color(.45f, .45f, .45f, 1f), .30f),
			new GradientColorKey(new Color(.45f, .45f, .45f, 1f), .70f),
			new GradientColorKey(new Color(.45f, .45f, .45f, 1f), .75f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get sky tint.
	public Color SkyTint{get{return skyTint.Evaluate(CGCycle);}}

	public AnimationCurve atmoshphereThickness = AnimationCurve.Linear(0, 1f, 1f, 1f); 

	// Get atmosphere thickness.
	public float AtmoshphereThickness{get{return atmoshphereThickness.Evaluate(CGCycle);}}  

	// Ground skybox color.
	public Color groundColor = new Color(.412f, .384f, .365f, 1f); 

	public Gradient nightHorizonColor = new Gradient ()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(.027f, .075f, .125f, 1f), .20f),
			new GradientColorKey(new Color(   0f,    0f,    0f, 1f), .30f),
			new GradientColorKey(new Color(   0f,    0f,    0f, 1f), .70f),
			new GradientColorKey(new Color(.027f, .075f, .125f, 1f), .80f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get night horizon color.
	public Color NightHorizonColor{get{return nightHorizonColor.Evaluate(CGCycle);}}

	public AnimationCurve nightHorizonExponent = AnimationCurve.Linear(0, 3.5f, 1f, 3.5f); 

	// Get night horizon exponent.
	public float NightHorizonExponent{get{return nightHorizonExponent.Evaluate(CGCycle);}}

	// HDR exposure.
	public float exposure = 1.3f;
	//=============================================================================================
	#endregion




	#region Sun
	public AnimationCurve sunSize = AnimationCurve.Linear(0, .05f, 1f, .05f);

	// Get sun size.
	public float SunSize {get{return sunSize.Evaluate(CGCycle);}} 

	public AnimationCurve sunLightIntensity = new AnimationCurve()
	{
		keys = new Keyframe[]
		{
			new Keyframe(  0f, 0f), 
			new Keyframe(.25f, 0f), 
			new Keyframe(.30f, 1f), 
			new Keyframe(.70f, 1f), 
			new Keyframe(.75f, 0f), 
			new Keyframe(  1f, 0f) 
		}

	};

	// Get sun light intensity.
	public float SunLightIntensity{get{return sunLightIntensity.Evaluate(CGCycle);}} 

	public Gradient sunColor = new Gradient()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(1f, .639f, .482f, 1f), .25f),
			new GradientColorKey(new Color(1f, .725f, .482f, 1f), .30f),
			new GradientColorKey(new Color(1f, .851f, .722f, 1f), .50f),
			new GradientColorKey(new Color(1f, .725f, .482f, 1f), .70f),
			new GradientColorKey(new Color(1f, .639f, .482f, 1f), .75f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get sun color.
	public Color SunColor{get{return sunColor.Evaluate(CGCycle);}}

	// Sun direction.
	private Vector3 SunDirection{get{return -m_Sun.forward;}}
	//=============================================================================================
	#endregion


	#region Moon
	public bool useMoon = true;

	public Gradient moonLightColor = new Gradient()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(.345f, .459f, .533f, 1f), 0f),
			new GradientColorKey(new Color(.345f, .459f, .533f, 1f), 1f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get moon light color.
	public Color MoonLightColor{get{return moonLightColor.Evaluate(CGCycle);}}


	public AnimationCurve moonLightIntensity = new AnimationCurve()
	{
		keys = new Keyframe[]
		{
			new Keyframe(  0f, .2f), new Keyframe(.25f, .2f), 
			new Keyframe(.30f,  0f), new Keyframe(.70f,  0f), 
			new Keyframe(.75f, .2f), new Keyframe( 1f, .2f) 
		}

	};

	// Get moon light intensity.
	public float  MoonLightIntensity{get{return moonLightIntensity.Evaluate(CGCycle);}}


	public Gradient moonColor = new Gradient()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(1f, 1f, 1f, 1f), 0f),
			new GradientColorKey(new Color(1f, 1f, 1f, 1f), 1f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get moon color.
	public Color MoonColor{get{return moonColor.Evaluate(CGCycle);}}


	// Moon texture size.
	public AnimationCurve moonSize =  AnimationCurve.Linear (0, .14f, 1f, .14f); 

	// Get moon texture size.
	public float MoonSize{get{return moonSize.Evaluate(CGCycle);}}

	// Moon texture instensity.
	public AnimationCurve moonIntensity = new AnimationCurve()
	{
		keys = new Keyframe[]
		{
			new Keyframe(  0f, 1f),  new Keyframe(.15f, 1f), 
			new Keyframe(.30f, 0f),  new Keyframe(.70f, 0f), 
			new Keyframe(.85f, 1f),  new Keyframe(  1f, 1f), 
		}

	}; 

	// Get moon texture intensity.
	public float MoonIntensity{get{return moonIntensity.Evaluate(CGCycle);}}

	public bool useMoonHalo = true;

	public Gradient moonHaloColor = new Gradient()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(.145f, .243f, .318f, 1f), .20f),
			new GradientColorKey(new Color(.145f, .243f, .318f, 1f), .80f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, .20f),
			new GradientAlphaKey(1f, .70f)
		}
	};

	// Get moon halo color.
	public Color MoonHaloColor{get{return moonHaloColor.Evaluate(CGCycle);}}

	public AnimationCurve moonHaloPower =  AnimationCurve.Linear (0, 120f, 1f, 120f); 

	// Get moon halo power.
	public float  MoonHaloPower{get{return  moonHaloPower.Evaluate(CGCycle);}}

	public AnimationCurve moonHaloIntensity  = new AnimationCurve()
	{
		keys = new Keyframe[]
		{
			new Keyframe(  0f, 1f),  new Keyframe(.20f, 1f), 
			new Keyframe(.25f, 0f),  new Keyframe(.75f,  0f), 
			new Keyframe(.80f, 1f),  new Keyframe(  1f,  1f), 
		}
	};  

	// Get moon halo intensity.
	public float  MoonHaloIntensity{get{return moonHaloIntensity.Evaluate(CGCycle);}}

	// Rotate in the opposite direction to the sun.
	public bool autoRotateMoon = false;

	[Range(-180f,180f)]
	public float moonLongitude = 40f;

	[Range(-180f, 180f)]
	public float moonLatitude = 12f;

	// Moon direction.
	private Vector3 MoonDirection{get{return -m_Moon.forward;}}
	//=============================================================================================
	#endregion


	#region stars
	// Use stars?
	[SerializeField]
	private bool    m_UseStars = true;

	public Gradient starsColor = new Gradient()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(1f, 1f, 1f, 1f), .20f),
			new GradientColorKey(new Color(1f, 1f, 1f, 1f), .80f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get stars color.
	public Color StarsColor{get{return starsColor.Evaluate(CGCycle);}}

	public AnimationCurve starsIntensity = new AnimationCurve()
	{
		keys = new Keyframe[]
		{
			new Keyframe(  0f, 1f),  new Keyframe(.20f, 1f), 
			new Keyframe(.30f, 0f),  new Keyframe(.70f, 0f), 
			new Keyframe(.85f, 1f),  new Keyframe(  1f, 1f), 
		}
	};  

	// Get stars intensity.
	public float StarsIntensity{get{return starsIntensity.Evaluate(CGCycle);}}


	public AnimationCurve starsTwinkleSpeed =  AnimationCurve.Linear (0, .5f, 1f, .5f); 

	// Get stars twinkle speed.
	public float StarsTwinkleSpeed{get{return starsTwinkleSpeed.Evaluate(CGCycle);}}
	private float starsTwinkleSpeedRot;


	public AnimationCurve starsTwinkle =  AnimationCurve.Linear (0, 1f, 1f, 1f); 

	// Get stars twinkle .
	public float StarsTwinkle{get{return starsTwinkle.Evaluate(CGCycle);}}


	// Rotate stars?
	public bool autoRotateStars = true;

	// Stars offset.
	public Vector3 starsOffset;
	//=============================================================================================
	#endregion


	#region Ambient
	// Ambient.
	private enum AmbientMode{color, gradient, sky}

	// Ambient source.
	[SerializeField]
	private AmbientMode m_AmbientMode = AmbientMode.sky;

	// Gradients of ambient.
	public Gradient ambientSkyColor = new Gradient () 
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(.110f, .125f, .157f, 1f), .22f),
			new GradientColorKey(new Color(.435f, .494f, .498f, 1f), .25f),
			new GradientColorKey(new Color(.463f, .576f, .769f, 1f), .30f),
			new GradientColorKey(new Color(.463f, .576f, .769f, 1f), .70f),
			new GradientColorKey(new Color(.435f, .494f, .498f, 1f), .75f),
			new GradientColorKey(new Color(.110f, .125f, .157f, 1f), .78f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, .20f),
			new GradientAlphaKey(1f, .70f)
		}
	};

	// Get ambient sky color.
	public Color AmbientSkyColor{get{return ambientSkyColor.Evaluate(CGCycle);}}

	public Gradient ambientEquatorColor = new Gradient ()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(.110f, .125f, .157f, 1f), .22f),
			new GradientColorKey(new Color(.859f, .780f, .561f, 1f), .25f),
			new GradientColorKey(new Color(.698f, .843f,    1f, 1f), .30f),
			new GradientColorKey(new Color(.698f, .843f,    1f, 1f), .70f),
			new GradientColorKey(new Color(.859f, .780f, .561f, 1f), .75f),
			new GradientColorKey(new Color(.110f, .125f, .157f, 1f), .78f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get ambient equator color.
	public Color AmbientEquatorColor{get{return ambientEquatorColor.Evaluate(CGCycle);}}

	public Gradient ambientGroundColor  = new Gradient ()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(0f, 0f, 0f, 1f), .22f),
			new GradientColorKey(new Color(.227f, .157f, .102f, 1f), .25f),
			new GradientColorKey(new Color(.467f, .435f, .416f, 1f), .30f),
			new GradientColorKey(new Color(.467f, .435f, .416f, 1f), .70f),
			new GradientColorKey(new Color(.227f, .157f, .102f, 1f), .75f),
			new GradientColorKey(new Color(0f, 0f, 0f, 1f), .78f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get ground color.
	public Color AmbientGroundColor{get{return ambientGroundColor.Evaluate(CGCycle);}}

	public AnimationCurve ambientIntensity = AnimationCurve.Linear (0, 1f, 1f, 1f);  

	// Get ambient intensity.
	public float   AmbientIntensity{get{return ambientIntensity.Evaluate (CGCycle);}}
	//=============================================================================================
	#endregion


	#region Fog

	public bool useFog = false;

	public FogMode fogMode = FogMode.ExponentialSquared;

	public AnimationCurve fogDensity = AnimationCurve.Linear (0, 0.0016f, 1f, 0.0016f);  

	// Get fog density.
	public float   FogDensity{get{return fogDensity.Evaluate (CGCycle);}}

	// Use to linear mode.
	public AnimationCurve fogStartDistance = AnimationCurve.Linear (0, 0f, 1f, 0f); 

	// Get fog start distance.
	public float   FogStartDistance{ get { return fogStartDistance.Evaluate (CGCycle); } }

	public AnimationCurve fogEndDistance = AnimationCurve.Linear (0, 300f, 1f, 300f); 

	// Get fog end distance.
	public float   FogEndDistance{ get { return fogEndDistance.Evaluate (CGCycle); } }

	public Gradient fogColor  = new Gradient ()
	{
		colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(.129f, .212f, .271f, 1f), .22f),
			new GradientColorKey(new Color(.682f, .655f, .584f, 1f), .25f),
			new GradientColorKey(new Color(.576f, .706f, .878f, 1f), .30f),
			new GradientColorKey(new Color(.576f, .706f, .878f, 1f), .70f),
			new GradientColorKey(new Color(.682f, .655f, .584f, 1f), .75f),
			new GradientColorKey(new Color(.129f, .212f, .271f, 1f), .78f)
		},
		alphaKeys = new GradientAlphaKey[] 
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		}
	};

	// Get fog color.
	public Color FogColor{get {return fogColor.Evaluate(CGCycle);}}
	//=============================================================================================
	#endregion



	void Update()
	{

		if (m_SkyMaterial == null || m_Sun == null) return;


		// Prevent the current time exceeds the day duration.
		#if UNITY_EDITOR
		if(!Application.isPlaying)
			currentTime = Mathf.Clamp (currentTime, 0 - .0001f, k_DayDuration + .0001f);
		#endif

		if (currentTime > k_DayDuration) 
			currentTime = 0;

		if (currentTime < 0) currentTime = 
			k_DayDuration;

		// Play time of day.
		if (playTime && Application.isPlaying) 
		{
			m_TimeCounter = Time.deltaTime / dayInSeconds;
			currentTime += m_TimeCounter * k_DayDuration;
		}


		// Auto assign sky material.
		if (m_AutoAssignSky)
			RenderSettings.skybox = m_SkyMaterial;


		m_Sun.localEulerAngles = new Vector3 (SunCycle, longitude, 0);

		m_SkyMaterial.SetVector("_SunDir", SunDirection);
		m_SkyMaterial.SetFloat("_SunSize", SunSize);
		m_SkyMaterial.SetColor("_SunColor", SunColor);


		if (m_SunLight != null) 
		{

			//if (m_SunLight.type != LightType.Directional)
			//	m_SunLight.type = LightType.Directional;

			m_SunLight.intensity = SunLightIntensity;
			m_SunLight.color = SunColor;

			if (currentTime <= 5.50f || currentTime >= 18.50f)
				m_SunLight.enabled = false;
			else
				m_SunLight.enabled = true;

		}


		if (m_Moon != null) 
		{

			if (useMoon) 
			{

				if (!autoRotateMoon) 
				{
					m_Moon.localEulerAngles = new Vector3 (moonLatitude, moonLongitude, 0f);
					m_Moon.localScale = new Vector3 (-1, 1, 1);
					m_Moon.parent = this.transform;
				} 
				else 
				{
					m_Moon.parent = m_Sun;
					m_Moon.localRotation = Quaternion.Euler (0, 180f, -180f);
					m_Moon.localScale = new Vector3 (-1, 1, 1);

					//m_Moon.forward = SunDirection;
				}

				//m_Moon.transform.localRotation = new Quaternion (m_Moon.localRotation.x, 0, m_Moon.localRotation.z, m_Moon.localRotation.w);

				if (m_MoonLight != null) 
				{

					m_MoonLight.color = MoonLightColor;
					m_MoonLight.intensity = MoonLightIntensity;

					if (currentTime < 5.50f || currentTime > 18.50f)
						m_MoonLight.enabled = true;
					else
						m_MoonLight.enabled = false;
				}

				m_SkyMaterial.EnableKeyword ("USEMOON");

				if (moonTexture != null)
				{
					m_SkyMaterial.SetMatrix ("_MoonMatrix", m_Moon.worldToLocalMatrix);
					m_SkyMaterial.SetTexture ("_MoonTexture", moonTexture);
					m_SkyMaterial.SetFloat ("_MoonSize", MoonSize);
					m_SkyMaterial.SetColor ("_MoonColor", MoonColor);
					m_SkyMaterial.SetFloat ("_MoonIntensity", MoonIntensity);
				} 
				else 
				{
					m_SkyMaterial.DisableKeyword ("USEMOON");
				}

				m_SkyMaterial.SetVector ("_MoonDir", MoonDirection);


				if (useMoonHalo) 
				{
					m_SkyMaterial.EnableKeyword ("USEMOONHALO");
					m_SkyMaterial.SetFloat ("_MoonHaloPower", MoonHaloPower);
					m_SkyMaterial.SetColor ("_MoonHaloColor", MoonHaloColor);
					m_SkyMaterial.SetFloat ("_MoonHaloIntensity", MoonHaloIntensity);
				} 
				else
				{
					m_SkyMaterial.DisableKeyword ("USEMOONHALO");
				}
			} 
			else 
			{
				m_MoonLight.enabled = false;
				m_SkyMaterial.DisableKeyword("USEMOON");
				m_SkyMaterial.DisableKeyword("USEMOONHALO");
			}
		}
		else 
		{
			m_SkyMaterial.DisableKeyword("USEMOON");
			m_SkyMaterial.DisableKeyword("USEMOONHALO");
		}

		// Stars.
		if ((starsCubemap != null && starsNoiseCubemap != null) && m_UseStars) 
		{

			m_SkyMaterial.EnableKeyword("USESTARS");
			m_SkyMaterial.SetTexture("_StarsCubemap", starsCubemap);
			m_SkyMaterial.SetColor("_StarsColor", StarsColor);

			// Used to rotate stars. 
			Matrix4x4 sunMatrix = autoRotateStars ? m_Sun.worldToLocalMatrix:Matrix4x4.identity;
			m_SkyMaterial.SetMatrix("_SunMatrix", sunMatrix);

			Matrix4x4 starsMatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler(starsOffset), Vector3.one);


			starsTwinkleSpeedRot +=	Time.deltaTime * StarsTwinkleSpeed;

			Matrix4x4 starsNoiseMatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (starsTwinkleSpeedRot, 0, 0), Vector3.one);

			m_SkyMaterial.SetMatrix("_StarsMatrix", starsMatrix);
			m_SkyMaterial.SetFloat("_StarsIntensity",StarsIntensity);
			m_SkyMaterial.SetTexture("_StarsNoiseCubemap", starsNoiseCubemap);
			m_SkyMaterial.SetMatrix("_StarsNoiseMatrix", starsNoiseMatrix);
			m_SkyMaterial.SetFloat("_StarsTwinkle", StarsTwinkle);
		}
		else 
		{
			m_SkyMaterial.DisableKeyword("USESTARS");
		}


		// HDR Exposure.
		m_SkyMaterial.SetFloat("_Exposure", exposure);

		// Sky.
		m_SkyMaterial.SetColor("_SkyTint",              SkyTint);
		m_SkyMaterial.SetFloat("_AtmosphereThickness",  AtmoshphereThickness);
		m_SkyMaterial.SetColor("_GroundColor",          groundColor);

		// NightSky.
		m_SkyMaterial.SetColor("_NightHorizonColor",    NightHorizonColor);
		m_SkyMaterial.SetFloat("_NightHorizonExponent", NightHorizonExponent);

		UpdateAmbientColor ();
		UpdateFog ();
	}

	void UpdateAmbientColor()
	{

		switch (m_AmbientMode) 
		{

			case AmbientMode.sky: 
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
			RenderSettings.ambientIntensity = AmbientIntensity;
			break;

			case AmbientMode.color: 
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
			RenderSettings.ambientSkyColor = AmbientSkyColor;
			break;

			case AmbientMode.gradient:
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
			RenderSettings.ambientSkyColor     =  AmbientSkyColor; 
			RenderSettings.ambientEquatorColor =  AmbientEquatorColor;
			RenderSettings.ambientGroundColor  =  AmbientGroundColor;
			break;
		}
	}

	// Render settings fog.
	void UpdateFog()
	{

		RenderSettings.fog = useFog;

		if(!useFog) return;

		RenderSettings.fogMode  = fogMode;
		RenderSettings.fogColor = FogColor;

		switch (fogMode) 
		{
			case FogMode.Exponential:
			RenderSettings.fogDensity = FogDensity;
			break;

			case FogMode.ExponentialSquared :
			RenderSettings.fogDensity = FogDensity;
			break;

			case FogMode.Linear:
			RenderSettings.fogStartDistance = FogStartDistance;
			RenderSettings.fogEndDistance = FogEndDistance;
			break;
		}
	}


}
