using UnityEngine;
using System.Collections;

public class FishJump : MonoBehaviour {
	public float upForce;
	public float forwardForce;
	public float gravity;
	public float deathTime;

	private Rigidbody rigidBody;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		Invoke ("Die", deathTime);
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y <= initialPosition.y) {
			transform.position = new Vector3(transform.position.x, initialPosition.y, transform.position.z);
			transform.rotation = initialRotation;
			Jump ();
		}

		rigidBody.AddForce (Vector3.down*gravity, ForceMode.Impulse);
		Vector3 rotation = Quaternion.LookRotation(rigidBody.velocity, transform.up).eulerAngles;
		transform.rotation = Quaternion.Euler (rotation);
	}

	void Jump(){
		rigidBody.velocity = Vector3.zero;
		rigidBody.AddForce (Vector3.up * upForce, ForceMode.Impulse);
		rigidBody.AddForce (transform.forward * forwardForce, ForceMode.Impulse);
	}

	void Die(){
		Destroy (gameObject);
	}
}
