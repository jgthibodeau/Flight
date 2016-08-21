using UnityEngine;
using System.Collections;

public class Oscillation : MonoBehaviour 
{
	GameObject go;

	void Start () 
	{
		//Find parent of sound sources
		go = GameObject.Find ("Sound Sources");
	}
	
	void Update () 
	{
		//Oscillate the parent up and down through the y axis
		go.transform.position = new Vector3(0f, (Mathf.Cos (Time.time) * 2f) - 2f, 0f);
	}
}
