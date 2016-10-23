using UnityEngine;
using System.Collections;

public class Grab : MonoBehaviour {
	public bool grab = false;
	public float forwardAmount, upAmount;
	public float grabDistance;
	public float grabRadius;
	public LayerMask grabMask;

	public bool hasObject;
	public Transform grabbedObject;
	public Vector3 grabbedLocation;
	public Vector3 grabbedNormal;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (grab) {
			//start trying to grab
			RaycastHit hit;
			Vector3 grabDirection = (forwardAmount * transform.forward - upAmount * transform.up).normalized;
			Debug.DrawRay (transform.position, grabDirection * grabDistance, Color.green);
			if (Physics.Raycast (transform.position, grabDirection, out hit, grabDistance, grabMask)) {
				hasObject = true;
				grabbedObject = hit.transform;
				grabbedLocation = hit.point;
				grabbedNormal = hit.normal;
			}
		}
	}

	public void ResetGrabbedObject(){
		hasObject = false;
		grab = false;
	}
}
