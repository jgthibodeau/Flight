using UnityEngine;
using System.Collections;

public class CameraFollowVelocty : MonoBehaviour {
	public Transform target;
	private Rigidbody targetRigidBody;

	public float distance;
	public float maxDistance;
	public float height;
	public float forwardDistance;

	public bool followVelocity;
	public float minVelocity;

	public bool rotateWithTarget;

	public bool smoothMove;
	public float moveSpeed;

	public bool smoothRotate;
	public float rotateSpeed;

	// Use this for initialization
	void Start () {
		targetRigidBody = target.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()	{
		Vector3 desiredPosition;
		Vector3 lookAt;
		Quaternion desiredRotation;

		//determine desired position and rotation if following velocity or not
		if (followVelocity && targetRigidBody.velocity.magnitude > minVelocity) {
			Vector3 up = Vector3.Cross (targetRigidBody.velocity.normalized, transform.right);
			desiredPosition = target.position - targetRigidBody.velocity.normalized * distance + up * height;
			lookAt = target.position + targetRigidBody.velocity.normalized * forwardDistance;
			desiredRotation = target.rotation;
		} else {
			desiredPosition = target.position - target.forward * distance + target.up * height;
			lookAt = target.position + target.forward * forwardDistance;
			desiredRotation = target.rotation;
		}

		//move
		float moveAmount = 1f;
		if (smoothMove) {
			moveAmount = Time.fixedDeltaTime * moveSpeed;
		}
		transform.position = Vector3.Slerp (transform.position, desiredPosition, moveAmount);

		//keep within max distance
		if (Vector3.Distance (transform.position, target.position) > maxDistance) {
			transform.position = target.position + (transform.position - target.position).normalized * maxDistance;
		}

		//rotate
		float rotateAmount = 1f;
		if (smoothRotate) {
			rotateAmount = Time.fixedDeltaTime * rotateSpeed;
		}
		transform.rotation = Quaternion.Slerp (transform.rotation, desiredRotation, rotateAmount);

		//look at
		transform.LookAt (lookAt, transform.up);
	}
}
