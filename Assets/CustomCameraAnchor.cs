using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraAnchor : MonoBehaviour {
	public bool keepUpright;
	
	// Update is called once per frame
	void Update () {
		if (keepUpright) {
			Vector3 euler = transform.eulerAngles;
			euler.z = 0;
			transform.eulerAngles = euler;
		}
	}
}
