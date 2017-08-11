using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LPWAsset.LowPolyWaterScript))]
[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class Ocean : MonoBehaviour {
	public LPWAsset.LowPolyWaterScript waterScript;

	// Use this for initialization
	void OnEnable () {
	}
	
	// Update is called once per frame
	void Update () {
	}


}
