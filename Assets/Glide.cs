using UnityEngine;
using System.Collections;

public class Glide : MonoBehaviour {
	public float gravity;
	public bool rotateTowardsMotion;
	public bool stabilizeRotation;
	public Vector3 stabilizingDrag = new Vector3(2.0f,1.0f,0.0f);

	public float liftCoef;
	public float lift;

	public float dragCoef;
	public float dragDistance;
	public float drag;

	public float flapUpCoef;
	public float flapForwardCoef;

	public float wingLiftSurfaceArea;
	public float wingDragSurfaceArea;
	public float airDensity;

	public float speed;
	public float maxSpeed;
	public float terminalSpeed;

	public float pitchScale = 1.5f;
	public float volumeScale = 0.7f;
	public float trailScale = 0.3f;

	private Rigidbody rigidBody;
	private Animator animator;
	public AnimationClip flapClip;

	public bool flap = false;
	private bool isFlapping = false;
	private bool brake = false;
	private bool isBraking = false;
	public float wingOutDistance = 0.5f;
	public float wingForwardDistance = 0.5f;

	public bool staticAngleOffset;
	public float angleOffset = 0.015f;
	public float angleScale = 0.05f;

	private AudioSource audioSource;
	private AudioEchoFilter echoFilter;
	public AudioClip airClip;

	private Collider characterCollider;

	private TrailRenderer[] trails;

	public LayerMask layerMaskForGround;

	public float angleOfAttackLeft;
	public float angleOfAttackRight;

	public Transform leftWing;
	private Vector3 leftWingInitialRotation;
	public Transform rightWing;
	private Vector3 rightWingInitialRotation;

	// Use this for initialization
	void Start () {
		rigidBody = transform.GetComponent<Rigidbody> ();
		animator = transform.GetComponentInChildren<Animator> ();
		audioSource = transform.GetComponent<AudioSource> ();
		echoFilter = transform.GetComponent<AudioEchoFilter> ();
		trails = transform.GetComponentsInChildren<TrailRenderer> ();
		characterCollider = transform.GetComponent<Collider> ();

		leftWingInitialRotation = leftWing.localRotation.eulerAngles;
		rightWingInitialRotation = rightWing.localRotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update() {
		//set air density based on height
		airDensity = 1.2238f * Mathf.Pow(1f - (0.0000226f * transform.position.y), 5.26f);

		//get current speed
		speed = rigidBody.velocity.magnitude;//.z;
		terminalSpeed = Mathf.Sqrt (2*gravity/(airDensity*wingDragSurfaceArea*dragCoef));

		//flap wings
		bool flapAnimationPlaying = animator.GetCurrentAnimatorStateInfo(0).IsName (flapClip.name) && !animator.IsInTransition(0);
		if (Input.GetButton ("Jump") || Input.GetAxisRaw ("Flap") != 0) {
			if (!isFlapping){// && !flapAnimationPlaying) {
				Debug.Log ("Flap");
				flap = true;
				isFlapping = true;
				animator.Play (flapClip.name);

				rigidBody.AddForceAtPosition (transform.up*flapUpCoef*airDensity, transform.position + wingForwardDistance*transform.forward, ForceMode.Impulse);
				rigidBody.AddForceAtPosition (transform.forward*flapForwardCoef*airDensity, transform.position + wingForwardDistance*transform.forward, ForceMode.Impulse);

//				WaitForAnimation (animator);
			}
		} else {
			isFlapping = false;
		}

		if (Input.GetButton ("Brake") || Input.GetAxisRaw ("Brake") != 0) {
			if (!isBraking && !flapAnimationPlaying) {
				Debug.Log ("Brake");
				brake = true;
				isBraking = true;
				animator.Play (flapClip.name);

//				rigidBody.AddForceAtPosition (transform.up*flapUpCoef*airDensity, transform.position + wingForwardDistance*transform.forward, ForceMode.Impulse);
				Vector3 brakeForce = Vector3.ClampMagnitude (-transform.forward * flapForwardCoef * airDensity, speed);
				rigidBody.AddForceAtPosition (brakeForce, transform.position + wingForwardDistance*transform.forward, ForceMode.Impulse);

				//				WaitForAnimation (animator);
			}
		} else {
			isBraking = false;
		}

		//audio based on speed
		audioSource.pitch = drag * pitchScale;
		audioSource.volume = drag * volumeScale;

		foreach(TrailRenderer trail in trails){
			trail.endWidth = drag * trailScale;
			trail.startWidth = 0f;
			trail.time = 0.5f;
		}

		//rotate wings based on forces and movement
		UpdateRendering();

//		transform.rotation *= Quaternion.Euler (new Vector3 (pitch, 0, -roll));

		//d = r * t
		//t = d / r
//		Quaternion targetRotation = Quaternion.LookRotation (rigidBody.velocity);
//		float rotatePercent = Quaternion.Angle (transform.rotation, targetRotation);
//		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime*rotatePercent/10);
	}

	void UpdateRendering(){
		//rotate wings
		//TODO if not playing a flap animation
		leftWing.localRotation = Quaternion.Euler (leftWingInitialRotation + new Vector3((angleOfAttackLeft-2*angleOffset)*-200000,0,0));
		rightWing.localRotation = Quaternion.Euler (rightWingInitialRotation + new Vector3((angleOfAttackRight-2*angleOffset)*-200000,0,0));

		//TODO rotate more if braking
	}

	void FixedUpdate () {
		//rotate towards motion
		if (rotateTowardsMotion) {
			Vector3 rotation = Quaternion.LookRotation (rigidBody.velocity, transform.up).eulerAngles;
			transform.rotation = Quaternion.Euler (rotation);
		}

		//
		if (stabilizeRotation) {
			//stabilization (to keep the plane facing into the direction it's moving)
			Vector3 dragDirection = -rigidBody.velocity.normalized;
			Vector3 stabilizationForces = -Vector3.Scale (dragDirection, stabilizingDrag) * rigidBody.velocity.magnitude;
			rigidBody.AddForceAtPosition (transform.TransformDirection (stabilizationForces), transform.position - transform.forward * 10);
			rigidBody.AddForceAtPosition (-transform.TransformDirection (stabilizationForces), transform.position + transform.forward * 10);
		}

		//apply lift
//		if (!isFlapping){// || flapAnimationPlaying) {
			WingLiftOneStickV4 ();
//		}

		//apply gravity
		GravityV2();

		//apply drag
		AirDragV3 ();
	}

	void OnTriggerEnter(Collider collisionInfo) {
		Debug.Log (collisionInfo);
		if (collisionInfo.gameObject.CompareTag ("Fish")) {
			GameObject.Destroy (collisionInfo.gameObject);
		}

		//TODO handle crashing: close wings and tumble, slowing down if on ground
	}

	float AngleSigned(Vector3 v1, Vector3 v2, Vector3 normal){
		return Mathf.Atan2 (
			Vector3.Dot (normal, Vector3.Cross (v1, v2)),
			Vector3.Dot (v1, v2)) * Mathf.Rad2Deg;
	}

	void GravityV1(){
		if (!isGrounded ()) {
			Vector3 gravityForce = Vector3.down * gravity;
			rigidBody.AddForce (gravityForce, ForceMode.Force);
			Debug.DrawRay (transform.position+rigidBody.centerOfMass, gravityForce, Color.blue);
		}
	}

	void GravityV2(){
		if (!isGrounded ()) {
			Vector3 gravityForce = Vector3.down * gravity;
			rigidBody.AddForceAtPosition (gravityForce, transform.position + transform.forward * wingForwardDistance, ForceMode.Force);
			Debug.DrawRay (transform.position + transform.forward * wingForwardDistance, gravityForce, Color.blue);
		}
	}

	void AirDragV1(){
		drag = 0.5f * airDensity * speed * speed * dragCoef * wingDragSurfaceArea;
		Vector3 dragForce = transform.forward * (-1) * drag;
		rigidBody.AddForceAtPosition (dragForce, transform.position);
		Debug.DrawRay (transform.position, dragForce, Color.red);
	}

	void AirDragV2(){
		drag = 0.5f * airDensity * speed * speed * dragCoef * wingDragSurfaceArea;
		Vector3 dragForce = transform.forward * (-1) * drag;
		rigidBody.AddForceAtPosition (dragForce, transform.position - transform.forward*wingForwardDistance);
		Debug.DrawRay (transform.position - transform.forward*dragDistance, dragForce, Color.red);
	}

	void AirDragV3(){
		drag = 0.5f * airDensity * speed * speed * dragCoef * wingDragSurfaceArea;
		Vector3 dragForce = rigidBody.velocity.normalized * (-1) * drag;
		rigidBody.AddForceAtPosition (dragForce, transform.position - transform.forward*wingForwardDistance);
		Debug.DrawRay (transform.position - transform.forward*dragDistance, dragForce, Color.red);
	}

	void WingLiftV1(){
		float pitchLeft = -Input.GetAxis ("Vertical");
		float pitchRight = -Input.GetAxis ("Vertical Right");

		//apply lift
		float liftPercent = 1f;// - Input.GetAxis ("Flap");
		float angleBetweenForwardAndSpeed = 0;
		angleOfAttackLeft = Mathf.Deg2Rad * (angleOffset + angleScale * pitchLeft + angleBetweenForwardAndSpeed);
		angleOfAttackRight = Mathf.Deg2Rad * (angleOffset + angleScale * pitchRight + angleBetweenForwardAndSpeed);

		float liftLeft = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackLeft * liftPercent;
		float liftRight = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackRight * liftPercent;

		Debug.Log (liftLeft + " " + liftRight);

		rigidBody.AddForceAtPosition (transform.up * liftLeft, transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftRight, Color.magenta);

		//clamp to maxspeed
		float brakePercent = 1f - Input.GetAxis ("Brake");
		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void WingLiftV2(){
		float pitchLeft = -Input.GetAxis ("Vertical");
		float pitchRight = -Input.GetAxis ("Vertical Right");

		//apply lift
		float liftPercent = 1f;// - Input.GetAxis ("Flap");

		if(staticAngleOffset || pitchLeft >= 0)
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft + angleOffset);
		else
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft - angleOffset);
		
		if(staticAngleOffset || pitchRight >= 0)
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight + angleOffset);
		else
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight - angleOffset);

		float liftLeft = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackLeft * liftPercent;
		float liftRight = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackRight * liftPercent;

//		Debug.Log (angleOfAttackLeft + " " + angleOfAttackRight);

		rigidBody.AddForceAtPosition (transform.up * liftLeft, transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftRight, Color.magenta);

		//clamp to maxspeed
//		float brakePercent = 1f - Input.GetAxis ("Brake");
//		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void WingLiftV3(){
		float pitchLeft = -Input.GetAxis ("Vertical");
		float pitchRight = -Input.GetAxis ("Vertical Right");

		//apply lift
		float liftPercent = 1f;// - Input.GetAxis ("Flap");

		if(staticAngleOffset || pitchLeft >= 0)
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft + angleOffset);
		else
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft - angleOffset);

		if(staticAngleOffset || pitchRight >= 0)
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight + angleOffset);
		else
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight - angleOffset);

		float liftLeft = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackLeft * liftPercent;
		float liftRight = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackRight * liftPercent;

		//		Debug.Log (angleOfAttackLeft + " " + angleOfAttackRight);
		Vector3 up = Vector3.Cross(rigidBody.velocity, transform.right).normalized;
		rigidBody.AddForceAtPosition (up * liftLeft, transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (up * liftRight, transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, up * liftRight, Color.magenta);

		//clamp to maxspeed
		//		float brakePercent = 1f - Input.GetAxis ("Brake");
		//		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);

		Debug.DrawRay (transform.position, transform.right*10, Color.cyan);
	}

	void WingLiftOneStickV1(){
		float y = Input.GetAxis ("Vertical");
		float x = Input.GetAxis ("Horizontal");

		float angle = (4 * Mathf.Abs (Mathf.Atan2 (Mathf.Abs (y), Mathf.Abs (x)) / Mathf.PI) - 1);
		float magnitude = Mathf.Clamp (new Vector2 (x, y).magnitude, 0, 1);

		float pitchLeft = 0f;
		float pitchRight = 0f;

		if (x >= 0) {
			//up right
			if (y >= 0) {
				pitchRight = -magnitude;
				pitchLeft = -Mathf.Clamp (angle*magnitude, -1, 1);
			}
			//down right
			else {
				pitchRight = Mathf.Clamp (angle*magnitude, -1, 1);
				pitchLeft = magnitude;
			}
		} else {
			//up left
			if (y >= 0) {
				pitchRight = -Mathf.Clamp (angle*magnitude, -1, 1);
				pitchLeft = -magnitude;
			}
			//down left
			else {
				pitchRight = magnitude;
				pitchLeft = Mathf.Clamp (angle*magnitude, -1, 1);
			}
		}

		Debug.Log(pitchLeft+" "+pitchRight);

//		float pitchLeft = -Input.GetAxis ("Vertical");
//		float pitchRight = -Input.GetAxis ("Vertical Right");

		//apply lift
		float liftPercent = 1f;// - Input.GetAxis ("Flap");
		if(pitchLeft >= 0)
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft + angleOffset);
		else
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft - angleOffset);

		if(pitchRight >= 0)
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight + angleOffset);
		else
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight - angleOffset);

		//		angleOfAttackLeft = Mathf.Deg2Rad * (angleOffset + angleScale * pitchLeft);
		//		angleOfAttackRight = Mathf.Deg2Rad * (angleOffset + angleScale * pitchRight);

		float liftLeft = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackLeft * liftPercent;
		float liftRight = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackRight * liftPercent;

		Debug.Log (angleOfAttackLeft + " " + angleOfAttackRight);

		rigidBody.AddForceAtPosition (transform.up * liftLeft, transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftRight, Color.magenta);

		//clamp to maxspeed
		//		float brakePercent = 1f - Input.GetAxis ("Brake");
		//		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void WingLiftOneStickV2(){
		float y = Input.GetAxis ("Vertical");
		float x = Input.GetAxis ("Horizontal");

		float angle = (2 * Mathf.Abs (Mathf.Atan2 (Mathf.Abs (y), Mathf.Abs (x)) / Mathf.PI));
		Debug.Log (angle);
		float magnitude = Mathf.Clamp (new Vector2 (x, y).magnitude, 0, 1);

		float pitchLeft = 0f;
		float pitchRight = 0f;

		if (x >= 0) {
			//up right
			if (y >= 0) {
				pitchRight = -magnitude;
				pitchLeft = -Mathf.Clamp (angle*magnitude, -1, 1);
			}
			//down right
			else {
				pitchRight = Mathf.Clamp (angle*magnitude, -1, 1);
				pitchLeft = magnitude;
			}
		} else {
			//up left
			if (y >= 0) {
				pitchRight = -Mathf.Clamp (angle*magnitude, -1, 1);
				pitchLeft = -magnitude;
			}
			//down left
			else {
				pitchRight = magnitude;
				pitchLeft = Mathf.Clamp (angle*magnitude, -1, 1);
			}
		}

//		Debug.Log(pitchLeft+" "+pitchRight);

		//		float pitchLeft = -Input.GetAxis ("Vertical");
		//		float pitchRight = -Input.GetAxis ("Vertical Right");

		//apply lift
		float liftPercent = 1f;// - Input.GetAxis ("Flap");
		if(pitchLeft >= 0)
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft + angleOffset);
		else
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft - angleOffset);

		if(pitchRight >= 0)
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight + angleOffset);
		else
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight - angleOffset);

		//		angleOfAttackLeft = Mathf.Deg2Rad * (angleOffset + angleScale * pitchLeft);
		//		angleOfAttackRight = Mathf.Deg2Rad * (angleOffset + angleScale * pitchRight);

		float liftLeft = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackLeft * liftPercent;
		float liftRight = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackRight * liftPercent;

//		Debug.Log (angleOfAttackLeft + " " + angleOfAttackRight);

		rigidBody.AddForceAtPosition (transform.up * liftLeft, transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftRight, Color.magenta);

		//clamp to maxspeed
		//		float brakePercent = 1f - Input.GetAxis ("Brake");
		//		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void WingLiftOneStickV3(){
		float y = Input.GetAxis ("Vertical");
		float x = Input.GetAxis ("Horizontal");

		float angle = (2 * Mathf.Abs (Mathf.Atan2 (Mathf.Abs (y), Mathf.Abs (x)) / Mathf.PI));
		float doubleAngle = 2 * angle - 1;
		Debug.Log (angle);
		float magnitude = Mathf.Clamp (new Vector2 (x, y).magnitude, 0, 1);

		float pitchLeft = 0f;
		float pitchRight = 0f;

		if (x >= 0) {
			//up right
			if (y >= 0) {
				pitchRight = -magnitude;
				pitchLeft = -Mathf.Clamp (angle*magnitude, -1, 1);
			}
			//down right
			else {
				pitchRight = Mathf.Clamp (doubleAngle*magnitude, -1, 1);
				pitchLeft = Mathf.Clamp (angle*magnitude, -1, 1);
			}
		} else {
			//up left
			if (y >= 0) {
				pitchRight = -Mathf.Clamp (angle*magnitude, -1, 1);
				pitchLeft = -magnitude;
			}
			//down left
			else {
				pitchRight = Mathf.Clamp (angle*magnitude, -1, 1);
				pitchLeft = Mathf.Clamp (doubleAngle*magnitude, -1, 1);
			}
		}

//		Debug.Log(angle+" "+doubleAngle+"\n"+pitchLeft+" "+pitchRight);

		//apply lift
		float liftPercent = 1f;// - Input.GetAxis ("Flap");
		if(pitchLeft >= 0)
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft + angleOffset);
		else
			angleOfAttackLeft = Mathf.Deg2Rad * (angleScale * pitchLeft - angleOffset);

		if(pitchRight >= 0)
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight + angleOffset);
		else
			angleOfAttackRight = Mathf.Deg2Rad * (angleScale * pitchRight - angleOffset);

		//		angleOfAttackLeft = Mathf.Deg2Rad * (angleOffset + angleScale * pitchLeft);
		//		angleOfAttackRight = Mathf.Deg2Rad * (angleOffset + angleScale * pitchRight);

		float liftLeft = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackLeft * liftPercent;
		float liftRight = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * angleOfAttackRight * liftPercent;

		//		Debug.Log (angleOfAttackLeft + " " + angleOfAttackRight);

		rigidBody.AddForceAtPosition (transform.up * liftLeft, transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftRight, Color.magenta);

		//clamp to maxspeed
		//		float brakePercent = 1f - Input.GetAxis ("Brake");
		//		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void WingLiftOneStickV4(){
		float y = Input.GetAxis ("Vertical");
		float x = Input.GetAxis ("Horizontal");

		//		angleOfAttackLeft = Mathf.Deg2Rad * (angleOffset + angleScale * pitchLeft);
		//		angleOfAttackRight = Mathf.Deg2Rad * (angleOffset + angleScale * pitchRight);

		float liftLeft = 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * x;
		float liftRight = -0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * x;
		float liftForward = -0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * y;

		//		Debug.Log (angleOfAttackLeft + " " + angleOfAttackRight);

		rigidBody.AddForceAtPosition (transform.up * liftLeft, transform.position - wingOutDistance*transform.right, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftForward, transform.position + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - wingOutDistance*transform.right, transform.up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + wingOutDistance*transform.right, transform.up * liftRight, Color.magenta);
		Debug.DrawRay (transform.position + wingForwardDistance*transform.forward, transform.up * liftRight, Color.yellow);

		//clamp to maxspeed
		//		float brakePercent = 1f - Input.GetAxis ("Brake");
		//		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void OriginalLift (){
		float roll = Input.GetAxis ("Horizontal");
		float pitch = Input.GetAxis ("Vertical");

		//apply gravity
		rigidBody.AddForce (Vector3.down*gravity, ForceMode.Force);

		speed = rigidBody.velocity.magnitude;//.z;

		//apply lift
		if (!Input.GetButton ("Jump")) {
			float angleOfAttack = 0.05f + -0.5f * pitch;
			lift = (0.5f) * liftCoef * 1.29f * wingLiftSurfaceArea * speed * speed * Mathf.Deg2Rad * angleOfAttack;
			rigidBody.AddForce (transform.up * lift, ForceMode.Force);
		}

		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		var lookPos = rigidBody.velocity;
		//		lookPos.x = 0;
		var rotation = Quaternion.LookRotation(lookPos).eulerAngles;
		if (rotation.x < 270 && rotation.x > 230)
			rotation.x = 275;
		if (rotation.x > 80 && rotation.x < 120)
			rotation.x = 80;
		rotation.z -= roll;// * rollSpeed;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler (rotation), Time.deltaTime * 8);
	}

	private bool isGrounded(){
		Debug.DrawLine (characterCollider.bounds.center, new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z), Color.red);
		return Physics.CheckCapsule (
			characterCollider.bounds.center,
			new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z),
			0.18f,
			layerMaskForGround.value
		);
	}

	private IEnumerable WaitForAnimation(Animation animation){
		do {
			yield return null;
		} while(animation.isPlaying);
	}
}