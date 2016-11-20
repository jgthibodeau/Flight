using UnityEngine;
using System.Collections;

public class CameraFollowVelocty : MonoBehaviour {
	public Transform target;
	public Transform targetPosition;
	private Rigidbody targetRigidBody;
	private Camera cam;

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

	public bool zoomed;
	public Transform regularPosition;
	public Transform zoomedPosition;
	public float regularFov = 70;
	public float zoomedFov = 20;
	public float zoomSpeed;

	// Use this for initialization
	void Start () {
		targetRigidBody = target.GetComponent<Rigidbody> ();
		cam = GetComponent<Camera> ();
	}

	void Update(){
		zoomed = zoomed ^ Input.GetButtonDown ("Zoom");
	}
	
	// Update is called once per frame
	void FixedUpdate ()	{
		Vector3 desiredPosition = targetPosition.position;
		Vector3 lookAt;
		Quaternion desiredRotation;

		//determine desired position and rotation if following velocity or not
		if (followVelocity && targetRigidBody.velocity.magnitude > minVelocity) {
//			Vector3 up = Vector3.Cross (targetRigidBody.velocity.normalized, transform.right);
//			desiredPosition = target.position - targetRigidBody.velocity.normalized * distance + up * height;
			lookAt = target.position + targetRigidBody.velocity.normalized * forwardDistance;
			desiredRotation = target.rotation;
		} else {
//			desiredPosition = target.position - target.forward * distance + target.up * height;
			lookAt = target.position + target.forward * forwardDistance;
			desiredRotation = target.rotation;
		}

		//move
		if (zoomed) {
			targetPosition = zoomedPosition;
			transform.position = targetPosition.position;
		} else {
			targetPosition = regularPosition;
			float moveAmount = 1f;
			if (smoothMove) {
				moveAmount = Time.fixedDeltaTime * moveSpeed;
			}
			transform.position = Vector3.Slerp (transform.position, desiredPosition, moveAmount);
		}

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

		Zoom ();
	}

	void Zoom(){
		if (zoomed){
			if (cam.fieldOfView > zoomedFov) {
				cam.fieldOfView = Mathf.Clamp (cam.fieldOfView - zoomSpeed * Time.deltaTime, zoomedFov, regularFov);
			}
		}
		else if (!zoomed){
			if (cam.fieldOfView < regularFov) {
				cam.fieldOfView = Mathf.Clamp (cam.fieldOfView + zoomSpeed * Time.deltaTime, zoomedFov, regularFov);
			}
		}
	}
}
