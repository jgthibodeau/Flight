using UnityEngine;
using System.Collections;
 
public class UnderwaterCamera : MonoBehaviour {
 
	//This script enables underwater effects. Attach to main camera.
 
	private bool isUnderwater;
	public bool setHeightFog;
	public bool setFog;
	public float underwaterFogDensity = 0.03f;

	public Material waterMaterial;
	private UnityStandardAssets.ImageEffects.GlobalFog globalFog;

	public GameObject underwaterFilter;

	void Start () {
		globalFog = GetComponent<UnityStandardAssets.ImageEffects.GlobalFog> ();
    }
 
    void LateUpdate () {
		isUnderwater = transform.position.y < Util.GetWaterLevel (transform.position, false, false);
		if (isUnderwater) {
			SetUnderwater ();
		}
		else {
			SetNormal ();
		}
    }

	private void SetNormal(){
		if (setHeightFog) {
			globalFog.heightFog = false;
		}

		if (setFog) {
			globalFog.excludeFarPixels = true;
		}

		underwaterFilter.SetActive (false);
		waterMaterial.SetFloat ("_UnderwaterMode", 0);
	}

	private void SetUnderwater(){
		if (setHeightFog) {
			globalFog.heightFog = true;
			globalFog.height = 350;
			globalFog.heightDensity = 10f;
		}

		if (setFog) {
			globalFog.excludeFarPixels = false;

			RenderSettings.fogColor = RenderSettings.ambientSkyColor * waterMaterial.GetColor ("_Color");
			;
			RenderSettings.fogDensity = underwaterFogDensity;
		}

		//underwaterFilter.SetActive (true);
		waterMaterial.SetFloat ("_UnderwaterMode", 1);
	}
}