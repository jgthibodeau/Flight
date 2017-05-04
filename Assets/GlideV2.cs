using UnityEngine;
using System.Collections;

public class GlideV2 : MonoBehaviour {
	[HideInInspector]
	public BirdAnimator birdAnimator;
	[HideInInspector]
	public Rigidbody rigidBody;
	[HideInInspector]
	private Collider characterCollider;
	[HideInInspector]
	private TrailRenderer[] trails;
	[HideInInspector]
	public bool isGrounded;
	[HideInInspector]
	public Vector3 groundNormal;
	public float speed;
	[HideInInspector]
	public Player playerScript;

	public float gravity;

	public bool rotateTowardsMotion;

	public float liftCoef;
	public float lift;
	public float liftForwardAmount;
	public float maxLift;

	public float rollScale;
	public float maxRoll;

	public float maxTailDrag;
	public float dragCoef;
	public float tailDragCoef;
	public float dragForwardDistance;
	public float drag;

	public float flapAnimationScale;
	public float flapScale;
	public float flapUpCoef;
	public float flapForwardCoef;
	public ForceMode flapForceMode;
	public float antiFlapTorqueScale;

	public float wingLiftSurfaceArea;
	public float wingDragSurfaceArea;
	public float wingSpan;
	public float airDensity;

	public float flapTime;
	public bool flap = false;
	public bool impulseFlapping = false;
	private float[] prevFlapSpeeds = new float[]{0f,0f,0f};
	private bool flapAccelerating;
	private float flapAcceleration = 0.0f;
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
	public float trailStartWidth = 0f;
	public float trailTime = 0.5f;

	public float angleOfAttackLeft;
	public float angleOfAttackRight;

	public Transform leftWing;
	private Vector3 leftWingInitialRotation;
	public Transform rightWing;
	private Vector3 rightWingInitialRotation;

	//Inputs
	[HideInInspector]
	public float roll, pitch, tailPitch, yaw, forward, right, flapSpeed, flapDirection;
	[HideInInspector]
	public bool wingsOut;

	// Use this for initialization
	void Start () {
		trails = transform.GetComponentsInChildren<TrailRenderer> ();

		playerScript = GetComponent<Player> ();

		airDensity = 1.2238f;

		StartCoroutine(DiscreteFlap());
	}

	IEnumerator DiscreteFlap() {
		while(true)
		{
			if (isFlapping) {
				//				birdAnimator.Flapping = true;
				impulseFlapping = true;
				yield return new WaitForSeconds (flapTime * 0.5f);
				yield return new WaitForSeconds (flapTime * 0.5f);
				impulseFlapping = false;
				//				birdAnimator.Flapping = false;
			} else {
				yield return null;
			}
		}
	}

	// Update is called once per frame
	void Update() {
		//		prevFlapSpeeds [0] = prevFlapSpeeds [1];
		//		prevFlapSpeeds [1] = prevFlapSpeeds [2];
		//		prevFlapSpeeds [2] = flapSpeed;
		//		float newRate = Mathf.Clamp (prevFlapSpeeds [2] - prevFlapSpeeds [1], 0f, 1f);
		//		float oldRate = Mathf.Clamp (prevFlapSpeeds [1] - prevFlapSpeeds [0], 0f, 1f);
		//
		//		bool wasFlapAcceleration = flapAccelerating;
		//		flapAccelerating = newRate > oldRate;
		//
		//		if (wasFlapAcceleration && !flapAccelerating) {
		//			flapAcceleration = oldRate;
		//			impulseFlapping = true;
		//		} else {
		//			impulseFlapping = false;
		//		}

		//set air density based on height
		//		airDensity = 1.2238f * Mathf.Pow(1f - (0.0000226f * transform.position.y), 5.26f);

		speed = rigidBody.velocity.magnitude;
		//		terminalSpeed = Mathf.Sqrt (2*gravity/(airDensity*wingDragSurfaceArea*dragCoef));

		//flap wings
		if (flapSpeed != 0) {
			isFlapping = true;
			//			animator.SetBool ("Flapping", true);
			birdAnimator.FlapSpeed = 2f;// + flapAnimationScale * flapSpeed;
			birdAnimator.Flapping = true;

			if(!playingFlapSound){
				StartCoroutine(PlayFlapSound(flapSoundRate*(1-flapSpeed) + minFlapRate));
				playingFlapSound = true;
				flapAudioSource.pitch = Random.Range (flapMinPitch, flapMaxPitch);
			}
		} else {
			//			animator.SetBool ("Flapping", false);
			isFlapping = false;
			//			animator.speed = 1f;
			birdAnimator.Flapping = false;
		}

		//audio based on speed
		airAudioSource.pitch = speed * pitchScale;
		airAudioSource.volume = speed * volumeScale;

		foreach(TrailRenderer trail in trails){
			trail.endWidth = speed * trailScale;
			trail.startWidth = trailStartWidth;
			trail.time = trailTime;
		}

		birdAnimator.WingsOut = wingsOut;
	}

	IEnumerator PlayFlapSound(float wait){
		flapAudioSource.Play();
		yield return new WaitForSeconds(wait);
		playingFlapSound = false;
	}

	void FixedUpdate () {
		if (impulseFlapping) {
			//			FlapImpulse ();
			//			SteadyFlap ();
			SteadyFlapDirectional ();
		}

		if (!isGrounded) {
			//			rigidBody.constraints = RigidbodyConstraints.None;
			rigidBody.drag = 0;

			//rotate towards motion
			if (rotateTowardsMotion) {
				Vector3 rotation = Quaternion.LookRotation (rigidBody.velocity, transform.up).eulerAngles;
				transform.rotation = Quaternion.Euler (rotation);
			}

			//			rigidBody.constraints = RigidbodyConstraints.None;
			rigidBody.drag = 0;
			AngledDragLift ();
		} else {
			drag = 0;
		}
	}

	void SteadyFlap(){
		Vector3 flapForce = (transform.forward + transform.up).normalized * flapForwardCoef * flapScale * flapSpeed;

		rigidBody.AddForce (flapForce);

		Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce, Color.red);
	}

	void SteadyFlapDirectional(){
		float realFlapSpeed = flapSpeed * (flapDirection + 3) / 4;
		Vector3 flapForce = (transform.forward * flapDirection + transform.up * (1 - Mathf.Abs (flapDirection))).normalized * flapForwardCoef * flapScale * realFlapSpeed;

		Vector3 flapPosition = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z;
		//		Vector3 aVel = rigidBody.angularVelocity;
		rigidBody.AddForce (flapForce, flapForceMode);
		//		rigidBody.angularVelocity = aVel;
		//		rigidBody.AddTorque (transform.right * antiFlapTorqueScale * flapForce.magnitude);

		Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce, Color.red);
	}

	void FlapImpulse(){
		impulseFlapping = false;

		Vector3 direction = (transform.up + transform.forward*flapDirection).normalized;

		Vector3 flapForce = direction * flapForwardCoef * flapScale * flapAcceleration;

		rigidBody.AddForce (flapForce, ForceMode.Force);

		Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce);
	}

	void AngledDragLift(){
		float realLiftCoef = 0f;
		float lift = 0f;

		//get angle of attack realistically by comparing velocity to forward direction
		float angleOfAttack = angleOffset + Util.SignedVectorAngle (transform.forward, rigidBody.velocity, transform.right);// - pitch*angleScale;
		if (angleOfAttack > 180)
			angleOfAttack -= 360;
		if (angleOfAttack < -180)
			angleOfAttack += 360;

		if(wingsOut){

			realLiftCoef = liftCoef * Mathf.Sin (angleOfAttack * Mathf.PI / 180f);

			//			realLiftCoef *= (1f - 0.5f*flapDirection);
			//			realLiftCoef *= (1f - 1*flapDirection);

			lift = 0.5f * airDensity * speed * speed * wingLiftSurfaceArea * realLiftCoef;
			lift = Mathf.Clamp (lift, -maxLift, maxLift);
			Vector3 liftForce = transform.up * lift;

			//TODO make this more seamless
//			if (liftForward > 0 && transform.rotation.eulerAngles.x < -270) {
//				Debug.Log ("flying too high");
//				liftForce = Vector3.zero;
//			}
//
//			else if (liftForward < 0 && transform.rotation.eulerAngles.x > 80) {
//				Debug.Log ("flying too low");
//				liftForce = Vector3.zero;
//			}

//			rigidBody.AddForceAtPosition (liftForce, transform.position, ForceMode.Force);
//			rigidBody.AddForce (liftForce, ForceMode.Force);
//			Util.DrawRigidbodyRay(rigidBody, transform.position, liftForce, Color.yellow);

			Vector3 leftDirection = transform.up;
			if (roll < 0) {
				leftDirection -= transform.right * roll;
				leftDirection.Normalize ();
			}
			Vector3 leftForce = leftDirection * lift;
			Vector3 leftPosition = transform.position - transform.right * wingOutDistance;
			rigidBody.AddForceAtPosition (leftForce, leftPosition, ForceMode.Force);
			Util.DrawRigidbodyRay(rigidBody, leftPosition, leftForce, Color.yellow);

			Vector3 rightDirection = transform.up;
			if (roll > 0) {
				rightDirection -= transform.right * roll;
				rightDirection.Normalize ();
			}
			Vector3 rightForce = rightDirection * lift;
			Vector3 rightPosition = transform.position + transform.right * wingOutDistance;
			rigidBody.AddForceAtPosition (rightForce, rightPosition, ForceMode.Force);
			Util.DrawRigidbodyRay(rigidBody, rightPosition, rightForce, Color.magenta);

//			//roll lift
//			float liftRoll = Mathf.Clamp (rollScale * 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed, -maxRoll, maxRoll);
//			liftRoll *= Mathf.Abs (roll);
//			//		float liftRight = rollScale * -0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * roll;
//
//			rigidBody.AddForceAtPosition (transform.up * liftRoll, transform.position - wingOutDistance*transform.right * Mathf.Sign (roll), ForceMode.Force);
//			//		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right, ForceMode.Force);
//
//			Util.DrawRigidbodyRay(rigidBody, transform.position + wingOutDistance*transform.right * Mathf.Sign (roll), transform.up * liftRoll, Color.magenta);
//			//		Util.DrawRigidbodyRay(rigidBody, transform.position + wingOutDistance*transform.right, transform.up * liftRight, Color.magenta);
		}

		Drag (realLiftCoef, angleOfAttack, Mathf.Abs (lift));

//		//induced drag
//		float aspectRatio = 1f/wingLiftSurfaceArea;
//		float inducedDragCoef = realLiftCoef * realLiftCoef * wingLiftSurfaceArea/ Mathf.PI;
//		float realDragCoef = dragCoef + inducedDragCoef;
//
//		//drag based on wingspan
////		realDragCoef *= (1f - 0.5f*flapDirection);
//
//		drag = realDragCoef * 0.5f * airDensity * speed * speed * wingDragSurfaceArea;
//		Vector3 dragForce = rigidBody.velocity.normalized * (-1) * drag;
//		Vector3 dragPosition = transform.position - transform.forward * dragForwardDistance;
//		rigidBody.AddForceAtPosition (dragForce, dragPosition);
//		Util.DrawRigidbodyRay(rigidBody, dragPosition, dragForce, Color.green);
//
//		//tail drag
//		float realTailDragCoef = tailDragCoef * 0.5f * airDensity * speed * speed;
//		Vector3 tailDragAngle = (tailPitch * transform.up/*  + yaw * transform.right*/).normalized;
//		Vector3 tailDrag = realTailDragCoef * new Vector2 (pitch, yaw).magnitude * tailDragAngle;
//		tailDrag = Vector3.ClampMagnitude (tailDrag, maxTailDrag);
//		rigidBody.AddForceAtPosition (tailDrag, transform.position);
//		Util.DrawRigidbodyRay(rigidBody, transform.position, tailDrag, Color.magenta);


		//velocity
		Util.DrawRigidbodyRay(rigidBody, transform.position, rigidBody.velocity, Color.cyan);
	}

	public void Drag (float liftCoef, float angleOfAttack, float lift) {
		//induced drag
		float aspectRatio = 1f/wingLiftSurfaceArea;
		float inducedDragCoef = liftCoef * liftCoef / (Mathf.PI * aspectRatio);
		float inducedDragMagnitude = 0.5f * airDensity * speed * speed * wingLiftSurfaceArea * inducedDragCoef;
		Vector3 inducedDragForce = inducedDragMagnitude * rigidBody.velocity.normalized * (-1);

		Vector3 position = transform.position + transform.right * wingOutDistance;
		rigidBody.AddForceAtPosition (inducedDragForce, position, ForceMode.Force);
		Util.DrawRigidbodyRay(rigidBody, position, inducedDragForce, Color.white);

		position = transform.position - transform.right * wingOutDistance;
		rigidBody.AddForceAtPosition (inducedDragForce, position, ForceMode.Force);
		Util.DrawRigidbodyRay(rigidBody, position, inducedDragForce, Color.white);

		//parasitic drag
		float parasiticDragMagnitude = 0.5f * airDensity * speed * speed * wingDragSurfaceArea * tailDragCoef;

		Vector3 parasiticDirection = (rigidBody.velocity * (-1) + transform.up * pitch * angleScale).normalized;
		Vector3 parasiticDragForce = parasiticDragMagnitude * parasiticDirection;
		Vector3 parasiticPosition = transform.position - transform.forward * dragForwardDistance;
		rigidBody.AddForceAtPosition (parasiticDragForce, parasiticPosition, ForceMode.Force);
		Util.DrawRigidbodyRay(rigidBody, parasiticPosition, parasiticDragForce, Color.red);



//		//induced drag
//		float aspectRatio = 1f/wingLiftSurfaceArea;
//		float inducedDragCoef = liftCoef * liftCoef * wingLiftSurfaceArea/ Mathf.PI;
//		float realDragCoef = dragCoef + inducedDragCoef;
//
//		//drag based on wingspan
//		//		realDragCoef *= (1f - 0.5f*flapDirection);
//
//		drag = realDragCoef * 0.5f * airDensity * speed * speed * wingDragSurfaceArea;
//		Vector3 dragForce = rigidBody.velocity.normalized * (-1) * drag;
//		Vector3 dragPosition = transform.position - transform.forward * dragForwardDistance;
//		rigidBody.AddForceAtPosition (dragForce, dragPosition);
//		Util.DrawRigidbodyRay(rigidBody, dragPosition, dragForce, Color.green);
//
//		//tail drag
//		float realTailDragCoef = tailDragCoef * 0.5f * airDensity * speed * speed;
//		Vector3 tailDragAngle = (tailPitch * transform.up/*  + yaw * transform.right*/).normalized;
//		Vector3 tailDrag = realTailDragCoef * new Vector2 (pitch, yaw).magnitude * tailDragAngle;
//		tailDrag = Vector3.ClampMagnitude (tailDrag, maxTailDrag);
//		rigidBody.AddForceAtPosition (tailDrag, transform.position);
//		Util.DrawRigidbodyRay(rigidBody, transform.position, tailDrag, Color.magenta);
	}

//	// Use this for initialization
//	void Start () {
//		trails = transform.GetComponentsInChildren<TrailRenderer> ();
//
//		playerScript = GetComponent<Player> ();
//
//		airDensity = 1.2238f;
//
//		StartCoroutine(DiscreteFlap());
//	}
//
//	IEnumerator DiscreteFlap() {
//		while(true)
//		{
//			if (isFlapping) {
////				birdAnimator.Flapping = true;
//				impulseFlapping = true;
//				yield return new WaitForSeconds (flapTime * 0.5f);
//				yield return new WaitForSeconds (flapTime * 0.5f);
//				impulseFlapping = false;
////				birdAnimator.Flapping = false;
//			} else {
//				yield return null;
//			}
//		}
//	}
//
//	// Update is called once per frame
//	void Update() {
////		prevFlapSpeeds [0] = prevFlapSpeeds [1];
////		prevFlapSpeeds [1] = prevFlapSpeeds [2];
////		prevFlapSpeeds [2] = flapSpeed;
////		float newRate = Mathf.Clamp (prevFlapSpeeds [2] - prevFlapSpeeds [1], 0f, 1f);
////		float oldRate = Mathf.Clamp (prevFlapSpeeds [1] - prevFlapSpeeds [0], 0f, 1f);
////
////		bool wasFlapAcceleration = flapAccelerating;
////		flapAccelerating = newRate > oldRate;
////
////		if (wasFlapAcceleration && !flapAccelerating) {
////			flapAcceleration = oldRate;
////			impulseFlapping = true;
////		} else {
////			impulseFlapping = false;
////		}
//
//		//set air density based on height
//		//		airDensity = 1.2238f * Mathf.Pow(1f - (0.0000226f * transform.position.y), 5.26f);
//
//		speed = rigidBody.velocity.magnitude;
////		terminalSpeed = Mathf.Sqrt (2*gravity/(airDensity*wingDragSurfaceArea*dragCoef));
//
//		//flap wings
//		if (flapSpeed != 0) {
//			isFlapping = true;
////			animator.SetBool ("Flapping", true);
//			birdAnimator.FlapSpeed = 2f;// + flapAnimationScale * flapSpeed;
//			birdAnimator.Flapping = true;
//
//			if(!playingFlapSound){
//				StartCoroutine(PlayFlapSound(flapSoundRate*(1-flapSpeed) + minFlapRate));
//				playingFlapSound = true;
//				flapAudioSource.pitch = Random.Range (flapMinPitch, flapMaxPitch);
//			}
//		} else {
////			animator.SetBool ("Flapping", false);
//			isFlapping = false;
////			animator.speed = 1f;
//			birdAnimator.Flapping = false;
//		}
//
//		//audio based on speed
//		airAudioSource.pitch = speed * pitchScale;
//		airAudioSource.volume = speed * volumeScale;
//
//		foreach(TrailRenderer trail in trails){
//			trail.endWidth = speed * trailScale;
//			trail.startWidth = trailStartWidth;
//			trail.time = trailTime;
//		}
//
//		birdAnimator.WingsOut = wingsOut;
//	}
//
//	IEnumerator PlayFlapSound(float wait){
//		flapAudioSource.Play();
//		yield return new WaitForSeconds(wait);
//		playingFlapSound = false;
//	}
//
//	void FixedUpdate () {
//		if (impulseFlapping) {
////			FlapImpulse ();
////			SteadyFlap ();
//			SteadyFlapDirectional ();
//		}
//
//		if (!isGrounded) {
////			rigidBody.constraints = RigidbodyConstraints.None;
//			rigidBody.drag = 0;
//
//			//rotate towards motion
//			if (rotateTowardsMotion) {
//				Vector3 rotation = Quaternion.LookRotation (rigidBody.velocity, transform.up).eulerAngles;
//				transform.rotation = Quaternion.Euler (rotation);
//			}
//
////			rigidBody.constraints = RigidbodyConstraints.None;
//			rigidBody.drag = 0;
//			AngledDragLift ();
//		} else {
//			drag = 0;
//		}
//	}
//
//	void SteadyFlap(){
//		Vector3 flapForce = (transform.forward + transform.up).normalized * flapForwardCoef * flapScale * flapSpeed;
//
//		rigidBody.AddForce (flapForce);
//
//		Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce, Color.red);
//	}
//
//	void SteadyFlapDirectional(){
//		float realFlapSpeed = flapSpeed * (flapDirection + 3) / 4;
//		Vector3 flapForce = (transform.forward * flapDirection + transform.up * (1 - Mathf.Abs (flapDirection))).normalized * flapForwardCoef * flapScale * realFlapSpeed;
//
//		Vector3 flapPosition = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z;
////		Vector3 aVel = rigidBody.angularVelocity;
//		rigidBody.AddForce (flapForce, flapForceMode);
////		rigidBody.angularVelocity = aVel;
////		rigidBody.AddTorque (transform.right * antiFlapTorqueScale * flapForce.magnitude);
//
//		Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce, Color.red);
//	}
//
//	void FlapImpulse(){
//		impulseFlapping = false;
//
//		Vector3 direction = (transform.up + transform.forward*flapDirection).normalized;
//
//		Vector3 flapForce = direction * flapForwardCoef * flapScale * flapAcceleration;
//
//		rigidBody.AddForce (flapForce, ForceMode.Force);
//
//		Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce);
//	}
//
//	void AngledDragLift(){
//		float realLiftCoef = 0f;
//
//		if(wingsOut){
//			float angleOfAttack = Util.SignedVectorAngle (transform.forward, rigidBody.velocity, transform.right) - pitch*angleScale;
//			if (angleOfAttack > 180)
//				angleOfAttack -= 360;
//			if (angleOfAttack < -180)
//				angleOfAttack += 360;
//
//			realLiftCoef = liftCoef * Mathf.Sin (angleOfAttack * Mathf.PI / 180f);
//
////			realLiftCoef *= (1f - 0.5f*flapDirection);
////			realLiftCoef *= (1f - 1*flapDirection);
//
//			float liftForward = 0.5f * realLiftCoef * airDensity * wingLiftSurfaceArea * speed * speed;
//			liftForward = Mathf.Clamp (liftForward, -maxLift, maxLift);
//			Vector3 liftForce = transform.up * liftForward;
//
//			//TODO make this more seamless
//			if (liftForward > 0 && transform.rotation.eulerAngles.x < -270) {
//				Debug.Log ("flying too high");
//				liftForce = Vector3.zero;
//			}
//
//			else if (liftForward < 0 && transform.rotation.eulerAngles.x > 80) {
//				Debug.Log ("flying too low");
//				liftForce = Vector3.zero;
//			}
//
//			rigidBody.AddForceAtPosition (liftForce, transform.position, ForceMode.Force);
//			Util.DrawRigidbodyRay(rigidBody, transform.position, liftForce, Color.yellow);
//
//
//			//roll lift
//			float liftRoll = Mathf.Clamp (rollScale * 0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed, -maxRoll, maxRoll);
//			liftRoll *= Mathf.Abs (roll);
//			//		float liftRight = rollScale * -0.5f * liftCoef * airDensity * wingLiftSurfaceArea * speed * speed * roll;
//
//			rigidBody.AddForceAtPosition (transform.up * liftRoll, transform.position - wingOutDistance*transform.right * Mathf.Sign (roll), ForceMode.Force);
//			//		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right, ForceMode.Force);
//
//			Util.DrawRigidbodyRay(rigidBody, transform.position + wingOutDistance*transform.right * Mathf.Sign (roll), transform.up * liftRoll, Color.magenta);
//			//		Util.DrawRigidbodyRay(rigidBody, transform.position + wingOutDistance*transform.right, transform.up * liftRight, Color.magenta);
//		}
//
//		//induced drag
//		float aspectRatio = 1f/wingLiftSurfaceArea;
//		float inducedDragCoef = realLiftCoef * realLiftCoef * wingLiftSurfaceArea/ Mathf.PI;
//		float realDragCoef = dragCoef + inducedDragCoef;
//
//		//drag based on wingspan
//		realDragCoef *= (1f - 0.5f*flapDirection);
//
//		drag = realDragCoef * 0.5f * airDensity * speed * speed * wingDragSurfaceArea;
//		Vector3 dragForce = rigidBody.velocity.normalized * (-1) * drag;
//		rigidBody.AddForceAtPosition (dragForce, transform.position - transform.forward*dragForwardDistance);
//		Util.DrawRigidbodyRay(rigidBody, transform.position - transform.forward*dragForwardDistance, dragForce, Color.green);
//
//		//tail drag
//		float realTailDragCoef = tailDragCoef * 0.5f * airDensity * speed * speed;
//		Vector3 tailDragAngle = (tailPitch * transform.up/*  + yaw * transform.right*/).normalized;
//		Vector3 tailDrag = realTailDragCoef * new Vector2 (pitch, yaw).magnitude * tailDragAngle;
//		tailDrag = Vector3.ClampMagnitude (tailDrag, maxTailDrag);
//		rigidBody.AddForceAtPosition (tailDrag, transform.position);
//		Util.DrawRigidbodyRay(rigidBody, transform.position, tailDrag, Color.magenta);
//
//
//		//velocity
//		Util.DrawRigidbodyRay(rigidBody, transform.position, rigidBody.velocity, Color.cyan);
//	}
}