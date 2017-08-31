using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Quality : MonoBehaviour {
	public Dropdown qualityDropdown;
	public CameraQuality cameraQuality;
	public Material waterMaterial;

	void Awake(){
		qualityDropdown.ClearOptions ();
		qualityDropdown.AddOptions (QualitySettings.names.ToList ());
		qualityDropdown.value = QualitySettings.GetQualityLevel ();
	}

	public void SetQuality (int newQuality) {
		QualitySettings.SetQualityLevel (newQuality, true);
	}

	public void SetReflections(bool newReflections) {
		float reflectionValue = newReflections ? 1 : 0;
		waterMaterial.SetFloat ("_EnableReflections", reflectionValue);
	}

	public void SetFog(bool newFog) {
		cameraQuality.SetFog(newFog);
	}

	public void SetDrawDistance (float newDrawDistance) {
		cameraQuality.SetDrawDistance (newDrawDistance);
//		PlayerPrefs.SetFloat (DRAW_DISTANCE, newDrawDistance);
	}

	public void SetFoliageDistance (float newFoliageDistance) {
//		PlayerPrefs.SetFloat (FOLIAGE_DISTANCE, newFoliageDistance);
	}

	public void SetGrassDensity (float newGrassDensity) {
//		PlayerPrefs.SetFloat (GRASS_DENSITY, newGrassDensity);
	}
}
