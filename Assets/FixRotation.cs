using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour {
	private Quaternion rotation;
	public bool useObjectRotation;
	public Vector3 initialRotation;

	// Use this for initialization
	void Start () {
		if (useObjectRotation) {
			rotation = transform.rotation;
		} else {
			rotation = Quaternion.Euler (initialRotation);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = rotation;
	}
}
