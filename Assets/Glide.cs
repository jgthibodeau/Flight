using UnityEngine;
using System.Collections;

public class Glide : MonoBehaviour {
	public float gravity;

	public float liftCoef;
	public float lift;

	public float dragCoef;
	public float drag;

	public float flapUpCoef;
	public float flapForwardCoef;

	public float wingSurfaceArea;
	public float crossSectionalArea;

	public float speed;
	public float maxSpeed;

	private Rigidbody rigidBody;
	private Animator animator;
	public AnimationClip flapClip;

	public bool flap = false;
	private bool isFlapping = false;
	public float wingOutDistance = 0.5f;
	public float wingForwardDistance = 0.5f;
	public float angleOffset = 0.015f;
	public float angleScale = 0.05f;

	private AudioSource audioSource;
	private AudioEchoFilter echoFilter;
	public AudioClip airClip;

	private Collider characterCollider;

	private TrailRenderer[] trails;

	public LayerMask layerMaskForGround;

	// Use this for initialization
	void Start () {
		rigidBody = transform.GetComponent<Rigidbody> ();
		animator = transform.GetComponentInChildren<Animator> ();
		audioSource = transform.GetComponent<AudioSource> ();
		echoFilter = transform.GetComponent<AudioEchoFilter> ();
		trails = transform.GetComponentsInChildren<TrailRenderer> ();
		characterCollider = transform.GetComponent<Collider> ();
	}
	
	// Update is called once per frame
	void Update() {
		//flap wings
		if (Input.GetButton ("Jump") || Input.GetAxisRaw ("Flap") != 0) {
			if (!isFlapping) {
				flap = true;
				isFlapping = true;
				animator.Play (flapClip.name);
//				WaitForAnimation (animator);
			}
		} else {
			isFlapping = false;
		}

		//set random echo delay between 300 and 1500
//		echoFilter.delay = Random.Range (1000, 1500);

		audioSource.pitch = (1f - (maxSpeed - speed) / maxSpeed) * (1.5f);
		audioSource.volume = (1f - (maxSpeed - speed) / maxSpeed) * (0.7f);

		foreach(TrailRenderer trail in trails){
			trail.endWidth = (1f - (maxSpeed - speed) / maxSpeed) * 0.3f;
			trail.startWidth = 0f;
			trail.time = 0.5f;
		}

//		transform.rotation *= Quaternion.Euler (new Vector3 (pitch, 0, -roll));

		//d = r * t
		//t = d / r
//		Quaternion targetRotation = Quaternion.LookRotation (rigidBody.velocity);
//		float rotatePercent = Quaternion.Angle (transform.rotation, targetRotation);
//		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime*rotatePercent/10);
	}

	void FixedUpdate () {
		Vector3 oldRotation = transform.rotation.eulerAngles;
		Vector3 rotation = Quaternion.LookRotation(rigidBody.velocity, transform.up).eulerAngles;

//		rotation.z = oldRotation.z;

		transform.rotation = Quaternion.Euler (rotation);

		//apply upward and forward forces if flapping
		if (animator.GetCurrentAnimatorStateInfo(0).IsName (flapClip.name) && !animator.IsInTransition(0)) {
			rigidBody.AddForceAtPosition (transform.up*flapUpCoef, transform.position + wingForwardDistance*transform.forward, ForceMode.Impulse);
			rigidBody.AddForceAtPosition (transform.forward*flapForwardCoef, transform.position + wingForwardDistance*transform.forward, ForceMode.Impulse);
		}

//		OriginalLift ();
		if(!isFlapping)
			WingLift ();

		//apply gravity
		if (!isGrounded ()) {
			rigidBody.AddForce (Vector3.down * gravity, ForceMode.Force);
			Debug.DrawRay (transform.position+rigidBody.centerOfMass, Vector3.down * gravity, Color.blue);
		}

		//apply drag
//		drag = (-.5f) * liftCoef * area * speed * speed;
//		rigidBody.AddForce (transform.forward*drag);
	}

	float AngleSigned(Vector3 v1, Vector3 v2, Vector3 normal){
		return Mathf.Atan2 (
			Vector3.Dot (normal, Vector3.Cross (v1, v2)),
			Vector3.Dot (v1, v2)) * Mathf.Rad2Deg;
	}

	void WingLift(){
		float pitchLeft = -Input.GetAxis ("Vertical");
		float pitchRight = -Input.GetAxis ("Vertical Right");

		speed = rigidBody.velocity.magnitude;//.z;

		//apply lift
		float liftPercent = 1f - Input.GetAxis ("Flap");
		float angleBetweenForwardAndSpeed = 0;
		float angleOfAttackLeft = angleOffset + angleScale * pitchLeft + angleBetweenForwardAndSpeed;
		float angleOfAttackRight = angleOffset + angleScale * pitchRight + angleBetweenForwardAndSpeed;

		float liftLeft = (0.5f) * liftCoef * 1.29f * wingSurfaceArea * speed * speed * Mathf.Deg2Rad * angleOfAttackLeft * liftPercent;
		float liftRight = (0.5f) * liftCoef * 1.29f * wingSurfaceArea * speed * speed * Mathf.Deg2Rad * angleOfAttackRight * liftPercent;

		Debug.Log (angleOfAttackLeft + " " + liftLeft + " , " + angleOfAttackRight + " " + liftRight + ", " + liftPercent);

		rigidBody.AddForceAtPosition (transform.up * liftLeft, transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, ForceMode.Force);

		Debug.DrawRay (transform.position - wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + wingOutDistance*transform.right + wingForwardDistance*transform.forward, transform.up * liftRight, Color.magenta);

		//clamp to maxspeed
		float brakePercent = 1f - Input.GetAxis ("Brake");
		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		Debug.DrawRay (transform.position, rigidBody.velocity, Color.cyan);
	}

	void OriginalLift (){
		float roll = Input.GetAxis ("Horizontal");
		float pitch = Input.GetAxis ("Vertical");

		//apply gravity
		rigidBody.AddForce (Vector3.down*gravity, ForceMode.Force);

		speed = rigidBody.velocity.magnitude;//.z;

		//apply lift
		if (!Input.GetButton ("Jump")) {
			float angleOfAttack = 0.05f + -0.5f * pitch;
			lift = (0.5f) * liftCoef * 1.29f * wingSurfaceArea * speed * speed * Mathf.Deg2Rad * angleOfAttack;
			rigidBody.AddForce (transform.up * lift, ForceMode.Force);
		}

		rigidBody.velocity = Vector3.ClampMagnitude (rigidBody.velocity, maxSpeed);

		var lookPos = rigidBody.velocity;
		//		lookPos.x = 0;
		var rotation = Quaternion.LookRotation(lookPos).eulerAngles;
		Debug.Log (rotation.x);
		if (rotation.x < 270 && rotation.x > 230)
			rotation.x = 275;
		if (rotation.x > 80 && rotation.x < 120)
			rotation.x = 80;
		rotation.z -= roll;// * rollSpeed;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler (rotation), Time.deltaTime * 8);
	}

	private bool isGrounded(){
		Debug.DrawLine (characterCollider.bounds.center, new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z), Color.red);
		return Physics.CheckCapsule (
			characterCollider.bounds.center,
			new Vector3(characterCollider.bounds.center.x, characterCollider.bounds.min.y-0.1f, characterCollider.bounds.center.z),
			0.18f,
			layerMaskForGround.value
		);
	}

	private IEnumerable WaitForAnimation(Animation animation){
		do {
			yield return null;
		} while(animation.isPlaying);
	}
}