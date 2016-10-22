using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMng : MonoBehaviour 
{


	public TimeOfDayManager mng;

	public GameObject UIObject;

	public Slider longitude;

	public Toggle playTime;

	public Slider timeInSeconds;

	public Slider timeLine;


	bool enableUI = true;


	void Start()
	{

		longitude.value     = mng.longitude;
		playTime.isOn       = mng.playTime;
		timeLine.value      = mng.currentTime;
		timeInSeconds.value = mng.dayInSeconds;
	}



	void Update()
	{

		if (Input.GetKeyDown (KeyCode.G))
			enableUI = !enableUI;

		UIObject.SetActive (enableUI);

		mng.longitude = longitude.value;
		mng.playTime = playTime.isOn;

		if (!mng.playTime) 
		{
			mng.currentTime = timeLine.value;
		} 
		else 
		{

			mng.dayInSeconds = timeInSeconds.value;
			timeLine.value =	mng.currentTime;
		}
			


	}



}
