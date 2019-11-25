using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraQuality : MonoBehaviour {
//	private Camera[] cameras;
	public AC.LSky.LSky lSky;
	public float minFarClip, maxFarClip;
	public float minFog, maxFog;
	private float drawDistance;

	// Use this for initialization
	void Start () {
	}
	
	public void SetDrawDistance(float percent, GameObject parentTerrain)
    {
		drawDistance = percent;
		float newFarClip = minFarClip + (maxFarClip - minFarClip) * percent;
//		cameras = GetComponentsInChildren<Camera> ();
		foreach (Camera c in gameObject.GetComponentsInChildren<Camera> ()) {
			c.farClipPlane = newFarClip;
		}

		float newFog = minFog - (minFog - maxFog) * percent;
		lSky.unityFogDensity.inputValue = newFog;

        if (parentTerrain != null)
        {
            Terrain[] terrains = parentTerrain.GetComponentsInChildren<Terrain>();
            foreach (Terrain terrain in terrains)
            {
                TreeTerrain treeTerrain = terrain.GetComponent<TreeTerrain>();
                if (treeTerrain != null)
                {
                    treeTerrain.SetTreeDistance(newFarClip);
                }
            }
        }
    }

	public float GetDrawDistance(){
		return drawDistance;
	}

	public void SetFog(bool fog){
		lSky.enableUnityFog = fog;
	}

	public bool GetFog(){
		return lSky.enableUnityFog;
	}
}
