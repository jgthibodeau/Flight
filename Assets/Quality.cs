using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Quality : MonoBehaviour {
	public Dropdown qualityDropdown;
	public CameraQuality cameraQuality;
	public Material waterMaterial;

	public int minFoliageDistance;
	public int maxFoliageDistance;
	public int minFoliageDensity;
	public int maxFoliageDensity;
	public GameObject parentTerrain;

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
		Terrain[] terrains = parentTerrain.GetComponentsInChildren<Terrain> ();
		foreach (Terrain terrain in terrains) {
			terrain.detailObjectDensity = newFoliageDistance;
		}
//		PlayerPrefs.SetFloat (FOLIAGE_DISTANCE, newFoliageDistance);
	}

	public void SetGrassDensity (float newGrassDensity) {
		Terrain[] terrains = parentTerrain.GetComponentsInChildren<Terrain> ();
		foreach (Terrain terrain in terrains) {
			terrain.detailObjectDistance = newGrassDensity;
		}
//		PlayerPrefs.SetFloat (GRASS_DENSITY, newGrassDensity);
	}
}
