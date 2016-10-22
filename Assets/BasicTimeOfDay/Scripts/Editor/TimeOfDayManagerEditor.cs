using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TimeOfDayManager))] 
public class TimeOfDayManagerEditor : Editor
{


	SerializedObject   sObj; 
	//TimeOfDayManager m_Target; 

	// Resources.
	//========================================
	SerializedProperty autoAssignSky;
	SerializedProperty skyMaterial;
	SerializedProperty sun;
	SerializedProperty sunLight;
	SerializedProperty moon;
	SerializedProperty moonLight;
	SerializedProperty moonTexture;
	SerializedProperty starsCubemap;
	SerializedProperty starsNoiseCubemap;


	// Time Of Day.
	//========================================
	SerializedProperty longitude;
	SerializedProperty playTime;
	SerializedProperty dayInSeconds;
	SerializedProperty currentTime;


	// Sky.
	//========================================
	SerializedProperty skyTint;
	SerializedProperty atmoshphereThickness;
	SerializedProperty groundColor;
	SerializedProperty nightHorizonColor;
	SerializedProperty nightHorizonExponent;
	SerializedProperty exposure;

	// Sun.
	//========================================
	SerializedProperty sunSize;
	SerializedProperty sunLightIntensity;
	SerializedProperty sunColor;


	// Moon.
	//========================================
	SerializedProperty useMoon;
	SerializedProperty moonLightColor;
	SerializedProperty moonLightIntensity;
	SerializedProperty moonColor;
	SerializedProperty moonSize;
	SerializedProperty moonIntensity;
	SerializedProperty useMoonHalo;
	SerializedProperty moonHaloPower;
	SerializedProperty moonHaloColor;
	SerializedProperty moonHaloIntensity;
	SerializedProperty autoRotateMoon;
	SerializedProperty moonLongitude;
	SerializedProperty moonLatitude;

	// Stars.
	//========================================
	SerializedProperty useStars;
	SerializedProperty starsColor;
	SerializedProperty starsIntensity;
	SerializedProperty starsTwinkle;
	SerializedProperty starsTwinkleSpeed;
	SerializedProperty autoRotateStars;
	SerializedProperty starsOffset;


	// Ambient.
	//========================================
	SerializedProperty ambientMode;
	SerializedProperty ambientSkyColor;
	SerializedProperty ambientEquatorColor;
	SerializedProperty ambientGroundColor;
	SerializedProperty ambientIntensity;


	// Fog.
	//========================================
	SerializedProperty useFog;
	SerializedProperty fogMode;
	SerializedProperty fogDensity;
	SerializedProperty fogStartDistance;
	SerializedProperty fogEndDistance;
	SerializedProperty fogColor;


	// Fodouts.
	//========================================

	bool m_ResourcesFoldout;
	bool m_TimeOfDayFoldout;
	bool m_SkyFoldout;
	bool m_SunFoldout;
	bool m_MoonFoldout;
	bool m_StarsFoldout;
	bool m_AmbientFoldout;
	bool m_FogFoldout;
	bool m_OtherSettingsFoldout;

	void OnEnable()
	{
		sObj = new SerializedObject (target);
		//m_Target = (TimeOfDayManager)target;

		// Resources.
		//==========================================================================================
		autoAssignSky     = sObj.FindProperty ("m_AutoAssignSky");
		skyMaterial       = sObj.FindProperty ("m_SkyMaterial");
		sun               = sObj.FindProperty ("m_Sun");
		sunLight          = sObj.FindProperty ("m_SunLight");
		moon              = sObj.FindProperty ("m_Moon");
		moonLight         = sObj.FindProperty ("m_MoonLight");
	    moonTexture       = sObj.FindProperty ("moonTexture");
		starsCubemap      = sObj.FindProperty ("starsCubemap");
		starsNoiseCubemap = sObj.FindProperty ("starsNoiseCubemap");


		// Time of day.
		//==========================================================================================
		longitude         = sObj.FindProperty ("longitude");
		playTime          = sObj.FindProperty ("playTime");
		dayInSeconds      = sObj.FindProperty ("dayInSeconds");
		currentTime       = sObj.FindProperty ("currentTime");


		// Atmosphere.
		//==========================================================================================
		skyTint              = sObj.FindProperty ("skyTint");
		atmoshphereThickness = sObj.FindProperty ("atmoshphereThickness");
		groundColor          = sObj.FindProperty ("groundColor");
		nightHorizonColor    = sObj.FindProperty ("nightHorizonColor");
		nightHorizonExponent = sObj.FindProperty ("nightHorizonExponent");


		// Sun.
		//==========================================================================================
		sunSize           = sObj.FindProperty ("sunSize");
		sunLightIntensity = sObj.FindProperty ("sunLightIntensity");
		sunColor          = sObj.FindProperty ("sunColor");


		// Moon.
		//==========================================================================================
		useMoon            = sObj.FindProperty("useMoon");
		moonLightColor     = sObj.FindProperty("moonLightColor");
		moonLightIntensity = sObj.FindProperty("moonLightIntensity");
		moonColor          = sObj.FindProperty("moonColor");
		moonSize           = sObj.FindProperty("moonSize");
		moonIntensity      = sObj.FindProperty("moonIntensity");
		useMoonHalo        = sObj.FindProperty("useMoonHalo");
		moonHaloPower      = sObj.FindProperty("moonHaloPower");
		moonHaloColor      = sObj.FindProperty("moonHaloColor");
		moonHaloIntensity  = sObj.FindProperty("moonHaloIntensity");
		autoRotateMoon     = sObj.FindProperty("autoRotateMoon");
		moonLongitude      = sObj.FindProperty("moonLongitude");
		moonLatitude       = sObj.FindProperty("moonLatitude");


		// Stars.
		//===========================================================================================
		useStars  		 = sObj.FindProperty("m_UseStars");
		starsColor       = sObj.FindProperty("starsColor");
		starsIntensity   = sObj.FindProperty("starsIntensity");
		starsTwinkle     = sObj.FindProperty("starsTwinkle");
		starsTwinkleSpeed = sObj.FindProperty("starsTwinkleSpeed");
		autoRotateStars  = sObj.FindProperty("autoRotateStars");
		starsOffset      = sObj.FindProperty("starsOffset");


		// Ambient.
		//===========================================================================================
		ambientMode         = sObj.FindProperty ("m_AmbientMode");
		ambientSkyColor     = sObj.FindProperty ("ambientSkyColor");
		ambientEquatorColor = sObj.FindProperty ("ambientEquatorColor");
		ambientGroundColor  = sObj.FindProperty ("ambientGroundColor");
		ambientIntensity    = sObj.FindProperty ("ambientIntensity");


		// Fog.
		//===========================================================================================
		useFog           = sObj.FindProperty ("useFog");
		fogMode          = sObj.FindProperty ("fogMode");
		fogDensity       = sObj.FindProperty ("fogDensity");
		fogStartDistance = sObj.FindProperty ("fogStartDistance");
		fogEndDistance   = sObj.FindProperty ("fogEndDistance");
		fogColor         = sObj.FindProperty ("fogColor");

		// Other.
		//===========================================================================================
		exposure = sObj.FindProperty ("exposure");

	}


	public override void OnInspectorGUI()
	{

		sObj.Update ();

		HorizontalSeparator (Color.white, 2);
		TexTitle ("Time of Day Manager [Basic]");
		HorizontalSeparator (Color.white, 2);


		// Resources
		//==========================================================================================
		m_ResourcesFoldout = EditorGUILayout.Foldout (m_ResourcesFoldout, "Resources");
		if(m_ResourcesFoldout)
		{

			HorizontalSeparator (Color.white, 2);
			TexTitle ("Resources");
			HorizontalSeparator (Color.white, 2);


			EditorGUILayout.PropertyField(autoAssignSky, new GUIContent("Auto Assign Sky?"));


			EditorGUILayout.PropertyField(skyMaterial, new GUIContent("Sky Material"));
			if (skyMaterial.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox ("Please Assign Sky Material", MessageType.Warning);
			}

			EditorGUILayout.PropertyField(sun, new GUIContent("Sun Transform"));
			if (sun.objectReferenceValue != null) 
			{
				EditorGUILayout.PropertyField(sunLight, new GUIContent("Sun Light"));
			} 
			else
			{
				EditorGUILayout.HelpBox ("Please Assign Sun Transform", MessageType.Warning);
			}

			EditorGUILayout.PropertyField(moon, new GUIContent("Moon Transform"));
			if (moon.objectReferenceValue != null) 
			{
				EditorGUILayout.PropertyField(moonLight, new GUIContent("Moon Light"));
			} 
			else
			{
				EditorGUILayout.HelpBox ("Please Assign Moon Transform", MessageType.Warning);
			}

			EditorGUILayout.PropertyField(moonTexture, new GUIContent("Moon Texture"));
			if (moonTexture.objectReferenceValue == null) 
			{
				EditorGUILayout.HelpBox ("Please Assign Moon Texture", MessageType.Warning);
			}


			EditorGUILayout.PropertyField(starsCubemap, new GUIContent("Stars Cubemap")); 
			if (starsCubemap.objectReferenceValue == null) 
			{
				EditorGUILayout.HelpBox ("Please Assign Stars Cubemap", MessageType.Warning);
			}

			EditorGUILayout.PropertyField(starsNoiseCubemap, new GUIContent("Stars Noise Cubemap")); 
			if (starsNoiseCubemap.objectReferenceValue == null) 
			{
				EditorGUILayout.HelpBox ("Please Assign Stars Noise Cubemap", MessageType.Warning);
			}
		}
		//==========================================================================================


		// Time of day.
		//==========================================================================================
		m_TimeOfDayFoldout = EditorGUILayout.Foldout (m_TimeOfDayFoldout, "Time Of Day");
		if(m_TimeOfDayFoldout)
		{

			HorizontalSeparator (Color.white, 2);
			TexTitle ("Time Of Day");
			HorizontalSeparator (Color.white, 2);

			EditorGUILayout.PropertyField(longitude, new GUIContent("Longitude"));
			EditorGUILayout.Separator ();

			EditorGUILayout.PropertyField(playTime, new GUIContent("Play Time?"));

			if(playTime.boolValue)
				EditorGUILayout.PropertyField(dayInSeconds, new GUIContent("Day In Seconds"));

			EditorGUILayout.PropertyField(currentTime, new GUIContent("Current Time"));
		}
		//==========================================================================================

		// Atmosphere.
		//==========================================================================================
		m_SkyFoldout = EditorGUILayout.Foldout (m_SkyFoldout, "Atmosphere");
		if(m_SkyFoldout)
		{
			HorizontalSeparator (Color.white, 2);
			TexTitle ("Atmosphere");
			HorizontalSeparator (Color.white, 2);
			ColorField (skyTint, "SkyTint",75);
			CurveField ("Atmosphere Thickness", atmoshphereThickness, Color.white, new Rect (0, 0, 1, 7), 75);
			ColorField (groundColor, "Ground Color",75);
			EditorGUILayout.Separator ();

			ColorField (nightHorizonColor, "Night Horizon Color",75);
			CurveField ("Night Horizon Exponent", nightHorizonExponent, Color.white, new Rect (0, 0, 1, 30), 75);
			EditorGUILayout.Separator ();

		}
		//==========================================================================================

		// Sun.
		//==========================================================================================
		m_SunFoldout = EditorGUILayout.Foldout (m_SunFoldout, "Sun");
		if(m_SunFoldout)
		{
			HorizontalSeparator (Color.white, 2);
			TexTitle ("Sun");
			HorizontalSeparator (Color.white, 2);

			CurveField ("Sun Size", sunSize, Color.white, new Rect (0, 0, 1, 0.3f), 75);
			CurveField ("Sun Light Intensity", sunLightIntensity, Color.white, new Rect (0, 0, 1, 8), 75);
			ColorField (sunColor, "Sun Color",75);
		}
		//==========================================================================================

		// Moon.
		//==========================================================================================
		m_MoonFoldout = EditorGUILayout.Foldout (m_MoonFoldout, "Moon");
		if (m_MoonFoldout)
		{
			HorizontalSeparator (Color.white, 2);
			TexTitle ("Moon");
			HorizontalSeparator (Color.white, 2);

			EditorGUILayout.PropertyField (useMoon, new GUIContent ("Use Moon"));

			if (useMoon.boolValue)
			{

				ColorField (moonLightColor, "Moon Light Color", 75);
				CurveField ("Moon Light Intensity", moonLightIntensity, Color.white, new Rect (0, 0, 1, 2), 75);
				EditorGUILayout.Separator ();

				ColorField (moonColor, "Moon Color", 75);
				CurveField ("Moon Size", moonSize, Color.white, new Rect (0, 0f, 1, 1.5f), 75);
				CurveField ("Moon Intensity", moonIntensity, Color.white, new Rect (0, 0, 1, 2f), 75);
				EditorGUILayout.Separator ();

				EditorGUILayout.PropertyField (useMoonHalo, new GUIContent ("use Moon Halo?"));

				if (useMoonHalo.boolValue) 
				{
					ColorField (moonHaloColor, "Moon Halo Color", 75);
					CurveField ("Moon Halo Power", moonHaloPower, Color.white, new Rect (0, 0, 1, 300f), 75);
					CurveField ("Moon Halo Intensity", moonHaloIntensity, Color.white, new Rect (0, 0, 1, 5f), 75);
				}
				EditorGUILayout.Separator ();

				EditorGUILayout.PropertyField (autoRotateMoon, new GUIContent ("Auto Rotate Moon?"));

				if (!autoRotateMoon.boolValue) 
				{
					EditorGUILayout.PropertyField (moonLongitude, new GUIContent ("Moon Longitude"));
					EditorGUILayout.PropertyField (moonLatitude, new GUIContent ("Moon Latitude"));
				}
			}
		}
		//==========================================================================================

		// Stars.
		//===========================================================================================
		m_StarsFoldout = EditorGUILayout.Foldout (m_StarsFoldout , "Stars");
		if (m_StarsFoldout) 
		{
			
			HorizontalSeparator (Color.white, 2);
			TexTitle ("Stars");
			HorizontalSeparator (Color.white, 2);

			EditorGUILayout.PropertyField (useStars, new GUIContent ("Use Stars"));

			if (useStars.boolValue) 
			{

				ColorField (starsColor, "Stars Color", 75);
				CurveField ("Stars Intensity", starsIntensity, Color.white, new Rect (0, 0, 1, 5), 75);
				EditorGUILayout.Separator ();

				CurveField ("Stars Twinkle", starsTwinkle, Color.white, new Rect (0, 0, 1, 1), 75); 
				CurveField ("Stars Twinkle Speed", starsTwinkleSpeed, Color.white, new Rect (0, 0, 1, 2), 75);

				EditorGUILayout.PropertyField (autoRotateStars, new GUIContent ("Auto Rotate Stars?"));
				EditorGUILayout.PropertyField (starsOffset, new GUIContent ("Stars Offset"));
			}
		}
		//===========================================================================================

		// Ambient.
		//===========================================================================================
		m_AmbientFoldout = EditorGUILayout.Foldout (m_AmbientFoldout , "Ambient");
		if (m_AmbientFoldout) 
		{
			HorizontalSeparator (Color.white, 2);
			TexTitle ("Ambient");
			HorizontalSeparator (Color.white, 2);

			EditorGUILayout.PropertyField (ambientMode, new GUIContent ("Ambient Mode"));

			string ambientColorName = (ambientMode.enumValueIndex == 0) ? "Ambient Color" : "Sky Color";

			if (ambientMode.enumValueIndex != 2) 
			{
				ColorField (ambientSkyColor, ambientColorName, 75);

			} 
			else 
			{
				CurveField ("Ambient Intensity", ambientIntensity, Color.white, new Rect (0, 0, 1, 8), 75);
			}

			if (ambientMode.enumValueIndex == 1) 
			{

				ColorField (ambientEquatorColor, "Ambient Equator Color", 75);
				ColorField (ambientGroundColor, "Ambient Ground Color", 75);
			}
		}
		//===========================================================================================


		// Fog.
		//===========================================================================================
		m_FogFoldout = EditorGUILayout.Foldout (m_FogFoldout , "Fog");
		if (m_FogFoldout) 
		{
			HorizontalSeparator (Color.white, 2);
			TexTitle ("Fog");
			HorizontalSeparator (Color.white, 2);

			EditorGUILayout.PropertyField (useFog, new GUIContent ("Use Fog"));

			if (useFog.boolValue)
			{

				EditorGUILayout.PropertyField (fogMode, new GUIContent ("Fog Mode"));

				if (fogMode.enumValueIndex == 0) 
				{
					CurveField ("Fog Start Distance", fogStartDistance, Color.white, new Rect (0, 0, 1, 500), 75);
					CurveField ("Fog End Distance", fogEndDistance, Color.white, new Rect (0, 0, 1, 500), 75);
				} 
				else
				{
					CurveField ("Fog Density", fogDensity, Color.white, new Rect (0, 0, 1, 1), 75);
				}

				ColorField (fogColor, "Fog Color", 75);
			}
		}
		//===========================================================================================



		// Other Settings.
		//==========================================================================================
		m_OtherSettingsFoldout = EditorGUILayout.Foldout (m_OtherSettingsFoldout, "Other Settings");
		if(m_OtherSettingsFoldout)
		{
			HorizontalSeparator (Color.white, 2);
			TexTitle ("Other Settings");
			HorizontalSeparator (Color.white, 2);
			EditorGUILayout.PropertyField(exposure, new GUIContent("Exposure"));
		}
		//==========================================================================================

		sObj.ApplyModifiedProperties ();
	}




	void HorizontalSeparator(Color color, int height)
	{
		GUI.color = color;
		GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(height)});
		GUI.color = Color.white;
	}


	void TexTitle(string text)
	{

		GUIStyle texStyle = new GUIStyle(EditorStyles.label); 
		texStyle.fontStyle = FontStyle.Bold;
		texStyle.fontSize = 12;

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(text, texStyle);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		GUI.backgroundColor =  Color.white;
	}


	void ColorField(SerializedProperty color, string name, int width)
	{
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (name);
		EditorGUILayout.PropertyField(color, new GUIContent(""), GUILayout.Width(width));
		EditorGUILayout.EndHorizontal ();
	}


	void CurveField(string name,  SerializedProperty curve, Color color, Rect rect, int width)
	{
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (name);
		curve.animationCurveValue = EditorGUILayout.CurveField ("", curve.animationCurveValue, color , rect, GUILayout.Width(width));
		EditorGUILayout.EndHorizontal ();
	}

}
