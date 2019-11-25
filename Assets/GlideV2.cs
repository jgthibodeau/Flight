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
    public float minLift;
	public float maxLift;
    public float maxLiftSpeed;

    public float yawScale;
    public float rollScale;

    public float minSpeedAngularDrag = 0;
    public float maxSpeedAngularDrag = 30;

    public float rigidBodyDrag = 0f;
    public float minRigidBodyAngularDrag = 1f;
    public float rigidBodyAngularDrag = 10f;

    public float minRigidBodyAngularDragFlapping = 1f;
    public float rigidBodyAngularDragFlapping = 10f;

    public float inducedDragCoef;
	public float parasiticDragCoef;
    public float backFlapDragCoef = 2f;
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

    public bool canBoost = false;
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
    public bool backFlapTriggered;
    public float rollAmountTriggersBackflap = 0.9f;
	public float rollAmountHoldBackflap = 0f;
    public float backFlapStopTime = 0.25f;
    private float backFlapStopCurrentTime;

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
    public float maxTrailWidth = 5f;
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
    public float wingOutAmountLeft, wingOutAmountRight, wingAngleLeft, wingAngleRight, yaw, flapSpeed, flapDirection;
	public bool boostTriggered, boostHeld, boosting;

    public float controlAcceleration = 15f;
    public float controlResetAcceleration = 5f;
    public float minControlValue = 0.0001f;
    public void ResetInput()
    {
        setWingOutAmountLeft(0);
        setWingOutAmountRight(0);
        setWingAngleLeft(0);
        setWingAngleRight(0);
        setYaw(0);
        setFlapSpeed(0);
        setFlapDirection(0);
    }
    public void ResetWings()
    {
        wingOutAmountLeft = 0;
        wingOutAmountRight = 0;
        wingAngleLeft = 0;
        wingAngleRight = 0;
    }
    public void setWingOutAmountLeft(float newVal)
    {
        wingOutAmountLeft = smoothInputValue(newVal, wingOutAmountLeft, true);
    }
    public void setWingOutAmountRight(float newVal)
    {
        wingOutAmountRight = smoothInputValue(newVal, wingOutAmountRight, true);
    }
    public void setWingAngleLeft(float newVal)
    {
        wingAngleLeft = smoothInputValue(newVal, wingAngleLeft, true);
    }
    public void setWingAngleRight(float newVal)
    {
        wingAngleRight = smoothInputValue(newVal, wingAngleRight, true);
    }
    public void setYaw(float newVal)
    {
        yaw = smoothInputValue(newVal, yaw, true);
    }
    public void setFlapSpeed(float newVal)
    {
        //flapSpeed = smoothInputValue(newVal, flapSpeed, false);
        flapSpeed = newVal;
    }
    public void setFlapDirection(float newVal)
    {
        flapDirection = smoothInputValue(newVal, flapDirection, true);
    }
    public float smoothInputValue(float newVal, float oldVal, bool deriveAcceleration)
    {
        float acc = (!deriveAcceleration || newVal != 0) ? controlAcceleration : controlResetAcceleration;
        newVal = Mathf.SmoothStep(oldVal, newVal, Time.deltaTime * acc);
        if (Mathf.Abs(newVal) < minControlValue) { newVal = 0; }
        return newVal;
    }

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
		if ((flapping/* || backFlapTriggered*/) && !boosting && !boostHeld) {
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
        dragonAnimator.BackFlap = isBackFlapping;

        //audio based on speed
        if (!isGrounded) {
			SetAirAudio (speed * airAudioPitchScale, speed * airAudioVolumeScale);
            foreach (TrailRenderer trail in trails)
            {
                if (speed > minTrailSpeed)
                {
                    trail.endWidth = Mathf.Min((speed - minTrailSpeed) * trailScale, maxTrailWidth);
                    trail.startWidth = trailStartWidth;
                    trail.time = trailTime;
                }
                else
                {
                    trail.endWidth = 0f;
                }
            }
        } else {
			SetAirAudio (0, 0);
            foreach (TrailRenderer trail in trails)
            {
                trail.endWidth = 0f;
            }
        }
        
		birdAnimator.WingsOut = WingsOut ();
		birdAnimator.pitchLeft = wingAngleLeft;
		birdAnimator.pitchRight = wingAngleRight;
		birdAnimator.rollLeft = -wingOutAmountLeft;
		birdAnimator.rollRight = -wingOutAmountRight;


		dragonAnimator.WingsOut = WingsOut ();
		dragonAnimator.Boosting = (boostState == BoostState.STARTING) || (boostState == BoostState.GOING);
		dragonAnimator.pitchLeft = wingAngleLeft;
		dragonAnimator.pitchRight = wingAngleRight;
		dragonAnimator.rollLeft = -wingOutAmountLeft;
		dragonAnimator.rollRight = -wingOutAmountRight;
	}

	void SetAirAudio (float desiredPitch, float desiredVolume) {
		desiredPitch = Mathf.Clamp (desiredPitch, 0, maxAirAudioPitch);
		desiredVolume = Mathf.Clamp (desiredVolume, 0, maxAirAudioVolume);
		airAudioSource.pitch = Mathf.Lerp(airAudioSource.pitch, desiredPitch, airAudioChangeSpeed * Time.deltaTime);
		airAudioSource.volume = Mathf.Lerp(airAudioSource.volume, desiredVolume, airAudioChangeSpeed * Time.deltaTime);
	}

	public bool CanBoost() {
		return canBoost && (boostState == BoostState.STOPPED) && !boostTriggered;
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

        //if (!flapping || isGrounded || wingAngleLeft > 0 || wingAngleRight > 0 || (wingAngleLeft > rollAmountTriggersBackflap && wingAngleRight > rollAmountTriggersBackflap))
        if (isGrounded || !backFlapTriggered)
        {
            isBackFlapping = false;
		}

		if (boostTriggered) {
			flapping = false;
			dragonAnimator.BoostTriggered = boostTriggered;
			boostTriggered = false;
			boosting = true;
			StartCoroutine (StartBoost ());
		}

        if (boosting)
        {
            rigidBody.constraints = RigidbodyConstraints.None;
            ApplyBoostForce ();
		} else if (WingsOut ()) {
			WingFlap ();
		}

		if (!isGrounded) {
			rigidBody.drag = rigidBodyDrag;
            if (isBackFlapping && speed < 5)
            {
                rigidBody.angularDrag = rigidBodyAngularDrag;
            } else if (IsFlapping())
            {
                rigidBody.angularDrag = Util.ConvertScale(minSpeedAngularDrag, maxSpeedAngularDrag, minRigidBodyAngularDragFlapping, rigidBodyAngularDragFlapping, speed);
            } else
            {
                rigidBody.angularDrag = Util.ConvertScale(minSpeedAngularDrag, maxSpeedAngularDrag, minRigidBodyAngularDrag, rigidBodyAngularDrag, speed);
            }

            //rotate towards motion
            if (rotateTowardsMotion && flapSpeed == 0) {
				Vector3 rotation = Quaternion.LookRotation (rigidBody.velocity, transform.up).eulerAngles;
				transform.rotation = Quaternion.Euler (rotation);
			}

			rigidBody.drag = rigidBodyDrag;
			AngledDragLift ();
		} else {
			wingAngleLeft = 0;
			wingAngleRight = 0;

			wingOutAmountLeft = 0;
			wingOutAmountRight = 0;

			drag = 0;
		}
	}

	void SteadyFlap(){
		Vector3 flapForce = (transform.forward + transform.up).normalized * flapForwardCoef * flapScale * flapSpeed;

		rigidBody.AddForce (flapForce);

		Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce, Color.red);
	}
    
	public int currentFlapTick = 0;
	public bool flapping;
	public bool backFlapHover;
	public float[] flapForces;
	public float[] flapLiftPercents;

	void SteadyFlapOverTime(){
        int flapTicks = flapForces.Length;

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
        float realFlapSpeed = GetFlapSpeed();

        if (!isGrounded)
        {
            rigidBody.constraints = RigidbodyConstraints.None;
        }

        if (backFlapTriggered && CanBackFlap())
        {
            ResetWings();

            backFlapStopCurrentTime = backFlapStopTime;
            isBackFlapping = true;

            Vector3 forceDirection = -rigidBody.velocity.normalized;
            Quaternion desiredForward;

            if (Vector3.Angle(forceDirection, Vector3.up) > 0.1f)
            {
                Vector3 flapForce = forceDirection * realFlapSpeed;
                rigidBody.AddForce(flapForce, flapForceMode);
                Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce, Color.green);


                //rotate to become upright
                Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                desiredForward = Quaternion.Lerp(rigidBody.rotation, Quaternion.LookRotation(projectedForward, Vector3.up), Time.fixedDeltaTime * flapHoverUprightRotationSpeed);
                Util.DrawRigidbodyRay(rigidBody, transform.position, projectedForward * 5, Color.white);
                Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);
                rigidBody.MoveRotation(desiredForward);

                float rotateAmount = Util.GetAxis("Roll");
                Vector3 rotateForward = transform.forward * (1 - Mathf.Abs(rotateAmount)) + transform.right * rotateAmount;
                desiredForward = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(rotateForward, Vector3.up), Time.fixedDeltaTime * flapHoverRotationSpeed);
                rigidBody.MoveRotation(desiredForward);
                Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);
            }
            else
            {
                rigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rigidBody.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

                //rotate instead of rolling
                float rotateAmount = Util.GetAxis("Roll");
                Vector3 rotateForward = transform.forward * (1 - Mathf.Abs(rotateAmount)) + transform.right * rotateAmount;
                desiredForward = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(rotateForward, Vector3.up), Time.fixedDeltaTime * flapHoverRotationSpeed);
                rigidBody.rotation = desiredForward;
                Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);
            }
        }

        else if (flapping)
        {
            Vector3 flapPositionLeft = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z - transform.right * flapOutDistance;
			Vector3 flapPositionRight = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.right * flapOutDistance;

			float flapLeft = -2 * wingOutAmountLeft;
            Vector3 flapForceDirectionLeft = CalculateFlapForceDirectionV2();
            Vector3 flapForceLeft = flapForceDirectionLeft * realFlapSpeed;
			rigidBody.AddForceAtPosition (flapForceLeft, flapPositionLeft, flapForceMode);
			Util.DrawRigidbodyRay (rigidBody, flapPositionLeft, flapForceLeft, Color.red);

			float flapRight = -2 * wingOutAmountRight;
            Vector3 flapForceDirectionRight = CalculateFlapForceDirectionV2();
            Vector3 flapForceRight = flapForceDirectionRight * realFlapSpeed;
			rigidBody.AddForceAtPosition (flapForceRight, flapPositionRight, flapForceMode);
			Util.DrawRigidbodyRay (rigidBody, flapPositionRight, flapForceRight, Color.blue);
		}
    }

    public float backflapHeight = 2f;
    public bool CanBackFlap()
    {
        return !isGrounded/* && !Physics.Raycast(transform.position, Vector3.down, backflapHeight, playerScript.layerMaskForGround)*/;
    }

    public float GetFlapSpeed()
    {
        int flapTicks = flapForces.Length;
        float flapForce;

        if (isBackFlapping)
        {
            //if (flapSpeed != 0)
            //{
                flapping = true;
            //} else
            //{
                //flapping = false;
            //}
            currentFlapTick = flapTicks;
            flapForce = flapForces[(int)(flapForces.Length * 0.5f)];
        }

        else if (backFlapStopCurrentTime > 0)
        {
            flapping = flapSpeed != 0;
            flapForce = 0;
            //if (flapSpeed != 0)
            //{
            //    flapForce = flapForces[(int)(flapForces.Length * 0.5f)];
            //    flapForce = Util.ConvertScale(backFlapStopTime, 0, flapForce / 4, flapForce, backFlapStopCurrentTime);
                backFlapStopCurrentTime -= Time.fixedDeltaTime;

                //rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            //} else
            //{
            //    backFlapStopCurrentTime = 0;
            //    flapForce = 0;
            //}
        }

        else
        {
            if (flapSpeed != 0)
            {
                if (CanFlap())
                {
                    flapping = true;
                }
                else if (flapping && currentFlapTick < flapTicks)
                {
                    currentFlapTick++;
                    if (currentFlapTick >= flapTicks)
                    {
                        currentFlapTick = 0;
                    }
                }
                else
                {
                    currentFlapTick = 0;
                    //flapping = false;
                }
            }
            else if (flapping)
            {
                if (currentFlapTick < flapTicks)
                {
                    currentFlapTick++;
                }
                if (currentFlapTick >= flapTicks)
                {
                    currentFlapTick = 0;
                    flapping = false;
                }
            }
            
            flapForce = flapForces[(int)(flapForces.Length * 0.5f)];
        }

        return flapForce * flapForwardCoef * flapScale * 0.5f;
    }

    private Vector3 flapForceDirection;
    public Vector3 CalculateFlapForceDirectionV2()
    {
        if (currentFlapTick == 0)
        {
            flapForceDirection = CalculateFlapForceDirection();
        }
        return flapForceDirection;
    }

    public bool limitFlapWhenGrounded = false;
    public float minVelToFlapForward = 10f;
    public float maxVelToFlapForward = 20f;
    public float flappingForwardScale = 0.5f;
    public float rollAmountTriggersForwardFlap = 0.9f;
    public float minForwardPercent = 0f;
    public Vector3 CalculateFlapForceDirection()
    {
        ////base direction off forward velocity and pitch
        //if ((isGrounded && limitFlapWhenGrounded) || rigidBody.velocity.magnitude < minVelToFlapForward)
        //{
        //    //return transform.up + transform.forward;
        //    return transform.up;
        //} else
        //{
        //    float forwardPercent;

        //    //if (backFlapHover && (isBackFlapping || ((wingAngleLeft < rollAmountTriggersBackflap && wingAngleRight < rollAmountTriggersBackflap) && !isGrounded)))
        //    if (wingAngleLeft > rollAmountTriggersForwardFlap && wingAngleRight > rollAmountTriggersForwardFlap) {
        //        forwardPercent = 1;
        //    } else {
        //        float vel = Mathf.Clamp(rigidBody.velocity.magnitude, minVelToFlapForward, maxVelToFlapForward);
        //        forwardPercent = (vel - minVelToFlapForward) / (maxVelToFlapForward - minVelToFlapForward);
        //    }

        //    Vector3 dir = (transform.forward * (forwardPercent)) + (transform.up * (1 - forwardPercent)).normalized;

        //    float scale = 1 - (forwardPercent * (1 - flappingForwardScale));

        //    return dir * scale;
        //}

        float forwardPercent;
        //base direction off forward velocity and pitch
        if ((isGrounded && limitFlapWhenGrounded) || rigidBody.velocity.magnitude < minVelToFlapForward)
        {
            forwardPercent = minForwardPercent;
        }
        else if (wingAngleLeft > rollAmountTriggersForwardFlap && wingAngleRight > rollAmountTriggersForwardFlap)
        {
            forwardPercent = 1;
        }
        else
        {
            float vel = Mathf.Clamp(rigidBody.velocity.magnitude, minVelToFlapForward, maxVelToFlapForward);
            forwardPercent = (vel - minVelToFlapForward) / (maxVelToFlapForward - minVelToFlapForward);
            forwardPercent = Mathf.Max(forwardPercent, minForwardPercent);
        }

        Vector3 dir = (transform.forward * (forwardPercent)) + (transform.up * (1 - forwardPercent)).normalized;

        float scale = 1 - (forwardPercent * (1 - flappingForwardScale));

        return dir * scale;



        //return (transform.forward * (0.5f)) + (transform.up * (1 - 0.5f));

        //float minVelToFlapForward = 10f;
        //float maxVelToFlapForward = 100f;
        //if (isGrounded || rigidBody.velocity.magnitude < minVelToFlapForward)
        //{
        //    return transform.up;
        //}
        //else
        //{
        //    float vel = Mathf.Clamp(rigidBody.velocity.magnitude, minVelToFlapForward, maxVelToFlapForward);

        //    float forwardPercent = (vel - minVelToFlapForward) / (maxVelToFlapForward - minVelToFlapForward);

        //    return (transform.forward * (forwardPercent)) + (transform.up * (1 - forwardPercent));
        //}

        //if (isGrounded)
        //{
        //    return transform.up;
        //}
        //else
        //{
        //    float forwardPercent = 0.25f;

        //    return (transform.forward * (forwardPercent)) + (transform.up * (1 - forwardPercent));
        //}
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
        Util.DrawRigidbodyRay(rigidBody, transform.position, liftDirection * 10, Color.blue);
        Util.DrawRigidbodyRay(rigidBody, transform.position, transform.forward * 10, Color.blue);
        Util.DrawRigidbodyRay(rigidBody, transform.position, transform.up * 10, Color.blue);

        if (WingsOut ()) {
			/**
			 * Left
			*/
			float wingAngleLeftAbs = Mathf.Abs (wingAngleLeft);
			Vector3 wingForwardDirectionLeft = (transform.forward * (1 - wingAngleLeftAbs * wingUpDirectionScale) - transform.up * (wingAngleLeft) * wingUpDirectionScale).normalized;

            angleOfAttackLeft = CalculateAngleOfAttack (wingForwardDirectionLeft);
			realLiftCoefLeft = CalculateLiftCoef (angleOfAttackLeft);
            
			birdAnimator.liftLeft = wingOutAmountLeft;
			dragonAnimator.liftLeft = wingOutAmountLeft;

			liftLeft = CalculateLift (realLiftCoefLeft, wingOutAmountLeft, wingAngleLeft);

			Vector3 leftDirection = liftDirection;
			leftPosition = -transform.right;
			if (wingOutAmountLeft < 0) {
				leftPosition -= transform.right * wingOutAmountLeft * rollScale;
			}
			leftPosition = CalculateWingPosition (leftPosition.normalized);
			Vector3 leftForce = leftDirection * liftLeft;
			rigidBody.AddForceAtPosition (leftForce, leftPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftPosition, leftForce, Color.yellow);


			/**
			 * Right
			*/
			float wingAngleRightAbs = Mathf.Abs (wingAngleRight);
			Vector3 wingForwardDirectionRight = (transform.forward * (1 - wingAngleRightAbs * wingUpDirectionScale) - transform.up * (wingAngleRight) * wingUpDirectionScale).normalized;

			angleOfAttackRight = CalculateAngleOfAttack (wingForwardDirectionRight);
			realLiftCoefRight = CalculateLiftCoef (angleOfAttackRight);
            
			birdAnimator.liftRight = wingOutAmountRight;
			dragonAnimator.liftRight = wingOutAmountRight;

			liftRight = CalculateLift (realLiftCoefRight, wingOutAmountRight, wingAngleRight);

			Vector3 rightDirection = liftDirection;
			rightPosition = transform.right;
			if (wingOutAmountRight < 0) {
				rightPosition += transform.right * wingOutAmountRight * rollScale;
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
		return liftCoef * Mathf.Sin (angleOfAttack * Mathf.Deg2Rad);
	}

	public float CalculateLift(float calculatedLiftCoef, float wingOutAmount, float wingAngle) {
        float liftSpeed = Mathf.Clamp(speed, 0, maxLiftSpeed);

		float calculatedLift = 0.5f * airDensity * liftSpeed * liftSpeed * wingLiftSurfaceArea * calculatedLiftCoef * (1 + wingOutAmount);
		calculatedLift = Mathf.Clamp(calculatedLift, -maxLift, maxLift);

        if (calculatedLift < minLift && calculatedLift > -minLift && wingAngle != 0)
        {
            calculatedLift = -minLift * wingAngle;
        }

        return calculatedLift;
    }

	public Vector3 CalculateWingPosition (Vector3 direction) {
		return direction * wingOutDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance + transform.position;
	}

	public void SeparateDrag (float liftCoefLeft, float liftCoefRight, float angleOfAttackLeft, float angleOfAttackRight, float liftLeft, float liftRight, Vector3 leftPosition, Vector3 rightPosition){
		if (rigidBody.velocity.magnitude > 0 && !isBackFlapping) {
			float dragScaleLeft = wingAngleLeft * (-0.75f) + 1;
			float dragScaleRight = wingAngleRight * (-0.75f) + 1;

			dragScaleLeft *= (1 + wingOutAmountLeft * rollDragScale);
			dragScaleRight *= (1 + wingOutAmountLeft * rollDragScale);

			//parasitic
			float parasiticDragMagnitudeLeft = 0.25f * airDensity * speed * speed * wingDragSurfaceArea * parasiticDragCoef;
            if (isBackFlapping)
            {
                parasiticDragMagnitudeLeft *= backFlapDragCoef;
            }
			float parasiticDragMagnitudeRight = parasiticDragMagnitudeLeft;
			parasiticDragMagnitudeLeft *= dragScaleLeft;
			parasiticDragMagnitudeRight *= dragScaleRight;
			Vector3 parasiticDirection = rigidBody.velocity.normalized * (-1);

            //left
            leftPosition = transform.position - transform.right * wingOutDragDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			Vector3 leftParasiticDirection = parasiticDirection;
			Vector3 leftParasiticDragForce = parasiticDragMagnitudeLeft * leftParasiticDirection * 1;
			Vector3 leftParasiticPosition = leftPosition - transform.forward * dragForwardDistance;
			rigidBody.AddForceAtPosition (leftParasiticDragForce, leftParasiticPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, leftParasiticPosition, leftParasiticDragForce, Color.red);

			//right
			rightPosition = transform.position + transform.right * wingOutDragDistance + transform.forward * wingForwardDistance + transform.up * wingUpDistance;
			Vector3 rightParasiticDirection = parasiticDirection;
			Vector3 rightParasiticDragForce = parasiticDragMagnitudeRight * rightParasiticDirection * 1;
			Vector3 rightParasiticPosition = rightPosition - transform.forward * dragForwardDistance;
			rigidBody.AddForceAtPosition (rightParasiticDragForce, rightParasiticPosition, ForceMode.Force);
			Util.DrawRigidbodyRay (rigidBody, rightParasiticPosition, rightParasiticDragForce, Color.red);


            //yaw drag
            float leftYaw = 0f;
            float rightYaw = 0f;
            if (yaw < 0)
            {
                parasiticDirection = transform.forward * (-1);
                leftYaw = -yaw * yawScale;
                rightYaw = (1 + yaw) * yawScale;
            }
            else if ((yaw > 0))
            {
                parasiticDirection = transform.forward * (-1);
                rightYaw = yaw * yawScale;
                leftYaw = (1 - yaw) * yawScale;
            }

            rigidBody.AddForceAtPosition(transform.right * yaw * yawScale, transform.position + transform.forward);
            Util.DrawRigidbodyRay(rigidBody, transform.position + transform.forward, transform.right * yaw * yawScale, Color.green);

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