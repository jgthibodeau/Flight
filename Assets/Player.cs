using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GlideV2))]
[RequireComponent(typeof(Grab))]
[RequireComponent(typeof(Perch))]
[RequireComponent(typeof(Walk))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(DiscreteStamina))]
[RequireComponent(typeof(Interactor))]
[RequireComponent(typeof(FlameBreath))]
[RequireComponent(typeof(Health))]

public class Player : MonoBehaviour {
	private GlideV2 glideV2Script;
	private Grab grabScript;
	private Perch perchScript;
	private Walk walkScript;
	private Stamina staminaScript;
	private DiscreteStamina discreteStaminaScript;
	private Interactor interactorScript;
	private FlameBreath flameBreathScript;
	private Health healthScript;
    public PlayerCameraController playerCameraController;
    public FreeFormCameraTarget freeFormCameraTarget;
    public PrefabSpawner gustSpawner;

    private int PerchableLayer;
	private int EnemyLayer;
	private int PreyLayer;

	public bool twoStickFlight = true;
    public Transform[] headCameraTargets;

	public ThirdPersonCamera.Follow follow;

	public LayerMask layerMaskForGround;
	public LayerMask layerMaskForWater;
	public float waterBobAmount, waterBobTime, timeSinceWaterBob;
	public float airGroundDistance = 0.1f;
	public float groundDistance = 0.15f;
    public float groundCheckRadius = 1f;
    public bool inWater;
	public bool isGrounded;
    public bool isFlying;
	public bool isUpright;
	public bool isFlaming;
	public bool isGusting;

    private Coroutine takeOffTransition;
    private Coroutine landTransition;

    public float uprightThreshold;
	public float speed;
	public float ragdollSpeed;
	public Vector3 groundNormal;
	private Collider characterCollider;
	public BirdAnimator birdAnimator;
	public DragonAnimator dragonAnimator;
	private Rigidbody rigidBody;
	private bool isFlapping;
	private Vector3 center;
	private bool landed;
	public float gravity;
	public float groundGravity;
	public float gravityForwardDistance;
	public float gravityDownDistance;
	public Vector3 centerOfGravity;
	public ForceMode gravityForceMode;
	public bool keepUprightAlways;

	public float minHealRate;
	public float maxHealRate;
	public float healRateRate;
	public float currentHealRate;

	public float minWaterPitch = 0.9f;
	public float maxWaterPitch = 1.1f;
	public float minWaterVolume = 0.25f;
	public float maxWaterVolume = 1f;
	public AudioSource waterAudioSource;

	public Vector3 centerOfMass = new Vector3 (0, 0, 0);
	public Vector3 inertiaTensor = new Vector3 (0, 0, 0);
	public Quaternion inertiaTensorRotation = new Quaternion (0.3f, 0, 0, 1f);
    
    public float stickYTriggersBackflap = -0.9f;
    public float stickYReleaseBackflap = 0.9f;
    public bool backflapTriggered;

    [Range(0, 1)]
	public float leftRightWiggle = 0.01f;

	// Use this for initialization
	void Start () {
		PerchableLayer = LayerMask.NameToLayer ("Perchable");
		EnemyLayer = LayerMask.NameToLayer ("Enemy");
		PreyLayer = LayerMask.NameToLayer ("Prey");

		characterCollider = transform.GetComponent<Collider> ();
		rigidBody = transform.GetComponent<Rigidbody> ();

		glideV2Script = transform.GetComponent<GlideV2> ();
		grabScript = transform.GetComponent<Grab> ();
		perchScript = transform.GetComponent<Perch> ();
		walkScript = transform.GetComponent<Walk> ();
		staminaScript = transform.GetComponent<Stamina> ();
		discreteStaminaScript = transform.GetComponent<DiscreteStamina> ();
		interactorScript = transform.GetComponent<Interactor> ();
		flameBreathScript = transform.GetComponent<FlameBreath> ();
		healthScript = transform.GetComponent<Health> ();

		glideV2Script.birdAnimator = birdAnimator;
		glideV2Script.dragonAnimator = dragonAnimator;

		walkScript.birdAnimator = birdAnimator;
		walkScript.dragonAnimator = dragonAnimator;

		glideV2Script.rigidBody = rigidBody;
		walkScript.rigidBody = rigidBody;

		glideV2Script.gravity = gravity;
	}

	void FixedUpdate () {
		CheckGround ();

		//assume not fully grounded
		landed = false;
		speed = rigidBody.velocity.magnitude;

		//not on ground
		if (!isGrounded || glideV2Script.IsFlapping ()) {
//			if (keepUprightAlways) {
			AirGravity ();
//			}
		}
		//TODO on ground, but moving fast
//		else if (speed > ragdollSpeed) {
//			RagdollGravity ();
//		}
		//on ground and not upright
		else if (!isUpright) {
			GroundGravity ();
		}
		//on ground and upright
		else {
			GroundGravity ();
//			if (rigidBody.velocity.magnitude <= 0.01f) {
				landed = true;
//			}
		}

        //glideV2Script.isGrounded = isGrounded;// && !glideV2Script.IsFlapping();
        //walkScript.isGrounded = isGrounded;// && !glideV2Script.IsFlapping();

        //if isGrounded, then walkScript.isGrounded is true and glideV2Script.isFlying is false by default

        if (isGrounded)
        {
            if (takeOffTransition != null)
            {
                Debug.Log("isGrounded and takeOffTransition running - killing takeOffTransition");
                StopCoroutine(takeOffTransition);
                //takeOffTransition = null;
                FinishTakeOffTransition();
            }

            if (isFlying && landTransition == null)
            {
                Debug.Log("isGrounded, isFlying, and landTransition null - starting landTransition");
                landTransition = StartCoroutine(StartLandTransition());
            }// else if (!isFlying && landTransition != null)
            //{
            //    Debug.Log("isGrounded, !isFlying, and landTransition running - killing landTransition");
            //    StopCoroutine(landTransition);
            //    landTransition = null;
            //}
        } else
        {
            if (landTransition != null)
            {
                Debug.Log("!isGrounded and landTransition running - killing landTransition");
                StopCoroutine(landTransition);
                //landTransition = null;
                FinishLandTransition();
            }
            if (!isFlying && takeOffTransition == null)
            {
                Debug.Log("!isGrounded, !isFlying, and takeOffTransition null - starting takeOffTransition");
                takeOffTransition = StartCoroutine(StartTakeOffTransition());
            }// else if (isFlying && takeOffTransition != null)
            //{
            //    Debug.Log("!isGrounded, isFlying, and takeOffTransition running - killing takeOffTransition");
            //    StopCoroutine(takeOffTransition);
            //    takeOffTransition = null;
            //}
        }



        //		walkScript.isGrounded = isGrounded && !isFlapping;
        //		if (isFlapping) {
        //			glideV2Script.isGrounded = false;
        //		} else if (isGrounded) {
        //			glideV2Script.isGrounded = true;
        //		}
        //		if (isGrounded) {
        //			glideV2Script.wingsOut = false;
        //		}
    }

    public float landTime = 0.5f, takeOffTime = 0.5f;
    IEnumerator StartLandTransition()
    {
        Debug.Log("StartLandTransition");
        glideV2Script.isGrounded = true;
        yield return new WaitForSeconds(landTime);
        FinishLandTransition();
    }

    void FinishLandTransition()
    {
        Debug.Log("FinishLandTransition");
        walkScript.isGrounded = true;
        walkScript.isFlying = false;

        isFlying = false;
        landTransition = null;
    }

    IEnumerator StartTakeOffTransition()
    {
        Debug.Log("StartTakeOffTransition");
        walkScript.isGrounded = false;
        yield return new WaitForSeconds(takeOffTime);
        FinishTakeOffTransition();
    }

    void FinishTakeOffTransition()
    {
        Debug.Log("FinishTakeOffTransition");
        glideV2Script.isGrounded = false;
        walkScript.isFlying = true;

        isFlying = true;
        takeOffTransition = null;
    }


    // Update is called once per frame
    void Update () {
		GetInput ();

		UpdateRendering ();

		WaterSound ();

		rigidBody.freezeRotation = isGrounded;

        if (!isGrounded && wasGrounded)
        {
            if (delayPitchRollInstance != null)
            {
                StopCoroutine(delayPitchRollInstance);
            }
            delayPitchRollInstance = StartCoroutine(DelayPitchRoll());
        } else if (isGrounded)
        {
            canPitchRoll = false;
        }
        wasGrounded = isGrounded;
	}
    public float flapDelayTime = 1f;
    public bool canPitchRoll;
    public bool wasGrounded;
    Coroutine delayPitchRollInstance;
    IEnumerator DelayPitchRoll()
    {
        canPitchRoll = false;
        yield return new WaitForSeconds(flapDelayTime);
        canPitchRoll = true;
    }

	void WaterSound(){
//		if ((inWater || NearWater()) && !waterAudioSource.isPlaying) {
//			waterAudioSource.pitch = Random.Range (minWaterPitch, maxWaterPitch);
//			waterAudioSource.volume = Random.Range (minWaterVolume, maxWaterVolume);
//			if (!waterAudioSource.isPlaying) {
//				waterAudioSource.Play ();
//			}
//		}
	}

	bool NearWater(){
		//down
		//foward down
		//far forward down
		//raycast back down, raycast far forward down
		return false;
	}

	void AirGravity(){
		Vector3 gravityForce = Vector3.down * gravity;
//		rigidBody.AddForceAtPosition (gravityForce, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, ForceMode.Force);
		rigidBody.AddForceAtPosition (gravityForce, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, gravityForceMode);
//		rigidBody.AddForce (gravityForce, gravityForceMode);
		Util.DrawRigidbodyRay(rigidBody, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, gravityForce, Color.gray);
	}

	void GroundGravity(){
//		Vector3 gravityForce = -groundNormal * gravity;
		Vector3 gravityForce = Vector3.down * groundGravity;
		rigidBody.AddForceAtPosition (gravityForce, transform.position - transform.up, ForceMode.Acceleration);
//		rigidBody.AddForceAtPosition (gravityForce/2, transform.position - transform.up * 1 + transform.forward, ForceMode.Force);
//		rigidBody.AddForceAtPosition (gravityForce/2, transform.position - transform.up * 1 - transform.forward, ForceMode.Force);

		Util.DrawRigidbodyRay(rigidBody, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, gravityForce, Color.gray);
	}

	void RagdollGravity(){
		Vector3 gravityForce = Vector3.down * gravity;
		rigidBody.AddForceAtPosition (gravityForce, transform.position, ForceMode.Force);
		Util.DrawRigidbodyRay(rigidBody, transform.position, gravityForce, Color.gray);
	}

	void CheckGround(){
		//if flapping, not grounded
		//if (glideV2Script.IsFlapping()) {
			//isGrounded = false;
		//} else {
		float groundCheckDistance = 0;
        float groundCapsuleHeight = 0;

		//if in air
		//if (!isGrounded || glideV2Script.IsFlapping()) {
        if (!glideV2Script.isGrounded) {
			//check for ground with small distance below player
			groundCheckDistance = airGroundDistance;
        }
		else {
			//check for ground with more generous distance
			groundCheckDistance = groundDistance;
        }

        //if (!isGrounded)
        if (!glideV2Script.isGrounded)
        {
            groundCapsuleHeight = 1f;
        }
        else
        {
            groundCapsuleHeight = 0.5f;
        }

        //         Debug.DrawLine(characterCollider.bounds.center, new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y - groundCheckDistance, characterCollider.bounds.center.z), Color.red);
        //         Debug.DrawLine(characterCollider.bounds.center, new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.center.y + groundCheckRadius, characterCollider.bounds.center.z), Color.green);
        //         Debug.DrawLine(new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y - groundCheckDistance, characterCollider.bounds.center.z), new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y - groundCheckDistance - groundCheckRadius, characterCollider.bounds.center.z), Color.green);

        //         isGrounded = Physics.CheckCapsule (
        //	characterCollider.bounds.center,
        //	new Vector3 (characterCollider.bounds.center.x, characterCollider.bounds.min.y - groundCheckDistance, characterCollider.bounds.center.z),
        //	groundCheckRadius,
        //	layerMaskForGround.value
        //);


        //Vector3 capsuleStart = characterCollider.bounds.center + (characterCollider.bounds.max.z - groundCheckRadius) * transform.forward - groundCheckDistance * Vector3.up;
        //Vector3 capsuleEnd = characterCollider.bounds.center - (characterCollider.bounds.min.z - groundCheckRadius) * transform.forward - groundCheckDistance * Vector3.up;


        Vector3 capsuleStart = rigidBody.position + groundCapsuleHeight * transform.forward + groundCheckDistance * Vector3.down;
        Vector3 capsuleEnd = rigidBody.position - groundCapsuleHeight * transform.forward + groundCheckDistance * Vector3.down;

        Vector3 rbOff = rigidBody.velocity * Time.fixedDeltaTime;
        DebugExtension.DebugCapsule(capsuleStart + rbOff, capsuleEnd + rbOff, Color.red, groundCheckRadius);
        Debug.DrawLine(capsuleStart + rbOff, capsuleEnd + rbOff, Color.red);
        Debug.DrawLine(capsuleStart + rbOff, capsuleStart + groundCheckRadius * Vector3.down + rbOff, Color.green);
        Debug.DrawLine(capsuleEnd + rbOff, capsuleEnd + groundCheckRadius * Vector3.down + rbOff, Color.green);

        isGrounded = Physics.CheckCapsule(
            capsuleStart,
            capsuleEnd,
            groundCheckRadius,
            layerMaskForGround.value
        );

        if (isGrounded) {
				RaycastHit hit;
				if (Physics.Raycast (transform.position, -transform.up, out hit, 5f, layerMaskForGround)) {
					groundNormal = hit.normal;

					if (groundNormal.y <= 0.5f) {
						groundNormal = Vector3.up;
					}
				}
			}

			inWater = Physics.CheckCapsule (
				characterCollider.bounds.center,
				new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z),
				groundCheckDistance,
				layerMaskForWater.value
			);

			//maybe?
			//if grounded and not upright, use ragdoll gravity and forces (don't let walk script take over yet)
			//else, use ground gravity and let walk script take over
		//}


			
//		if (inWater) {
//			if (timeSinceWaterBob > 0) {
//				timeSinceWaterBob -= Time.deltaTime;
//			} else {
//				timeSinceWaterBob = Random.Range (0, waterBobTime);
//				groundNormal = Vector3.up + Vector3.forward * Random.Range (-waterBobAmount, waterBobAmount) + Vector3.right * Random.Range (-waterBobAmount, waterBobAmount);
//			}
//		}

		float uprightAngle = Vector3.Angle (transform.up, groundNormal);
		isUpright = uprightAngle < uprightThreshold;
		walkScript.groundNormal = groundNormal;
	}

	void UpdateRendering(){
//		birdAnimator.WingsOut = glideV2Script.wingsOut;
//		dragonAnimator.WingsOut = glideV2Script.wingsOut;

		birdAnimator.InWater = inWater;
		birdAnimator.Grounded = landed;//isGrounded && !isFlapping;

		dragonAnimator.InWater = inWater;
        //dragonAnimator.Grounded = isGrounded;
        dragonAnimator.Grounded = walkScript.isGrounded;
    }

	public void GetInput () {
		flameBreathScript.flameOn = false;
		dragonAnimator.Flame = false;
		walkScript.isFlaming = false;
		dragonAnimator.Attack = false;
		dragonAnimator.Healing = false;

		walkScript.forward = 0;
		walkScript.right = 0;

		glideV2Script.setFlapSpeed(0);

		bool heal = Util.GetButton ("Heal");
		bool isHealing = heal && isGrounded && !glideV2Script.IsFlapping() && speed <= 5f;
		if (isHealing && healthScript.Heal (currentHealRate * Time.deltaTime)) {
			currentHealRate = Mathf.Clamp (currentHealRate + healRateRate * Time.deltaTime, minHealRate, maxHealRate);
			dragonAnimator.Healing = true;
			return;
		} else {
			currentHealRate = minHealRate;
		}

		isFlaming = Util.GetButton ("Flame") && !interactorScript.itemHolder.HasItem ();
		flameBreathScript.flameOn = isFlaming;
		dragonAnimator.Flame = isFlaming;
		walkScript.isFlaming = isFlaming;

		bool attack = Util.GetButton ("Attack");
		dragonAnimator.Attack = attack;

		if (isFlaming) {
			rotateHead = true;
		} else {
			rotateHead = false;
		}

        if (canPitchRoll)
        {
            //if (twoStickFlight)
            //{
            //    TwoStickFlight();
            //}
            //else
            //{
                OneStickFlight();
            //}
        } else
        {
            glideV2Script.ResetInput();
        }

        if (!isGusting)
        {
            walkScript.forward = Util.GetAxis("Vertical");
            walkScript.right = Util.GetAxis("Horizontal");
        }

		float flapSpeed = Util.GetAxis ("Flap");
		if (staminaScript.HasStamina ()) {
            glideV2Script.setFlapSpeed(flapSpeed);
        } else {
			glideV2Script.setFlapSpeed(0);
		}
		staminaScript.usingStamina = flapSpeed != 0;

		isFlapping = flapSpeed > 0;

		bool grabHeld = Util.GetButton ("Grab");
		bool grab = Util.GetButtonDown ("Grab");
        //		grabScript.grab = grab;
        //if (grab) {
        //	interactorScript.Pickup ();
        //}
        //if (!grabHeld) {
        //	interactorScript.Drop ();
        //}
        if (grab)
        {
            if (interactorScript.itemHolder.HasItem())
            {
                interactorScript.Drop();
            }
            else
            {
                interactorScript.Pickup();
            }
        }

        if (interactorScript.itemHolder.HasItem()) {
            Eatable eatable = interactorScript.itemHolder.heldItem.GetComponent<Eatable>();
            if (eatable != null)
            {
                interactorScript.Drop();
                eatable.Eat(healthScript, interactorScript.itemHolder.heldLocation.position);
            }
        }

		bool gustHeld = Util.GetButton ("Gust");
        bool gustTriggered = Util.GetButtonDown ("Gust");

        //if (backflapTriggered)
        //{
        //    //backflapTriggered = !isGrounded && (flapSpeed > 0) && Util.GetAxis("Vertical") < stickYReleaseBackflap;
        //    backflapTriggered = !isGrounded && gustHeld && Util.GetAxis("Vertical") < stickYReleaseBackflap;
        //}
        //else if (!isGrounded && /*flapSpeed > 0*/gustHeld)
        //{
        //    //backflapTriggered = (flapSpeed > 0) && Util.GetAxis("Vertical") < stickYTriggersBackflap;
        //    backflapTriggered = gustHeld && Util.GetAxis("Vertical") < stickYTriggersBackflap;
        //} else
        //{
        //    backflapTriggered = false;
        //}
        backflapTriggered = Util.GetButton("Backflap");

        glideV2Script.backFlapTriggered = backflapTriggered;

        //if (backflapTriggered)
        //{
        //    glideV2Script.boostHeld = false;
        //} else
        //{
            //glideV2Script.boostHeld = gustHeld;
            glideV2Script.boostHeld = gustHeld && !backflapTriggered;

            if (gustTriggered && !backflapTriggered && discreteStaminaScript.HasStamina())
            {
                if (!isGrounded)
                {
                    if (!glideV2Script.isBackFlapping)
                    {
                        if (glideV2Script.CanBoost())
                        {
                            glideV2Script.boostTriggered = true;
                            discreteStaminaScript.UseStamina();
                        }
                    }
                    else
                    {
                        SpawnGust();
                    }
                }
                else
                {
                    SpawnGust();
                }
            }
        //}
	}

    void SpawnGust()
    {
        //if (!isGusting)
        //{
        //    isGusting = true;
        //    gustSpawner.Spawn();
        //    discreteStaminaScript.UseStamina();
        //    dragonAnimator.GustTriggered = true;
        //}
    }

	//void TwoStickFlight() {
	//	glideV2Script.wingAngleLeft = Util.GetAxis ("Vertical");
	//	glideV2Script.wingOutAmountLeft = -Util.GetAxis ("Horizontal");

	//	glideV2Script.wingAngleRight = Util.GetAxis ("Vertical Right");
	//	glideV2Script.wingOutAmountRight = -Util.GetAxis ("Horizontal Right");

	//	float pitchDiff = glideV2Script.wingAngleLeft - glideV2Script.wingAngleRight;
	//	if (pitchDiff > -leftRightWiggle && pitchDiff < leftRightWiggle) {
	//		float halfDiff = pitchDiff / 2;
	//		glideV2Script.wingAngleLeft -= halfDiff;
	//		glideV2Script.wingAngleRight += halfDiff;
	//	}

	//	float rollDiff = glideV2Script.wingOutAmountLeft - glideV2Script.wingOutAmountRight;
	//	if (rollDiff > -leftRightWiggle && rollDiff < leftRightWiggle) {
	//		float halfDiff = rollDiff / 2;
	//		glideV2Script.wingOutAmountLeft -= halfDiff;
	//		glideV2Script.wingOutAmountRight += halfDiff;
	//	}
	//	glideV2Script.wingOutAmountRight *= -1;
	//}

	public float oneStickRollScale = 0.5f;
	public float oneStickForwardPitchScale = 1f;
	public float oneStickBackwardPitchScale = 1f;
	public float oneStickWingInScale = 0.5f;
	public float oneStickWingMinYToPointDown = -0.9f;
	public float oneStickWingMaxYToPointDown = -0.99f;
	public float oneStickWingInScalePointingDown = 1f;
	public float oneStickWingOutScale = 1f;

	public float minPitch = -1f;
	public float maxPitch = 0.75f;

	void OneStickFlight()
    {
        //OneStickFlightClassic();
        //OneStickFlightYaw();
        OneStickFlightCleanup();
  //      return;

		//Vector2 input = new Vector2 (Util.GetAxis ("Horizontal"), Util.GetAxis ("Vertical"));
		//input = Vector2.ClampMagnitude (input, 1);
		//float vert = input.y;
		//float horiz = -input.x;

		////left/right -> more lift on that side and less on the opposite side
		//if (horiz > 0) {
		//	glideV2Script.wingAngleLeft = 0;
		//	glideV2Script.wingAngleRight = -horiz * oneStickRollScale;
		//} else if (horiz < 0) {
		//	glideV2Script.wingAngleLeft = horiz * oneStickRollScale;
		//	glideV2Script.wingAngleRight = 0;
		//} else {
		//	glideV2Script.wingAngleLeft = 0;
		//	glideV2Script.wingAngleRight = 0;
		//}

		////forward/back -> wings in/out
		//if (vert > 0) {
		//	glideV2Script.wingAngleLeft += vert * oneStickForwardPitchScale;
		//	glideV2Script.wingAngleRight += vert * oneStickForwardPitchScale;

		//	float wingScale = oneStickWingInScale;
		//	float percent = 0;
		//	if (transform.forward.y < oneStickWingMinYToPointDown) {
		//		percent = (1 + transform.forward.y) / (1 + oneStickWingMinYToPointDown);
		//		wingScale = Mathf.Lerp (oneStickWingInScale, oneStickWingInScalePointingDown, 1 - percent);
		//	}

		//	glideV2Script.wingOutAmountLeft = -vert * wingScale;
		//	glideV2Script.wingOutAmountRight = -vert * wingScale;

		//} else {
		//	glideV2Script.wingAngleLeft += vert * oneStickBackwardPitchScale;
		//	glideV2Script.wingAngleRight += vert * oneStickBackwardPitchScale;

		//	glideV2Script.wingOutAmountLeft = -vert * oneStickWingOutScale;
		//	glideV2Script.wingOutAmountRight = -vert * oneStickWingOutScale;
		//}

		//glideV2Script.wingAngleLeft = Mathf.Clamp (glideV2Script.wingAngleLeft, minPitch, maxPitch);
		//glideV2Script.wingAngleRight = Mathf.Clamp (glideV2Script.wingAngleRight, minPitch, maxPitch);
	}

    //void OneStickFlightClassic()
    //{
    //    //TODO clean up input so rolling is smoother
    //    Vector2 input = new Vector2(Util.GetAxis("Horizontal"), Util.GetAxis("Vertical"));
    //    input = Vector2.ClampMagnitude(input, 1);
    //    float vert = input.y;
    //    float horiz = -input.x;

    //    //forward/back -> wings in/out
    //    if (vert > 0)
    //    {
    //        glideV2Script.wingAngleLeft = vert * oneStickForwardPitchScale;
    //        glideV2Script.wingAngleRight = vert * oneStickForwardPitchScale;

    //        float wingScale = oneStickWingInScale;
    //        if (transform.forward.y < oneStickWingMinYToPointDown)
    //        {
    //            float percent = (1 + transform.forward.y) / (1 + oneStickWingMinYToPointDown);
    //            wingScale = Mathf.Lerp(oneStickWingInScale, oneStickWingInScalePointingDown, 1 - percent);
    //        }

    //        glideV2Script.wingOutAmountLeft = vert * wingScale;
    //        glideV2Script.wingOutAmountRight = vert * wingScale;
            
    //        if (transform.forward.y < oneStickWingMaxYToPointDown)
    //        {
    //            glideV2Script.wingOutAmountLeft = -1;
    //            glideV2Script.wingOutAmountRight = -1;
    //        }
    //    }
    //    else
    //    {
    //        glideV2Script.wingAngleLeft = vert * oneStickBackwardPitchScale;
    //        glideV2Script.wingAngleRight = vert * oneStickBackwardPitchScale;

    //        glideV2Script.wingOutAmountLeft = -vert * oneStickWingOutScale;
    //        glideV2Script.wingOutAmountRight = -vert * oneStickWingOutScale;
    //    }

    //    //		left/right -> more lift on that side and less on the opposite side
    //    if (horiz > 0)
    //    {
    //        glideV2Script.wingAngleRight -= horiz * oneStickRollScale;
    //        glideV2Script.wingAngleLeft += horiz * oneStickRollScale;
    //    }
    //    else if (horiz < 0)
    //    {
    //        glideV2Script.wingAngleRight -= horiz * oneStickRollScale;
    //        glideV2Script.wingAngleLeft += horiz * oneStickRollScale;
    //    }

    //    glideV2Script.wingAngleLeft = Mathf.Clamp(glideV2Script.wingAngleLeft, minPitch, maxPitch);
    //    glideV2Script.wingAngleRight = Mathf.Clamp(glideV2Script.wingAngleRight, minPitch, maxPitch);
    //}

    //void OneStickFlightYaw()
    //{
    //    float vert = Util.GetAxis("Vertical");
    //    float yaw = Util.GetAxis("Horizontal");
    //    float horizLeft = Util.GetAxis("Wing Left");
    //    float horizRight = Util.GetAxis("Wing Right");

    //    //forward/back -> wings in/out
    //    if (vert > 0)
    //    {
    //        glideV2Script.wingAngleLeft = vert * oneStickForwardPitchScale;
    //        glideV2Script.wingAngleRight = vert * oneStickForwardPitchScale;

    //        float wingScale = oneStickWingInScale;
    //        float percent = 0;
    //        if (transform.forward.y < oneStickWingMinYToPointDown)
    //        {
    //            percent = (1 + transform.forward.y) / (1 + oneStickWingMinYToPointDown);
    //            wingScale = Mathf.Lerp(oneStickWingInScale, oneStickWingInScalePointingDown, 1 - percent);
    //        }

    //        glideV2Script.wingOutAmountLeft = -vert * wingScale;
    //        glideV2Script.wingOutAmountRight = -vert * wingScale;



    //        if (transform.forward.y < oneStickWingMaxYToPointDown)
    //        {
    //            glideV2Script.wingOutAmountLeft = -1;
    //            glideV2Script.wingOutAmountRight = -1;
    //        }

    //    }
    //    else
    //    {
    //        glideV2Script.wingAngleLeft = vert * oneStickBackwardPitchScale;
    //        glideV2Script.wingAngleRight = vert * oneStickBackwardPitchScale;

    //        glideV2Script.wingOutAmountLeft = -vert * oneStickWingOutScale;
    //        glideV2Script.wingOutAmountRight = -vert * oneStickWingOutScale;
    //    }
    //    glideV2Script.wingAngleRight += horizLeft * oneStickRollScale;
    //    glideV2Script.wingAngleLeft += horizRight * oneStickRollScale;

    //    glideV2Script.yawV2 = yaw;
    //    glideV2Script.yaw = yaw;

    //    glideV2Script.wingAngleLeft = Mathf.Clamp(glideV2Script.wingAngleLeft, minPitch, maxPitch);
    //    glideV2Script.wingAngleRight = Mathf.Clamp(glideV2Script.wingAngleRight, minPitch, maxPitch);
    //}

    public float minSpeedForYaw, maxSpeedForYaw;
    void OneStickFlightCleanup()
    {
        //TODO clean up input so rolling is smoother
        Vector2 input = new Vector2(Util.GetAxis("Roll"), Util.GetAxis("Pitch"));
        input = Vector2.ClampMagnitude(input, 1);
        float vert = input.y;
        float horiz = -input.x;

        float wingAngleLeft = 0;
        float wingAngleRight = 0;
        float wingOutAmountLeft = 0;
        float wingOutAmountRight = 0;

        //forward/back -> wings in/out
        float percent = 1f;
        if (vert > 0)
        {
            wingAngleLeft = vert * oneStickForwardPitchScale;
            wingAngleRight = vert * oneStickForwardPitchScale;

            float wingScale = oneStickWingInScale;
            if (transform.forward.y < oneStickWingMinYToPointDown)
            {
                percent = (1 + transform.forward.y) / (1 + oneStickWingMinYToPointDown);
                wingScale = Mathf.Lerp(oneStickWingInScale, oneStickWingInScalePointingDown, 1 - percent);
            }

            //glideV2Script.wingOutAmountLeft = vert * wingScale;
            //glideV2Script.wingOutAmountRight = vert * wingScale;

            if (transform.forward.y < oneStickWingMaxYToPointDown)
            {
                //glideV2Script.wingOutAmountLeft = -1;
                //glideV2Script.wingOutAmountRight = -1;
                wingOutAmountLeft = oneStickWingMaxYToPointDown;
                wingOutAmountRight = oneStickWingMaxYToPointDown;


                wingAngleLeft = 0.1f;
                wingAngleRight = 0.1f;
            }
        }
        else
        {
            wingAngleLeft = vert * oneStickBackwardPitchScale;
            wingAngleRight = vert * oneStickBackwardPitchScale;

            wingOutAmountLeft = -vert * oneStickWingOutScale;
            wingOutAmountRight = -vert * oneStickWingOutScale;
        }

        //		left/right -> more lift on that side and less on the opposite side
        if (horiz > 0)
        {
            wingAngleRight -= horiz * oneStickRollScale * percent;
            //glideV2Script.wingAngleLeft += horiz * oneStickRollScale;
        }
        else if (horiz < 0)
        {
            //glideV2Script.wingAngleRight -= horiz * oneStickRollScale;
            wingAngleLeft += horiz * oneStickRollScale * percent;
        }

        wingAngleLeft = Mathf.Clamp(wingAngleLeft, minPitch, maxPitch);
        wingAngleRight = Mathf.Clamp(wingAngleRight, minPitch, maxPitch);

        float yaw = input.x;
        if (speed < minSpeedForYaw)
        {
            yaw *= Util.ConvertScale(0, minSpeedForYaw, 0, 1, speed);
        }
        else if (speed < maxSpeedForYaw)
        {
            yaw *= Util.ConvertScale(minSpeedForYaw, maxSpeedForYaw, 1, 0, speed);
        }
        else
        {
            yaw = 0;
        }
        
        glideV2Script.setYaw(yaw);
        
        glideV2Script.setWingAngleLeft(wingAngleLeft);
        glideV2Script.setWingAngleRight(wingAngleRight);
        glideV2Script.setWingOutAmountLeft(wingOutAmountLeft);
        glideV2Script.setWingOutAmountRight(wingOutAmountRight);
    }

    void OnTriggerEnter(Collider collisionInfo) {
		Debug.Log (collisionInfo+" "+collisionInfo.gameObject.tag);
		if (collisionInfo.gameObject.CompareTag ("Edible")) {
			Edible edible = collisionInfo.gameObject.GetComponent<Edible> ();
			float newStamina = edible.Eat ();
			staminaScript.AddStamina (newStamina);
		}

		//TODO handle crashing: close wings and tumble, slowing down if on ground
	}

	public Transform[] headComponents;
    public float headRotateUpScaleBackflap;
    public float headRotateDownScaleBackflap;
    public float headRotateSideScaleBackflap;

    public float headRotateUpScaleAir;
	public float headRotateDownScaleAir;
	public float headRotateSideScaleAir;

	public float headRotateUpScaleGround;
	public float headRotateDownScaleGround;
	public float headRotateSideScaleGround;

	public float regularHeadRotateSpeed;
    public float flameHeadRotateSpeed = 5;
    private float headHoriz = 0;
	private float headVert = 0;
	private bool rotateHead = false;
	void LateUpdate() {
		RotateHead ();
	}

	void RotateHead(){
		float desiredHeadHoriz = 0f;
		float desiredHeadVert = 0f;

		desiredHeadHoriz = Util.GetAxis ("Horizontal Right") * (isGrounded ? headRotateSideScaleGround : backflapTriggered ? headRotateSideScaleBackflap : headRotateSideScaleAir);
		desiredHeadVert = Util.GetAxis ("Vertical Right");
		if (desiredHeadVert > 0) {
			desiredHeadVert *= (isGrounded ? headRotateUpScaleGround : backflapTriggered ? headRotateUpScaleBackflap : headRotateUpScaleAir);
		} else {
			desiredHeadVert *= (isGrounded ? headRotateDownScaleGround : backflapTriggered ? headRotateDownScaleBackflap :  headRotateDownScaleAir);
		}

        float rotateSpeed = isFlaming ? flameHeadRotateSpeed : regularHeadRotateSpeed;
        headHoriz = Mathf.Lerp(headHoriz, desiredHeadHoriz, rotateSpeed * Time.deltaTime);
		headVert = Mathf.Lerp(headVert, desiredHeadVert, rotateSpeed * Time.deltaTime);

		foreach (Transform t in headComponents) {
			Vector3 rot = t.eulerAngles;
			rot.y += headHoriz;
			rot.z += headVert;
			t.eulerAngles = rot;
		}

        float rotY = headHoriz * headComponents.Length;
        float rotX = -headVert * headComponents.Length;

        foreach (Transform t in headCameraTargets)
        {
            Vector3 headCameraTargetRot = t.localEulerAngles;
            headCameraTargetRot.y = rotY;
            headCameraTargetRot.x = rotX;
            t.localEulerAngles = headCameraTargetRot;
        }
    }
}
