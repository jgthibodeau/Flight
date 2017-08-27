using UnityEngine;
using System.Collections;
 
public class UnderwaterCamera : MonoBehaviour {
 
	//This script enables underwater effects. Attach to main camera.
 
    private bool isUnderwater;
	public float underwaterFogDensity = 0.03f;
	public bool setHeightFog;

	public Material waterMaterial;
	private UnityStandardAssets.ImageEffects.GlobalFog globalFog;

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

		globalFog.excludeFarPixels = true;
	}

	private void SetUnderwater(){
		if (setHeightFog) {
			globalFog.heightFog = true;
			globalFog.height = 350;
			globalFog.heightDensity = 10f;
		}

		globalFog.excludeFarPixels = false;

		RenderSettings.fogColor = RenderSettings.ambientSkyColor * waterMaterial.GetColor ("_Color");;
		RenderSettings.fogDensity = underwaterFogDensity;
	}
}