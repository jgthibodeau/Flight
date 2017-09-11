using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Quality : MonoBehaviour {
	public Dropdown qualityDropdown;
	public Toggle reflectionToggle;
	public Toggle fogToggle;
	public Slider drawDistanceSlider;
	public Slider foliageDistanceSlider;
	public Slider grassDensitySlider;

	public CameraQuality cameraQuality;
	public Material waterMaterial;

	public GameObject parentTerrain;

	void Awake(){
		qualityDropdown.ClearOptions ();
		qualityDropdown.AddOptions (QualitySettings.names.ToList ());
		qualityDropdown.value = QualitySettings.GetQualityLevel ();

		reflectionToggle.isOn = waterMaterial.GetFloat ("_EnableReflections") == 1;
		fogToggle.isOn = cameraQuality.GetFog ();
		drawDistanceSlider.value = cameraQuality.GetDrawDistance ();

		foliageDistanceSlider.value = parentTerrain.GetComponentInChildren<Terrain> ().detailObjectDistance;
		grassDensitySlider.value = parentTerrain.GetComponentInChildren<Terrain> ().detailObjectDensity;
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
		Terrain[] terrains = parentTerrain.GetComponentsInChildren<Terrain> ();
		foreach (Terrain terrain in terrains) {
			terrain.detailObjectDistance = newFoliageDistance;
		}
//		PlayerPrefs.SetFloat (FOLIAGE_DISTANCE, newFoliageDistance);
	}

	public void SetGrassDensity (float newGrassDensity) {
		Terrain[] terrains = parentTerrain.GetComponentsInChildren<Terrain> ();
		foreach (Terrain terrain in terrains) {
			terrain.detailObjectDensity = newGrassDensity;
		}
//		PlayerPrefs.SetFloat (GRASS_DENSITY, newGrassDensity);
	}
}
