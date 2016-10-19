using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour {
	public Color dayColor; //FFEAB2FF
	public Color twilightColor; //FFB2B2FF;
	public Color nightColor; //0A2C65FF;

	public float dayBrightness, nightBrightness;

	public Material dayBox;
	public Material nightBox;
	public Material transitionBox;

	public float transitionPercent;
	public float dayTime;
	public float currentTime;
	public string time;

	private float transitionAmount;
	private float sunriseStart, sunriseMid, sunriseEnd, sunsetStart, sunsetMid, sunsetEnd;

	private Vector3 rotation;

	private Light light;

	// Use this for initialization
	void Start () {
		light = transform.GetComponent<Light> ();
		transitionAmount = transitionPercent * dayTime;

		sunriseMid = dayTime;
		sunriseStart = dayTime - transitionAmount;
		sunriseEnd = transitionAmount;
		sunsetMid = dayTime / 2f;
		sunsetStart = sunsetMid - transitionAmount;
		sunsetEnd = sunsetMid + transitionAmount;

		rotation = transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		if (currentTime > dayTime)
			currentTime -= dayTime;
//
		UpdateLight ();
//
		RotateLight ();
//
		UpdateSkybox ();

//		transform.RotateAround (Vector3.zero, Vector3.right, 40f * Time.deltaTime);
//		transform.LookAt (Vector3.zero);
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

	void RotateLight(){
		rotation.x = 360f * (currentTime / dayTime);
//		if (rotation.x > 200f) {
//			rotation.x += 160f;
		if (rotation.x > 180) {
			rotation.x -= 180;
//			rotation.x *= -1;
//			rotation.x = 270;
		}
		transform.rotation = Quaternion.Euler (rotation);
	}

	void UpdateLight(){
		if (currentTime > sunriseEnd && currentTime < sunsetStart) {
			light.color = dayColor;
			light.intensity = dayBrightness;
		}

		else if (currentTime > sunsetEnd && currentTime < sunriseStart) {
			light.color = nightColor;
			light.intensity = nightBrightness;
		}

		else if(currentTime <= sunriseEnd){
			float colorAmount = currentTime / transitionAmount;
			light.color = Color.Lerp (twilightColor, dayColor, colorAmount);
			time = "sunrise ending";
			light.intensity = colorAmount * dayBrightness;
		}

		else if(currentTime >= sunsetStart && currentTime <= sunsetMid){
			float colorAmount = (currentTime - sunsetStart) / transitionAmount;
			light.color = Color.Lerp (dayColor, twilightColor, colorAmount);
			time = "sunset starting";
			light.intensity = (1f-colorAmount) * dayBrightness;
		}

		else if(currentTime >= sunsetMid && currentTime <= sunsetEnd){
			float colorAmount = (currentTime - sunsetMid) / transitionAmount;
			light.color = Color.Lerp (twilightColor, nightColor, colorAmount);
			time = "sunset ending";
			light.intensity = colorAmount * nightBrightness;
		}

		else if(currentTime >= sunriseStart && currentTime <= sunriseMid){
			float colorAmount = (currentTime - sunriseStart) / transitionAmount;
			light.color = Color.Lerp (nightColor, twilightColor, colorAmount);
			time = "sunrise starting";
			light.intensity = (1f-colorAmount) * nightBrightness;
		}
	}
}
