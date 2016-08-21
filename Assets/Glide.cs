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

	public float acceleration;
	public float speed;
	public float maxSpeed;

	private Rigidbody rigidBody;
	private Animator animator;
	public AnimationClip flapClip;

	public bool flap = false;
	private bool isFlapping = false;

	public float pitchSpeed;
	public float rollSpeed;
	private float pitch;
	private float roll;

	public Transform leftWing;
	public Transform rightWing;

	private AudioSource audioSource;
	private AudioEchoFilter echoFilter;
	public AudioClip airClip;

	private TrailRenderer[] trails;

	// Use this for initialization
	void Start () {
		rigidBody = transform.GetComponent<Rigidbody> ();
		animator = transform.GetComponentInChildren<Animator> ();
		audioSource = transform.GetComponent<AudioSource> ();
		echoFilter = transform.GetComponent<AudioEchoFilter> ();
		trails = transform.GetComponentsInChildren<TrailRenderer> ();
	}
	
	// Update is called once per frame
	void Update() {
		//flap wings
		Debug.Log (Input.GetAxisRaw ("Flap"));
		if (Input.GetAxisRaw ("Flap") != 0) {
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
			trail.startWidth = (1f - (maxSpeed - speed) / maxSpeed) * 0.3f;
			trail.endWidth = 0f;
			trail.time = 1f;
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
		Vector3 rotation = Quaternion.LookRotation(rigidBody.velocity).eulerAngles;
		rotation.z = oldRotation.z;
		transform.rotation = Quaternion.Euler (rotation);

		//apply upward and forward forces if flapping
		if (animator.GetCurrentAnimatorStateInfo(0).IsName (flapClip.name) && !animator.IsInTransition(0)) {
			rigidBody.AddForce (transform.up*flapUpCoef, ForceMode.Impulse);
			rigidBody.AddForce (transform.forward*flapForwardCoef, ForceMode.Impulse);
		}

//		OriginalLift ();
		WingLift ();

		//apply drag
//		drag = (-.5f) * liftCoef * area * speed * speed;
//		rigidBody.AddForce (transform.forward*drag);
	}

	void WingLift(){
		float pitchLeft = -Input.GetAxis ("Vertical");
		float pitchRight = -Input.GetAxis ("Vertical Right");

		//apply gravity
		rigidBody.AddForce (Vector3.down*gravity, ForceMode.Force);
		Debug.DrawRay (transform.position, Vector3.down*gravity, Color.blue);

		speed = rigidBody.velocity.magnitude;//.z;

		//apply lift
		float liftPercent = 1f - Input.GetAxis ("Flap");
		float angleOfAttackLeft = 0.01f + 0.05f * pitchLeft;
		float angleOfAttackRight = 0.01f + 0.05f * pitchRight;

		float liftLeft = (0.5f) * liftCoef * 1.29f * wingSurfaceArea * speed * speed * Mathf.Deg2Rad * angleOfAttackLeft * liftPercent;
		float liftRight = (0.5f) * liftCoef * 1.29f * wingSurfaceArea * speed * speed * Mathf.Deg2Rad * angleOfAttackRight * liftPercent;

		Debug.Log (angleOfAttackLeft + " " + liftLeft + " , " + angleOfAttackRight + " " + liftRight + ", " + liftPercent);

		rigidBody.AddForceAtPosition (transform.up * liftLeft, transform.position - transform.right, ForceMode.Force);
		rigidBody.AddForceAtPosition (transform.up * liftRight, transform.position + transform.right, ForceMode.Force);

		Debug.DrawRay (transform.position - 0.5f*transform.right, transform.up * liftLeft, Color.green);
		Debug.DrawRay (transform.position + 0.5f*transform.right, transform.up * liftRight, Color.magenta);

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
		rotation.z -= roll * rollSpeed;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler (rotation), Time.deltaTime * 8);
	}

	private IEnumerable WaitForAnimation(Animation animation){
		do {
			yield return null;
		} while(animation.isPlaying);
	}
}