using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragonAnimator : MonoBehaviour {
	public float FadeLength;
	public bool Flapping, Grounded, WingsOut, Walking, Hopping, InWater, Flame, Attack, Healing, Boosting, BoostTriggered;
    private bool AnimationFlapping;
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

    public GameObject tailBase;
    public Vector3 defaultRotation;
    public Vector3 maxTailAngle;//?

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

        AnimationFlapping |= Flapping;

		if (manuallyAdjustWings) {
			if (WingsOut && !Grounded  && !IsFlapping() && !Boosting) {
				UpdateWings ();
			} else {
				ResetWings ();
			}
		}

        if (!Grounded)
        {
            UpdateTail ();
        }
	}

    bool IsFlapping()
    {
        return Flapping || animator.GetCurrentAnimatorStateInfo(0).IsName("Flap") || animator.GetCurrentAnimatorStateInfo(0).IsName("Back Flap");
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

        //if (Flapping) {
        //	UpdateWingsFlap ();
        //} else {
        //Debug.Log(AnimationFlapping);
        //if (!AnimationFlapping)
        //{
            UpdateWingsGlide();
        //}
		//}

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

    void UpdateTail()
    {
        //TODO
    }


    public AudioSource audioSource;

	public AudioClip flapAudioClip;
	public float flapVolume = 1f;
	public float flapMinPitch = 0.8f;
	public float flapMaxPitch = 1.5f;

	public AudioClip backFlapAudioClip;
	public float backFlapVolume = 1f;
	public float backFlapMinPitch = 0.6f;
	public float backFlapMaxPitch = 1.2f;

	public AudioClip boostAudioClip;
	public float boostVolume = 1f;
	public float boostMinPitch = 1f;
	public float boostMaxPitch = 1f;

	public List<AudioClip> walkAudioClips;
	public float walkVolume = 1f;
	public float walkMinPitch = 1f;
	public float walkMaxPitch = 1f;

	public List<AudioClip> runAudioClips;
	public float runVolume = 1f;
	public float runMinPitch = 1f;
	public float runMaxPitch = 1f;

	void PlayFlapAudio() {
		audioSource.volume = flapVolume;
		audioSource.pitch = Random.Range (flapMinPitch, flapMaxPitch);
		audioSource.PlayOneShot (flapAudioClip);
	}

	void PlayBackFlapAudio() {
		audioSource.volume = backFlapVolume;
		audioSource.pitch = Random.Range (backFlapMinPitch, backFlapMaxPitch);
		audioSource.PlayOneShot (backFlapAudioClip);
	}

	void PlayBoostAudio() {
		audioSource.volume = boostVolume;
		audioSource.pitch = Random.Range (boostMinPitch, boostMaxPitch);
		audioSource.PlayOneShot (boostAudioClip);
	}

	void PlayWalkAudio() {
		audioSource.volume = walkVolume;
		audioSource.pitch = Random.Range (walkMinPitch, walkMaxPitch);
		audioSource.PlayOneShot (walkAudioClips[Random.Range(0, walkAudioClips.Count - 1)]);
	}

	void PlayRunAudio() {
		audioSource.volume = runVolume;
		audioSource.pitch = Random.Range (runMinPitch, runMaxPitch);
		audioSource.PlayOneShot (runAudioClips[Random.Range(0, runAudioClips.Count - 1)]);
	}
}
