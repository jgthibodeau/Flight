using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraQuality : MonoBehaviour {
	private Camera[] cameras;
	public AC.LSky.LSky lSky;
	public float minFarClip, maxFarClip;
	public float minFog, maxFog;

	// Use this for initialization
	void Start () {
		cameras = GetComponentsInChildren<Camera> ();
	}
	
	public void SetDrawDistance(float percent){
		float newFarClip = minFarClip + (maxFarClip - minFarClip) * percent;
		foreach (Camera camera in cameras) {
			camera.farClipPlane = newFarClip;
		}

		float newFog = minFog - (minFog - maxFog) * percent;
		lSky.unityFogDensity.inputValue = newFog;
	}

	public void SetFog(bool fog){
		lSky.enableUnityFog = fog;
	}
}
