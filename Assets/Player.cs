using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GlideV2))]
[RequireComponent(typeof(Grab))]
[RequireComponent(typeof(Perch))]
[RequireComponent(typeof(Walk))]
[RequireComponent(typeof(Stamina))]

public class Player : MonoBehaviour {
	private GlideV2 glideV2Script;
	private Grab grabScript;
	private Perch perchScript;
	private Walk walkScript;
	private Stamina staminaScript;
	private Interactor interactorScript;

	private int PerchableLayer;
	private int EnemyLayer;
	private int PreyLayer;

	public ThirdPersonCamera.Follow follow;

	public LayerMask layerMaskForGround;
	public LayerMask layerMaskForWater;
	public float waterBobAmount, waterBobTime, timeSinceWaterBob;
	public float groundDistance = 0.18f;
	public bool inWater;
	public bool isGrounded;
	public bool isUpright;
	public float uprightThreshold;
	public float speed;
	public float ragdollSpeed;
	public Vector3 groundNormal;
	private Collider characterCollider;
	public BirdAnimator birdAnimator;
	private Rigidbody rigidBody;
	private bool isFlapping;
	private Vector3 center;
	private bool landed;
	public float gravity;
	public float gravityForwardDistance;
	public float gravityDownDistance;
	public Vector3 centerOfGravity;
	public ForceMode gravityForceMode;
	public bool keepUprightAlways;


	public float minWaterPitch = 0.9f;
	public float maxWaterPitch = 1.1f;
	public float minWaterVolume = 0.25f;
	public float maxWaterVolume = 1f;
	public AudioSource waterAudioSource;

	public Vector3 centerOfMass = new Vector3 (0, 0, 0);
	public Vector3 inertiaTensor = new Vector3 (0, 0, 0);
	public Quaternion inertiaTensorRotation = new Quaternion (0.3f, 0, 0, 1f);

	public ParticleSystem flameParticles;

	[Range(0, 1)]
	public float leftRightWiggle = 0.01f;

	// Use this for initialization
	void Start () {
		PerchableLayer = LayerMask.NameToLayer ("Perchable");
		EnemyLayer = LayerMask.NameToLayer ("Enemy");
		PreyLayer = LayerMask.NameToLayer ("Prey");

		characterCollider = transform.GetComponent<Collider> ();
		rigidBody = transform.GetComponent<Rigidbody> ();
//		rigidBody = transform.GetComponentInChildren<Rigidbody> ();

		glideV2Script = transform.GetComponent<GlideV2> ();
		grabScript = transform.GetComponent<Grab> ();
		perchScript = transform.GetComponent<Perch> ();
		walkScript = transform.GetComponent<Walk> ();
		staminaScript = transform.GetComponent<Stamina> ();
		interactorScript = transform.GetComponent<Interactor> ();

		glideV2Script.birdAnimator = birdAnimator;
		walkScript.birdAnimator = birdAnimator;

		glideV2Script.rigidBody = rigidBody;
		walkScript.rigidBody = rigidBody;

		glideV2Script.gravity = gravity;

//		rigidBody.ResetCenterOfMass ();
//		rigidBody.ResetInertiaTensor ();
		rigidBody.centerOfMass = centerOfMass;
		rigidBody.inertiaTensorRotation = inertiaTensorRotation;
//		Debug.Log ("centerOfMass: "+rigidBody.centerOfMass);
//		Debug.Log ("inertiaTensor: "+rigidBody.inertiaTensor);
//		Debug.Log ("inertiaTensorRotation: "+rigidBody.inertiaTensorRotation);


		//		leftWing = transform.Find ("bird2/1/Bird_rig_3/1_2/Backbones_null_3/Wing_3");
		//		rightWing = transform.Find ("bird2/1/Bird_rig_3/1_2/Backbones_null_3/Wing_1_3");

		//		leftWingInitialRotation = leftWing.localRotation.eulerAngles;
		//		rightWingInitialRotation = rightWing.localRotation.eulerAngles;
	}

	void FixedUpdate () {
		rigidBody.centerOfMass = centerOfMass;
		rigidBody.inertiaTensorRotation = inertiaTensorRotation;
//		Debug.Log ("centerOfMass: "+rigidBody.centerOfMass);
//		Debug.Log ("inertiaTensor: "+rigidBody.inertiaTensor);
//		Debug.Log ("inertiaTensorRotation: "+rigidBody.inertiaTensorRotation);

//		center = transform.position + centerOfGravity.x * transform.right + centerOfGravity.y * transform.up + centerOfGravity.z * transform.forward;

		//assume not fully grounded
		landed = false;
		speed = rigidBody.velocity.magnitude;

		//not on ground
		if (!isGrounded) {
//			if (keepUprightAlways) {
			AirGravity ();
//			}
		}
		//trying to get off ground
		else if (glideV2Script.flapSpeed != 0) {
			AirGravity ();
		}
		//TODO on ground, but moving fast
		else if (speed > ragdollSpeed) {
			RagdollGravity ();
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

		glideV2Script.isGrounded = landed;
		walkScript.isGrounded = landed;
	}


	// Update is called once per frame
	void Update () {
		GetInput ();

		CheckGround ();

		UpdateRendering ();

		WaterSound ();
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
		Vector3 gravityForce = -groundNormal * gravity;
		//		rigidBody.AddForceAtPosition (gravityForce, transform.position - transform.up * 1 + transform.forward * centerOfGravity.z, ForceMode.Force);
		rigidBody.AddForceAtPosition (gravityForce/2, transform.position - transform.up * 1 + transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (gravityForce/2, transform.position - transform.up * 1 - transform.forward, ForceMode.Force);

		Util.DrawRigidbodyRay(rigidBody, transform.position + transform.up * centerOfGravity.y + transform.forward * centerOfGravity.z, gravityForce, Color.gray);
	}

	void RagdollGravity(){
		Vector3 gravityForce = Vector3.down * gravity;
		rigidBody.AddForceAtPosition (gravityForce, transform.position, ForceMode.Force);
		Util.DrawRigidbodyRay(rigidBody, transform.position, gravityForce, Color.gray);
	}

	void CheckGround(){
		Debug.DrawLine (characterCollider.bounds.center, new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z), Color.red);

		isGrounded = Physics.CheckCapsule (
			characterCollider.bounds.center,
			new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z),
			groundDistance,
			layerMaskForGround.value
		);

		if (isGrounded) {
			RaycastHit hit;
			if (Physics.Raycast (transform.position, -transform.up, out hit, 1.2f, layerMaskForGround)) {
				groundNormal = hit.normal;
			}
		}

		inWater = Physics.CheckCapsule (
			characterCollider.bounds.center,
			new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z),
			groundDistance,
			layerMaskForWater.value
		);
			
		if (inWater) {
			if (timeSinceWaterBob > 0) {
				timeSinceWaterBob -= Time.deltaTime;
			} else {
				timeSinceWaterBob = Random.Range (0, waterBobTime);
				groundNormal = Vector3.up + Vector3.forward * Random.Range (-waterBobAmount, waterBobAmount) + Vector3.right * Random.Range (-waterBobAmount, waterBobAmount);
			}
		}

		float uprightAngle = Vector3.Angle (transform.up, groundNormal);
		isUpright = uprightAngle < uprightThreshold;
		walkScript.groundNormal = groundNormal;
	}

	void UpdateRendering(){
//		//flap wings
//		if (glideV2Script.flapSpeed != 0) {
//			isFlapping = true;
//			birdAnimator.FlapSpeed = 2f;// + flapAnimationScale * flapSpeed;
//			birdAnimator.Flapping = true;
//
//			//			if(!playingFlapSound){
//			//				StartCoroutine(PlayFlapSound(flapSoundRate*(1-flapSpeed) + minFlapRate));
//			//				playingFlapSound = true;
//			//				flapAudioSource.pitch = Random.Range (flapMinPitch, flapMaxPitch);
//			//			}
//		} else {
//			isFlapping = false;
//			birdAnimator.Flapping = false;
//		}


		birdAnimator.WingsOut = glideV2Script.wingsOut;

		//rotate wings
		if (!isGrounded) {
			//			leftWing.localRotation = Quaternion.Euler (leftWingInitialRotation + new Vector3 ((flapDirection) * 15, -(flapDirection) * 20, 0));
			//			rightWing.localRotation = Quaternion.Euler (rightWingInitialRotation + new Vector3 ((flapDirection) * 15, (flapDirection) * 20, 0));
		} else {
			//			leftWing.localRotation = Quaternion.Euler (leftWingInitialRotation);
			//			rightWing.localRotation = Quaternion.Euler (rightWingInitialRotation);
		}

		birdAnimator.InWater = inWater;
		birdAnimator.Grounded = landed;//isGrounded && !isFlapping;
	}

	public void GetInput () {
		if (Util.GetButton ("Flame")) {
			flameParticles.Play ();
		} else if (flameParticles.isPlaying) {
			flameParticles.Stop ();
		}

		//handle if have grabbed object
		if (grabScript.hasObject) {
			int grabbedLayer = grabScript.grabbedObject.gameObject.layer;
			if (grabbedLayer == PerchableLayer) {
				perchScript.SetPerch (grabScript.grabbedObject, grabScript.grabbedLocation, grabScript.grabbedNormal, rigidBody.velocity.magnitude);
			} else if (grabbedLayer == EnemyLayer) {
			} else if (grabbedLayer == PreyLayer) {
			}

			grabScript.ResetGrabbedObject();
		}

		if (perchScript.isPerching) {
			glideV2Script.pitchLeft = 0;
			glideV2Script.pitchRight = 0;
			glideV2Script.yaw = 0;
			glideV2Script.rollLeft = 0;
			glideV2Script.rollRight = 0;
			glideV2Script.forward = 0;
			glideV2Script.right = 0;
			glideV2Script.flapSpeed = 0;
			glideV2Script.flapDirection = 0;

			walkScript.forward = 0;
			walkScript.right = 0;

			rigidBody.velocity = Vector3.zero;
			rigidBody.constraints = RigidbodyConstraints.FreezePosition;

			grabScript.grab = false;

			if ((perchScript.isPerching && Input.GetAxis ("Flap") != 0)) {
				perchScript.ResetPerch ();
			}
		}

		//as long as we aren't perched, do normal controls
		if(!perchScript.isPerching){
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

			walkScript.forward = Util.GetAxis ("Vertical");
			walkScript.right = Util.GetAxis ("Horizontal");

			float flapSpeed = Util.GetAxis ("Flap");
			if (staminaScript.HasStamina ()) {
				glideV2Script.flapSpeed = Util.GetAxis ("Flap");
			} else {
				glideV2Script.flapSpeed = 0;
			}
			staminaScript.usingStamina = flapSpeed != 0;


			if (glideV2Script.flapSpeed == 0) {
				glideV2Script.wingsOut = Util.GetButtonDown ("Close Wings") ^ glideV2Script.wingsOut;
			} else {
				glideV2Script.wingsOut = true;
			}

			rigidBody.constraints = RigidbodyConstraints.None;

			bool grabHeld = Util.GetButton ("Grab");
			bool grab = Util.GetButtonDown ("Grab");
			grabScript.grab = grab;

			if (grab && interactorScript.itemHolder.HasItem () || grabHeld && !interactorScript.itemHolder.HasItem ()) {
				interactorScript.Pickup ();
			}
		}
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

}
