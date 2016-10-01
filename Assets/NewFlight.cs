using UnityEngine;
using System.Collections;

public class NewFlight : MonoBehaviour {
	public float brakeDrag = 0.0f;
	public float elevatorCenterSetting = -0.25f;
	public float elevator = 0.3f;
	public float ailerons = 0.3f;
	public Vector3 dragForce = new Vector3(2.0f,8.0f,0.05f);
	public Vector3 stabilizingDrag = new Vector3(2.0f,1.0f,0.0f);
	Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
		rigidBody = transform.GetComponent<Rigidbody> ();

		rigidBody.mass = 120;
		rigidBody.angularDrag = 0.5f;
		rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidBody.drag = 0f;
		rigidBody.useGravity = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		float pitchInput = Input.GetAxis("Vertical");
		float rollInput = Input.GetAxis("Horizontal");
		float brakeForce=Input.GetButton("Jump")?1.0f:0.0f;;

		//wings and drag
		float forwardVelo = Vector3.Dot(rigidBody.velocity,transform.forward);
		float sqrVelo = forwardVelo*forwardVelo;

		Vector3 dragDirection = transform.InverseTransformDirection(rigidBody.velocity);
		Vector3 dragAndBrake = dragForce+new Vector3(0,0f,brakeDrag*brakeForce);
		Vector3 dragForces = -Vector3.Scale(dragDirection,dragAndBrake)*rigidBody.velocity.magnitude;
		rigidBody.AddForce(transform.TransformDirection(dragForces));

		//stabilization (to keep the plane facing into the direction it's moving)
		Vector3 stabilizationForces = -Vector3.Scale(dragDirection,stabilizingDrag)*rigidBody.velocity.magnitude;
		rigidBody.AddForceAtPosition(transform.TransformDirection(stabilizationForces),transform.position-transform.forward*10);
		rigidBody.AddForceAtPosition(-transform.TransformDirection(stabilizationForces),transform.position+transform.forward*10);

		//elevator
		rigidBody.AddTorque(transform.right*sqrVelo*elevator*(pitchInput+elevatorCenterSetting));  

		//ailerons
		rigidBody.AddTorque(-transform.forward*sqrVelo*ailerons*rollInput);
	}
}
