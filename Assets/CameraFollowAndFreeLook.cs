using UnityEngine;
using System.Collections;

public class CameraFollowAndFreeLook : MonoBehaviour {
	public Transform target;
	public float distance;
	public float height;
	public float lookOffset;
	public float accuracy;
	public bool smooth;
	public float translationSmoothness;
	public float rotationSmoothness;

	public float horizontalSpeed;
	public float verticalSpeed;

	public bool lookAt;
	public bool rotateWithTarget;

	private Rigidbody targetRigidbody;
	public Vector3 targetDirection;
	public Vector3 freeLookDirection;
	public bool freeLook;

	// Use this for initialization
	void Start () {
		targetRigidbody = target.GetComponent<Rigidbody> ();
	}

	void Update(){
		float horizontal = Input.GetAxis ("Horizontal Right") * horizontalSpeed;
		float vertical = Input.GetAxis ("Vertical Right") * verticalSpeed;

		if (Input.GetButton ("Center Camera")) {
			freeLook = false;
		}
		else if (horizontal != 0f || vertical != 0f) {
			if (!freeLook) {
				freeLookDirection = target.forward;
				freeLook = true;
			}

			freeLookDirection = Quaternion.Euler (Time.deltaTime * vertical, Time.deltaTime * horizontal, 0) * freeLookDirection;
//			freeLookDirection.y += horizontal;
//			freeLookDirection.x += vertical;
		}

		if (!freeLook)
			targetDirection = target.forward;
		else
			targetDirection = freeLookDirection;
	}

	void LateUpdate () {
		Vector3 targetPosition = target.position + targetDirection * lookOffset;
		Vector3 newPos = target.position - targetDirection * distance + target.up * height;

		Quaternion newRotation;
		if (lookAt) {
			Vector3 lookRotation;
			lookRotation = Quaternion.LookRotation(targetPosition - transform.position).eulerAngles;
			if(rotateWithTarget)
				lookRotation.z = target.rotation.eulerAngles.z;
			newRotation = Quaternion.Euler (lookRotation);
		} else {
			newRotation = target.rotation;
		}

		transform.rotation = newRotation;

		if (rotateWithTarget) {
			Vector3 lookRotation2 = newRotation.eulerAngles;
			lookRotation2.z = target.rotation.eulerAngles.z;
			newRotation = Quaternion.Euler (lookRotation2);
		}

		if (smooth) {
			transform.position = Vector3.Slerp (transform.position, newPos, Time.deltaTime * translationSmoothness);
			transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * rotationSmoothness);
		} else {
			transform.position = newPos;
			transform.rotation = newRotation;
		}
	}
}
