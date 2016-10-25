using UnityEngine;
using System.Collections;

public class Glide : MonoBehaviour {
	public bool isGrounded;
	public Vector3 centerOfGravity;
	private Vector3 center;
	public bool landed;
	public float gravity;
	public float gravityForwardDistance;
	public float gravityDownDistance;
	public bool keepUprightAlways;

	private Vector3 groundNormal;

	public bool rotateTowardsMotion;

	public float liftCoef;
	public float lift;
	public float rollScale;

	public float dragCoef;
	public float dragForwardDistance;
	public float drag;

	public float flapAnimationScale;
	public float flapScale;
	public float flapUpCoef;
	public float flapForwardCoef;

	public float wingLiftSurfaceArea;
	public float wingDragSurfaceArea;
	public float airDensity;

	public float walkSpeed;
	public float walkTurnSpeed;

	public float speed;
	public float maxSpeed;
	public float terminalSpeed;

	public Rigidbody rigidBody;
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

	public AudioSource airAudioSource;
	public AudioSource flapAudioSource;
	private bool playingFlapSound = false;
	public float flapSoundRate;
	public float minFlapRate;
	public float flapMinPitch;
	public float flapMaxPitch;

	public float pitchScale = 1.5f;
	public float volumeScale = 0.7f;
	public float trailScale = 0.3f;

	private Collider characterCollider;

	private TrailRenderer[] trails;

	public LayerMask layerMaskForGround;

	public float angleOfAttackLeft;
	public float angleOfAttackRight;

	public Transform leftWing;
	private Vector3 leftWingInitialRotation;
	public Transform rightWing;
	private Vector3 rightWingInitialRotation;

	public float roll;
	public float pitch;
	public float forward;
	public float right;
	public float turn;
	public float flapSpeed;
	public float flapDirection;
	public float flapHorizontalDirection;

	// Use this for initialization
	void Start () {
		rigidBody = transform.GetComponent<Rigidbody> ();
		animator = transform.GetComponentInChildren<Animator> ();
		trails = transform.GetComponentsInChildren<TrailRenderer> ();
		characterCollider = transform.GetComponent<Collider> ();

		leftWingInitialRotation = leftWing.localRotation.eulerAngles;
		rightWingInitialRotation = rightWing.localRotation.eulerAngles;

		airDensity = 1.2238f;

		rigidBody.centerOfMass = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update() {
		CheckGround ();

		//set air density based on height
//		airDensity = 1.2238f * Mathf.Pow(1f - (0.0000226f * transform.position.y), 5.26f);

		//get current speed
		speed = rigidBody.velocity.magnitude;//.z;
		terminalSpeed = Mathf.Sqrt (2*gravity/(airDensity*wingDragSurfaceArea*dragCoef));

		//flap wings
		if (flapSpeed != 0) {
			isFlapping = true;
			animator.SetBool ("flap", true);
			animator.speed = 1f + flapAnimationScale * flapSpeed;

			if(!playingFlapSound){
				StartCoroutine(PlayFlapSound(flapSoundRate*(1-flapSpeed) + minFlapRate));
				playingFlapSound = true;
				flapAudioSource.pitch = Random.Range (flapMinPitch, flapMaxPitch);
			}
		} else {
			animator.SetBool ("flap", false);
			isFlapping = false;
			animator.speed = 1f;
		}

		//audio based on speed
		airAudioSource.pitch = drag * pitchScale;
		airAudioSource.volume = drag * volumeScale;

		foreach(TrailRenderer trail in trails){
			trail.endWidth = drag * trailScale;
			trail.startWidth = 0f;
			trail.time = 0.5f;
		}

		//rotate wings based on forces and movement
		UpdateRendering();
	}

	IEnumerator PlayFlapSound(float wait){
		flapAudioSource.Play();
		yield return new WaitForSeconds(wait);
		playingFlapSound = false;
	}

	void UpdateRendering(){
		//rotate wings
		if (!isGrounded) {
			leftWing.localRotation = Quaternion.Euler (leftWingInitialRotation + new Vector3 ((flapDirection) * 15, -(flapDirection) * 20, 0));
			rightWing.localRotation = Quaternion.Euler (rightWingInitialRotation + new Vector3 ((flapDirection) * 15, (flapDirection) * 20, 0));
		} else {
			leftWing.localRotation = Quaternion.Euler (leftWingInitialRotation);
			rightWing.localRotation = Quaternion.Euler (rightWingInitialRotation);
		}

		animator.SetBool ("wingsClosed", isGrounded);
	}

	void FixedUpdate () {
		center = transform.position + centerOfGravity.x * transform.right + centerOfGravity.y * transform.up + centerOfGravity.z * transform.forward;

		//rotate towards motion
		if (rotateTowardsMotion) {
			Vector3 rotation = Quaternion.LookRotation (rigidBody.velocity, transform.up).eulerAngles;
			transform.rotation = Quaternion.Euler (rotation);
		}

		if (isFlapping) {
			Flap ();
//			FlapCentered ();
		}
			
		//apply gravity
		if (!isGrounded) {
			landed = false;
			if (keepUprightAlways) {
				GravityV3 ();
//				GravityCentered ();
			} else {
				GravityV1 ();
			}
		} else if (flapSpeed != 0) {
			GravityV3 ();
//			GravityCentered ();
		} else {
			GravityV4 ();
			if (rigidBody.velocity.magnitude <= 0.01f) {
				landed = true;
			}
		}

		if (landed) {
//			rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
			rigidBody.drag = 1;
			Walk ();
		} else {
			rigidBody.constraints = RigidbodyConstraints.None;
			rigidBody.drag = 0;
			RealisticLift();
//			LiftCentered ();
		}
	}

	void Walk(){
//		Vector3 walkDirection = Vector3.ClampMagnitude (transform.forward * forward + transform.right * right, 1);
//		rigidBody.AddForceAtPosition (walkDirection * walkSpeed, transform.position);

		rigidBody.AddForceAtPosition (transform.forward*forward*walkSpeed, transform.position);
		rigidBody.AddForceAtPosition (transform.right*right*walkSpeed, transform.position);
//		rigidBody.AddTorque (transform.up*turn*walkTurnSpeed);

//		Quaternion desiredNormal = Quaternion.LookRotation(Vector3.Exclude(groundNormal, transform.forward), groundNormal);
//		Vector3 forwardVector = rigidBody.velocity.magnitude > 0f ? rigidBody.velocity : transform.forward;
		Vector3 forwardVector = transform.forward + transform.right*turn*walkTurnSpeed;
		Quaternion desiredNormal = Quaternion.LookRotation(Vector3.Exclude(groundNormal, forwardVector), groundNormal);
		transform.rotation = Quaternion.Slerp (transform.rotation, desiredNormal, Time.fixedDeltaTime);
	}

	void OnTriggerEnter(Collider collisionInfo) {
		Debug.Log (collisionInfo);
		if (collisionInfo.gameObject.CompareTag ("Fish")) {
			GameObject.Destroy (collisionInfo.gameObject);
		}

		//TODO handle crashing: close wings and tumble, slowing down if on ground
	}

	float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
	{
		Vector3 perpVector;
		float angle;

		//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
		perpVector = Vector3.Cross(normal, referenceVector);

		//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
		angle = Vector3.Angle(referenceVector, otherVector);
		angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

		return angle;
	}

	void Flap(){
		Vector3 flapAngle = (flapHorizontalDirection * transform.right * 0.5f + flapDirection * transform.forward + (1f - Mathf.Abs (flapDirection)) * transform.up).normalized;
		Vector3 flapForce = flapAngle * flapForwardCoef * flapScale * flapSpeed;

		rigidBody.AddForceAtPosition (flapForce, transform.position + transform.forward*wingForwardDistance*flapDirection);

		Debug.DrawRay (transform.position + transform.forward*wingForwardDistance*flapDirection, flapForce);
	}

	void FlapCentered(){
		Vector3 flapAngle = (flapHorizontalDirection * transform.right * 0.5f + flapDirection * transform.forward + (1f - Mathf.Abs (flapDirection)) * transform.up).normalized;
		Vector3 flapForce = flapAngle * flapForwardCoef * flapScale * flapSpeed;

		rigidBody.AddForceAtPosition (flapForce, center);

		Debug.DrawRay (center, flapForce);
	}

	void GravityV1(){
		Vector3 gravityForce = Vector3.down * gravity;
		rigidBody.AddForceAtPosition (gravityForce, transform.position + transform.forward * gravityForwardDistance, ForceMode.Force);
		Debug.DrawRay (transform.position + transform.forward * gravityForwardDistance, gravityForce, Color.blue);
	}

	void GravityV2(){
		Vector3 gravityForce = Vector3.down * gravity;
		rigidBody.AddForceAtPosition (gravityForce, transform.position + transform.forward * wingForwardDistance, ForceMode.Force);
		Debug.DrawRay (transform.position + transform.forward * wingForwardDistance, gravityForce, Color.blue);
	}

	void GravityV3(){
		Vector3 gravityForce = Vector3.down * gravity;
		rigidBody.AddForceAtPosition (gravityForce, transform.position - transform.up * gravityDownDistance + transform.forward * gravityForwardDistance, ForceMode.Force);
		Debug.DrawRay (transform.position - transform.up * gravityDownDistance + transform.forward * gravityForwardDistance, gravityForce, Color.gray);
	}

	void GravityV4(){
		Vector3 gravityForce = -groundNormal * gravity;
//		rigidBody.AddForceAtPosition (gravityForce, transform.position - transform.up * 1 + transform.forward * gravityForwardDistance, ForceMode.Force);
		rigidBody.AddForceAtPosition (gravityForce, transform.position - transform.up * 1 + transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (gravityForce, transform.position - transform.up * 1 - transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - transform.up * gravityDownDistance + transform.forward * gravityForwardDistance, gravityForce, Color.gray);
	}

	void GravityCentered(){
		Vector3 gravityForce = -groundNormal * gravity;
		rigidBody.AddForceAtPosition (gravityForce, center, ForceMode.Force);
		Debug.DrawRay (center, gravityForce, Color.gray);
	}

	void RealisticLift(){
		float angleOfAttack = SignedVectorAngle(transform.forward, rigidBody.velocity, transform.right) - pitch*angleScale;

		if (angleOfAttack > 180)
			angleOfAttack -= 360;
		if (angleOfAttack < -180)
			angleOfAttack += 360;

		float realLiftCoef = liftCoef * Mathf.Sin (angleOfAttack * Mathf.PI / 180f);

		float liftForward = 0.5f * realLiftCoef * airDensity * wingLiftSurfaceArea * speed * speed;
		rigidBody.AddForceAtPosition (transform.up * liftForward, transform.position + wingForwardDistance*transform.forward, ForceMode.Force);
		Debug.DrawRay (transform.position + wingForwardDistance*transform.forward, transform.up * liftForward, Color.yellow);


		//roll lift
		float liftRoll = Mathf.Abs (rollScale * 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * roll);
//		float liftRight = rollScale * -0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * roll;

		rigidBody.AddForceAtPosition (transform.up * liftRoll, transform.position - wingOutDistance*transform.right * Mathf.Sign (roll), ForceMode.Force);
//		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right, ForceMode.Force);

		Debug.DrawRay (transform.position + wingOutDistance*transform.right * Mathf.Sign (roll), transform.up * liftRoll, Color.magenta);
//		Debug.DrawRay (transform.position + wingOutDistance*transform.right, transform.up * liftRight, Color.magenta);


		//induced drag
		float aspectRatio = 1f/wingLiftSurfaceArea;
		float inducedDragCoef = realLiftCoef * realLiftCoef * wingLiftSurfaceArea / Mathf.PI;
		float realDragCoef = dragCoef + inducedDragCoef;

		drag = realDragCoef * 0.5f * airDensity * speed * speed * wingDragSurfaceArea;
		Vector3 dragForce = rigidBody.velocity.normalized * (-1) * drag;
//		Vector3 dragForce = transform.forward * (-1) * drag;
		rigidBody.AddForceAtPosition (dragForce, transform.position - transform.forward*dragForwardDistance);
		Debug.DrawRay (transform.position - transform.forward*dragForwardDistance, dragForce, Color.red);


		//velocity
		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void LiftCentered(){
		float angleOfAttack = SignedVectorAngle(transform.forward, rigidBody.velocity, transform.right) - pitch*angleScale;

		if (angleOfAttack > 180)
			angleOfAttack -= 360;
		if (angleOfAttack < -180)
			angleOfAttack += 360;

		float realLiftCoef = liftCoef * Mathf.Sin (angleOfAttack * Mathf.PI / 180f);

		float liftForward = 0.5f * realLiftCoef * airDensity * wingLiftSurfaceArea * speed * speed;
		rigidBody.AddForceAtPosition (transform.up * liftForward, center, ForceMode.Force);
		Debug.DrawRay (center, transform.up * liftForward, Color.yellow);


		//roll lift
		float liftRoll = Mathf.Abs (rollScale * 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * roll);
		//		float liftRight = rollScale * -0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * roll;

		rigidBody.AddForceAtPosition (transform.up * liftRoll, center - wingOutDistance*transform.right * Mathf.Sign (roll), ForceMode.Force);
		//		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right, ForceMode.Force);

		Debug.DrawRay (center + wingOutDistance*transform.right * Mathf.Sign (roll), transform.up * liftRoll, Color.magenta);
		//		Debug.DrawRay (transform.position + wingOutDistance*transform.right, transform.up * liftRight, Color.magenta);


		//induced drag
		float aspectRatio = 1f/wingLiftSurfaceArea;
		float inducedDragCoef = realLiftCoef * realLiftCoef * wingLiftSurfaceArea / Mathf.PI;
		float realDragCoef = dragCoef + inducedDragCoef;

		drag = realDragCoef * 0.5f * airDensity * speed * speed * wingDragSurfaceArea;
		Vector3 dragForce = rigidBody.velocity.normalized * (-1) * drag;
		//		Vector3 dragForce = transform.forward * (-1) * drag;
		rigidBody.AddForceAtPosition (dragForce, transform.position - transform.forward*dragForwardDistance);
		Debug.DrawRay (transform.position - transform.forward*dragForwardDistance, dragForce, Color.red);


		//velocity
		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void CheckGround(){
		Debug.DrawLine (characterCollider.bounds.center, new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z), Color.red);

		isGrounded = Physics.CheckCapsule (
			characterCollider.bounds.center,
			new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z),
			0.18f,
			layerMaskForGround.value
		);
		if (isGrounded) {
			RaycastHit hit;
			if (Physics.Raycast (transform.position, -transform.up, out hit, 1.2f, layerMaskForGround)) {
				groundNormal = hit.normal;
			}
		}
	}

	private IEnumerable WaitForAnimation(Animation animation){
		do {
			yield return null;
		} while(animation.isPlaying);
	}
}
