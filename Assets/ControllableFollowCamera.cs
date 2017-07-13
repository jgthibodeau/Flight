using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableFollowCamera : MonoBehaviour {
	public bool shake;
	public float shakeScale;

	public Transform target;
	public bool lookAtVelocity;
	public float minVelocity;
	public bool adjustHeight;
	public Rigidbody targetRigidbody;
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

	public bool rotate;

	public float freeCameraSpeed;
	private bool freeCamera;
	public Vector3 freeCameraPosition;

	// Use this for initialization
	void Start () {
		prevLookPosition = CalcTargetPositionV1(0, 0);
	}

	Vector3 CalcTargetPositionV1(float cameraX, float cameraY){
		Vector3 targetPositon = Util.RigidBodyPosition (targetRigidbody);
		//		if (lookAtVelocity && targetRigidbody.velocity.magnitude > minVelocity) {
		//			targetPositon += targetRigidbody.velocity * forwardOffset;
		//			if (adjustHeight) {
		//				targetPositon += Vector3.up * upOffset;
		//			}
		//		} else {
		//			targetPositon += target.forward * forwardOffset;
		//			if (adjustHeight) {
		//				targetPositon += target.up * upOffset;
		//			}
		//		}

		if (cameraX == 0 && cameraY == 0) {
			if (lookAtVelocity && targetRigidbody.velocity.magnitude > minVelocity) {
				targetPositon += targetRigidbody.velocity * forwardOffset;
				if (adjustHeight) {
					targetPositon += Vector3.up * upOffset;
				}
			} //else {
			//			targetPositon += target.forward * forwardOffset;
			//			if (adjustHeight) {
			//				targetPositon += target.up * upOffset;
			//			}
			//		}
		} else {
			targetPositon -= (target.right * cameraX + target.forward * cameraY).normalized * forwardOffset;
			if (adjustHeight) {
				targetPositon += target.up * upOffset;
			}
		}

		return targetPositon;
	}

	// Update is called once per frame
	void FixedUpdate () {
//		CameraV1 ();

		float cameraX = -Input.GetAxis ("Horizontal Right");
		float cameraY = -Input.GetAxis ("Vertical Right");
		bool resetCamera = Input.GetButton ("Reset Camera");

		if (resetCamera) {
			freeCamera = false;
		} else if (!freeCamera && (cameraX != 0 || cameraY != 0)) {
			freeCameraPosition = (transform.position - Util.RigidBodyPosition (targetRigidbody));//.normalized * maxDistance;
			freeCamera = true;
		}

		if (freeCamera) {
			FreeCamera (cameraX, cameraY);
		} else {
			FollowCamera ();
		}

		if (shake) {
			float shakeAmount = target.GetComponent<Rigidbody> ().velocity.magnitude * shakeScale;
			Vector3 shakedPosition = transform.position;
			shakedPosition.x += Random.Range (-shakeAmount, shakeAmount);
			shakedPosition.y += Random.Range (-shakeAmount, shakeAmount);
			shakedPosition.z += Random.Range (-shakeAmount, shakeAmount);
			transform.position = shakedPosition;
		}
	}

	void CameraV1(){
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
		Vector3 targetPosition = Util.RigidBodyPosition (targetRigidbody) + target.up * height;

		float cameraX = -Input.GetAxis ("Horizontal Right");
		float cameraY = -Input.GetAxis ("Vertical Right");

		if (cameraX == 0 && cameraY == 0) {
			if (Vector3.Distance (transform.position, targetPosition) > maxDistance) {
				desiredPosition = targetPosition + (transform.position - targetPosition).normalized * maxDistance;
			} else if (Vector3.Distance (transform.position, targetPosition) < minDistance) {
				desiredPosition = targetPosition + (transform.position - targetPosition).normalized * minDistance;
			}
		} else {
			desiredPosition = targetPosition + (target.forward * cameraY + target.right * cameraX).normalized * maxDistance;
		}

		if (moveDamp && Vector3.Distance (transform.position, targetPosition) <= maxDistance) {
			transform.position = Vector3.Lerp (transform.position, desiredPosition, Time.fixedDeltaTime * moveDamping);
		} else {
			transform.position = desiredPosition;
		}
		//
		//		//set height
		//		float targetHeight = Util.RigidBodyPosition (targetRigidbody).y + height;
		//		float newHeight = Mathf.Lerp (transform.position.y, targetHeight, Time.fixedDeltaTime * heightDamping);
		//		transform.position = new Vector3 (transform.position.x, newHeight, transform.position.z);

		//look at target
		//		Vector3 lookPosition = CalcTargetPosition(cameraX, cameraY);
		//		Vector3 targetUp = Vector3.Slerp (transform.up, target.up, Time.fixedDeltaTime * rotationUpDamping);
		//		Vector3 targetLook = Vector3.Slerp (prevLookPosition, lookPosition, Time.fixedDeltaTime * rotationDamping);
		//		prevLookPosition = targetLook;
		//		this.transform.LookAt (targetLook, targetUp);
		Vector3 lookPosition = CalcTargetPositionV1 (cameraX, cameraY);
		Quaternion lookDirection;
		if (rotate) {
			lookDirection = Quaternion.LookRotation (lookPosition - transform.position, target.up);
		} else {
			lookDirection = Quaternion.LookRotation (lookPosition - transform.position, Vector3.up);
		}
		if (rotationDamp) {
			lookDirection = Quaternion.Lerp (transform.rotation, lookDirection, Time.fixedDeltaTime * rotationDamping);
		}
		transform.rotation = lookDirection;

		//shake
		if (shake) {
			float shakeAmount = target.GetComponent<Rigidbody> ().velocity.magnitude * shakeScale;
			Vector3 shakedPosition = transform.position;
			shakedPosition.x += Random.Range (-shakeAmount, shakeAmount);
			shakedPosition.y += Random.Range (-shakeAmount, shakeAmount);
			shakedPosition.z += Random.Range (-shakeAmount, shakeAmount);
			transform.position = shakedPosition;
		}
	}

	void FreeCamera (float cameraX, float cameraY){
		//rotate horizontal
		Vector3 horizDir = Vector3.Cross (freeCameraPosition, Vector3.up).normalized;
		Vector3 horiz = horizDir * cameraX * freeCameraSpeed * Time.fixedDeltaTime;
		Debug.DrawRay (Util.RigidBodyPosition (targetRigidbody) + freeCameraPosition, horizDir, Color.blue);

		//rotate vertical
		Vector3 vertDir = Vector3.Cross (freeCameraPosition, horizDir).normalized;
		Vector3 vert = vertDir * cameraY * freeCameraSpeed * Time.fixedDeltaTime;
		Debug.DrawRay (Util.RigidBodyPosition (targetRigidbody) + freeCameraPosition, vertDir, Color.red);

		freeCameraPosition = (freeCameraPosition + horiz + vert).normalized;

		Vector3 targetPosition = Util.RigidBodyPosition (targetRigidbody);
		Vector3 desiredPosition = targetPosition + freeCameraPosition * maxDistance;

		if (moveDamp && Vector3.Distance (transform.position, targetPosition) <= maxDistance) {
			transform.position = Vector3.Lerp (transform.position, desiredPosition, Time.fixedDeltaTime * moveDamping);
		} else {
			transform.position = desiredPosition;
		}


		Vector3 lookPosition = targetPosition;
		Quaternion lookDirection;
		if (rotate) {
			lookDirection = Quaternion.LookRotation (lookPosition - transform.position, target.up);
		} else {
			lookDirection = Quaternion.LookRotation (lookPosition - transform.position, Vector3.up);
		}
//		if (rotationDamp) {
//			lookDirection = Quaternion.Lerp (transform.rotation, lookDirection, Time.fixedDeltaTime * rotationDamping);
//		}
		transform.rotation = lookDirection;
	}

	void FollowCamera(){
		//if not at a good distance, adjust
		Vector3 desiredPosition = transform.position;
		Vector3 targetPosition = Util.RigidBodyPosition (targetRigidbody) + target.up * height;

		if (Vector3.Distance (transform.position, targetPosition) > maxDistance) {
			desiredPosition = targetPosition + (transform.position - targetPosition).normalized * maxDistance;
		} else if (Vector3.Distance (transform.position, targetPosition) < minDistance) {
			desiredPosition = targetPosition + (transform.position - targetPosition).normalized * minDistance;
		}

		if (moveDamp && Vector3.Distance (transform.position, targetPosition) <= maxDistance) {
			transform.position = Vector3.Lerp (transform.position, desiredPosition, Time.fixedDeltaTime * moveDamping);
		} else {
			transform.position = desiredPosition;
		}

		Vector3 lookPosition = CalcTargetPositionV1 (0, 0);
		Quaternion lookDirection;
		if (rotate) {
			lookDirection = Quaternion.LookRotation (lookPosition - transform.position, target.up);
		} else {
			lookDirection = Quaternion.LookRotation (lookPosition - transform.position, Vector3.up);
		}
		if (rotationDamp) {
			lookDirection = Quaternion.Lerp (transform.rotation, lookDirection, Time.fixedDeltaTime * rotationDamping);
		}
		transform.rotation = lookDirection;

		//shake
		if (shake) {
			float shakeAmount = target.GetComponent<Rigidbody> ().velocity.magnitude * shakeScale;
			Vector3 shakedPosition = transform.position;
			shakedPosition.x += Random.Range (-shakeAmount, shakeAmount);
			shakedPosition.y += Random.Range (-shakeAmount, shakeAmount);
			shakedPosition.z += Random.Range (-shakeAmount, shakeAmount);
			transform.position = shakedPosition;
		}
	}
}
