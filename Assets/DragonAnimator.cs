using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragonAnimator : MonoBehaviour {
    public Player player;

	public float FadeLength;
	public bool Flapping, BackFlap, Grounded, WingsOut, Walking, Hopping, InWater, Flame, Attack, Healing, Boosting, BoostTriggered, Gusting, GustTriggered;
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

	//[HideInInspector]
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
        player = GetComponentInParent<Player>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		animator.SetBool ("Flapping", FlapSpeed != 0);
		animator.SetBool ("BackFlap", FlapSpeed != 0 && BackFlap);
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

        if (GustTriggered)
        {
            animator.SetTrigger("GustTriggered");
            GustTriggered = false;
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
        
        if (tailV1)
        {
            UpdateTail();
        }
        else
        {
            UpdateTailV2();
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

    public float minYToPointDown;
    bool leftWingIn;
    bool rightWingIn;
    void UpdateLeftWingGlide() {
        //float pointDownScale = 1 - Util.ConvertScale(-1f, minYToPointDown, 0, 1, transform.forward.y);

        //desiredLeftWingRotation.y += wingsInScale * pointDownScale;

        //desiredLeftWingScale.z -= wingsInSizeScale * pointDownScale;
        //desiredLeftWingScale.x -= wingsInLengthScale * pointDownScale;



        //desiredLeftWingRotation.z -= rollScale * rollLeft * (1 + liftLeft);

        //if (pitchLeft < 0)
        //{
        //    desiredLeftWingRotation.x += pitchUpScale * pitchLeft;

        //    desiredLeftWingRotation.y -= wingsOutScale * liftLeft;
        //    desiredLeftWingScale.z += wingsOutSizeScale * liftLeft;
        //    desiredLeftWingScale.x += wingsOutLengthScale * liftLeft;
        //}
        //else
        //{
        //    desiredLeftWingRotation.x += pitchDownScale * pitchLeft;
        //}


        if (transform.forward.y < minYToPointDown)
        {
            leftWingIn = true;
        }
        if (pitchLeft < 0 || transform.forward.y >= minYToPointDown)
        {
            leftWingIn = false;
        }

        float liftRollScale = 1f;
        if (leftWingIn)
        {
            float pointDownScale = 1 - Util.ConvertScale(-1f, minYToPointDown, 0, 1, transform.forward.y);
            desiredLeftWingRotation.y += wingsInScale * pointDownScale;

            desiredLeftWingScale.z -= wingsInSizeScale * pointDownScale;
            desiredLeftWingScale.x -= wingsInLengthScale * pointDownScale;

            liftRollScale += liftLeft;
        }
        else {
            desiredLeftWingRotation.y -= wingsOutScale * liftLeft;

            desiredLeftWingScale.z += wingsOutSizeScale * liftLeft;
            desiredLeftWingScale.x += wingsOutLengthScale * liftLeft;
        }

        desiredLeftWingRotation.z -= rollScale * rollLeft * liftRollScale;

        if (pitchLeft < 0)
        {
            desiredLeftWingRotation.x += pitchUpScale * pitchLeft;
        }
        else
        {
            desiredLeftWingRotation.x += pitchDownScale * pitchLeft;
        }
    }

	void UpdateLeftWingFlap() {
		desiredLeftWingRotation.x -= flapRotateScale * rollLeft;
	}

	void UpdateRightWingGlide() {
        if (transform.forward.y < minYToPointDown)
        {
            rightWingIn = true;
        }
        if (pitchRight < 0 || transform.forward.y >= minYToPointDown)
        {
            rightWingIn = false;
        }

        float liftRollScale = 1f;
        if (rightWingIn)
        {
            float pointDownScale = 1 - Util.ConvertScale(-1f, minYToPointDown, 0, 1, transform.forward.y);
            desiredRightWingRotation.y -= wingsInScale * pointDownScale;

            desiredRightWingScale.z -= wingsInSizeScale * pointDownScale;
            desiredRightWingScale.x -= wingsInLengthScale * pointDownScale;

            liftRollScale += liftRight;
        }
        else {
			desiredRightWingRotation.y += wingsOutScale * liftRight;

			desiredRightWingScale.z += wingsOutSizeScale * liftRight;
			desiredRightWingScale.x += wingsOutLengthScale * liftRight;
		}

		desiredRightWingRotation.z -= rollScale * rollRight * liftRollScale;
        
        if (pitchRight < 0)
        {
            desiredRightWingRotation.x -= pitchUpScale * pitchRight;
        }
        else
        {
            desiredRightWingRotation.x -= pitchDownScale * pitchRight;
        }
    }

	void UpdateRightWingFlap() {
		desiredRightWingRotation.x += flapRotateScale * rollRight;
	}

    public Transform tailRoot;
    public bool tailV1;
    public bool exactTailAngle;

    public float tailRotateScaleVertAir;
    public float tailRotateScaleHorizAir;

    public float tailRotateHorizSpeed;
    public float tailRotateVertSpeed;
    private float tailHoriz = 0;
    private float tailVert = 0;

    public float minVelocityForTail;

    void UpdateTail()
    {
        if (Grounded || transform.GetComponentInParent<Rigidbody>().velocity.magnitude < minVelocityForTail)
        {
            return;
        }

        float zAnglePct = (Util.ConvertScale(0, 360, 1, -1, player.transform.eulerAngles.z));
        if (zAnglePct > 0.5f)
        {
            zAnglePct = Util.ConvertScale(0.5f, 1f, 0.5f, 0f, zAnglePct);
        }
        if (zAnglePct < -0.5f)
        {
            zAnglePct = Util.ConvertScale(-1f, -0.5f, 0f, -0.5f, zAnglePct);
        }
        float desiredHoriz = zAnglePct * tailRotateScaleHorizAir;


        float desiredVert = 0;
        //Vector3 velocity = transform.GetComponentInParent<Rigidbody>().velocity;
        //Vector3 upVelocity = Vector3.ProjectOnPlane(velocity, transform.right);
        //desiredVert = Vector3.SignedAngle(transform.forward, upVelocity, transform.right) * tailRotateScaleVertAir;
        Vector3 velocity = transform.GetComponentInParent<Rigidbody>().angularVelocity;
        desiredVert = velocity.z * tailRotateScaleVertAir;

        tailHoriz = Mathf.Lerp(tailHoriz, desiredHoriz, tailRotateHorizSpeed * Time.deltaTime);
        tailVert = Mathf.Lerp(tailVert, desiredVert, tailRotateVertSpeed * Time.deltaTime);
        

        UpdateTailTransform(tailRoot);

        if (exactTailAngle)
        {
            Vector3 rot = tailRoot.localEulerAngles;
            rot.z += 90;
            tailRoot.localEulerAngles = rot;
        }
    }

    void UpdateTailTransform(Transform t)
    {
        if (t == null)
        {
            return;
        }

        if (exactTailAngle)
        {
            Vector3 rot = t.localEulerAngles;
            rot.x = 0;
            rot.y = tailHoriz;
            rot.z = tailVert;
            t.localEulerAngles = rot;
        }
        else
        {
            Vector3 rot = t.localEulerAngles;
            rot.y += tailHoriz;
            rot.z += tailVert;
            t.localEulerAngles = rot;
        }

        foreach (Transform child in t)
        {
            UpdateTailTransform(child);
        }
    }

    void UpdateTailV2()
    {
        if (Grounded || transform.GetComponentInParent<Rigidbody>().velocity.magnitude < minVelocityForTail)
        {
            return;
        }

        Vector3 back = -transform.forward;
        Vector3 desiredBack = Vector3.RotateTowards(back, Vector3.down, tailRotateScaleHorizAir, 0).normalized;
        Vector3 localBack = desiredBack;// transform.InverseTransformDirection(desiredBack);
        Debug.Log(localBack);

        Debug.DrawRay(transform.position, desiredBack * 20, Color.yellow);

        //tailHoriz = Mathf.Lerp(tailHoriz, localBack.x * 360, tailRotateHorizSpeed * Time.deltaTime);
        //tailVert = Mathf.Lerp(tailVert, localBack.y * 360, tailRotateVertSpeed * Time.deltaTime);

        UpdateTailTransformV2(tailRoot, desiredBack);

        //if (exactTailAngle)
        //{
        Vector3 rot = tailRoot.localEulerAngles;
        rot.z += 90;
        tailRoot.localEulerAngles = new Vector3(0, 0, 90);
        //}
    }

    void UpdateTailTransformV2(Transform t, Vector3 desiredBack)
    {
        if (t == null)
        {
            return;
        }

        foreach (Transform child in t)
        {
            //Vector3 rot = transform.forward;
            //rot = Vector3.RotateTowards(rot, -desiredBack, tailRotateScaleVertAir, 0);
            //rot.y -= 90;
            //child.eulerAngles = rot;


            //Quaternion lookRotation = Quaternion.LookRotation(-desiredBack);
            //child.rotation = Quaternion.Slerp(child.rotation, lookRotation, 0.1f);

            //if (exactTailAngle)
            //{
            //????
            //}
            //else
            //{
            //    Vector3 rot = t.eulerAngles;
            //    rot.y += tailHoriz;
            //    rot.x += tailVert;
            //    t.eulerAngles = rot;
            //}

            child.localEulerAngles = new Vector3(0, 0, 0);

            //Quaternion down = Quaternion.LookRotation(Vector3.down);
            //child.rotation = Quaternion.RotateTowards(child.rotation, down, tailRotateScaleHorizAir);

            child.right = Vector3.RotateTowards(child.right, Vector3.up, tailRotateScaleHorizAir, 0);

            Vector3 rot = child.localEulerAngles;
            rot.x = 0;
            child.localEulerAngles = rot;

            UpdateTailTransformV2(child, desiredBack);
        }
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

    public AudioClip gustAudioClip;
    public float groundGustVolume = 1f;
    public float airGustVolume = 1f;
    public float gustMinPitch = 0.6f;
    public float gustMaxPitch = 1.2f;

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

    void PlayGustAudio()
    {
        audioSource.volume = Grounded ? groundGustVolume : airGustVolume;
        audioSource.pitch = Random.Range(gustMinPitch, gustMaxPitch);
        audioSource.PlayOneShot(gustAudioClip);
    }

    void StopGusting()
    {
        Debug.Log("gusting stopped");
        player.isGusting = false;
    }
}
