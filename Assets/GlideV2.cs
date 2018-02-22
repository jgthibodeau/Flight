using UnityEngine;
using System.Collections;

public class GlideV2 : MonoBehaviour {
	[HideInInspector]
	public BirdAnimator birdAnimator;
	[HideInInspector]
	public Rigidbody rigidBody;
	[HideInInspector]
	private TrailRenderer[] trails;
	[HideInInspector]
	public bool isGrounded;
	[HideInInspector]
	public Player playerScript;

	public float speed;
	public float gravity;

	public bool rotateTowardsMotion;

	public float liftCoef;
	public float maxLift;

	public float yawScale;
	public float rollScale;

//	public float dragCoef;
	public float inducedDragCoef;
	public float parasiticDragCoef;
	public float dragForwardDistance;
	public float drag;
	public float rollDragScale;

	public float flapAnimationScale;
	public float flapScale;
	public float flapUpCoef;
	public float flapForwardCoef;
	public float flapForwardDistance;
	public float flapUpDistance;
	public float flapOutDistance;
	public ForceMode flapForceMode;

	public float wingLiftSurfaceArea;
	public float wingDragSurfaceArea;
	public float airDensity;

	public bool flap = false;
	private bool isFlapping = false;
	public float wingOutDistance = 0.5f;
	public float wingOutDragDistance = 0.5f;
	public float wingForwardDistance = 0.5f;
	public float wingUpDistance = 0.5f;

	public float angleOffset = 0.015f;
	public float angleScale = 0.05f;
	public float liftAngle = 0;

	public AudioSource airAudioSource;
	public AudioSource flapAudioSource;
	private bool playingFlapSound = false;
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
//	public float roll, pitch, tailPitch, yaw, forward, right, flapSpeed, flapDirection;
	public float rollLeft, rollRight, pitchLeft, pitchRight, tailPitch, yaw, forward, right, flapSpeed, flapDirection;
	[HideInInspector]
	public bool wingsOut;

	// Use this for initialization
	void Start () {
		trails = transform.GetComponentsInChildren<TrailRenderer> ();

		playerScript = GetComponent<Player> ();

		airDensity = 1.2238f;
	}

	// Update is called once per frame
	void Update() {
		//set air density based on height
		//		airDensity = 1.2238f * Mathf.Pow(1f - (0.0000226f * transform.position.y), 5.26f);

		speed = rigidBody.velocity.magnitude;

		//flap wings
		if (flapping) {
			isFlapping = true;
			birdAnimator.FlapSpeed = 2f;// + flapAnimationScale * flapSpeed;
			birdAnimator.Flapping = true;
		} else {
			isFlapping = false;
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

		birdAnimator.pitchLeft = pitchLeft;
		birdAnimator.pitchRight = pitchRight;
		birdAnimator.rollLeft = -rollLeft;
		birdAnimator.rollRight = -rollRight;
		birdAnimator.tailPitch = flapDirection;
	}

	IEnumerator PlayFlapSound(float wait){
		flapAudioSource.Play();
		yield return new WaitForSeconds(wait);
		playingFlapSound = false;
	}

	void FixedUpdate () {
//		SteadyFlapOverTime ();
		WingFlap ();

		if (!isGrounded) {
			rigidBody.drag = 0;

			//rotate towards motion
			if (rotateTowardsMotion) {
				Vector3 rotation = Quaternion.LookRotation (rigidBody.velocity, transform.up).eulerAngles;
				transform.rotation = Quaternion.Euler (rotation);
			}

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
			float realFlapSpeed = flapForces [currentFlapTick] * flapSpeed;
			Vector3 flapForceDirection = (transform.forward * (1 + flapDirection) / 2 + transform.up * (1 - flapDirection) / 2).normalized;
			Vector3 flapForce = flapForceDirection * flapForwardCoef * flapScale * realFlapSpeed;

			Vector3 flapPosition = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.up * flapUpDistance + transform.forward * flapForwardDistance;
			rigidBody.AddForceAtPosition (flapForce, flapPosition, flapForceMode);

			Util.DrawRigidbodyRay (rigidBody, flapPosition, flapForce, Color.red);
		}
	}

	void WingFlap() {
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
			float realFlapSpeed = flapForces [currentFlapTick] * flapSpeed;

//			Vector3 flapForceDirectionLeft = (transform.forward * (1 + pitchLeft) / 2) + (transform.up * (1 - pitchLeft) / 2);

//			Vector3 flapForceDirectionLeft = (transform.forward * (pitchLeft) / 3) + (transform.up * (1 - pitchLeft) / 3) + (transform.right * (rollLeft) / 2);

//			Vector3 flapForceDirectionLeft = Vector3.ClampMagnitude(transform.forward * pitchLeft - transform.right * rollLeft, 1);
//			float flapForceMagnitudeLeft = flapForceDirectionLeft.magnitude;
//			flapForceDirectionLeft += transform.up * (1 - flapForceMagnitudeLeft*0.9f);

			float flapLeft = -rollLeft;
			Vector3 flapForceDirectionLeft = (transform.forward * (flapLeft) / 2) + (transform.up * (1 - Mathf.Abs(flapLeft)) / 2);

			Vector3 flapForceLeft = flapForceDirectionLeft.normalized * flapForwardCoef * flapScale * realFlapSpeed / 2;
//			Vector3 flapPositionLeft = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.up * flapUpDistance + transform.forward * flapForwardDistance - transform.right * flapOutDistance;
//			Vector3 flapPositionLeft = transform.position + transform.up * flapUpDistance + transform.forward * flapForwardDistance - transform.right * flapOutDistance;
			Vector3 flapPositionLeft = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z - transform.right * flapOutDistance;
			rigidBody.AddForceAtPosition (flapForceLeft, flapPositionLeft, flapForceMode);
			Util.DrawRigidbodyRay (rigidBody, flapPositionLeft, flapForceLeft, Color.red);

//			Vector3 flapForceDirectionRight = (transform.forward * (1 + pitchRight) / 2) + (transform.up * (1 - pitchRight) / 2);

//			Vector3 flapForceDirectionRight = (transform.forward * (1 + pitchRight) / 2) + (transform.up * (1 - pitchRight) / 2) + (transform.right * (rollRight) / 2);

//			Vector3 flapForceDirectionRight = Vector3.ClampMagnitude(transform.forward * pitchRight + transform.right * rollRight, 1);
//			float flapForceMagnitudeRight = flapForceDirectionRight.magnitude;
//			flapForceDirectionRight += transform.up * (1 - flapForceMagnitudeRight*0.9f);

			float flapRight = -rollRight;
			Vector3 flapForceDirectionRight = (transform.forward * (flapRight) / 2) + (transform.up * (1 - Mathf.Abs(flapRight)) / 2);

			Vector3 flapForceRight = flapForceDirectionRight.normalized * flapForwardCoef * flapScale * realFlapSpeed / 2;
//			Vector3 flapPositionRight = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.up * flapUpDistance + transform.forward * flapForwardDistance + transform.right * flapOutDistance;
//			Vector3 flapPositionRight = transform.position + transform.up * flapUpDistance + transform.forward * flapForwardDistance + transform.right * flapOutDistance;
			Vector3 flapPositionRight = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.right * flapOutDistance;
			rigidBody.AddForceAtPosition (flapForceRight, flapPositionRight, flapForceMode);
			Util.DrawRigidbodyRay (rigidBody, flapPositionRight, flapForceRight, Color.blue);
		}
	}

	void AngledDragLift(){
		float realLiftCoefLeft = 0f;
		float realLiftCoefRight = 0f;
		float liftLeft = 0f;
		float liftRight = 0f;

		//get angle of attack realistically by comparing velocity to forward direction
		float angleOfAttackLeft;
		float angleOfAttackRight;

		Vector3 leftPosition = Vector3.zero;
		Vector3 rightPosition = Vector3.zero;

		float defaultLift = 1;

		float wingUpDirectionScale = 0.5f;

		Vector3 liftDirection = (transform.up * (1 - liftAngle) + transform.forward * liftAngle).normalized;

		if (wingsOut) {
			/**
			 * Left
			*/
			float pitchAbsLeft = Mathf.Abs (pitchLeft);
			Vector3 wingUpDirectionLeft = (transform.forward * (pitchLeft) * wingUpDirectionScale + transform.up * (1 - pitchAbsLeft * wingUpDirectionScale)).normalized;
			Vector3 wingForwardDirectionLeft = (transform.forward * (1 - pitchAbsLeft * wingUpDirectionScale) - transform.up * (pitchLeft) * wingUpDirectionScale).normalized;

			angleOfAttackLeft = angleOffset + Util.SignedVectorAngle (wingForwardDirectionLeft, rigidBody.velocity, transform.right);// - pitch*angleScale;
			if (angleOfAttackLeft > 180)
				angleOfAttackLeft -= 360;
			if (angleOfAttackLeft < -180)
				angleOfAttackLeft += 360;

			realLiftCoefLeft = liftCoef * Mathf.Sin (angleOfAttackLeft * Mathf.PI / 180f);

			float liftAmountLeft = rollLeft;
//			float liftAmountLeft = -pitchLeft;
			birdAnimator.liftLeft = liftAmountLeft;
//			liftAmountLeft = 0.5f * (1 + liftAmountLeft);
			liftAmountLeft = (1 + liftAmountLeft);

			liftLeft = 0.5f * airDensity * speed * speed * wingLiftSurfaceArea * realLiftCoefLeft * liftAmountLeft;
			liftLeft = Mathf.Clamp (liftLeft, -maxLift, maxLift);

			Vector3 leftDirection = liftDirection;
			leftPosition = -transform.right;
			if (rollLeft < 0) {
				leftPosition -= transform.right * rollLeft * rollScale;
			}
			leftPosition = leftPosition.normalized * wingOutDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			leftPosition += transform.position;
			Vector3 leftForce = leftDirection * liftLeft;
			rigidBody.AddForceAtPosition (leftForce, leftPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftPosition, leftForce, Color.yellow);


			/**
			 * Right
			*/
			float pitchAbsRight = Mathf.Abs (pitchRight);
			Vector3 wingUpDirectionRight = (transform.forward * (pitchRight) * wingUpDirectionScale + transform.up * (1 - pitchAbsRight * wingUpDirectionScale)).normalized;
			Vector3 wingForwardDirectionRight = (transform.forward * (1 - pitchAbsRight * wingUpDirectionScale) - transform.up * (pitchRight) * wingUpDirectionScale).normalized;

			angleOfAttackRight = angleOffset + Util.SignedVectorAngle (wingForwardDirectionRight, rigidBody.velocity, transform.right);// - pitch*angleScale;
			if (angleOfAttackRight > 180)
				angleOfAttackRight -= 360;
			if (angleOfAttackRight < -180)
				angleOfAttackRight += 360;

			realLiftCoefRight = liftCoef * Mathf.Sin (angleOfAttackRight * Mathf.PI / 180f);

			float liftAmountRight = rollRight;
//			float liftAmountRight = -pitchRight;
			birdAnimator.liftRight = liftAmountRight;
//			liftAmountRight = 0.5f * (1 + liftAmountRight);
			liftAmountRight = (1 + liftAmountRight);

			liftRight = 0.5f * airDensity * speed * speed * wingLiftSurfaceArea * realLiftCoefRight * liftAmountRight;
			liftRight = Mathf.Clamp (liftRight, -maxLift, maxLift);

			Vector3 rightDirection = liftDirection;
			rightPosition = transform.right;
			if (rollRight < 0) {
				rightPosition += transform.right * rollRight * rollScale;
			}
			rightPosition = rightPosition.normalized * wingOutDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			rightPosition += transform.position;
			Vector3 rightForce = rightDirection * liftRight;
			rigidBody.AddForceAtPosition (rightForce, rightPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, rightPosition, rightForce, Color.magenta);

		} else {
			angleOfAttackLeft = angleOffset + Util.SignedVectorAngle (transform.forward, rigidBody.velocity, transform.right);// - pitch*angleScale;
			if (angleOfAttackLeft > 180)
				angleOfAttackLeft -= 360;
			if (angleOfAttackLeft < -180)
				angleOfAttackLeft += 360;
			
			angleOfAttackRight = angleOffset + Util.SignedVectorAngle (transform.forward, rigidBody.velocity, transform.right);// - pitch*angleScale;
			if (angleOfAttackRight > 180)
				angleOfAttackRight -= 360;
			if (angleOfAttackRight < -180)
				angleOfAttackRight += 360;
		}

		SeparateDrag (realLiftCoefLeft, realLiftCoefRight, angleOfAttackLeft, angleOfAttackRight, Mathf.Abs (liftLeft), Mathf.Abs (liftRight), leftPosition, rightPosition);

		//velocity
		Util.DrawRigidbodyRay(rigidBody, transform.position, rigidBody.velocity, Color.cyan);
	}

	public void SeparateDrag (float liftCoefLeft, float liftCoefRight, float angleOfAttackLeft, float angleOfAttackRight, float liftLeft, float liftRight, Vector3 leftPosition, Vector3 rightPosition){
		if (rigidBody.velocity.magnitude > 0) {
			float dragScaleLeft = pitchLeft * (-0.75f) + 1;
			float dragScaleRight = pitchRight * (-0.75f) + 1;

//			if (rollLeft > 0) {
				dragScaleLeft *= (1 + rollLeft * rollDragScale);
//			}
//			if (rollRight > 0) {
				dragScaleRight *= (1 + rollLeft * rollDragScale);
//			}

			//parasitic
			float parasiticDragMagnitudeLeft = 0.25f * airDensity * speed * speed * wingDragSurfaceArea * parasiticDragCoef;
			float parasiticDragMagnitudeRight = parasiticDragMagnitudeLeft;
			parasiticDragMagnitudeLeft *= dragScaleLeft;
			parasiticDragMagnitudeRight *= dragScaleRight;
			Vector3 parasiticDirection = rigidBody.velocity.normalized * (-1);

			//left
			leftPosition = transform.position - transform.right * wingOutDragDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			Vector3 leftParasiticDirection = parasiticDirection;
			Vector3 leftParasiticDragForce = parasiticDragMagnitudeLeft * leftParasiticDirection * (yaw * yawScale + 1);
			Vector3 leftParasiticPosition = leftPosition - transform.forward * dragForwardDistance;
			rigidBody.AddForceAtPosition (leftParasiticDragForce, leftParasiticPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftParasiticPosition, leftParasiticDragForce, Color.red);

			//right
			rightPosition = transform.position + transform.right * wingOutDragDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			Vector3 rightParasiticDirection = parasiticDirection;
			Vector3 rightParasiticDragForce = parasiticDragMagnitudeRight * rightParasiticDirection * (-yaw * yawScale + 1);
			Vector3 rightParasiticPosition = rightPosition - transform.forward * dragForwardDistance;
			rigidBody.AddForceAtPosition (rightParasiticDragForce, rightParasiticPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, rightParasiticPosition, rightParasiticDragForce, Color.red);

			//induced
			float aspectRatio = 1f / wingLiftSurfaceArea;
			float inducedDragMagnitudeLeft = inducedDragCoef / (airDensity * speed * speed * wingLiftSurfaceArea * Mathf.PI * aspectRatio);
			float inducedDragMagnitudeRight = inducedDragMagnitudeLeft;
			inducedDragMagnitudeLeft *= liftLeft * liftLeft * dragScaleLeft;
			inducedDragMagnitudeRight *= liftRight * liftRight * dragScaleRight;
			Vector3 inducedDirection = rigidBody.velocity.normalized * (-1);

			//left
			Vector3 leftInducedDirection = inducedDirection;
			Vector3 leftInducedDragForce = inducedDragMagnitudeLeft * leftInducedDirection;// * (yaw * yawScale + 1);
			Vector3 leftInducedPosition = leftPosition;
			rigidBody.AddForceAtPosition (leftInducedDragForce, leftInducedPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftInducedPosition, leftInducedDragForce, Color.blue);

			//right
			Vector3 rightInducedDirection = inducedDirection;
			Vector3 rightInducedDragForce = inducedDragMagnitudeRight * rightInducedDirection;// * (-yaw * yawScale + 1);
			Vector3 rightInducedPosition = rightPosition;
			rigidBody.AddForceAtPosition (rightInducedDragForce, rightInducedPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, rightInducedPosition, rightInducedDragForce, Color.blue);
		}
	}
}