using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRopeNode : MonoBehaviour {
	public HingeJoint joint;
	public float maxDistance;
	public Vector3 offset;

	private Rigidbody rb;

	private Vector3 prevPosition;

	void Start() {
		rb = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 parentPosition = joint.connectedBody.transform.position - joint.connectedBody.transform.up * offset.y;
		Vector3 position = transform.position + transform.up * offset.y;

		Vector3 direction = parentPosition - position;

		float distance = direction.magnitude;
		if (distance > maxDistance) {
			transform.position = transform.position + direction.normalized * (distance - maxDistance);
//			transform.up = direction.normalized;
//			rb.velocity = Vector3.zero;
		}
	}
}
