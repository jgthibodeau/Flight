using UnityEngine;
using System.Collections;

public class Perch : MonoBehaviour {
	public float minMoveSpeed, moveSpeed, rotateSpeed, perchOffset;
	public float distanceMargin = 0.2f;
	public float angleMargin = 5f;

	public float minAngleY;
	public float maxLandingSpeed;

//	public bool perched = false;
	public Transform perchedObject;
	public Vector3 perchedLocation;

	public bool isPerching;
	private Vector3 perchNormal;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isPerching) {
			Debug.DrawRay (perchedLocation, perchNormal);
			Debug.DrawRay (perchedLocation, transform.right);

			bool correctAngle = false;
			bool correctPosition = false;

			//rotate towards perchNormal
			transform.up = Vector3.Slerp (transform.up, perchNormal, Time.deltaTime * rotateSpeed);

			if (Vector3.Angle (transform.up, perchNormal) <= angleMargin) {
				transform.up = perchNormal;
				correctAngle = true;
			}

			//move towards perchedLocation
			transform.position = Vector3.Slerp (transform.position, perchedLocation, Time.deltaTime * moveSpeed);

			if (Vector3.Distance (transform.position, perchedLocation) <= distanceMargin) {
				transform.position = perchedLocation;
				correctPosition = true;
			}

			//if rotated enough and moved enough
			if (correctAngle && correctPosition) {
//				isPerching = false;
				ResetPerch ();
			}
		}
	}

	public void SetPerch(Transform perchedObject, Vector3 perchedLocation, Vector3 perchNormal, float moveSpeed){
		if(perchNormal.y >= minAngleY && moveSpeed <= maxLandingSpeed){
			isPerching = true;
//			perched = true;
			this.perchedObject = perchedObject;
			this.perchNormal = perchNormal;
			this.perchedLocation = perchedLocation + perchOffset * perchNormal;
			this.moveSpeed = Mathf.Clamp (moveSpeed, minMoveSpeed, moveSpeed);
		}
	}

	public void ResetPerch(){
		isPerching = false;
//		perched = false;
	}
}
