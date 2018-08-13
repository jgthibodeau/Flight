using UnityEngine;
using System.Collections;

public class DragonAnimator : MonoBehaviour {
	public float FadeLength;
	public bool Flapping, Grounded, WingsOut, Walking, Hopping, InWater, Flame, Attack, Healing, Boosting, BoostTriggered;
	private bool Attacking;
	public float FlapSpeed;
	public float MoveSpeed;
	public float WalkSpeed;
	public float WalkScale;
	public float HopSpeed;
	public float RunScale;
	private string idle = "CB_Dragon Stand";
	public Animator animator;

	public Transform baseObj, leftWing, rightWing;

	public bool manuallyAdjustWings = true;

	[HideInInspector]
	public float pitchLeft, pitchRight, rollLeft, rollRight, tailPitch, liftLeft, liftRight;

	public float lerpSpeed;
	public float pitchUpScale, pitchDownScale, pitchUpSizeScale, pitchDownSizeScale;
	public float wingsInScale, wingsOutScale, wingsInSizeScale, wingsOutSizeScale, wingsInLengthScale, wingsOutLengthScale;
	public float rollScale, flapRotateScale;

	public Vector3 defaultLeftWingRotation = new Vector3(8.491f, -90f, 172.775f);
	public Vector3 defaultRightWingRotation = new Vector3(-8.491f, 90f, 172.775f);
	public Vector3 defaultLeftWingScale = new Vector3(1, 1, 1);
	public Vector3 defaultRightWingScale = new Vector3(1, 1, 1);

	public Vector3 desiredLeftWingRotation;
	public Vector3 desiredRightWingRotation;
	public Vector3 desiredLeftWingScale;
	public Vector3 desiredRightWingScale;

	private Quaternion previousLeftWingRotation;
	private Quaternion previousRightWingRotation;
	private Vector3 previousLeftWingScale;
	private Vector3 previousRightWingScale;
	private bool wingsReset = true;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		animator.SetBool ("Flapping", FlapSpeed != 0);
		animator.SetBool ("FlapBack", FlapSpeed != 0 && liftLeft > 0 && liftRight > 0);
		animator.SetBool ("Grounded", Grounded);
		animator.SetBool ("WingsOut", WingsOut);
		animator.SetBool ("Walking", MoveSpeed != 0);
		animator.SetBool ("InWater", InWater);
		animator.SetBool ("Flame", Flame);
		animator.SetBool ("Attacking", Attack);
		animator.SetBool ("Healing", Healing);
		animator.SetBool ("Boosting", Boosting);

		if (BoostTriggered) {
			animator.SetTrigger ("BoostTriggered");
			BoostTriggered = false;
		}

		animator.SetFloat ("MoveSpeed", MoveSpeed);
		animator.SetFloat ("WalkSpeed", WalkScale * MoveSpeed);
		animator.SetFloat ("RunSpeed", RunScale * MoveSpeed);
		animator.SetFloat ("FlapSpeed", FlapSpeed);

		if (manuallyAdjustWings) {
			if (WingsOut && !Grounded && !Flapping && !Boosting) {
				UpdateWings ();
			} else {
				ResetWings ();
			}
		}
	}

	void ChooseIdle(){
//		idle = "idle" + Random.Range (1, 3);
	}

	void ResetWings() {
		leftWing.localScale = defaultRightWingScale;
		rightWing.localScale = defaultLeftWingScale;
		wingsReset = true;
	}

	void UpdateWings() {
		desiredLeftWingRotation = defaultLeftWingRotation;
		desiredLeftWingScale = defaultLeftWingScale;
		desiredRightWingRotation = defaultRightWingRotation;
		desiredRightWingScale = defaultRightWingScale;

		if (wingsReset) {
			previousLeftWingRotation = leftWing.localRotation;
			previousRightWingRotation = rightWing.localRotation;
			previousLeftWingScale = leftWing.localScale;
			previousRightWingScale = rightWing.localScale;

			wingsReset = false;
		}

		if (Flapping) {
			UpdateWingsFlap ();
		} else {
			UpdateWingsGlide ();
		}

		float lerpAmount = Time.deltaTime * lerpSpeed;

		leftWing.localRotation = Quaternion.Slerp (previousLeftWingRotation, Quaternion.Euler (desiredLeftWingRotation), lerpAmount);
		leftWing.localScale = Vector3.Lerp (previousLeftWingScale, desiredLeftWingScale, lerpAmount);

		rightWing.localRotation = Quaternion.Slerp (previousRightWingRotation, Quaternion.Euler (desiredRightWingRotation), lerpAmount);
		rightWing.localScale = Vector3.Lerp (previousRightWingScale, desiredRightWingScale, lerpAmount);


		previousLeftWingRotation = leftWing.localRotation;
		previousRightWingRotation = rightWing.localRotation;
		previousLeftWingScale = leftWing.localScale;
		previousRightWingScale = rightWing.localScale;

//		leftWing.localRotation = Quaternion.Euler(desiredLeftWingRotation);
//		leftWing.localScale = desiredLeftWingScale;

//		rightWing.localRotation = Quaternion.Euler(desiredRightWingRotation);
//		rightWing.localScale = desiredRightWingScale;
	}

	void UpdateWingsGlide(){
		UpdateLeftWingGlide ();
		UpdateRightWingGlide ();
	}

	void UpdateWingsFlap(){
		UpdateLeftWingFlap ();
		UpdateRightWingFlap ();
	}

	void UpdateLeftWingGlide() {
		bool wingsIn = liftLeft < 0;
		bool wingsOut = liftLeft > 0;

		float liftRollScale = 1f;
		if (wingsIn) {
			desiredLeftWingRotation.y -= wingsInScale * liftLeft;

			desiredLeftWingScale.z += wingsInSizeScale * liftLeft;
			desiredLeftWingScale.x += wingsInLengthScale * liftLeft;

			liftRollScale += liftLeft;
		} else if (wingsOut) {
			desiredLeftWingRotation.y -= wingsOutScale * liftLeft;

			desiredLeftWingScale.z += wingsOutSizeScale * liftLeft;
			desiredLeftWingScale.x += wingsOutLengthScale * liftLeft;
		}

		desiredLeftWingRotation.z -= rollScale * rollLeft * liftRollScale;

		desiredLeftWingRotation.x -= pitchUpScale * pitchLeft;
	}

	void UpdateLeftWingFlap() {
		desiredLeftWingRotation.x -= flapRotateScale * rollLeft;
	}

	void UpdateRightWingGlide() {
		bool wingsIn = liftRight < 0;
		bool wingsOut = liftRight > 0;

		float liftRollScale = 1f;
		if (wingsIn) {
			desiredRightWingRotation.y += wingsInScale * liftRight;

			desiredRightWingScale.z += wingsInSizeScale * liftRight;
			desiredRightWingScale.x += wingsInLengthScale * liftRight;

			liftRollScale += liftRight;
		} else if (wingsOut) {
			desiredRightWingRotation.y += wingsOutScale * liftRight;

			desiredRightWingScale.z += wingsOutSizeScale * liftRight;
			desiredRightWingScale.x += wingsOutLengthScale * liftRight;
		}

		desiredRightWingRotation.z -= rollScale * rollRight * liftRollScale;

		desiredRightWingRotation.x += pitchUpScale * pitchRight;
	}

	void UpdateRightWingFlap() {
		desiredRightWingRotation.x += flapRotateScale * rollRight;
	}
}
