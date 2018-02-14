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

	public float yawScale;
	public float rollScale;
	public float maxRoll;

	public bool useYaw;

	public float maxTailDrag;
//	public float dragCoef;
	public float inducedDragCoef;
	public float parasiticDragCoef;
	public float dragForwardDistance;
	public float drag;

	public float flapAnimationScale;
	public float flapScale;
	public float flapUpCoef;
	public float flapForwardCoef;
	public float flapForwardDistance;
	public float flapUpDistance;
	public bool flapOverTime;
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
	public float wingOutDragDistance = 0.5f;
	public float wingForwardDistance = 0.5f;
	public float wingUpDistance = 0.5f;

	public bool staticAngleOffset;
	public float angleOffset = 0.015f;
	public float angleScale = 0.05f;
	public float liftAngle = 0;

	public AudioSource airAudioSource;
	public AudioSource flapAudioSource;
	private bool playingFlapSound = false;
	public float flapSoundRate;
	public float minFlapRate;
	public float flapMinPitch;
	public float flapMaxPitch;

	public float pitchScale = 1.5f;
	public float volumeScale = 0.7f;
	public float minTrailSpeed = 10f;
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
		if (flapping) {
			isFlapping = true;
			//			animator.SetBool ("Flapping", true);
			birdAnimator.FlapSpeed = 2f;// + flapAnimationScale * flapSpeed;
			birdAnimator.Flapping = true;

//			if(!playingFlapSound){
//				StartCoroutine(PlayFlapSound(flapSoundRate*(1-flapSpeed) + minFlapRate));
//				playingFlapSound = true;
//				flapAudioSource.pitch = Random.Range (flapMinPitch, flapMaxPitch);
//			}
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
			if (speed > minTrailSpeed) {
				trail.endWidth = (speed - minTrailSpeed) * trailScale;
				trail.startWidth = trailStartWidth;
				trail.time = trailTime;
			} else {
				trail.endWidth = 0f;
			}
		}

		birdAnimator.WingsOut = wingsOut;

		birdAnimator.pitch = pitch;
		birdAnimator.roll = roll;
		birdAnimator.tailPitch = flapDirection;
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
//			SteadyFlapDirectional ();
		}
		if (flapOverTime) {
			SteadyFlapOverTime ();
		} else {
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
//		float realFlapSpeed = flapSpeed * (flapDirection + 3) / 4;
		float realFlapSpeed = flapSpeed;
//		Vector3 flapForceDirection = (transform.forward * flapDirection + transform.up/* * (1 - Mathf.Abs (flapDirection))*/).normalized;
//		Vector3 flapForceDirection = (transform.up + transform.forward).normalized;
//		Vector3 flapForceDirection = (transform.forward * (1+flapDirection)/2 + transform.up * (1-flapDirection)/2).normalized;
		Vector3 flapForceDirection = (transform.forward * (1+pitch)/2 + transform.up * (1-pitch)/2).normalized;
		Vector3 flapForce = flapForceDirection * flapForwardCoef * flapScale * realFlapSpeed;

		Vector3 flapPosition = transform.position + transform.up * playerScript.centerOfGravity.y  + transform.forward * playerScript.centerOfGravity.z + transform.up * flapUpDistance + transform.forward * flapForwardDistance;
		//		Vector3 aVel = rigidBody.angularVelocity;
//		rigidBody.AddForce (flapForce, flapForceMode);
		rigidBody.AddForceAtPosition (flapForce, flapPosition, flapForceMode);
//		rigidBody.AddForceAtPosition (flapForce, transform.position - 0.1f * transform.forward, flapForceMode);
		//		rigidBody.angularVelocity = aVel;
		//		rigidBody.AddTorque (transform.right * antiFlapTorqueScale * flapForce.magnitude);

		Util.DrawRigidbodyRay(rigidBody, flapPosition, flapForce, Color.red);
	}

	public int flapTicks = 10;
	public int currentFlapTick = 0;
	public bool flapping;
	public float[] flapForces;
	public float[] flapLiftPercents;

	void SteadyFlapOverTime(){
		if (flapSpeed != 0) {
			if (!flapping) {
				flapAudioSource.Play();
				flapAudioSource.pitch = Random.Range (flapMinPitch, flapMaxPitch);
				flapping = true;
			} else if (currentFlapTick < flapTicks) {
				currentFlapTick++;
			}
			if (currentFlapTick >= flapTicks) {
				currentFlapTick = 0;
				flapAudioSource.Play();
				flapAudioSource.pitch = Random.Range (flapMinPitch, flapMaxPitch);
			}
		} else if (flapping) {
			if (currentFlapTick < flapTicks) {
				currentFlapTick++;
			}
			if (currentFlapTick >= flapTicks) {
				currentFlapTick = 0;
				flapping = false;
			}
		}

		if (flapping) {
//			float realFlapSpeed = flapForces [currentFlapTick] * flapSpeed * (flapDirection + 3) / 4;
			float realFlapSpeed = flapForces [currentFlapTick] * flapSpeed;
			Vector3 flapForceDirection = (transform.forward * (1 + flapDirection) / 2 + transform.up * (1 - flapDirection) / 2).normalized;
			Vector3 flapForce = flapForceDirection * flapForwardCoef * flapScale * realFlapSpeed;

			Vector3 flapPosition = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.up * flapUpDistance + transform.forward * flapForwardDistance;
//			rigidBody.AddForce (flapForce, flapForceMode);
			rigidBody.AddForceAtPosition (flapForce, flapPosition, flapForceMode);

			Util.DrawRigidbodyRay (rigidBody, flapPosition, flapForce, Color.red);
		}
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
		float angleOfAttack;

		Vector3 leftPosition = Vector3.zero;
		Vector3 rightPosition = Vector3.zero;

		if (wingsOut) {
			float wingUpDirectionScale = 0.5f;
			float pitchAbs = Mathf.Abs (pitch);
			float rollAbs = Mathf.Abs (roll);
			Vector3 wingUpDirection = (transform.forward * (pitch) * wingUpDirectionScale + transform.up * (1 - pitchAbs * wingUpDirectionScale)).normalized;
			Vector3 wingForwardDirection = (transform.forward * (1 - pitchAbs * wingUpDirectionScale) - transform.up * (pitch) * wingUpDirectionScale).normalized;
			Util.DrawRigidbodyRay (rigidBody, transform.position, wingUpDirection, Util.orange);
			Util.DrawRigidbodyRay (rigidBody, transform.position, wingForwardDirection, Util.orange);

			angleOfAttack = angleOffset + Util.SignedVectorAngle (wingForwardDirection, rigidBody.velocity, transform.right);// - pitch*angleScale;
			if (angleOfAttack > 180)
				angleOfAttack -= 360;
			if (angleOfAttack < -180)
				angleOfAttack += 360;

			realLiftCoef = liftCoef * Mathf.Sin (angleOfAttack * Mathf.PI / 180f);
//			realLiftCoef = Mathf.Clamp (realLiftCoef, 0, realLiftCoef);

			//			realLiftCoef *= (1f - 0.5f*flapDirection);
			//			realLiftCoef *= (1f - 1*flapDirection);

//			lift = 0.5f * airDensity * speed * speed * wingLiftSurfaceArea * realLiftCoef;
//			float liftAmount = (1 - pitch); // from 0 to 2
			float liftAmount = (Mathf.Sign (pitch) * Mathf.Clamp (Mathf.Sqrt ((pitch * pitch) + (roll * roll)), 0, 1));
			if (Mathf.Abs (roll) > pitchAbs) {
				liftAmount *= pitchAbs / rollAbs;
			}
			//as pointed up, defaultLift -> -1
			//as leveled out, defaultLift -> 0
			//as pointed down, defaultLift -> 1
			float defaultLift = 1;
//			float forwardAngle = Vector3.Angle (Vector3.up, transform.forward);
//			float forwardAngle = Vector3.Angle (Vector3.up, rigidBody.velocity);
//			defaultLift = (forwardAngle / 90f) - 0.25f;

			liftAmount = defaultLift - liftAmount;
			liftAmount = defaultLift;
			liftAmount = -flapDirection;
			birdAnimator.lift = liftAmount;
			liftAmount = 0.5f * (1 + liftAmount);

			lift = 0.5f * airDensity * speed * speed * wingLiftSurfaceArea * realLiftCoef * liftAmount;
			lift = Mathf.Clamp (lift, -maxLift, maxLift);

			// only apply lift if not flapping
//			if (!isFlapping) {
//			Vector3 liftDirection = (transform.up * (1 - liftAngle) + transform.forward * liftAngle).normalized;
//			Vector3 liftForce = liftDirection * lift;

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


//			parasiticDirection = (parasiticDirection + transform.right*yaw).normalized;

			Vector3 liftDirection = (transform.up * (1 - liftAngle) + transform.forward * liftAngle).normalized;

//			Vector3 leftDirection = (transform.up * (1 - liftAngle) + transform.forward * liftAngle).normalized;//transform.up;
			Vector3 leftDirection = liftDirection;
			leftPosition = -transform.right;
			//			if (roll > 0) {
			if ((roll > 0 && lift > 0) || (roll < 0 && lift < 0)) {
//			if ((roll < 0 && lift > 0) || (roll > 0 && lift < 0)) {
				leftDirection = leftDirection * (1 - rollAbs * rollScale) + transform.right * roll * rollScale;
//				leftDirection += transform.forward * roll * yawScale;
				leftDirection = leftDirection.normalized * (1 - rollAbs * rollScale);

				leftPosition += transform.up * rollAbs * rollScale + transform.right * rollAbs * rollScale;
//				leftPosition = Util.RotatePointAroundPivot (leftPosition, Vector3.zero, new Vector3());
			}
			leftPosition = leftPosition.normalized * wingOutDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			leftPosition += transform.position;
			Vector3 leftForce = leftDirection * lift;
			rigidBody.AddForceAtPosition (leftForce, leftPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftPosition, leftForce, Color.yellow);

//			Vector3 rightDirection = (transform.up * (1 - liftAngle) + transform.forward * liftAngle).normalized;//transform.up;
			Vector3 rightDirection = liftDirection;
			rightPosition = transform.right;
			//			if (roll < 0) {
			if ((roll < 0 && lift > 0) || (roll > 0 && lift < 0)) {
//			if ((roll > 0 && lift > 0) || (roll < 0 && lift < 0)) {
				rightDirection = rightDirection * (1 - rollAbs * rollScale) + transform.right * roll * rollScale;
//				rightDirection -= transform.forward * roll * yawScale;
				rightDirection = rightDirection.normalized * (1 - rollAbs * rollScale);

				rightPosition += transform.up * rollAbs * rollScale - transform.right * rollAbs * rollScale;
			}
			rightPosition = rightPosition.normalized * wingOutDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			rightPosition += transform.position;
			Vector3 rightForce = rightDirection * lift;
			rigidBody.AddForceAtPosition (rightForce, rightPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, rightPosition, rightForce, Color.magenta);

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
		} else {
			angleOfAttack = angleOffset + Util.SignedVectorAngle (transform.forward, rigidBody.velocity, transform.right);// - pitch*angleScale;
			if (angleOfAttack > 180)
				angleOfAttack -= 360;
			if (angleOfAttack < -180)
				angleOfAttack += 360;
		}
//		}

//		Drag (realLiftCoef, angleOfAttack, Mathf.Abs (lift));
		if (useYaw) {
			SeparateDragWithYaw (realLiftCoef, angleOfAttack, Mathf.Abs (lift), leftPosition, rightPosition);
		} else {
			SeparateDragWithAreas (realLiftCoef, angleOfAttack, Mathf.Abs (lift));
		}

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
//		float realTailDragCoef = parasiticDragCoef * 0.5f * airDensity * speed * speed;
//		Vector3 tailDragAngle = (tailPitch * transform.up/*  + yaw * transform.right*/).normalized;
//		Vector3 tailDrag = realTailDragCoef * new Vector2 (pitch, yaw).magnitude * tailDragAngle;
//		tailDrag = Vector3.ClampMagnitude (tailDrag, maxTailDrag);
//		rigidBody.AddForceAtPosition (tailDrag, transform.position);
//		Util.DrawRigidbodyRay(rigidBody, transform.position, tailDrag, Color.magenta);


		//velocity
		Util.DrawRigidbodyRay(rigidBody, transform.position, rigidBody.velocity, Color.cyan);
	}

	public void SeparateDragWithYaw (float liftCoef, float angleOfAttack, float lift, Vector3 leftPosition, Vector3 rightPosition){
		if (rigidBody.velocity.magnitude > 0) {
			float dragScale = pitch * (-0.75f) + 1;

			//parasitic
			float parasiticDragMagnitude = 0.25f * airDensity * speed * speed * wingDragSurfaceArea * parasiticDragCoef;
			parasiticDragMagnitude *= dragScale;
//			Vector3 parasiticDirection = (rigidBody.velocity.normalized * (-1) + transform.right * yaw * yawScale).normalized;
			Vector3 parasiticDirection = rigidBody.velocity.normalized * (-1);

			//left
			leftPosition = transform.position - transform.right * wingOutDragDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			Vector3 leftParasiticDirection = parasiticDirection;
			Vector3 leftParasiticDragForce = parasiticDragMagnitude * leftParasiticDirection * (yaw * yawScale + 1);
			Vector3 leftParasiticPosition = leftPosition - transform.forward * dragForwardDistance;
			rigidBody.AddForceAtPosition (leftParasiticDragForce, leftParasiticPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftParasiticPosition, leftParasiticDragForce, Color.red);

			//right
			rightPosition = transform.position + transform.right * wingOutDragDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			Vector3 rightParasiticDirection = parasiticDirection;
			Vector3 rightParasiticDragForce = parasiticDragMagnitude * rightParasiticDirection * (-yaw * yawScale + 1);
			Vector3 rightParasiticPosition = rightPosition - transform.forward * dragForwardDistance;
			rigidBody.AddForceAtPosition (rightParasiticDragForce, rightParasiticPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, rightParasiticPosition, rightParasiticDragForce, Color.red);


			//induced
			float aspectRatio = 1f / wingLiftSurfaceArea;
			float inducedDragMagnitude = lift * lift * inducedDragCoef / (airDensity * speed * speed * wingLiftSurfaceArea * Mathf.PI * aspectRatio);
			inducedDragMagnitude *= dragScale;
//			Vector3 inducedDirection = (rigidBody.velocity.normalized * (-1) + transform.right * yaw * yawScale).normalized;
			Vector3 inducedDirection = rigidBody.velocity.normalized * (-1);

			//left
			Vector3 leftInducedDirection = inducedDirection;
			Vector3 leftInducedDragForce = inducedDragMagnitude * leftInducedDirection;// * (yaw * yawScale + 1);
			Vector3 leftInducedPosition = leftPosition;
			rigidBody.AddForceAtPosition (leftInducedDragForce, leftInducedPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftInducedPosition, leftInducedDragForce, Color.blue);

			//right
			Vector3 rightInducedDirection = inducedDirection;
			Vector3 rightInducedDragForce = inducedDragMagnitude * rightInducedDirection;// * (-yaw * yawScale + 1);
			Vector3 rightInducedPosition = rightPosition;
			rigidBody.AddForceAtPosition (rightInducedDragForce, rightInducedPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, rightInducedPosition, rightInducedDragForce, Color.blue);
		}
	}

	public void SeparateDragWithAreas (float liftCoef, float angleOfAttack, float lift){
		if (rigidBody.velocity.magnitude > 0) {
//			float dragScale = flapDirection * (-0.75f) + 1;
			float dragScale = pitch * (-0.75f) + 1;

//			parasitic drag = .5 * airDensity * wingDragSurfaceArea * speed^2 * parasiticDragCoef
			float parasiticDragMagnitude = 0.5f * airDensity * speed * speed * wingDragSurfaceArea * parasiticDragCoef;
			parasiticDragMagnitude *= dragScale;
//			Vector3 parasiticDirection = (rigidBody.velocity * (-1) + transform.up * flapDirection * angleScale + transform.right * yaw * angleScale).normalized;
//			Vector3 parasiticDirection = (transform.forward * (-1) + transform.up * flapDirection * angleScale + transform.right * yaw * angleScale).normalized;
			Vector3 parasiticDirection = rigidBody.velocity.normalized * (-1);
//			parasiticDirection = (parasiticDirection + transform.right*yaw).normalized;
//			float direction = flapDirection * angleScale;
//			float absDirection = Mathf.Abs (direction);
//			Vector3 inverseVelocity = rigidBody.velocity.normalized * (-1);
//			Vector3 parasiticDirection = (inverseVelocity * (1 - absDirection) + transform.up * direction).normalized;
//			parasiticDirection = (parasiticDirection + transform.right * yaw * angleScale).normalized;
//			Vector3 inducedDirection = transform.forward * (-1);
			Vector3 parasiticDragForce = parasiticDragMagnitude * parasiticDirection;
//			Vector3 parasiticPosition = transform.position - transform.forward * dragForwardDistance;

			Vector3 parasiticPosition = transform.position;
//			if (flapDirection >= 0) {
				parasiticPosition -= transform.forward * dragForwardDistance;
//			} else {
//				parasiticPosition -= (1 + flapDirection) * transform.forward * dragForwardDistance;
//			}
//			parasiticPosition += transform.right * yaw;
				
//			Vector3 parasiticPosition = transform.position;
//			Vector3 parasiticPosition = transform.position - rigidBody.velocity.normalized * dragForwardDistance;
			rigidBody.AddForceAtPosition (parasiticDragForce, parasiticPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, parasiticPosition, parasiticDragForce, Color.red);

//			lift induced drag = 2 * lift^2 / ( airDensity * speed^2 * wingLiftSurfaceArea * PI * aspectRatio )
			float aspectRatio = 1f / wingLiftSurfaceArea;
			float inducedDragMagnitude = 2f * lift * lift * inducedDragCoef / (airDensity * speed * speed * wingLiftSurfaceArea * Mathf.PI * aspectRatio);
			inducedDragMagnitude *= dragScale;
//			Vector3 inducedDirection = (rigidBody.velocity * (-1) + transform.up * flapDirection * angleScale + transform.right * yaw * angleScale).normalized;
//			Vector3 inducedDirection = (transform.forward * (-1) + transform.up * flapDirection * angleScale + transform.right * yaw * angleScale).normalized;
			Vector3 inducedDirection = rigidBody.velocity.normalized * (-1);
//			inducedDirection = (inducedDirection + transform.right*yaw*2).normalized;

//			Vector3 inducedDirection = (inverseVelocity * (1 - absDirection) + transform.up * direction).normalized;
//			inducedDirection = (inducedDirection + transform.right * yaw * angleScale).normalized;
//			Vector3 inducedDirection = transform.forward * (-1);
			Vector3 inducedDragForce = inducedDragMagnitude * inducedDirection;
//			Vector3 inducedPosition = transform.position - transform.forward * dragForwardDistance;
			Vector3 inducedPosition = transform.position;
//			Vector3 inducedPosition = transform.position - rigidBody.velocity.normalized * dragForwardDistance;

//			inducedPosition += transform.right * yaw;

			rigidBody.AddForceAtPosition (inducedDragForce, inducedPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, inducedPosition, inducedDragForce, Color.blue);
		}
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
//		float dragAmount = (1 + flapDirection)/2f;
//		Vector3 reverseVelocity = (rigidBody.velocity * -1).normalized;
//		Vector3 backward = (transform.forward * -1);
//		Vector3 tailDragDirection = ((backward * dragAmount) + (reverseVelocity * (1 - dragAmount))).normalized;
//		Util.DrawRigidbodyRay (rigidBody, transform.position, 10 * tailDragDirection, Color.white);

		float parasiticDragMagnitude = 0.5f * airDensity * speed * speed * wingDragSurfaceArea * parasiticDragCoef;

		Vector3 parasiticDirection = (rigidBody.velocity * (-1) + transform.right * yaw * angleScale).normalized;
//		Vector3 parasiticDirection = tailDragDirection;
		Vector3 parasiticDragForce = parasiticDragMagnitude * parasiticDirection;
		Vector3 parasiticPosition = transform.position - transform.forward * dragForwardDistance;
		rigidBody.AddForceAtPosition (parasiticDragForce, parasiticPosition, ForceMode.Force);
		Util.DrawRigidbodyRay(rigidBody, parasiticPosition, parasiticDragForce, Color.red);


		float tailDragMagnitude = 0.5f * airDensity * speed * speed * wingDragSurfaceArea * parasiticDragCoef * (1 - flapDirection);

		Vector3 tailDirection = (rigidBody.velocity * (-1) + transform.right * yaw * angleScale).normalized;
//		Vector3 tailDirection = tailDragDirection;
		Vector3 tailDragForce = tailDragMagnitude * tailDirection;
		Vector3 tailPosition = transform.position - transform.forward * dragForwardDistance;
		rigidBody.AddForceAtPosition (tailDragForce, tailPosition, ForceMode.Force);
		Util.DrawRigidbodyRay(rigidBody, tailPosition, tailDragForce, Color.blue);


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
//		float realTailDragCoef = parasiticDragCoef * 0.5f * airDensity * speed * speed;
//		Vector3 tailDragAngle = (tailPitch * transform.up/*  + yaw * transform.right*/).normalized;
//		Vector3 tailDrag = realTailDragCoef * new Vector2 (pitch, yaw).magnitude * tailDragAngle;
//		tailDrag = Vector3.ClampMagnitude (tailDrag, maxTailDrag);
//		rigidBody.AddForceAtPosition (tailDrag, transform.position);
//		Util.DrawRigidbodyRay(rigidBody, transform.position, tailDrag, Color.magenta);
	}
}