using UnityEngine;
using System.Collections;

public class JetFlight : MonoBehaviour {
	public float gravity;
	public bool scaleGravity;

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

	public float freeRotateScale;
	public float rollCoef;
	public float pitchCoef;

	public float jetCoef;
	public float jetOnForce;
	public float jetUpAmount;
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

	public float maxSlowedTime;
	public float slowedTime;
	public float slowedSpeed;

	private Collider characterCollider;

	private TrailRenderer[] trails;

	public LayerMask layerMaskForGround;

	private Animator animator;

	// Use this for initialization
	void Start () {
		rigidBody = transform.GetComponent<Rigidbody> ();
		trails = transform.GetComponentsInChildren<TrailRenderer> ();
		characterCollider = transform.GetComponent<Collider> ();
		animator = transform.GetComponent<Animator> ();
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
		bool wingsAlreadyOut = wingsOut;
		wingsOut = wingsOut ^ Input.GetButtonDown ("Wings");
		isBraking = Input.GetButton ("Brake");

		angleOfAttack = Mathf.Deg2Rad * (angleScale * -Input.GetAxis ("Vertical") + angleOffset);

		jetAmount = Input.GetAxis ("Jets");

		//slow down if wings closed
		if (wingsAlreadyOut && !wingsOut) {
			slowedTime = maxSlowedTime;
			Time.timeScale = slowedSpeed;
			Time.fixedDeltaTime = 0.02f * slowedSpeed;
		}
		else if (!wingsOut) {
			slowedTime -= slowedSpeed;
			if (slowedTime <= 0f) {
				Time.timeScale = 1f;
				Time.fixedDeltaTime = 0.02f;
			}
		}

		//add force for opening wings
		if (!wingsAlreadyOut && wingsOut) {
			Time.timeScale = 1f;
			Time.fixedDeltaTime = 0.02f;

			Vector3 jetForce = Vector3.Normalize (transform.forward + transform.up * jetUpAmount) * jetOnForce * jetAmount;
			rigidBody.AddForceAtPosition (jetForce, transform.position + transform.forward * wingForwardDistance, ForceMode.Impulse);
			Debug.DrawRay (transform.position + transform.forward * wingForwardDistance, jetForce, Color.yellow);
		}

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
		animator.SetBool ("WingsOut", wingsOut);
	}

	void FixedUpdate() {
		if (wingsOut) {
			Jets ();
			Lift ();
			Roll ();

//			Vector3 rotation = Quaternion.LookRotation(rigidBody.velocity, transform.up).eulerAngles;
//			transform.rotation = Quaternion.Euler (rotation);
		} else {
			FreeRotate ();
		}

		AirDrag ();
		Gravity ();

		//apply gravity

	}

	void Jets(){
		Vector3 jetForce = Vector3.Normalize (transform.forward + transform.up * jetUpAmount) * (jetCoef + jetCoef * jetAmount);
		rigidBody.AddForceAtPosition (jetForce, transform.position + transform.forward * wingForwardDistance, ForceMode.Force);
		Debug.DrawRay (transform.position + transform.forward * wingForwardDistance, jetForce, Color.red);
	}

	void AirDrag(){
		drag = 0.5f * airDensity * speed * speed * dragCoef * wingDragSurfaceArea;
		Vector3 dragForce = rigidBody.velocity.normalized * (-1) * drag;
		rigidBody.AddForceAtPosition (dragForce, transform.position - transform.forward * wingForwardDistance, ForceMode.Force);
		Debug.DrawRay (transform.position - transform.forward * wingForwardDistance, dragForce, Color.blue);
	}

	void Gravity(){
//		float maxSpeed = flapForwardCoef * 1.9f;
//		float speedPercent = Mathf.Clamp((maxSpeed - speed) / maxSpeed, 0, 1f);
//
//		if (!isGrounded ()) {
//			rigidBody.AddForceAtPosition (Vector3.down * gravity * speedPercent, transform.position + wingForwardDistance * transform.forward, ForceMode.Force);
//		}
		if (!isGrounded ()) {
			Vector3 gravityForce = Vector3.down * gravity;
			if(scaleGravity){
				float gravityPercent = -Input.GetAxis ("Jets")*0.5f+0.5f;
				gravityForce *= gravityPercent;
			}

			rigidBody.AddForceAtPosition (gravityForce, transform.position, ForceMode.Force);
			Debug.DrawRay (transform.position, gravityForce, Color.magenta);
		}
	}

	void Lift(){
		//apply lift
		float lift = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttack;
		Vector3 liftForce = transform.up * lift;
		rigidBody.AddForceAtPosition (liftForce, transform.position + transform.forward * wingForwardDistance, ForceMode.Force);

//		rigidBody.AddForceAtPosition (Vector3.up * Mathf.Clamp (lift, 0, gravity), transform.position + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position + transform.forward * wingForwardDistance, transform.up * lift, Color.green);
		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void FreeRotate(){
		float roll = -Input.GetAxis ("Horizontal");
		rigidBody.AddTorque (transform.forward * roll * rollCoef * freeRotateScale, ForceMode.Force);

		float pitch = Input.GetAxis ("Vertical");
		rigidBody.AddTorque (transform.right * pitch * pitchCoef * freeRotateScale, ForceMode.Force);
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