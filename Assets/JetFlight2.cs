using UnityEngine;
using System.Collections;

public class JetFlight2 : MonoBehaviour {
	public float gravity;
	public bool wingsOut;
	public float freeRotateSpeed;

	public float liftCoef;
	public float lift;

	public float wingLiftSurfaceArea;
	public float wingDragSurfaceArea;
	public float dragAreaScale;
	public float dragCoef;
	public float drag;

	public float airDensity;

	public float speed;
	public float maxSpeed;
	public float terminalSpeed;

	public float rollCoef;
	public float pitchCoef;

	public float jetCoef;
	public float jetAmount;

	public float brakeCoef;

	public float pitchScale = 1.5f;
	public float volumeScale = 0.7f;
	public float trailScale = 0.3f;

	private Rigidbody rigidBody;

	private bool brake = false;
	private bool isBraking = false;
	public float wingOutDistance = 0.5f;
	public float wingForwardDistance = 0.5f;

	public float angleOffset = 0.015f;
	public float angleScale = 0.05f;
	public float angleOfAttack;

	private Collider characterCollider;

	private TrailRenderer[] trails;

	public LayerMask layerMaskForGround;

	// Use this for initialization
	void Start () {
		rigidBody = transform.GetComponent<Rigidbody> ();
		trails = transform.GetComponentsInChildren<TrailRenderer> ();
		characterCollider = transform.GetComponent<Collider> ();
	}

	// Update is called once per frame
	void Update() {
		//set air density based on height
		//		airDensity = 1.2238f * Mathf.Pow(1f - (0.0000226f * transform.position.y), 5.26f);
		airDensity = 1.2238f;

		//get current speed
		speed = rigidBody.velocity.magnitude;
		terminalSpeed = Mathf.Sqrt (2*gravity/(airDensity*wingDragSurfaceArea*dragCoef));

		//inputs
		wingsOut = wingsOut ^ Input.GetButtonDown ("Wings");
		isBraking = Input.GetButton ("Brake");

		angleOfAttack = Mathf.Deg2Rad * (angleScale * -Input.GetAxis ("Vertical") + angleOffset);

		jetAmount = Input.GetAxis ("Jets");

		//drag and lift surface areas
		wingDragSurfaceArea = 1;
		if (wingsOut) {
			wingDragSurfaceArea += Mathf.Abs (Input.GetAxis ("Vertical") * dragAreaScale);
		}

		//trails
//		foreach(TrailRenderer trail in trails){
//			trail.endWidth = drag * trailScale;
//			trail.startWidth = 0f;
//			trail.time = 0.5f;
//		}

		UpdateRendering ();
	}

	void UpdateRendering(){
		if (wingsOut) {
		} else {
		}
	}

	void FixedUpdate() {
		if (wingsOut) {
			//TODO jetspeed faster as transform.forward.y aproaches -1 and slower as transform.forward.y aproaches 1
			Jets ();
			Lift ();
			//			Rotate ();
			Roll ();
		} else {
			Rotate ();
		}

//		AirDrag ();
		//TODO gravity inversely proportional to jetspeed
		Gravity ();
	}

	void Jets(){
//		float jetPercent = -transform.forward.y*0.5f+0.5f;
		float jetPercent = 1f;
		float jetSpeed = jetPercent * (jetCoef + jetCoef * jetAmount);
		Vector3 jetForce = transform.forward * jetSpeed;
		Debug.Log ("jetForce: "+jetForce);

		rigidBody.AddForceAtPosition (jetForce, transform.position, ForceMode.Force);
		Debug.DrawRay (transform.position, jetForce, Color.red);
	}

	void AirDrag(){
		drag = 0.5f * airDensity * speed * speed * dragCoef * wingDragSurfaceArea;
		Vector3 dragForce = rigidBody.velocity.normalized * (-1) * drag;
		rigidBody.AddForceAtPosition (dragForce, transform.position + wingForwardDistance * transform.forward, ForceMode.Force);
		Debug.DrawRay (transform.position + wingForwardDistance*transform.forward, dragForce, Color.blue);
	}

	void Gravity(){
		//		float maxSpeed = flapForwardCoef * 1.9f;
		//		float speedPercent = Mathf.Clamp((maxSpeed - speed) / maxSpeed, 0, 1f);
		//
		//		if (!isGrounded ()) {
		//			rigidBody.AddForceAtPosition (Vector3.down * gravity * speedPercent, transform.position + wingForwardDistance * transform.forward, ForceMode.Force);
		//		}
		if (!isGrounded ()) {
			float gravityPercent = -Input.GetAxis ("Jets")*0.5f+0.5f;
			Vector3 gravityForce = Vector3.down * gravity * gravityPercent;
			Debug.Log ("gravityForce: "+gravityForce);

			rigidBody.AddForceAtPosition (gravityForce, transform.position, ForceMode.Force);
			Debug.DrawRay (transform.position, gravityForce, Color.magenta);
		}
	}

	void Lift(){
		//apply lift
		float lift = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttack;
		Vector3 liftForce = transform.up * lift;
		rigidBody.AddForceAtPosition (liftForce, transform.position + wingForwardDistance*transform.forward, ForceMode.Force);

		//		rigidBody.AddForceAtPosition (Vector3.up * Mathf.Clamp (lift, 0, gravity), transform.position + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position + wingForwardDistance*transform.forward, transform.up * lift, Color.green);
		Debug.DrawRay (transform.position + wingForwardDistance*transform.forward, rigidBody.velocity, Color.cyan);
	}

	void Rotate(){
		float roll = -Input.GetAxis ("Horizontal");
		rigidBody.AddTorque (transform.forward * roll * rollCoef, ForceMode.Force);

		float pitch = Input.GetAxis ("Vertical");
		rigidBody.AddTorque (transform.right * pitch * pitchCoef, ForceMode.Force);
	}

	void Roll(){
		float roll = -Input.GetAxis ("Horizontal");
		rigidBody.AddTorque (transform.forward * roll * rollCoef, ForceMode.Force);
	}

	private bool isGrounded(){
		//		Debug.DrawLine (characterCollider.bounds.center, new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z), Color.red);
		return Physics.CheckCapsule (
			characterCollider.bounds.center,
			new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z),
			0.2f,
			layerMaskForGround.value
		);
	}
}