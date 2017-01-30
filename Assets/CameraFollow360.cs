using UnityEngine;
using System.Collections;

public class CameraFollow360 : MonoBehaviour {
	public Transform target;
	private Vector3 prevLookPosition;

	public float upOffset = 0.5f;
	public float forwardOffset = 0.5f;
	public float minDistance = 0.75f;
	public float maxDistance = 1f;
	public float height = 0.5f;
	public float cameraSpeed = 8;

	public bool moveDamp;
	public float moveDamping = 2.0f;
	public float heightDamping = 2.0f;
	public bool rotationDamp;
	public float rotationDamping = 2.0f;
	public float rotationUpDamping = 2.0f;

	// Use this for initialization
	void Start () {
		prevLookPosition = CalcTargetPosition();
	}

	Vector3 CalcTargetPosition(){
		return target.position + target.up * upOffset + target.forward * forwardOffset;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
//		float desiredRotationAngleSide = target.eulerAngles.y;
//		float currentRotationAngleSide = transform.eulerAngles.y;
//
//		float desiredRotationAngleUp = target.eulerAngles.x;
//		float currentRotationAngleUp = transform.eulerAngles.x;
//
//		currentRotationAngleSide = Mathf.LerpAngle (currentRotationAngleSide, desiredRotationAngleSide, rotationDamping * Time.fixedDeltaTime);
//		currentRotationAngleUp = Mathf.LerpAngle (currentRotationAngleUp, desiredRotationAngleUp, rotationDamping * Time.fixedDeltaTime);
//
//		Quaternion currentRotation = Quaternion.Euler (currentRotationAngleUp, currentRotationAngleSide, 0);

		//if not at a good distance, adjust
		Vector3 desiredPosition = transform.position;
		Vector3 targetPosition = target.position + target.up * height;
		if (Vector3.Distance (transform.position, targetPosition) > maxDistance) {
			desiredPosition = target.position + (transform.position - targetPosition).normalized * maxDistance + target.up * height;
		}
		else if (Vector3.Distance (transform.position, targetPosition) < minDistance) {
			desiredPosition = target.position + (transform.position - targetPosition).normalized * minDistance + target.up * height;
		}

		if (moveDamp) {
			transform.position = Vector3.Lerp (transform.position, desiredPosition, Time.fixedDeltaTime * moveDamping);
		} else {
			transform.position = desiredPosition;
		}
//
//		//set height
//		float targetHeight = target.position.y + height;
//		float newHeight = Mathf.Lerp (transform.position.y, targetHeight, Time.fixedDeltaTime * heightDamping);
//		transform.position = new Vector3 (transform.position.x, newHeight, transform.position.z);

		//look at target
//		Vector3 lookPosition = CalcTargetPosition();
//		Vector3 targetUp = Vector3.Slerp (transform.up, target.up, Time.fixedDeltaTime * rotationUpDamping);
//		Vector3 targetLook = Vector3.Slerp (prevLookPosition, lookPosition, Time.fixedDeltaTime * rotationDamping);
//		prevLookPosition = targetLook;
//		this.transform.LookAt (targetLook, targetUp);
		Vector3 lookPosition = CalcTargetPosition ();
		Quaternion lookDirection = Quaternion.LookRotation (lookPosition - transform.position, target.up);
		if (rotationDamp) {
			transform.rotation = Quaternion.Lerp (transform.rotation, lookDirection, Time.fixedDeltaTime * rotationDamping);
		} else {
			transform.rotation = lookDirection;
		}
	}
}
