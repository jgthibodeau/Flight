using UnityEngine;
using System.Collections;

public class BirdAnimator : MonoBehaviour {
	public float FadeLength;
	public bool Flapping, Grounded, WingsOut, Walking, Hopping, InWater;
	public float FlapSpeed;
	public float MoveSpeed;
	public float WalkSpeed;
	public float WalkScale;
	public float HopSpeed;
	public float HopScale;
	private string idle = "idle1";
	private Animation animation;

	public Transform leftWing, rightWing, leftTail, rightTail, middleTail;

	[HideInInspector]
	public float pitchLeft, pitchRight, rollLeft, rollRight, tailPitch, liftLeft, liftRight;

	public float lerpSpeed;
	public float pitchUpScale, pitchDownScale, pitchUpSizeScale, pitchDownSizeScale;
	public float wingsInScale, wingsOutScale, wingsInSizeScale, wingsOutSizeScale, wingsInLengthScale, wingsOutLengthScale;
	public float rollScale;
	public float tailPitchUpScale, tailPitchDownScale;

	public Vector3 defaultLeftWingRotation = new Vector3(8.491f, -90f, 172.775f);
	public Vector3 defaultRightWingRotation = new Vector3(-8.491f, 90f, 172.775f);
	public Vector3 defaultLeftWingScale = new Vector3(1, 1, 1);
	public Vector3 defaultRightWingScale = new Vector3(1, 1, 1);

	public Vector3 defaultLeftTailRotation = new Vector3(-16.479f, 133.909f, -17.728f);
	public Vector3 defaultRightTailRotation = new Vector3(-17.317f, 45.475f, -162.081f);
	public Vector3 defaultMiddleTailRotation = new Vector3(0, 0, 175.431f);

	// Use this for initialization
	void Start () {
		animation = transform.GetComponent<Animation> ();
		animation ["walk"].speed = WalkSpeed;
		animation ["hop"].speed = HopSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if (Grounded) {
			if (InWater) {
				if (!animation.IsPlaying ("water")) {
					animation.CrossFade ("water", FadeLength, PlayMode.StopAll);
				}
			} else if (Walking) {
				if (WalkSpeed <= 0f) {
					animation ["walk"].speed = WalkScale * MoveSpeed;
				} else {
					animation ["walk"].speed = WalkSpeed;
				}
				if (!animation.IsPlaying ("walk")) {
					animation.CrossFade ("walk", FadeLength, PlayMode.StopAll);
				}
			} else if (Hopping) {
				if (HopSpeed <= 0f) {
					animation ["hop"].speed = HopScale * MoveSpeed;
				} else {
					animation ["hop"].speed = HopSpeed;
				}
				if (!animation.IsPlaying ("hop")) {
					animation.CrossFade ("hop", FadeLength, PlayMode.StopAll);
				}
			} else {
				//TODO normal idle for random time, then play a unique idle
				animation ["hop"].speed = 1;
				animation ["walk"].speed = 1;
				if (!animation.IsPlaying (idle)) {
					ChooseIdle ();
					animation [idle].wrapMode = WrapMode.Once;
					animation.CrossFade (idle);
				}
			}
		} else {
			if (WingsOut) {
				//if just start to flap
//				if (just start to flap) {
//					Animation.CrossFade ("goToFly", FadeLength, PlayMode.StopAll);
//				}
				if (Flapping) {
					animation ["fly"].speed = FlapSpeed;
					animation.CrossFade ("fly", FadeLength, PlayMode.StopAll);
				} else {
					animation.CrossFade ("glide", FadeLength, PlayMode.StopAll);
					UpdateWings ();
				}

				UpdateTail ();
			} else {
				animation.CrossFade ("falling", FadeLength, PlayMode.StopAll);
			}
		}
	}

	void ChooseIdle(){
		idle = "idle" + Random.Range (1, 3);
	}

	void UpdateWings(){
		UpdateLeftWing ();
		UpdateRightWing ();
	}

	void UpdateLeftWing() {
		Vector3 leftRotation = defaultLeftWingRotation;
		Vector3 leftScale = defaultLeftWingScale;

		bool pointedDown = (pitchLeft > 0);
		bool pointedUp = !pointedDown;

		bool wingsIn = liftLeft < 0;
		bool wingsOut = liftLeft > 0;

		//if pitch > 0, rotate back and scale down both wings
		if (pointedDown) {
			leftRotation.x -= pitchUpScale * pitchLeft;
		}
		//if pitch < 0, rotate forward and scale up both wings
		else if (pointedUp) {
			leftRotation.x -= pitchUpScale * pitchLeft;
		}

		float liftRollScale = 1f;
		if (wingsIn) {
			leftRotation.y -= wingsInScale * liftLeft;

			leftScale.z += wingsInSizeScale * liftLeft;

			leftScale.x += wingsInLengthScale * liftLeft;

			liftRollScale += liftLeft;
		} else if (wingsOut) {
			leftRotation.y -= wingsOutScale * liftLeft;

			leftScale.z += wingsOutSizeScale * liftLeft;

			leftScale.x += wingsOutLengthScale * liftLeft;
		}

		leftRotation.z -= rollScale * rollLeft * liftRollScale;

		//set left/right clavicle rotation and scale based on player input
		float lerpAmount = Time.deltaTime * lerpSpeed;
		leftWing.localRotation = Quaternion.Lerp (leftWing.localRotation, Quaternion.Euler (leftRotation), lerpAmount);
		leftWing.localScale = Vector3.Lerp (leftWing.localScale, leftScale, lerpAmount);
	}

	void UpdateRightWing() {
		Vector3 rightRotation = defaultRightWingRotation;
		Vector3 rightScale = defaultRightWingScale;

		bool pointedDown = (pitchRight > 0);
		bool pointedUp = !pointedDown;

		bool wingsIn = liftRight < 0;
		bool wingsOut = liftRight > 0;

		//if pitch > 0, rotate back and scale down both wings
		if (pointedDown) {
			rightRotation.x += pitchUpScale * pitchRight;
		}
		//if pitch < 0, rotate forward and scale up both wings
		else if (pointedUp) {
			rightRotation.x += pitchUpScale * pitchRight;
		}

		float liftRollScale = 1f;
		if (wingsIn) {
			rightRotation.y += wingsInScale * liftRight;
			rightScale.z += wingsInSizeScale * liftRight;
			rightScale.x += wingsInLengthScale * liftRight;

			liftRollScale += liftRight;
		} else if (wingsOut) {
			rightRotation.y += wingsOutScale * liftRight;
			rightScale.z += wingsOutSizeScale * liftRight;
			rightScale.x += wingsOutLengthScale * liftRight;
		}

		rightRotation.z -= rollScale * rollRight * liftRollScale;

		//set left/right clavicle rotation and scale based on player input
		float lerpAmount = Time.deltaTime * lerpSpeed;
		rightWing.localRotation = Quaternion.Lerp (rightWing.localRotation, Quaternion.Euler (rightRotation), lerpAmount);
		rightWing.localScale = Vector3.Lerp (rightWing.localScale, rightScale, lerpAmount);
	}

	void UpdateTail(){
		Vector3 rightTailRotation = defaultRightTailRotation;
		Vector3 leftTailRotation = defaultLeftTailRotation;
		Vector3 middleTailRotation = defaultMiddleTailRotation;

//		//if tailPitch < 0, rotate down and out tail
//		if (tailPitch < 0) {
//			leftTailRotation.y += tailPitchDownScale * tailPitch;
//			rightTailRotation.y -= tailPitchDownScale * tailPitch;
//
//			leftTailRotation.z += tailPitchDownScale * tailPitch;
//			rightTailRotation.z -= tailPitchDownScale * tailPitch;
//
//			middleTailRotation.z -= tailPitchDownScale * tailPitch;
//		}
//		//if tailPitch > 0, rotate up and in tail
//		if (tailPitch > 0) {
//			leftTailRotation.y += tailPitchUpScale * tailPitch;
//			rightTailRotation.y -= tailPitchUpScale * tailPitch;
//
//			leftTailRotation.z += tailPitchUpScale * tailPitch;
//			rightTailRotation.z -= tailPitchUpScale * tailPitch;
//
//			middleTailRotation.z -= tailPitchDownScale * tailPitch;
//		}

		//if roll > 0, rotate left tail
//		if (roll > 0) {
//			leftTailRotation.z -= tailPitchUpScale * roll;
//			rightTailRotation.z -= tailPitchUpScale * roll;
//		}
//		//if roll < 0, rotate right tail
//		else if (roll < 0) {
			leftTailRotation.z += tailPitchUpScale * rollLeft;
			rightTailRotation.z -= tailPitchUpScale * rollRight;
//		}

//		if (pitch > 0) {
			leftTailRotation.y += tailPitchDownScale * pitchLeft;
			rightTailRotation.y -= tailPitchDownScale * pitchRight;
//		}
//		if pitch < 0, rotate forward and scale up both wings
//		else if (pitch <= 0) {
//			leftTailRotation.y -= tailPitchDownScale * pitch;
//			rightTailRotation.y += tailPitchDownScale * pitch;
//		}

		//set left/right tail rotation based on player input
		float lerpAmount = Time.deltaTime * lerpSpeed;
		rightTail.localRotation = Quaternion.Lerp (rightTail.localRotation, Quaternion.Euler (rightTailRotation), lerpAmount);
		leftTail.localRotation = Quaternion.Lerp (leftTail.localRotation, Quaternion.Euler (leftTailRotation), lerpAmount);
		middleTail.localRotation = Quaternion.Lerp (middleTail.localRotation, Quaternion.Euler (middleTailRotation), lerpAmount);
	}
}
