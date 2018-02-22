using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerV2 : MonoBehaviour {
	public Player target;
	private Rigidbody targetRb;
	public bool rotateWithTarget = false;
	public float minDistance = 5f;
	public float maxDistance = 5f;

	public float rotationSpeed = 1.0f;
	public float speed = 1.0f;
	public float offsetSpeed = 0.5f;

	public float minFollowSpeed = 2;
	public float angleThreshold = 170.0f;

	//TODO implement this
	public float desiredHeight = 0;
	public float minHeight = 0;
	public float maxHeight = 0;

	public bool useRelativeOffset;
	public Vector3 airOffset;
	public Vector3 groundOffset;
	public Vector3 waterOffset;

	Vector3 up = Vector3.up;

	Vector3 targetPosition = Vector3.zero;
	Vector3 desiredDirection;
	Vector3 desiredPosition;

	void Start () {
		targetRb = target.GetComponent<Rigidbody> ();
	}

	Vector3 offset = Vector3.zero;
	void FixedUpdate () {
		if (target.inWater) {
			offset = Vector3.Slerp (offset, waterOffset, Time.fixedDeltaTime * offsetSpeed);
		} else if (target.isGrounded) {
			offset = Vector3.Slerp (offset, groundOffset, Time.fixedDeltaTime * offsetSpeed);
		} else {
			offset = Vector3.Slerp (offset, airOffset, Time.fixedDeltaTime * offsetSpeed);
		}

		if (useRelativeOffset) {
			targetPosition = target.transform.position + offset.x * target.transform.right + offset.y * target.transform.up + offset.z * target.transform.forward;
		} else {
			targetPosition = target.transform.position + offset;
		}

		//if target is moving fast enough and not toward camera
		float movementAngle = Vector3.Angle(transform.forward, targetRb.velocity);
//		if (target.targetRb.magnitude > minFollowSpeed && movementAngle < angleThreshold) {
//			if (rotateWithTarget) {
//				up = target.transform.up;
//			} else {
//				up = Vector3.up;
//			}

			//adjust rotation to look at target
			desiredDirection = targetPosition - transform.position;
//		}

		//rotate towards target
		Quaternion desiredRotation = Quaternion.LookRotation (desiredDirection, up);

		float distance = Vector3.Distance (transform.position, targetPosition);
		Vector3 desiredPosition = transform.position;
		if (distance > maxDistance) {
			desiredPosition = targetPosition - desiredDirection.normalized * maxDistance;
		} else if (distance < minDistance) {
			desiredPosition = targetPosition - desiredDirection.normalized * minDistance;
		}

		//up vector relative to target position
		float currentDesiredHeight = Vector3.Dot (-desiredDirection, target.transform.up);
//		float currentDesiredHeight = Vector3.Dot (-desiredDirection, Vector3.up);

		//if negative									add (desiredHeight - currentDesiredHeight)
		//if positive, but less than desiredHeight		add (desiredHeight - currentDesiredHeight)
		//if positive, but greater than desiredHeight	add (desiredHeight - currentDesiredHeight)
//		desiredPosition += target.transform.up * (desiredHeight - currentDesiredHeight);
//		desiredPosition += Vector3.up * (desiredHeight - currentDesiredHeight);
		if (currentDesiredHeight < minHeight) {
			desiredPosition += target.transform.up * (minHeight - currentDesiredHeight);
		}else if (currentDesiredHeight > maxHeight) {
			desiredPosition += target.transform.up * (maxHeight - currentDesiredHeight);
		}

		//TODO add occlusion checks and move to adjust

		transform.position = Vector3.Slerp (transform.position, desiredPosition, Time.fixedDeltaTime * speed);
		desiredRotation = Quaternion.LookRotation (desiredDirection, up);
		transform.rotation = Quaternion.Slerp (transform.rotation, desiredRotation, Time.fixedDeltaTime * rotationSpeed);
	}
}
