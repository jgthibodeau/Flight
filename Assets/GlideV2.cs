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
    public float rotatingLiftCoef;
    public float minLift;
    public float maxLift;
    public float maxLiftRotate;
    public float maxLiftSpeed;

    public float yawScale;
    public float rollScale;

    public float minSpeedAngularDrag = 0;
    public float maxSpeedAngularDrag = 30;

    public float rigidBodyDrag = 0f, backFlapRigidBodyDrag = 1;
    public float minRigidBodyAngularDrag = 1f;
    public float rigidBodyAngularDrag = 10f;

    public float minRigidBodyAngularDragFlapping = 1f;
    public float rigidBodyAngularDragFlapping = 10f;

    public float minRigidBodyAngularDragRotate = 1f;
    public float rigidBodyAngularDragRotate = 10f;

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
    public float goodFlapScale = 2;
    public float flapHoverUprightRotationSpeed = 1f;
	public float flapHoverRotationSpeed = 1f;
    public float flapHoverMoveSpeed = 1f;
    public float backFlapStableAngle = 0.1f;
    public float backFlapStableSpeed = 2f;
    public float backFlapStopSpeed = 1f;
    public float maxBackFlapStopSpeed = 100f;
    public bool backFlapRotateOnly;
    public bool backflapVerticalMovement;

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
    public bool backFlapHeld;
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

    public float rotateSpeed = 3f;
    
    public Transform leftWing;
	private Vector3 leftWingInitialRotation;
	public Transform rightWing;
	private Vector3 rightWingInitialRotation;

    private bool overrideFlight, overrideBackFlap;

    //Inputs
    public float wingOutAmountLeft, wingOutAmountRight, wingAngleLeft, wingAngleRight, yaw, flapDirection;
	public bool boostTriggered, boostHeld, boosting, flapHeld, rotateHeld;

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
        setFlapDirection(0);
        flapHeld = false;
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
		if ((flapping/* || backFlapHeld*/) && !boosting && !boostHeld) {
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
        dragonAnimator.StableBackFlap = isBackFlapping && backFlapStable;

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
        
		birdAnimator.WingsOut = !rotateHeld && WingsOut ();
		birdAnimator.pitchLeft = wingAngleLeft;
		birdAnimator.pitchRight = wingAngleRight;
		birdAnimator.rollLeft = -wingOutAmountLeft;
		birdAnimator.rollRight = -wingOutAmountRight;


		dragonAnimator.WingsOut = !rotateHeld && WingsOut ();
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

	public bool WingsOut()
    {
        return !boosting && !boostHeld;
        //return !rotateHeld && !boosting && !boostHeld;
    }

	void FixedUpdate () {
        //		SteadyFlapOverTime ();
        //		if (!flapping) {
        //			isBackFlapping = false;
        //		}

        //if (!flapping || isGrounded || wingAngleLeft > 0 || wingAngleRight > 0 || (wingAngleLeft > rollAmountTriggersBackflap && wingAngleRight > rollAmountTriggersBackflap))
        if (isGrounded || !backFlapHeld)
        {
            isBackFlapping = false;
		}

        if (overrideFlight)
        {
            return;
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
		} else if (!rotateHeld && WingsOut ()) {
			WingFlap ();
		} else
        {
            ResetWings();
            flapping = false;
            isBackFlapping = false;
            backFlapStable = false;
        }

		if (!isGrounded) {
			rigidBody.drag = rigidBodyDrag;
            if (isBackFlapping && speed < 5)
            {
                rigidBody.drag = backFlapRigidBodyDrag;
                rigidBody.angularDrag = rigidBodyAngularDrag;
            } else if (IsFlapping())
            {
                rigidBody.angularDrag = Util.ConvertScale(minSpeedAngularDrag, maxSpeedAngularDrag, minRigidBodyAngularDragFlapping, rigidBodyAngularDragFlapping, speed);
            } else if (rotateHeld)
            {
                rigidBody.angularDrag = Util.ConvertScale(minSpeedAngularDrag, maxSpeedAngularDrag, minRigidBodyAngularDragRotate, rigidBodyAngularDragRotate, speed);
            } else
            {
                rigidBody.angularDrag = Util.ConvertScale(minSpeedAngularDrag, maxSpeedAngularDrag, minRigidBodyAngularDrag, rigidBodyAngularDrag, speed);
            }

            //rotate towards motion
            if (rotateTowardsMotion && !flapHeld) {
				Vector3 rotation = Quaternion.LookRotation (rigidBody.velocity, transform.up).eulerAngles;
				transform.rotation = Quaternion.Euler (rotation);
			}

			rigidBody.drag = rigidBodyDrag;
			AngledDragLift ();

            if (rotateHeld)
            {
                Vector3 velocity = rigidBody.velocity;
                Vector3 direction = velocity.normalized;
                //Vector3 desiredDirection = Quaternion.AngleAxis(90 * Util.GetAxis("Roll"), transform.up) * Quaternion.AngleAxis(90 * Util.GetAxis("Pitch"), transform.right) * direction;
                Vector3 desiredDirection = Quaternion.AngleAxis(90 * Util.GetAxis("Roll"), transform.up) * direction;

                direction = Vector3.MoveTowards(direction, desiredDirection, rotateSpeed * Time.fixedDeltaTime);

                rigidBody.velocity = direction * speed;

                //Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
                //rigidBody.MoveRotation(rotation);
            }
        } else {
			wingAngleLeft = 0;
			wingAngleRight = 0;

			wingOutAmountLeft = 0;
			wingOutAmountRight = 0;

			drag = 0;
		}
	}
    
	public int currentFlapTick = 0;
	public bool flapping;
	public float[] flapForces;
    public int goodFlapTick;
    
    void WingFlap() {
        float realFlapSpeed = GetFlapSpeed();
        
        if ((backFlapHeld && CanBackFlap()) || overrideBackFlap)
        {
            //BackFlap(realFlapSpeed);
            BackFlapV2(realFlapSpeed);
        } else
        {
            backFlapStable = false;
            if (!isGrounded)
            {
                rigidBody.constraints = RigidbodyConstraints.None;
            }
            if (flapping)
            {
                NormalFlap(realFlapSpeed);
            }
        }
    }

    private void BackFlap(float realFlapSpeed)
    {
        ResetWings();

        backFlapStopCurrentTime = backFlapStopTime;
        isBackFlapping = true;

        Quaternion desiredForward;

        bool backFlapStable = CheckBackFlapStability();

        if (!backFlapStable)
        {
            Vector3 flapForce = -rigidBody.velocity.normalized * realFlapSpeed;
            rigidBody.AddForce(flapForce, flapForceMode);
            Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce, Color.green);

            //rotate to become upright
            Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            desiredForward = Quaternion.Lerp(rigidBody.rotation, Quaternion.LookRotation(projectedForward, Vector3.up), Time.fixedDeltaTime * flapHoverUprightRotationSpeed);
            Util.DrawRigidbodyRay(rigidBody, transform.position, projectedForward * 5, Color.white);
            Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);
            rigidBody.MoveRotation(desiredForward);

            backFlapStable = CheckBackFlapStability();

            if (backFlapStable)
            {
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                rigidBody.rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
        }
        else
        {
            //rigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //rigidBody.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }

        //backflap movement
        if (backFlapRotateOnly)
        {
            //rotate instead of rolling
            float rotateAmount = Util.GetAxis("Roll");
            Vector3 rotateForward = transform.forward * (1 - Mathf.Abs(rotateAmount)) + transform.right * rotateAmount;
            desiredForward = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(rotateForward, Vector3.up), Time.fixedDeltaTime * flapHoverRotationSpeed);
            rigidBody.MoveRotation(desiredForward);
            Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);
        }
        else
        {
            float turnRight = Util.GetAxis("Horizontal Right");
            float turnUp = Util.GetAxis("Vertical Right");

            Vector3 rotateForward = transform.forward * (1 - Mathf.Abs(turnRight)) + transform.right * turnRight;
            desiredForward = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(rotateForward, Vector3.up), Time.fixedDeltaTime * flapHoverRotationSpeed);
            rigidBody.MoveRotation(desiredForward);
            Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);


            float forward = Util.GetAxis("Vertical");
            float right = Util.GetAxis("Horizontal");

            Vector3 movement = flapHoverMoveSpeed * (transform.forward * forward + transform.right * right).normalized;
            rigidBody.AddForce(movement, ForceMode.VelocityChange);

            //rigidBody.MovePosition(rigidBody.position + movement);
            //rigidBody.MovePosition(transform.position + movement);
        }
    }

    private bool backFlapStable;
    private void BackFlapV2(float realFlapSpeed)
    {
        ResetWings();

        backFlapStopCurrentTime = backFlapStopTime;
        isBackFlapping = true;

        Quaternion desiredForward;

        if (!backFlapStable)
        {
            backFlapStable = CheckBackFlapStabilityV2();
        }

        if (!backFlapStable)
        {
            rigidBody.constraints = RigidbodyConstraints.None;

            //Vector3 flapForce = -rigidBody.velocity.normalized * realFlapSpeed;
            //rigidBody.AddForce(flapForce, flapForceMode);
            //Util.DrawRigidbodyRay(rigidBody, transform.position, flapForce, Color.green);

            float stoppingSpeed = Util.ConvertScale(0, maxBackFlapStopSpeed, backFlapStopSpeed, maxBackFlapStopSpeed, rigidBody.velocity.magnitude);
            rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, Vector3.zero, backFlapStopSpeed * Time.fixedDeltaTime);

            //rotate to become upright
            Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            desiredForward = Quaternion.Lerp(rigidBody.rotation, Quaternion.LookRotation(projectedForward, Vector3.up), Time.fixedDeltaTime * flapHoverUprightRotationSpeed);
            Util.DrawRigidbodyRay(rigidBody, transform.position, projectedForward * 5, Color.white);
            Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);
            rigidBody.MoveRotation(desiredForward);

            //backFlapStable = CheckBackFlapStability();

            //backFlapStable = rigidBody.velocity.magnitude == 0;
            //if (backFlapStable)
            //{
            //    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            //    rigidBody.rotation = Quaternion.LookRotation(forward, Vector3.up);
            //}
        }
        else
        {
            //rigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //rigidBody.velocity = Vector3.zero;
            rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //rigidBody.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            rigidBody.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        //backflap movement
        if (backFlapRotateOnly)
        {
            //rotate instead of rolling
            float rotateAmount = Util.GetAxis("Roll");
            Vector3 rotateForward = transform.forward * (1 - Mathf.Abs(rotateAmount)) + transform.right * rotateAmount;
            desiredForward = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(rotateForward, Vector3.up), Time.fixedDeltaTime * flapHoverRotationSpeed);
            rigidBody.MoveRotation(desiredForward);
            Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);
        }
        else
        {
            if (!playerScript.isFlaming)
            {
                float turnRight = Util.GetAxis("Horizontal Right");
                
                Vector3 rotateForward = transform.forward * (1 - Mathf.Abs(turnRight)) + transform.right * turnRight;
                desiredForward = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(rotateForward, Vector3.up), Time.fixedDeltaTime * flapHoverRotationSpeed);
                rigidBody.MoveRotation(desiredForward);
                Util.DrawRigidbodyRay(rigidBody, transform.position, desiredForward.eulerAngles * 5, Color.black);
            }


            if (backFlapStable)
            {
                float forward = Util.GetAxis("Vertical");
                float right = Util.GetAxis("Horizontal");

                Vector3 forwardDirection = backflapVerticalMovement ? playerScript.camera.transform.forward : transform.forward;

                Vector3 movement = flapHoverMoveSpeed * (forwardDirection * forward + transform.right * right).normalized;
                //rigidBody.AddForce(movement, ForceMode.VelocityChange);

                //rigidBody.MovePosition(rigidBody.position + movement);
                //rigidBody.MovePosition(transform.position + movement);

                rigidBody.velocity = movement;
            }
        }
    }

    public bool CheckBackFlapStability()
    {
        return Vector3.Angle(-rigidBody.velocity.normalized, Vector3.up) <= backFlapStableAngle;
    }

    public bool CheckBackFlapStabilityV2()
    {
        return (rigidBody.velocity.magnitude <= backFlapStableSpeed) && (Vector3.Angle(transform.up, Vector3.up) <= backFlapStableAngle);
    }

    public float backflapHeight = 2f;
    public bool CanBackFlap()
    {
        return !isGrounded/* && !Physics.Raycast(transform.position, Vector3.down, backflapHeight, playerScript.layerMaskForGround)*/;
    }

    private void NormalFlap(float realFlapSpeed)
    {
        Vector3 flapPositionLeft = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z - transform.right * flapOutDistance;
        Vector3 flapPositionRight = transform.position + transform.up * playerScript.centerOfGravity.y + transform.forward * playerScript.centerOfGravity.z + transform.right * flapOutDistance;

        float flapLeft = -2 * wingOutAmountLeft;
        Vector3 flapForceDirectionLeft = CalculateFlapForceDirectionV2();
        Vector3 flapForceLeft = flapForceDirectionLeft * realFlapSpeed;
        rigidBody.AddForceAtPosition(flapForceLeft, flapPositionLeft, flapForceMode);
        Util.DrawRigidbodyRay(rigidBody, flapPositionLeft, flapForceLeft, Color.red);

        float flapRight = -2 * wingOutAmountRight;
        Vector3 flapForceDirectionRight = CalculateFlapForceDirectionV2();
        Vector3 flapForceRight = flapForceDirectionRight * realFlapSpeed;
        rigidBody.AddForceAtPosition(flapForceRight, flapPositionRight, flapForceMode);
        Util.DrawRigidbodyRay(rigidBody, flapPositionRight, flapForceRight, Color.blue);
    }
    
    private bool goodFlap;
    private bool nextFlapGood;
    public float GetFlapSpeed()
    {
        int flapTicks = flapForces.Length;
        float flapForce;

        if (isBackFlapping)
        {
            flapping = true;
            currentFlapTick = flapTicks;
            flapForce = flapForces[(int)(flapForces.Length * 0.5f)];
        }

        else if (backFlapStopCurrentTime > 0)
        {
            flapping = flapHeld;
            flapForce = 0;
            backFlapStopCurrentTime -= Time.fixedDeltaTime;
        }

        else
        {
            if (flapHeld)
            {
                if (CanFlap())
                {
                    flapping = true;
                    if (currentFlapTick > goodFlapTick)
                    {
                        nextFlapGood = true;
                    }
                }
                else if (flapping && currentFlapTick < flapTicks)
                {
                    currentFlapTick++;
                    if (currentFlapTick >= flapTicks)
                    {
                        currentFlapTick = 0;
                        goodFlap = nextFlapGood;
                        nextFlapGood = false;
                    }
                }
                else
                {
                    currentFlapTick = 0;
                    nextFlapGood = false;
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
                    nextFlapGood = false;
                    currentFlapTick = 0;
                    flapping = false;
                }
            }
            
            flapForce = flapForces[(int)(flapForces.Length * 0.5f)];

            //if (goodFlap)
            //{
            //    flapForce *= goodFlapScale;
            //}
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

	public float CalculateLiftCoef(float angleOfAttack)
    {
        if (rotateHeld)
        {
            return rotatingLiftCoef * Mathf.Sin(angleOfAttack * Mathf.Deg2Rad);
        } else
        {
            return liftCoef * Mathf.Sin(angleOfAttack * Mathf.Deg2Rad);
        }
    }

	public float CalculateLift(float calculatedLiftCoef, float wingOutAmount, float wingAngle) {
        float liftSpeed = Mathf.Clamp(speed, 0, maxLiftSpeed);

		float calculatedLift = 0.5f * airDensity * liftSpeed * liftSpeed * wingLiftSurfaceArea * calculatedLiftCoef * (1 + wingOutAmount);
        if (rotateHeld)
        {
            calculatedLift = Mathf.Clamp(calculatedLift, -maxLift, maxLiftRotate);
        } else
        {
            calculatedLift = Mathf.Clamp(calculatedLift, -maxLift, maxLift);
        }

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

    private Coroutine currentAltFlightCoroutine;
    private void StartFlightCoroutine(IEnumerator coroutine)
    {
        overrideFlight = true;
        if (currentAltFlightCoroutine != null)
        {
            StopCoroutine(currentAltFlightCoroutine);
        }
        currentAltFlightCoroutine = StartCoroutine(coroutine);
    }

    public float brakeForce, brakeTime;
    public void Brake()
    {
        Debug.Log("braking");
        //StartFlightCoroutine(BrakeRoutine());
        StartCoroutine(BrakeRoutine());
    }
    private IEnumerator BrakeRoutine()
    {
        //apply brake force for brake time seconds
        overrideBackFlap = true;
        yield return new WaitForSeconds(brakeTime);
        overrideBackFlap = false;
        
        //overrideFlight = false;
    }

    public void Dive()
    {
        Debug.Log("diving");
        StartFlightCoroutine(DiveRoutine());
    }
    public float diveRotateSpeed, diveSpeed, diveBrakeSpeed;
    private IEnumerator DiveRoutine()
    {
        //rotate velocity towards down at dive speed
        Vector3 velocity = rigidBody.velocity;
        Vector3 direction = velocity.normalized;
        Vector3 desiredDirection = Vector3.down;
        //float speed = Mathf.Max(velocity.magnitude, diveSpeed);
        float speed = diveSpeed;
        while (rigidBody.velocity.magnitude > 0)
        {
            rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, Vector3.zero, diveBrakeSpeed);
            if (rigidBody.velocity.magnitude == 0)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }

        while (direction != desiredDirection)
        {
            direction = Vector3.MoveTowards(direction, desiredDirection, diveRotateSpeed * Time.fixedDeltaTime);

            rigidBody.velocity = direction * speed;

            Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
            rigidBody.MoveRotation(rotation);

            yield return new WaitForFixedUpdate();
        }
        overrideFlight = false;
    }

    public float turnRotateSpeed, turnMoveSpeed, turnBrakeSpeed;
    public void Turn(float degrees)
    {
        Debug.Log("turning " + degrees);
        StartFlightCoroutine(TurnRoutine(degrees));
    }
    private IEnumerator TurnRoutine(float degrees)
    {
        //determine direction by rotating about up by degrees
        //rotate velocity towards direction at turn speed
        Vector3 velocity = rigidBody.velocity;
        Vector3 direction = velocity.normalized;
        Vector3 desiredDirection = Quaternion.AngleAxis(degrees, transform.up) * direction;
        //float speed = Mathf.Min(velocity.magnitude, turnMoveSpeed);
        float speed = turnMoveSpeed;
        while (rigidBody.velocity.magnitude > 0)
        {
            rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, Vector3.zero, turnBrakeSpeed);
            if (rigidBody.velocity.magnitude == 0)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }

        while (direction != desiredDirection)
        {
            direction = Vector3.MoveTowards(direction, desiredDirection, turnRotateSpeed * Time.fixedDeltaTime);

            rigidBody.velocity = direction * speed;

            Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
            rigidBody.MoveRotation(rotation);

            yield return new WaitForFixedUpdate();
        }
        overrideFlight = false;
    }
}