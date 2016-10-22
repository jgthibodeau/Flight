using UnityEngine;
using System.Collections;

public class DayNightCycleWithMoonPhases : MonoBehaviour {
	public Color dayColor; //FFEAB2FF
	public Color twilightColor; //FFB2B2FF;
	public Color nightColor; //0A2C65FF;

	public float dayBrightness, nightBrightness;

	public bool useBlendedSkybox;
	public bool useProceduralSkybox;
	public Material dayBox;
	public Material nightBox;
	public Material transitionBox;

	public float transitionPercent;
	public float dayTime;
	public float currentTime;
	public int currentDay = 1;

	public float monthTime;
	public float currentMonthTime;
	public int currentMonth = 1;

	public float yearTime;
	public float currentYearTime;
	public int currentYear = 1;

	private float transitionAmount;
	private float sunriseStart, sunriseMid, sunriseEnd, sunsetStart, sunsetMid, sunsetEnd;

	private Vector3 rotation;

	public Light sun;
	public Light moon;

	// Use this for initialization
	void Start () {
		transitionAmount = transitionPercent * dayTime;

		sunriseMid = dayTime;
		sunriseStart = dayTime - transitionAmount;
		sunriseEnd = transitionAmount;
		sunsetMid = dayTime / 2f;
		sunsetStart = sunsetMid - transitionAmount;
		sunsetEnd = sunsetMid + transitionAmount;

		rotation = transform.rotation.eulerAngles;

		monthTime = dayTime * 30;

		yearTime = monthTime * 12;
	}

	// Update is called once per frame
	void Update () {
		Vector3 newPosition = Camera.main.transform.position;
		newPosition.y = 0;
		transform.position = newPosition;

		UpdateTime ();

		UpdateLight ();
		
		RotateLight ();

		if(useBlendedSkybox)
			UpdateBlendedSkybox();
		else if(useProceduralSkybox)
			UpdateSkybox ();
	}

	void UpdateTime(){
		currentTime += Time.deltaTime;
		if (currentTime > dayTime) {
			currentTime -= dayTime;
			currentDay++;
			if (currentDay > 30)
				currentDay = 1;
		}

		currentMonthTime += Time.deltaTime;
		if (currentMonthTime > monthTime) {
			currentMonthTime -= monthTime;
			currentMonth++;
			if (currentMonth > 12)
				currentMonth = 1;
		}

		currentYearTime += Time.deltaTime;
		if (currentYearTime > yearTime) {
			currentYearTime -= yearTime;
			currentYear++;
		}
	}

	void UpdateSkybox(){
		if (currentTime >= 0 && currentTime <= sunsetMid) {
			RenderSettings.skybox = dayBox;
		} else if (currentTime >= sunsetMid && currentTime <= sunsetEnd) {
			float lerpAmount = (currentTime - sunsetMid) / transitionAmount;
			RenderSettings.skybox = transitionBox;
			RenderSettings.skybox.CopyPropertiesFromMaterial (dayBox);
			RenderSettings.skybox.Lerp (dayBox, nightBox, lerpAmount);
		} else if (currentTime >= sunriseStart && currentTime <= sunriseMid) {
			float lerpAmount = (currentTime - sunriseStart) / transitionAmount;
			RenderSettings.skybox = transitionBox;
			RenderSettings.skybox.CopyPropertiesFromMaterial (nightBox);
			RenderSettings.skybox.Lerp (nightBox, dayBox, lerpAmount);
		} else if(currentTime >= sunsetEnd && currentTime <= sunriseStart) {
			RenderSettings.skybox = nightBox;
		}
	}

	void UpdateBlendedSkybox(){
		//day
		if (currentTime >= 0 && currentTime <= sunsetMid) {
			RenderSettings.skybox.SetFloat ("_Blend", 0f);
		}
		//sunset
		else if (currentTime >= sunsetMid && currentTime <= sunsetEnd) {
			float lerpAmount = (currentTime - sunsetMid) / transitionAmount;
			RenderSettings.skybox.SetFloat ("_Blend", lerpAmount);
		}
		//sunrise
		else if (currentTime >= sunriseStart && currentTime <= sunriseMid) {
			float lerpAmount = (currentTime - sunriseStart) / transitionAmount;
			RenderSettings.skybox.SetFloat ("_Blend", lerpAmount);
		}
		//night
		else if(currentTime >= sunsetEnd && currentTime <= sunriseStart) {
			RenderSettings.skybox.SetFloat ("_Blend", 1f);
		}
	}

	void RotateLight(){
		rotation.x = 360f * (currentTime / dayTime);
		transform.rotation = Quaternion.Euler (rotation);

		Vector3 newMoonRotation = new Vector3(360f * (currentMonthTime / monthTime), 0, 0);
		moon.transform.localRotation = Quaternion.Euler(newMoonRotation);
	}

	void UpdateLight(){
		//day
		if (currentTime > sunriseEnd && currentTime < sunsetStart) {
			sun.color = dayColor;
			sun.intensity = dayBrightness;
		}

		//night
		else if (currentTime > sunsetEnd && currentTime < sunriseStart) {
			sun.color = nightColor;
			sun.intensity = 0;
		}

		//ending sunrise
		else if(currentTime <= sunriseEnd){
			float colorAmount = currentTime / transitionAmount;
			sun.color = Color.Lerp (twilightColor, dayColor, colorAmount);
			sun.intensity = colorAmount * dayBrightness;
		}

		//starting sunset
		else if(currentTime >= sunsetStart && currentTime <= sunsetMid){
			float colorAmount = (currentTime - sunsetStart) / transitionAmount;
			sun.color = Color.Lerp (dayColor, twilightColor, colorAmount);
			sun.intensity = (1f-colorAmount) * dayBrightness;
		}

		//ending sunset
		else if(currentTime >= sunsetMid && currentTime <= sunsetEnd){
			float colorAmount = (currentTime - sunsetMid) / transitionAmount;
			sun.color = Color.Lerp (twilightColor, nightColor, colorAmount);
			sun.intensity = colorAmount * nightBrightness;
		}

		//starting sunrise
		else if(currentTime >= sunriseStart && currentTime <= sunriseMid){
			float colorAmount = (currentTime - sunriseStart) / transitionAmount;
			sun.color = Color.Lerp (nightColor, twilightColor, colorAmount);
			sun.intensity = (1f-colorAmount) * nightBrightness;
		}
	}
}
