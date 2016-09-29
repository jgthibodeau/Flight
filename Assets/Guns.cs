using UnityEngine;
using System.Collections;

public class Guns : MonoBehaviour {
	public LayerMask layerMaskLaserSight;

	private LineRenderer lineRenderer;
	private Camera mainCamera;

	// Use this for initialization
	void Start () {
		lineRenderer = transform.GetComponent<LineRenderer> ();
		mainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		//get aiming center
		RaycastHit hit;
		if (Physics.Raycast (mainCamera.transform.position, mainCamera.transform.forward, out hit, mainCamera.farClipPlane, layerMaskLaserSight)) {
			lineRenderer.SetPositions (new Vector3[] { hit.point, transform.position });
		} else {
			lineRenderer.SetPositions (new Vector3[] { mainCamera.transform.position + mainCamera.transform.forward * Camera.main.farClipPlane, transform.position});
		}
	}
}
