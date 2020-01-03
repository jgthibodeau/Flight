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

public class Player : MonoBehaviour
{
    private GlideV2 glideV2Script;
    private Grab grabScript;
	private Perch perchScript;
	private Walk walkScript;
	private Stamina staminaScript;
	private DiscreteStamina discreteStaminaScript;
	private Interactor interactorScript;
	private FlameBreath flameBreathScript;
	private Health healthScript;
    public FreeFormCameraTarget freeFormCameraTarget;
    public PrefabSpawner gustSpawner;

    public Camera camera { get; private set; }

    private int PerchableLayer;
	private int EnemyLayer;
	private int PreyLayer;

	public bool twoStickFlight = true;
    public Transform[] headCameraTargets;
    public Transform headCameraTarget, headCameraBackFlapTarget;
    [Range(0, 1)]
    public float headCameraYScale = 1, headCameraXScale = 1, headCameraBackFlapYScale = 1, headCameraBackFlapXScale = 1;
    
	public LayerMask layerMaskForGround;
	public LayerMask layerMaskForWater;
	public float waterBobAmount, waterBobTime, timeSinceWaterBob;
	public float airGroundDistance = 0.1f;
	public float groundDistance = 0.15f;
    public float fallCheckDistance = 0.5f;
    public float groundCheckRadius = 1f;
    public bool inWater;
	public bool isGrounded;
    public bool isFlying;
    public bool isFalling;
	public bool isUpright;
	public bool isFlaming;
	public bool isGusting;
    public bool isRotating;

    public enum State { WALKING, FLYING, LANDING, JUMPING, SWIMMING }
    public State state = State.WALKING;

    private Coroutine takeOffTransition;
    private Coroutine landTransition;
    
    public float rotateSpeed;
    public float uprightThreshold;
	public float speed, previousSpeed;
	public float ragdollSpeed;
	public Vector3 groundNormal;
	private Collider characterCollider;
	public BirdAnimator birdAnimator;
	public DragonAnimator dragonAnimator;
	private Rigidbody rigidBody;
	private bool isFlapping;
    private bool flapTriggered, flapHeld;
    private Vector3 center;
	private bool landed;
	public float gravity;
    public float groundGravity;
    public float fallingGravity;
    public float gravityForwardDistance;
	public float gravityDownDistance;
    public Vector3 centerOfGravity;
    public Vector3 fallingCenterOfGravity;
    public ForceMode gravityForceMode;
    public ForceMode groundGravityForceMode;
    public ForceMode fallingGravityForceMode;

    public bool canJump, canFall;
    public float jumpForce;
    public ForceMode jumpForceMode;
    public bool keepUprightAlways;

    public float jumpDrag = 0;
    public float jumpAngularDrag = 10;

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
    public bool backFlapHeld, backFlapStuntsEnabled;

    [Range(0, 1)]
	public float leftRightWiggle = 0.01f;

    public float backflapTapTime = 0.1f;
    private float backflapStartTime;

    // Use this for initialization
    void Start () {
        camera = Camera.main;

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

    public float jumpWaitTime = 5;
    private float nextJumpTime = 5;
    public float landWaitTime = 20;
    private float nextLandTime = 20;
    public float flapWaitTime = 20;
    private float nextFlapTime = 20;
    void FixedUpdate () {
		CheckGround ();

		//assume not fully grounded
		landed = false;
        previousSpeed = speed;
        speed = rigidBody.velocity.magnitude;

//		//not on ground
//		if (!isGrounded || isFlapping) {
////			if (keepUprightAlways) {
//			AirGravity ();
////			}
//		}
//		//TODO on ground, but moving fast
////		else if (speed > ragdollSpeed) {
////			RagdollGravity ();
////		}
//		//on ground and not upright
//		else if (!isUpright) {
//			GroundGravity ();
//		}
//		//on ground and upright
//		else {
//			GroundGravity ();
////			if (rigidBody.velocity.magnitude <= 0.01f) {
//				landed = true;
////			}
//		}
		//not on ground
		if (isFlying || isFlapping) {
		//if (state == State.FLYING) {
//			if (keepUprightAlways) {
			AirGravity ();
//			}
		}
		//TODO on ground, but moving fast
//		else if (speed > ragdollSpeed) {
//			RagdollGravity ();
//		}
        else if (!isGrounded && isFalling)
        {
            FallingGravity();
        }
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


        //if grounded && (flying or falling), and not already transitioning to ground
        //kill current transition
        //start ground transition
        //if !grounded && !flying && flapping && not already transitioning to flying
        //kill current transition
        //start fly transition


        //FlightStateMachine();
        //TakeoffLandTransitions();
        TakeoffLandTransitionsV2();

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

    private void FlightStateMachine()
    {
        switch (state)
        {
            case State.WALKING:
                glideV2Script.isGrounded = true;
                walkScript.isGrounded = true;
                walkScript.isFlying = false;

                //if jump triggered, jump
                if (flapTriggered)
                {
                    FlapTriggeredState();
                }

                if (!isGrounded)
                {
                    if (Time.time >= nextLandTime)
                    {
                        if (canFall)
                        {
                            state = State.JUMPING;
                            nextJumpTime = Time.time + jumpWaitTime;
                        }
                        else
                        {
                            state = State.FLYING;
                        }
                    }
                }
                else
                {
                    nextLandTime = Time.time + landWaitTime;
                }

                break;
            case State.JUMPING:
                glideV2Script.isGrounded = true;
                walkScript.isGrounded = false;
                walkScript.isFlying = false;

                rigidBody.drag = jumpDrag;
                rigidBody.angularDrag = jumpAngularDrag;

                //if jump triggered, fly
                if (flapTriggered)
                {
                    Debug.Log("Flap");
                    staminaScript.usingStamina = true;
                    glideV2Script.flapHeld = true;
                    state = State.FLYING;
                    nextFlapTime = Time.time + flapWaitTime;
                }

                //if grounded, walk
                if (isGrounded && Time.time >= nextJumpTime)
                {
                    Debug.Log("jump stopped");
                    glideV2Script.isGrounded = true;
                    walkScript.isGrounded = true;
                    walkScript.isFlying = false;

                    rigidBody.drag = walkScript.rigidBodyDrag;
                    rigidBody.angularDrag = walkScript.rigidBodyAngularDrag;

                    state = State.WALKING;
                    nextLandTime = Time.time + landWaitTime;
                    flapTriggered = false;
                }
                break;
            case State.FLYING:
                glideV2Script.isGrounded = false;
                walkScript.isGrounded = false;
                walkScript.isFlying = true;

                //if flapping, keep flapping
                if (flapTriggered || flapHeld || Time.time < nextFlapTime)
                {
                    staminaScript.usingStamina = true;
                    glideV2Script.flapHeld = true;
                }
                else
                {
                    staminaScript.usingStamina = false;
                    glideV2Script.flapHeld = false;

                    //if grounded, land
                    if (isGrounded)
                    {
                        state = State.LANDING;
                    }
                }

                break;
            case State.LANDING:
                glideV2Script.isGrounded = false;
                walkScript.isGrounded = false;
                walkScript.isFlying = false;

                //if landTransition not started, kick it off
                if (landTransition == null)
                {
                    landTransition = StartCoroutine(StartLandTransition());
                }

                //if no longer grounded, stop landing
                if (!isGrounded)
                {
                    StopCoroutine(landTransition);
                    landTransition = null;

                    if (flapTriggered || flapHeld)
                    {
                        FlapTriggeredState();
                    }

                    else if (canFall)
                    {
                        state = State.JUMPING;
                        nextJumpTime = Time.time + jumpWaitTime;
                    }
                    else
                    {
                        state = State.FLYING;
                    }
                }
                break;
        }
    }

    private void FlapTriggeredState()
    {
        flapTriggered = false;
        if (canJump)
        {
            Debug.Log("Jump");
            flapTriggered = false;
            isFalling = true;
            walkScript.isGrounded = false;

            rigidBody.freezeRotation = false;
            rigidBody.drag = jumpDrag;
            rigidBody.angularDrag = jumpAngularDrag;

            Vector3 jumpPosition = rigidBody.position + transform.up * fallingCenterOfGravity.y + transform.forward * fallingCenterOfGravity.z;
            rigidBody.AddForceAtPosition(Vector3.up * jumpForce, jumpPosition, jumpForceMode);
            Util.DrawRigidbodyRay(rigidBody, jumpPosition, Vector3.up * jumpForce, Color.green);

            state = State.JUMPING;
            nextJumpTime = Time.time + jumpWaitTime;
        }
        else
        {
            Debug.Log("Flap");
            staminaScript.usingStamina = true;
            glideV2Script.flapHeld = true;
            state = State.FLYING;
            nextFlapTime = Time.time + flapWaitTime;
        }
    }

    private void TakeoffLandTransitionsV2()
    {
        //if we are not grounded, check for ground and attempt to land
        if (isGrounded)
        {
            bool landTransitionNeeded = (isFlying && !isFlapping) || isFalling;
            
            if (landTransitionNeeded && landTransition == null)
            {
                Debug.Log("Land - starting land transition");
                if (takeOffTransition != null)
                {
                    Debug.Log("Land - killing takeoff transition");
                    StopCoroutine(takeOffTransition);
                }
                landTransition = StartCoroutine(StartLandTransition());
            }
        }

        //if we are grounded, check for flapping
        else
        {
            bool takeoffTransitionNeeded = isFlapping && !isFlying;

            if (takeoffTransitionNeeded && takeOffTransition == null)
            {
                Debug.Log("Takeoff - starting transition");
                if (landTransition != null)
                {
                    Debug.Log("Takeoff - killing land transition");
                    StopCoroutine(landTransition);
                }
                takeOffTransition = StartCoroutine(StartTakeOffTransition());
            }
        }
    }

    private void TakeoffLandTransitions()
    {
        if (isGrounded)
        {
            if (takeOffTransition != null)
            {
                Debug.Log("isGrounded and takeOffTransition running - killing takeOffTransition");
                StopCoroutine(takeOffTransition);
                //takeOffTransition = null;
                if (landTransition == null)
                {
                    FinishTakeOffTransition();
                }
            }

            if (isFlying && landTransition == null)
            {
                Debug.Log("isGrounded, isFlying, and landTransition null - starting landTransition");
                landTransition = StartCoroutine(StartLandTransition());
                if (takeOffTransition != null)
                {
                    StopCoroutine(takeOffTransition);
                    takeOffTransition = null;
                }
            }// else if (!isFlying && landTransition != null)
            //{
            //    Debug.Log("isGrounded, !isFlying, and landTransition running - killing landTransition");
            //    StopCoroutine(landTransition);
            //    landTransition = null;
            //}
            else if (isFalling)
            {
                FinishLandTransition();
            }
        }
        else if (isFalling && !isFlapping && !isFlying)
        {
            walkScript.isGrounded = false;
            walkScript.isFlying = false;
            glideV2Script.isGrounded = true;
        }
        else
        {
            //if flapping, or not grounded for long enough
            if (landTransition != null)
            {
                Debug.Log("!isGrounded and landTransition running - killing landTransition");
                Debug.Break();
                StopCoroutine(landTransition);
                //landTransition = null;
                if (takeOffTransition == null)
                {
                    FinishLandTransition();
                }
            }
            if (isFlapping && !isFlying && takeOffTransition == null)
            //if (!isFlying && takeOffTransition == null)
            {
                Debug.Log("!isGrounded, !isFlying, and takeOffTransition null - starting takeOffTransition");
                takeOffTransition = StartCoroutine(StartTakeOffTransition());
                if (landTransition != null)
                {
                    StopCoroutine(landTransition);
                    landTransition = null;
                }
            }// else if (isFlying && takeOffTransition != null)
            //{
            //    Debug.Log("!isGrounded, isFlying, and takeOffTransition running - killing takeOffTransition");
            //    StopCoroutine(takeOffTransition);
            //    takeOffTransition = null;
            //}
        }
    }

    public float landTime = 0.5f, takeOffTime = 0.5f;
    IEnumerator StartLandTransition()
    {
        rigidBody.velocity = Vector3.ProjectOnPlane(transform.forward + transform.up, walkScript.groundNormal).normalized * previousSpeed;
        
        Debug.Log("StartLandTransition");
        glideV2Script.isGrounded = true;
        yield return new WaitForSeconds(landTime);
        FinishLandTransition(previousSpeed);
    }

    void FinishLandTransition(float landSpeed)
    {
        FinishLandTransition();

        float newWalkSpeed = Util.ConvertScale(10, 60, walkScript.walkSpeed, walkScript.runSpeed, landSpeed);
        Debug.Log("converted " + landSpeed + " to " + newWalkSpeed);
        walkScript.currentSpeed = newWalkSpeed;
        walkScript.currentRunDelay = 0;
    }

    void FinishLandTransition()
    {
        Debug.Log("FinishLandTransition");
        state = State.WALKING;
        nextLandTime = Time.time + landWaitTime;
        walkScript.isGrounded = true;
        walkScript.isFlying = false;

        isFlying = false;
        isFalling = false;
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
        //Vector3 gravityForce = Vector3.down * gravity;
        //rigidBody.AddForceAtPosition (gravityForce, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, gravityForceMode);
        //Util.DrawRigidbodyRay(rigidBody, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, gravityForce, Color.gray);

        ApplyGravity(gravity, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, gravityForceMode);
    }

    void FallingGravity(){
        //Vector3 gravityForce = Vector3.down * fallingGravity;
        //rigidBody.AddForceAtPosition (gravityForce, transform.position - transform.up, fallingGravityForceMode);
        //Util.DrawRigidbodyRay(rigidBody, transform.position + transform.up * fallingCenterOfGravity.y + transform.forward * fallingCenterOfGravity.z, gravityForce, Color.gray);

        rigidBody.freezeRotation = false;
        ApplyGravity(fallingGravity, transform.position + transform.up * fallingCenterOfGravity.y + transform.forward * fallingCenterOfGravity.z, fallingGravityForceMode);
    }

	void GroundGravity(){
		//Vector3 gravityForce = Vector3.down * groundGravity;
		//rigidBody.AddForceAtPosition (gravityForce, transform.position - transform.up, groundGravityForceMode);
		//Util.DrawRigidbodyRay(rigidBody, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, gravityForce, Color.gray);

        ApplyGravity(groundGravity, transform.position - transform.up, groundGravityForceMode);
    }

    void RagdollGravity(){
        ApplyGravity(gravity, transform.position, ForceMode.Force);
	}

    void ApplyGravity(float gravityAmount, Vector3 position, ForceMode forceMode)
    {
        Vector3 gravityForce = Vector3.down * gravityAmount;
        rigidBody.AddForceAtPosition(gravityForce, position, forceMode);
        Util.DrawRigidbodyRay(rigidBody, position, gravityForce, Color.gray);
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
        //if (!glideV2Script.isGrounded) {
        if (!isGrounded) {
			//check for ground with small distance below player
			groundCheckDistance = airGroundDistance;
        }
		else {
			//check for ground with more generous distance
			groundCheckDistance = groundDistance;
        }

        //if (!isGrounded)
        //if (!glideV2Script.isGrounded)
        if (!isGrounded)
        {
            groundCapsuleHeight = 1f;
        }
        else
        {
            groundCapsuleHeight = 0.5f;
        }

        isGrounded = DoGroundCheck(groundCapsuleHeight, groundCheckDistance);

        if (isGrounded) {
            FindGroundNormal();
		}

		inWater = Physics.CheckCapsule (
			characterCollider.bounds.center,
			new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z),
			groundCheckDistance,
			layerMaskForWater.value
		);

        //check fall height
        if (!isGrounded && !isFlying && !DoGroundCheck(groundCapsuleHeight, fallCheckDistance))
        {
            isFalling = true;
        }
			
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

    void FindGroundNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 5f, layerMaskForGround))
        {
            groundNormal = hit.normal;

            if (groundNormal.y <= 0.5f)
            {
                groundNormal = Vector3.up;
            }
        }
    }

    bool DoGroundCheck(float capsuleHeight, float groundDistance)
    {
        Vector3 capsuleStart = rigidBody.position + capsuleHeight * transform.forward + groundDistance * Vector3.down;
        Vector3 capsuleEnd = rigidBody.position - capsuleHeight * transform.forward + groundDistance * Vector3.down;

        Vector3 rbOff = rigidBody.velocity * Time.fixedDeltaTime;
        DebugExtension.DebugCapsule(capsuleStart + rbOff, capsuleEnd + rbOff, Color.red, groundCheckRadius);
        Debug.DrawLine(capsuleStart + rbOff, capsuleEnd + rbOff, Color.red);
        Debug.DrawLine(capsuleStart + rbOff, capsuleStart + groundCheckRadius * Vector3.down + rbOff, Color.green);
        Debug.DrawLine(capsuleEnd + rbOff, capsuleEnd + groundCheckRadius * Vector3.down + rbOff, Color.green);

        return Physics.CheckCapsule(
            capsuleStart,
            capsuleEnd,
            groundCheckRadius,
            layerMaskForGround.value
        );
    }

	void UpdateRendering(){
//		birdAnimator.WingsOut = glideV2Script.wingsOut;
//		dragonAnimator.WingsOut = glideV2Script.wingsOut;

		birdAnimator.InWater = inWater;
		birdAnimator.Grounded = landed;//isGrounded && !isFlapping;

		dragonAnimator.InWater = inWater;
        //dragonAnimator.Grounded = isGrounded;
        dragonAnimator.Grounded = walkScript.isGrounded;
        //dragonAnimator.Flying = state == State.FLYING;
        dragonAnimator.Flying = walkScript.isFlying;
    }

	public void GetInput () {
		flameBreathScript.flameOn = false;
		dragonAnimator.Flame = false;
		walkScript.isFlaming = false;
		dragonAnimator.Attack = false;
		dragonAnimator.Healing = false;

		walkScript.forward = 0;
		walkScript.right = 0;

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

        isRotating = Util.GetButton("Rotate");
        glideV2Script.rotateHeld = isRotating && !backFlapHeld;
        if (canPitchRoll)
        {
            OneStickFlight();
        } else
        {
            glideV2Script.ResetInput();
        }

        if (!isGusting)
        {
            walkScript.forward = Util.GetAxis("Vertical");
            walkScript.right = Util.GetAxis("Horizontal");
        }

        //flapTriggered |= Util.GetButtonDown("Flap");
        //flapHeld = flapTriggered | Util.GetButton("Flap");
        Flap();

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

		//bool gustHeld = Util.GetButton ("Gust");
  //      bool gustTriggered = Util.GetButtonDown ("Gust");


        BackFlap();




        //glideV2Script.boostHeld = gustHeld && !backFlapHeld;
        //glideV3Script.boostHeld = gustHeld && !backFlapHeld;

        //if (gustTriggered && !backFlapHeld && discreteStaminaScript.HasStamina())
        //{
        //    if (!isGrounded)
        //    {
        //        if (!glideV2Script.isBackFlapping)
        //        {
        //            if (glideV2Script.CanBoost())
        //            {
        //                glideV2Script.boostTriggered = true;
        //                discreteStaminaScript.UseStamina();
        //            }
        //        }
        //        else
        //        {
        //            SpawnGust();
        //        }
        //    }
        //    else
        //    {
        //        SpawnGust();
        //    }
        //}
    }

    void BackFlap()
    {
        bool backFlapTurn = false;
        if (Util.GetButtonDown("Backflap"))
        {
            backflapStartTime = Time.time;
        }
        if (Util.GetButtonUp("Backflap") && Time.time - backflapStartTime < backflapTapTime)
        {
            backFlapTurn = true;
        }
        backFlapHeld = Util.GetButton("Backflap");

        //Backflap held = do backflap
        glideV2Script.backFlapHeld = backFlapHeld && (Time.time - backflapStartTime) > backflapTapTime;

        if (backFlapTurn && backFlapStuntsEnabled)
        {
            float forward = Util.GetAxis("Vertical");
            float right = Util.GetAxis("Horizontal");

            float forwardAbs = Mathf.Abs(forward);
            float rightAbs = Mathf.Abs(right);

            Debug.Log(forward + " " + right);

            if (forwardAbs > 0 || rightAbs > 0)
            {
                if (forwardAbs > rightAbs)
                {
                    if (forward > 0)
                    {
                        //brake and hard dive
                        glideV2Script.Dive();
                    }
                    else
                    {
                        //brake and hard turn 180
                        glideV2Script.Turn(180);
                    }
                }
                else
                {
                    if (right > 0)
                    {
                        //brake and hard turn right
                        glideV2Script.Turn(90);
                    }
                    else
                    {
                        //brake and hard turn left
                        glideV2Script.Turn(-90);
                    }
                }
            }
            else
            {
                //brake
                glideV2Script.Brake();
            }
        }
    }

    void Flap()
    {
        isFlapping = Util.GetButton("Flap");
        if (staminaScript.HasStamina())
        {
            glideV2Script.flapHeld = isFlapping;
        }
        else
        {
            glideV2Script.flapHeld = false;
        }
        staminaScript.usingStamina = isFlapping;
        

        //bool flapTriggered = Util.GetButtonDown("Flap");
        //bool flap = Util.GetButton("Flap");

        //if (isGrounded)
        //{
        //    if (flapTriggered)
        //    {
        //        //Jump
        //        Debug.Log("Jump");
        //        isFalling = true;
        //        walkScript.isGrounded = false;
        //        rigidBody.AddForceAtPosition(transform.up * jumpForce, transform.position + transform.up * fallingCenterOfGravity.y + transform.forward * fallingCenterOfGravity.z, jumpForceMode);
        //    }
        //    isFlapping = false;
        //} else
        //{
        //    if (flapTriggered)
        //    {
        //        Debug.Log("Flap");
        //        isFlapping = true;
        //    }
        //    else if (!flap)
        //    {
        //        isFlapping = false;
        //    }
        //}

        //if (isFlapping)
        //{
        //    glideV2Script.setFlapSpeed(1);
        //} else
        //{
        //    glideV2Script.setFlapSpeed(0);
        //}
        //staminaScript.usingStamina = isFlapping;
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

    public float minSpeedForYaw, maxSpeedForYaw;
    [Range(0,1)]
    public float minYaw;
    void OneStickFlight()
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
        if (!isRotating)
        {
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

        }

        wingAngleLeft = Mathf.Clamp(wingAngleLeft, minPitch, maxPitch);
        wingAngleRight = Mathf.Clamp(wingAngleRight, minPitch, maxPitch);

        float yaw = 0;
        if (!isRotating)
        {
            if (speed < minSpeedForYaw)
            {
                yaw = input.x * Util.ConvertScale(0, minSpeedForYaw, minYaw, 1, speed);
            }
            else if (speed < maxSpeedForYaw)
            {
                yaw = input.x * Util.ConvertScale(minSpeedForYaw, maxSpeedForYaw, 1, 0, speed);
            }

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

		desiredHeadHoriz = Util.GetAxis ("Horizontal Right") * (isGrounded ? headRotateSideScaleGround : backFlapHeld ? headRotateSideScaleBackflap : headRotateSideScaleAir);
		desiredHeadVert = Util.GetAxis ("Vertical Right");
		if (desiredHeadVert > 0) {
			desiredHeadVert *= (isGrounded ? headRotateUpScaleGround : backFlapHeld ? headRotateUpScaleBackflap : headRotateUpScaleAir);
		} else {
			desiredHeadVert *= (isGrounded ? headRotateDownScaleGround : backFlapHeld ? headRotateDownScaleBackflap :  headRotateDownScaleAir);
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

        //foreach (Transform t in headCameraTargets)
        //{
        //    Vector3 headCameraTargetRot = t.localEulerAngles;
        //    headCameraTargetRot.y = rotY;
        //    headCameraTargetRot.x = rotX;
        //    t.localEulerAngles = headCameraTargetRot;
        //}
        Vector3 headCameraTargetRot = headCameraTarget.localEulerAngles;
        headCameraTargetRot.y = rotY * headCameraYScale;
        headCameraTargetRot.x = rotX * headCameraXScale;
        headCameraTarget.localEulerAngles = headCameraTargetRot;

        headCameraTargetRot = headCameraBackFlapTarget.localEulerAngles;
        headCameraTargetRot.y = rotY * headCameraBackFlapYScale;
        headCameraTargetRot.x = rotX * headCameraBackFlapXScale;
        headCameraBackFlapTarget.localEulerAngles = headCameraTargetRot;
    }
}
