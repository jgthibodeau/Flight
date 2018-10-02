using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

	public bool doReflection;
	public bool doFog;
	[Range(0,1)]
	public float drawDistance;
	[Range(0,250)]
	public float foliageDistance;
	[Range(0,1)]
	public float grassDensity;

	void Start(){
		qualityDropdown.ClearOptions ();
		qualityDropdown.AddOptions (QualitySettings.names.ToList ());
		qualityDropdown.value = QualitySettings.GetQualityLevel ();

		reflectionToggle.isOn = doReflection;
		fogToggle.isOn = doFog;
		drawDistanceSlider.value = drawDistance;
		foliageDistanceSlider.value = foliageDistance;
		grassDensitySlider.value = grassDensity;

        RefreshComponents();
        ResetAll();
    }
    
    void RefreshComponents()
    {
        if (cameraQuality == null)
        {
            Debug.Log("refreshing cameraQuality");
            cameraQuality = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraQuality>();
        }

        while (parentTerrain == null)
        {
            Debug.Log("refreshing parentTerrain");
            parentTerrain = GameObject.FindGameObjectWithTag("MainTerrain");
        }

    }

    private void ResetAll()
    {
        Debug.Log("Reseting quality...");
        RefreshComponents();

        SetReflections(doReflection);
        SetFog(doFog);
        SetDrawDistance(drawDistance);
        SetFoliageDistance(foliageDistance);
        SetGrassDensity(grassDensity);
    }

	public void SetQuality (int newQuality) {
		QualitySettings.SetQualityLevel (newQuality, true);
	}

	public void SetReflections(bool newReflections) {
		doReflection = newReflections;
		float reflectionValue = newReflections ? 1 : 0;
		waterMaterial.SetFloat ("_EnableReflections", reflectionValue);
	}

	public void SetFog(bool newFog) {
        RefreshComponents();

        doFog = newFog;
		cameraQuality.SetFog(newFog);
	}

	public void SetDrawDistance (float newDrawDistance)
    {
        RefreshComponents();

        drawDistance = newDrawDistance;
		cameraQuality.SetDrawDistance (newDrawDistance, parentTerrain);
//		PlayerPrefs.SetFloat (DRAW_DISTANCE, newDrawDistance);
	}

	public void SetFoliageDistance (float newFoliageDistance)
    {
        RefreshComponents();

        foliageDistance = newFoliageDistance;
		Terrain[] terrains = parentTerrain.GetComponentsInChildren<Terrain> ();
		foreach (Terrain terrain in terrains) {
			terrain.detailObjectDistance = newFoliageDistance;
		}
//		PlayerPrefs.SetFloat (FOLIAGE_DISTANCE, newFoliageDistance);
	}

	public void SetGrassDensity (float newGrassDensity)
    {
        RefreshComponents();

        grassDensity = newGrassDensity;
		Terrain[] terrains = parentTerrain.GetComponentsInChildren<Terrain> ();
		foreach (Terrain terrain in terrains) {
			terrain.detailObjectDensity = newGrassDensity;
		}
//		PlayerPrefs.SetFloat (GRASS_DENSITY, newGrassDensity);
	}
}
