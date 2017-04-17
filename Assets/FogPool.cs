using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FogPool : MonoBehaviour {
	public bool useCameraForScale;
	public Camera camera;
	public Vector3 scale;
	public float height;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	}
	void LateUpdate(){
		if (useCameraForScale) {
			float newScale = camera.farClipPlane / 100;
			transform.localScale = new Vector3 (newScale, newScale, newScale);
		} else {
			transform.localScale = scale;
		}

		Vector3 newPosition = camera.transform.position;
		newPosition.y += height;
		transform.position = newPosition;

		Vector3 rotation = transform.rotation.eulerAngles;
		rotation.x = -2.767f;
		rotation.z = 0;
		transform.rotation = Quaternion.Euler (rotation);
	}
}
