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
    public PrefabSpawner gustSpawner;

    private int PerchableLayer;
	private int EnemyLayer;
	private int PreyLayer;

	public bool twoStickFlight = true;
	public bool flameAdjustCamera = true;
	public bool useHeadCamera = false;
    public Transform headCameraTarget;

	public ThirdPersonCamera.Follow follow;

	public LayerMask layerMaskForGround;
	public LayerMask layerMaskForWater;
	public float waterBobAmount, waterBobTime, timeSinceWaterBob;
	public float airGroundDistance = 0.1f;
	public float groundDistance = 0.15f;
    public float groundCheckRadius = 1f;
    public bool inWater;
	public bool isGrounded;
	public bool isUpright;
	public bool isFlaming;
	public bool isGusting;
	public bool gustTriggered;
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

        glideV2Script.isGrounded = isGrounded;// && !glideV2Script.IsFlapping();
        walkScript.isGrounded = isGrounded;// && !glideV2Script.IsFlapping();

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


	// Update is called once per frame
	void Update () {
		GetInput ();

		UpdateRendering ();

		WaterSound ();

		rigidBody.freezeRotation = isGrounded;
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
		if (!isGrounded || glideV2Script.IsFlapping()) {
			//check for ground with small distance below player
			groundCheckDistance = airGroundDistance;
        }
		else {
			//check for ground with more generous distance
			groundCheckDistance = groundDistance;
        }

        if (!isGrounded)
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
		dragonAnimator.Grounded = isGrounded;

		if (useHeadCamera) {
			if (Util.GetButtonDown ("Center Camera")) {
				playerCameraController.ToggleCamera ();
			}
		}

		if (flameAdjustCamera) {
			if (isFlaming && rigidBody.velocity.magnitude < 5f) {
				playerCameraController.EnableHeadCamera ();
			} else {
				playerCameraController.EnableMainCamera ();
			}
		}

		playerCameraController.SetMainCameraTight (glideV2Script.isBackFlapping);
	}

	public void GetInput () {
		flameBreathScript.flameOn = false;
		dragonAnimator.Flame = false;
		walkScript.isFlaming = false;
		dragonAnimator.Attack = false;
		dragonAnimator.Healing = false;

		walkScript.forward = 0;
		walkScript.right = 0;

		glideV2Script.flapSpeed = 0;

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

		if (twoStickFlight) {
			TwoStickFlight ();
		} else {
			OneStickFlight ();
		}

		walkScript.forward = Util.GetAxis ("Vertical");
		walkScript.right = Util.GetAxis ("Horizontal");

		float flapSpeed = Util.GetAxis ("Flap");
		if (staminaScript.HasStamina ()) {
			glideV2Script.flapSpeed = Util.GetAxis ("Flap");
		} else {
			glideV2Script.flapSpeed = 0;
		}
		staminaScript.usingStamina = flapSpeed != 0;

		isFlapping = flapSpeed > 0;

		bool grabHeld = Util.GetButton ("Grab");
		bool grab = Util.GetButtonDown ("Grab");
//		grabScript.grab = grab;
		if (grab) {
			interactorScript.Pickup ();
		}
		if (!grabHeld) {
			interactorScript.Drop ();
		}

		isGusting = Util.GetButton ("Gust");
		gustTriggered = Util.GetButtonDown ("Gust");

		glideV2Script.boostHeld = isGusting && !glideV2Script.isBackFlapping;

		if (gustTriggered) {
			if (!isGrounded) {
				if (!glideV2Script.isBackFlapping) {
					if (glideV2Script.CanBoost () && discreteStaminaScript.HasStamina ()) {
						glideV2Script.boostTriggered = true;
						discreteStaminaScript.UseStamina ();
					}
				} else {
                    //TODO backflap gusts
                    if (gustSpawner.Spawn())
                    {
                        discreteStaminaScript.UseStamina();
                    }
                }
			} else {
                //TODO ground gusts
                if (gustSpawner.Spawn())
                {
                    discreteStaminaScript.UseStamina();
                }
			}
//			glideV2Script.boostHeld = true;
		} else if (!isGusting || isGrounded) {
//			glideV2Script.boostHeld = false;
		}
	}

	void TwoStickFlight() {
		glideV2Script.pitchLeft = Util.GetAxis ("Vertical");
		glideV2Script.rollLeft = -Util.GetAxis ("Horizontal");

		glideV2Script.pitchRight = Util.GetAxis ("Vertical Right");
		glideV2Script.rollRight = -Util.GetAxis ("Horizontal Right");

		float pitchDiff = glideV2Script.pitchLeft - glideV2Script.pitchRight;
		if (pitchDiff > -leftRightWiggle && pitchDiff < leftRightWiggle) {
			float halfDiff = pitchDiff / 2;
			glideV2Script.pitchLeft -= halfDiff;
			glideV2Script.pitchRight += halfDiff;
		}

		float rollDiff = glideV2Script.rollLeft - glideV2Script.rollRight;
		if (rollDiff > -leftRightWiggle && rollDiff < leftRightWiggle) {
			float halfDiff = rollDiff / 2;
			glideV2Script.rollLeft -= halfDiff;
			glideV2Script.rollRight += halfDiff;
		}
		glideV2Script.rollRight *= -1;
	}

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

	void OneStickFlight() {
		OneStickFlightV2 ();
		return;

		Vector2 input = new Vector2 (Util.GetAxis ("Horizontal"), Util.GetAxis ("Vertical"));
		input = Vector2.ClampMagnitude (input, 1);
		float vert = input.y;
		float horiz = -input.x;

		//left/right -> more lift on that side and less on the opposite side
		if (horiz > 0) {
			glideV2Script.pitchLeft = 0;
			glideV2Script.pitchRight = -horiz * oneStickRollScale;
		} else if (horiz < 0) {
			glideV2Script.pitchLeft = horiz * oneStickRollScale;
			glideV2Script.pitchRight = 0;
		} else {
			glideV2Script.pitchLeft = 0;
			glideV2Script.pitchRight = 0;
		}

		//forward/back -> wings in/out
		if (vert > 0) {
			glideV2Script.pitchLeft += vert * oneStickForwardPitchScale;
			glideV2Script.pitchRight += vert * oneStickForwardPitchScale;

			float wingScale = oneStickWingInScale;
			float percent = 0;
			if (transform.forward.y < oneStickWingMinYToPointDown) {
				percent = (1 + transform.forward.y) / (1 + oneStickWingMinYToPointDown);
				wingScale = Mathf.Lerp (oneStickWingInScale, oneStickWingInScalePointingDown, 1 - percent);
			}

			glideV2Script.rollLeft = -vert * wingScale;
			glideV2Script.rollRight = -vert * wingScale;

		} else {
			glideV2Script.pitchLeft += vert * oneStickBackwardPitchScale;
			glideV2Script.pitchRight += vert * oneStickBackwardPitchScale;

			glideV2Script.rollLeft = -vert * oneStickWingOutScale;
			glideV2Script.rollRight = -vert * oneStickWingOutScale;
		}

		glideV2Script.pitchLeft = Mathf.Clamp (glideV2Script.pitchLeft, minPitch, maxPitch);
		glideV2Script.pitchRight = Mathf.Clamp (glideV2Script.pitchRight, minPitch, maxPitch);
	}

	void OneStickFlightV2() {
		//TODO clean up input so rolling is smoother
		Vector2 input = new Vector2 (Util.GetAxis ("Horizontal"), Util.GetAxis ("Vertical"));
		input = Vector2.ClampMagnitude (input, 1);
		float vert = input.y;
		float horiz = -input.x;

		//forward/back -> wings in/out
		if (vert > 0) {
			glideV2Script.pitchLeft += vert * oneStickForwardPitchScale;
			glideV2Script.pitchRight += vert * oneStickForwardPitchScale;

			float wingScale = oneStickWingInScale;
			float percent = 0;
			if (transform.forward.y < oneStickWingMinYToPointDown) {
				percent = (1 + transform.forward.y) / (1 + oneStickWingMinYToPointDown);
				wingScale = Mathf.Lerp (oneStickWingInScale, oneStickWingInScalePointingDown, 1 - percent);
			}

			glideV2Script.rollLeft = -vert * wingScale;
			glideV2Script.rollRight = -vert * wingScale;



			if (transform.forward.y < oneStickWingMaxYToPointDown) {
				glideV2Script.rollLeft = -1;
				glideV2Script.rollRight = -1;
			}

//			if (transform.forward.y < oneStickWingMinYToPointDown) {
//				glideV2Script.rollLeft = -1;
//				glideV2Script.rollRight = -1;
//				pointingDown = true;
//			} else {
//				glideV2Script.rollLeft = -vert * oneStickWingInScale;
//				glideV2Script.rollRight = -vert * oneStickWingInScale;
//			}

		} else {
			glideV2Script.pitchLeft = vert * oneStickBackwardPitchScale;
			glideV2Script.pitchRight = vert * oneStickBackwardPitchScale;

			glideV2Script.rollLeft = -vert * oneStickWingOutScale;
			glideV2Script.rollRight = -vert * oneStickWingOutScale;
		}
			
//		left/right -> more lift on that side and less on the opposite side
		if (horiz > 0) {
			glideV2Script.pitchRight -= horiz * oneStickRollScale;
			glideV2Script.pitchLeft += horiz * oneStickRollScale;
		} else if (horiz < 0) {
			glideV2Script.pitchRight -= horiz * oneStickRollScale;
			glideV2Script.pitchLeft += horiz * oneStickRollScale;
		}

		glideV2Script.pitchLeft = Mathf.Clamp (glideV2Script.pitchLeft, minPitch, maxPitch);
		glideV2Script.pitchRight = Mathf.Clamp (glideV2Script.pitchRight, minPitch, maxPitch);
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
	public float headRotateUpScaleAir;
	public float headRotateDownScaleAir;
	public float headRotateSideScaleAir;

	public float headRotateUpScaleGround;
	public float headRotateDownScaleGround;
	public float headRotateSideScaleGround;

	public float rotateSpeed;
	private float headHoriz = 0;
	private float headVert = 0;
	private bool rotateHead = false;
	void LateUpdate() {
		RotateHead ();
	}

	void RotateHead(){
		float desiredHeadHoriz = 0f;
		float desiredHeadVert = 0f;

		desiredHeadHoriz = Util.GetAxis ("Horizontal Right") * (isGrounded ? headRotateSideScaleGround : headRotateSideScaleAir);
		desiredHeadVert = Util.GetAxis ("Vertical Right");
		if (desiredHeadVert > 0) {
			desiredHeadVert *= (isGrounded ? headRotateUpScaleGround : headRotateUpScaleAir);
		} else {
			desiredHeadVert *= (isGrounded ? headRotateDownScaleGround : headRotateDownScaleAir);
		}

		headHoriz = Mathf.Lerp (headHoriz, desiredHeadHoriz, rotateSpeed * Time.deltaTime);
		headVert = Mathf.Lerp (headVert, desiredHeadVert, rotateSpeed * Time.deltaTime);

		foreach (Transform t in headComponents) {
			Vector3 rot = t.eulerAngles;
			rot.y += headHoriz;
			rot.z += headVert;
			t.eulerAngles = rot;
		}

        Vector3 headCameraTargetRot = headCameraTarget.localEulerAngles;
        headCameraTargetRot.y = headHoriz * headComponents.Length;
        headCameraTargetRot.x = -headVert * headComponents.Length;

        headCameraTarget.localEulerAngles = headCameraTargetRot;
    }
}
