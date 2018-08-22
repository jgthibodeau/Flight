using UnityEngine;
using System.Collections;

public class GlideV2 : MonoBehaviour {
	[HideInInspector]
	public BirdAnimator birdAnimator;
	[HideInInspector]
	public DragonAnimator dragonAnimator;
	[HideInInspector]
	public Rigidbody rigidBody;
	[HideInInspector]
	private TrailRenderer[] trails;
	[HideInInspector]
	public Player playerScript;
	public bool isGrounded;

	public float speed;
	public float gravity;

	public bool rotateTowardsMotion;

	public float liftCoef;
	public float maxLift;

	public float yawScale;
	public float rollScale;

	public float rigidBodyDrag = 0f;
	public float rigidBodyAngularDrag = 4f;
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
	public float flapHoverUprightRotationSpeed = 1f;
	public float flapHoverRotationSpeed = 1f;
	public ForceMode flapForceMode;

	public enum BoostState {STARTING, GOING, STOPPING, STOPPED};
	public BoostState boostState = BoostState.STOPPED;
	public float boostRampUpSpeed = 10f;
	public float boostRampDownSpeed = 10f;
	public float currentBoostSpeed = 0f;
	public float maxBoostSpeed = 150f;
	public float boostTime = 10f;
	public float currentBoostTime = 0f;
	public ForceMode boostForceMode;

	public float wingLiftSurfaceArea;
	public float wingDragSurfaceArea;
	public float airDensity;

	public bool flap = false;
	public bool isBackFlapping;
	public float rollAmountTriggersBackflap = 0.9f;
	public float rollAmountHoldBackflap = 0f;
	public float wingOutDistance = 0.5f;
	public float wingOutDragDistance = 0.5f;
	public float wingForwardDistance = 0.5f;
	public float wingUpDistance = 0.5f;

	public float angleOffset = 0.015f;
	public float angleOfAttackScale = 1f;
	public float wingUpDirectionScale = 0.5f;
	public float angleScale = 0.05f;
	public float liftAngle = 0;

	public AudioSource airAudioSource;
	public float maxAirAudioPitch = 10f;
	public float maxAirAudioVolume = 0.5f;
	public float airAudioPitchScale = 0.05f;
	public float airAudioVolumeScale = 0.0005f;
	public float airAudioChangeSpeed = 0.5f;

	public float minTrailSpeed = 10f;
	public float trailScale = 0.3f;
	public float trailStartWidth = 0f;
	public float trailTime = 0.5f;

	public float angleOfAttackLeft;
	public float angleOfAttackRight;
	public float realLiftCoefLeft = 0f;
	public float realLiftCoefRight = 0f;
	public float liftLeft = 0f;
	public float liftRight = 0f;

	public Transform leftWing;
	private Vector3 leftWingInitialRotation;
	public Transform rightWing;
	private Vector3 rightWingInitialRotation;

	//Inputs
	public float rollLeft, rollRight, pitchLeft, pitchRight, tailPitch, yaw, forward, right, flapSpeed, flapDirection;
	public bool wingsOut, boostTriggered, boostHeld, boosting;

	// Use this for initialization
	void Start () {
		trails = transform.GetComponentsInChildren<TrailRenderer> ();

		playerScript = GetComponent<Player> ();

		airDensity = 1.2238f;
	}

	// Update is called once per frame
	void Update() {
		wingsOut = isGrounded || isBackFlapping || flapping || !boostHeld;

		//set air density based on height
		//		airDensity = 1.2238f * Mathf.Pow(1f - (0.0000226f * transform.position.y), 5.26f);

		speed = rigidBody.velocity.magnitude;

		//flap wings
		if (flapping && !boosting && !boostHeld) {
			birdAnimator.FlapSpeed = 2f;// + flapAnimationScale * flapSpeed;
			birdAnimator.Flapping = true;

			dragonAnimator.FlapSpeed = 2f;
			dragonAnimator.Flapping = true;
		} else {
			dragonAnimator.FlapSpeed = 0;
			birdAnimator.Flapping = false;

			dragonAnimator.FlapSpeed = 0;
			dragonAnimator.Flapping = false;
		}

		//audio based on speed
		if (!isGrounded) {
			SetAirAudio (speed * airAudioPitchScale, speed * airAudioVolumeScale);
		} else {
			SetAirAudio (0, 0);
		}

		foreach(TrailRenderer trail in trails){
			if (speed > minTrailSpeed) {
				trail.endWidth = (speed - minTrailSpeed) * trailScale;
				trail.startWidth = trailStartWidth;
				trail.time = trailTime;
			} else {
				trail.endWidth = 0f;
			}
		}

		birdAnimator.WingsOut = WingsOut ();
		birdAnimator.pitchLeft = pitchLeft;
		birdAnimator.pitchRight = pitchRight;
		birdAnimator.rollLeft = -rollLeft;
		birdAnimator.rollRight = -rollRight;
		birdAnimator.tailPitch = flapDirection;


		dragonAnimator.WingsOut = WingsOut ();
		dragonAnimator.Boosting = (boostState == BoostState.STARTING) || (boostState == BoostState.GOING);
		dragonAnimator.pitchLeft = pitchLeft;
		dragonAnimator.pitchRight = pitchRight;
		dragonAnimator.rollLeft = -rollLeft;
		dragonAnimator.rollRight = -rollRight;
		dragonAnimator.tailPitch = flapDirection;
	}

	void SetAirAudio (float desiredPitch, float desiredVolume) {
		desiredPitch = Mathf.Clamp (desiredPitch, 0, maxAirAudioPitch);
		desiredVolume = Mathf.Clamp (desiredVolume, 0, maxAirAudioVolume);
		airAudioSource.pitch = Mathf.Lerp(airAudioSource.pitch, desiredPitch, airAudioChangeSpeed * Time.deltaTime);
		airAudioSource.volume = Mathf.Lerp(airAudioSource.volume, desiredVolume, airAudioChangeSpeed * Time.deltaTime);
	}

	public bool CanBoost() {
		return (boostState == BoostState.STOPPED) && !boostTriggered;
	}

	public bool IsFlapping() {
		return flapping && !boostHeld;
	}

	public bool CanFlap() {
		return !flapping && WingsOut();
	}

	public bool WingsOut() {
		return !boosting && !boostHeld;
	}

	void FixedUpdate () {
//		SteadyFlapOverTime ();
//		if (!flapping) {
//			isBackFlapping = false;
//		}
		if (!flapping || rollLeft < rollAmountHoldBackflap || rollRight < rollAmountHoldBackflap) {
			isBackFlapping = false;
		}

		if (boostTriggered) {
			flapping = false;
			dragonAnimator.BoostTriggered = boostTriggered;
			boostTriggered = false;
			boosting = true;
			StartCoroutine (StartBoost ());
		}

		if (boosting) {
//			if (boostState == BoostState.STARTING) {
//				wingsOut = false;
//			}
			ApplyBoostForce ();
		} else if (WingsOut ()) {
			WingFlap ();
		}

		if (!isGrounded) {
			rigidBody.drag = rigidBodyDrag;
			rigidBody.angularDrag = rigidBodyAngularDrag;

			//rotate towards motion
			if (rotateTowardsMotion && flapSpeed == 0) {
				Vector3 rotation = Quaternion.LookRotation (rigidBody.velocity, transform.up).eulerAngles;
				transform.rotation = Quaternion.Euler (rotation);
			}

			rigidBody.drag = rigidBodyDrag;
			AngledDragLift ();
		} else {
			pitchLeft = 0;
			pitchRight = 0;

			rollLeft = 0;
			rollRight = 0;

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
	public bool backFlapHover;
	public float[] flapForces;
	public float[] flapLiftPercents;

	void SteadyFlapOverTime(){
		if (flapSpeed != 0) {
			if (CanFlap ()) {
				flapping = true;
			} else if (currentFlapTick < flapTicks) {
				currentFlapTick++;
			}
			if (currentFlapTick >= flapTicks) {
				currentFlapTick = 0;
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
			if (CanFlap ()) {
				flapping = true;
			} else if (flapping && currentFlapTick < flapTicks) {
				currentFlapTick++;
				if (currentFlapTick >= flapTicks) {
					currentFlapTick = 0;
				}
			} else {
				currentFlapTick = 0;
				flapping = false;
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
//			pitchLeft = 0f;
//			pitchRight = 0f;

			float realFlapSpeed = flapForces [currentFlapTick] * flapSpeed;
			realFlapSpeed *= flapForwardCoef * flapScale * 0.5f;

			Vector3 flapPositionLeft = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z - transform.right * flapOutDistance;
			Vector3 flapPositionRight = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.right * flapOutDistance;

			//if backFlapHover is selected, and we are flapping backwards
			if (backFlapHover && (isBackFlapping || rollLeft > rollAmountTriggersBackflap && rollRight > rollAmountTriggersBackflap && !isGrounded)) {
				//TODO have to triggers, 1 to start and 1 to keep
				isBackFlapping = true;

				realFlapSpeed = flapSpeed * flapForwardCoef * flapScale * 2;
				Vector3 forceDirection = -rigidBody.velocity.normalized;
				Vector3 flapForce = forceDirection * realFlapSpeed;
				rigidBody.AddForce (flapForce, flapForceMode);
				Util.DrawRigidbodyRay (rigidBody, transform.position, flapForce, Color.green);


				//rotate to become upright
				Quaternion desiredForward;
				Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
				desiredForward = Quaternion.Slerp (rigidBody.rotation, Quaternion.LookRotation(projectedForward, Vector3.up), Time.deltaTime * flapHoverUprightRotationSpeed);
				rigidBody.MoveRotation (desiredForward);

				//rotate instead of rolling
				float rotateAmount = Mathf.Abs(pitchLeft) - Mathf.Abs(pitchRight);
				Vector3 rotateForward = transform.forward * (1 - Mathf.Abs(rotateAmount)) + transform.right * rotateAmount;
				desiredForward = Quaternion.Slerp (rigidBody.rotation, Quaternion.LookRotation (rotateForward, Vector3.up), Time.deltaTime * flapHoverRotationSpeed);
				rigidBody.MoveRotation (desiredForward);

			} else {
				float flapLeft = -2 * rollLeft;
//				Vector3 flapForceDirectionLeft = (transform.forward * (flapLeft) * 0.75f) + (transform.up * (1 - Mathf.Abs (flapLeft)) * 0.25f);
                //Vector3 flapForceDirectionLeft = (transform.forward * (flapLeft)) + (transform.up * (1 - Mathf.Abs (flapLeft)));
                Vector3 flapForceDirectionLeft = CalculateFlapForceDirection();
                Vector3 flapForceLeft = flapForceDirectionLeft.normalized * realFlapSpeed;
				rigidBody.AddForceAtPosition (flapForceLeft, flapPositionLeft, flapForceMode);
				Util.DrawRigidbodyRay (rigidBody, flapPositionLeft, flapForceLeft, Color.red);

				float flapRight = -2 * rollRight;
//				Vector3 flapForceDirectionRight = (transform.forward * (flapRight) * 0.75f) + (transform.up * (1 - Mathf.Abs (flapRight)) * 0.25f);
				//Vector3 flapForceDirectionRight = (transform.forward * (flapRight)) + (transform.up * (1 - Mathf.Abs (flapRight)));
                Vector3 flapForceDirectionRight = CalculateFlapForceDirection();
                Vector3 flapForceRight = flapForceDirectionRight.normalized * realFlapSpeed;
				rigidBody.AddForceAtPosition (flapForceRight, flapPositionRight, flapForceMode);
				Util.DrawRigidbodyRay (rigidBody, flapPositionRight, flapForceRight, Color.blue);
			}
		}
	}

    public Vector3 CalculateFlapForceDirection()
    {
        //return (transform.forward * (0.5f)) + (transform.up * (1 - 0.5f));

        //float minVelToFlapForward = 10f;
        //float maxVelToFlapForward = 100f;
        //if (isGrounded || rigidBody.velocity.magnitude < minVelToFlapForward) {
        //    return transform.up;
        //} else
        //{
        //    float vel = Mathf.Clamp(rigidBody.velocity.magnitude, minVelToFlapForward, maxVelToFlapForward);

        //    float forwardPercent = (vel - minVelToFlapForward) / (maxVelToFlapForward - minVelToFlapForward);

        //    return (transform.forward * (forwardPercent)) + (transform.up * (1 - forwardPercent));
        //}
        if (isGrounded)
        {
            return transform.up;
        }
        else
        {
            float forwardPercent = 0.25f;

            return (transform.forward * (forwardPercent)) + (transform.up * (1 - forwardPercent));
        }
    }

	IEnumerator StartBoost() {
		boostState = BoostState.STARTING;
		currentBoostSpeed = 0f;
		while (currentBoostSpeed < maxBoostSpeed) {
			currentBoostSpeed = Mathf.Clamp (currentBoostSpeed + boostRampUpSpeed * Time.deltaTime, 0, maxBoostSpeed);
			yield return null;
		}

		StartCoroutine (SustainBoost ());
	}
	IEnumerator SustainBoost() {
		boostState = BoostState.GOING;
		currentBoostTime = 0f;
		while (currentBoostTime < boostTime && boostHeld) {
			currentBoostTime += Time.deltaTime;
			yield return null;
		}

		StartCoroutine (StopBoost ());
	}
	IEnumerator StopBoost() {
		boostState = BoostState.STOPPING;
		while (currentBoostSpeed > 0f) {
			currentBoostSpeed = Mathf.Clamp (currentBoostSpeed - boostRampDownSpeed * Time.deltaTime, 0, maxBoostSpeed);
			yield return null;
		}
		boosting = false;
		boostState = BoostState.STOPPED;
	}

	void ApplyBoostForce() {
		Vector3 flapPositionLeft = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z - transform.right * flapOutDistance;
		Vector3 flapPositionRight = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.right * flapOutDistance;

		Vector3 flapForceLeft = transform.forward * currentBoostSpeed;
		Vector3 flapForceRight = transform.forward * currentBoostSpeed;

		rigidBody.AddForceAtPosition (flapForceLeft, flapPositionLeft, boostForceMode);
		Util.DrawRigidbodyRay (rigidBody, flapPositionLeft, flapForceLeft, Color.yellow);

		rigidBody.AddForceAtPosition (flapForceRight, flapPositionRight, boostForceMode);
		Util.DrawRigidbodyRay (rigidBody, flapPositionRight, flapForceRight, Color.yellow);
	}

	void AngledDragLift(){
		Vector3 leftPosition = Vector3.zero;
		Vector3 rightPosition = Vector3.zero;

		float defaultLift = 1;

		Vector3 liftDirection = (transform.up * (1 - liftAngle) + transform.forward * liftAngle).normalized;

		if (WingsOut ()) {
			/**
			 * Left
			*/
			float pitchAbsLeft = Mathf.Abs (pitchLeft);
			Vector3 wingForwardDirectionLeft = (transform.forward * (1 - pitchAbsLeft * wingUpDirectionScale) - transform.up * (pitchLeft) * wingUpDirectionScale).normalized;

			angleOfAttackLeft = CalculateAngleOfAttack (wingForwardDirectionLeft);
			realLiftCoefLeft = CalculateLiftCoef (angleOfAttackLeft);

			float liftAmountLeft = rollLeft;
			birdAnimator.liftLeft = liftAmountLeft;
			dragonAnimator.liftLeft = liftAmountLeft;

			liftLeft = CalculateLift (realLiftCoefLeft, liftAmountLeft);

			Vector3 leftDirection = liftDirection;
			leftPosition = -transform.right;
			if (rollLeft < 0) {
				leftPosition -= transform.right * rollLeft * rollScale;
			}
			leftPosition = CalculateWingPosition (leftPosition.normalized);
			Vector3 leftForce = leftDirection * liftLeft;
			rigidBody.AddForceAtPosition (leftForce, leftPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftPosition, leftForce, Color.yellow);


			/**
			 * Right
			*/
			float pitchAbsRight = Mathf.Abs (pitchRight);
			Vector3 wingForwardDirectionRight = (transform.forward * (1 - pitchAbsRight * wingUpDirectionScale) - transform.up * (pitchRight) * wingUpDirectionScale).normalized;

			angleOfAttackRight = CalculateAngleOfAttack (wingForwardDirectionRight);
			realLiftCoefRight = CalculateLiftCoef (angleOfAttackRight);

			float liftAmountRight = rollRight;
			birdAnimator.liftRight = liftAmountRight;
			dragonAnimator.liftRight = liftAmountRight;

			liftRight = CalculateLift (realLiftCoefRight, liftAmountRight);

			Vector3 rightDirection = liftDirection;
			rightPosition = transform.right;
			if (rollRight < 0) {
				rightPosition += transform.right * rollRight * rollScale;
			}
			rightPosition = CalculateWingPosition (rightPosition.normalized);
			Vector3 rightForce = rightDirection * liftRight;
			rigidBody.AddForceAtPosition (rightForce, rightPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, rightPosition, rightForce, Color.magenta);

		} else {
			liftRight = 0;
			liftLeft = 0;

			angleOfAttackLeft = CalculateAngleOfAttack (transform.forward);
			angleOfAttackRight = CalculateAngleOfAttack (transform.forward);

			leftPosition = CalculateWingPosition (-transform.right);
			rightPosition = CalculateWingPosition (transform.right);

			realLiftCoefLeft = CalculateLiftCoef (angleOfAttackLeft);
			realLiftCoefRight = CalculateLiftCoef (angleOfAttackRight);
		}

		SeparateDrag (realLiftCoefLeft, realLiftCoefRight, angleOfAttackLeft, angleOfAttackRight, Mathf.Abs (liftLeft), Mathf.Abs (liftRight), leftPosition, rightPosition);

		//velocity
		Util.DrawRigidbodyRay(rigidBody, transform.position, rigidBody.velocity, Color.cyan);
	}

	public float CalculateAngleOfAttack(Vector3 direction) {
		float angleOfAttack = angleOffset + angleOfAttackScale * Util.SignedVectorAngle (direction, rigidBody.velocity, transform.right);// - pitch*angleScale;
		if (angleOfAttackRight > 180) {
			angleOfAttackRight -= 360;
		} else if (angleOfAttackRight < -180) {
			angleOfAttackRight += 360;
		}
		return angleOfAttack;
	}

	public float CalculateLiftCoef(float angleOfAttack) {
		return liftCoef * Mathf.Sin (angleOfAttack * Mathf.PI / 180f);
	}

	public float CalculateLift(float liftCoef, float liftAmount) {
		float calculatedLift = 0.5f * airDensity * speed * speed * wingLiftSurfaceArea * liftCoef * (1 + liftAmount);
		return Mathf.Clamp (calculatedLift, -maxLift, maxLift);
	}

	public Vector3 CalculateWingPosition (Vector3 direction) {
		return direction * wingOutDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance + transform.position;
	}

	public void SeparateDrag (float liftCoefLeft, float liftCoefRight, float angleOfAttackLeft, float angleOfAttackRight, float liftLeft, float liftRight, Vector3 leftPosition, Vector3 rightPosition){
		if (rigidBody.velocity.magnitude > 0 && !isBackFlapping) {
			float dragScaleLeft = pitchLeft * (-0.75f) + 1;
			float dragScaleRight = pitchRight * (-0.75f) + 1;

			dragScaleLeft *= (1 + rollLeft * rollDragScale);
			dragScaleRight *= (1 + rollLeft * rollDragScale);

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


//			//induced
//			float aspectRatio = 1f / wingLiftSurfaceArea;
//			float inducedDragMagnitudeLeft = inducedDragCoef / (airDensity * speed * speed * wingLiftSurfaceArea * Mathf.PI * aspectRatio);
//			float inducedDragMagnitudeRight = inducedDragMagnitudeLeft;
//			inducedDragMagnitudeLeft *= liftLeft * liftLeft * dragScaleLeft;
//			inducedDragMagnitudeRight *= liftRight * liftRight * dragScaleRight;
//			Vector3 inducedDirection = rigidBody.velocity.normalized * (-1);
//
//			//left
//			Vector3 leftInducedDirection = inducedDirection;
//			Vector3 leftInducedDragForce = inducedDragMagnitudeLeft * leftInducedDirection;// * (yaw * yawScale + 1);
//			Vector3 leftInducedPosition = leftPosition;
//			rigidBody.AddForceAtPosition (leftInducedDragForce, leftInducedPosition, ForceMode.Force);
//			Util.DrawRigidbodyRay (rigidBody, leftInducedPosition, leftInducedDragForce, Color.blue);
//
//			//right
//			Vector3 rightInducedDirection = inducedDirection;
//			Vector3 rightInducedDragForce = inducedDragMagnitudeRight * rightInducedDirection;// * (-yaw * yawScale + 1);
//			Vector3 rightInducedPosition = rightPosition;
//			rigidBody.AddForceAtPosition (rightInducedDragForce, rightInducedPosition, ForceMode.Force);
//			Util.DrawRigidbodyRay (rigidBody, rightInducedPosition, rightInducedDragForce, Color.blue);
		}
	}
}