using UnityEngine;
using System.Collections;

public class BirdAnimator : MonoBehaviour {
	public float FadeLength;
	public bool Flapping, Grounded, WingsOut, Walking, Hopping;
	public float FlapSpeed;
	public float MoveSpeed;
	public float WalkSpeed;
	public float WalkScale;
	public float HopSpeed;
	public float HopScale;
	private string idle = "idle1";
	private Animation Animation;

	public Transform leftWing, rightWing, leftTail, rightTail, middleTail;

	[HideInInspector]
	public float pitch, roll, tailPitch;

	public float lerpSpeed;
	public float pitchUpScale, pitchDownScale, pitchUpSizeScale, pitchDownSizeScale;
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
		Animation = GetComponent<Animation> ();
		Animation ["walk"].speed = WalkSpeed;
		Animation ["hop"].speed = HopSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if (Grounded) {
			if (Walking) {
				if (WalkSpeed <= 0f) {
					Animation ["walk"].speed = WalkScale * MoveSpeed;
				} else {
					Animation ["walk"].speed = WalkSpeed;
				}
				if (!Animation.IsPlaying ("walk")) {
					Animation.CrossFade ("walk", FadeLength, PlayMode.StopAll);
				}
			} else if (Hopping) {
				if (HopSpeed <= 0f) {
					Animation ["hop"].speed = HopScale * MoveSpeed;
				} else {
					Animation ["hop"].speed = HopSpeed;
				}
				if (!Animation.IsPlaying ("hop")) {
					Animation.CrossFade ("hop", FadeLength, PlayMode.StopAll);
				}
			} else {
				//TODO normal idle for random time, then play a unique idle
				Animation ["hop"].speed = 1;
				Animation ["walk"].speed = 1;
				if (!Animation.IsPlaying (idle)) {
					ChooseIdle ();
					Animation [idle].wrapMode = WrapMode.Once;
					Animation.CrossFade (idle);
				}
			}
		} else {
			if (WingsOut) {
				//if just start to flap
//				if (just start to flap) {
//					Animation.CrossFade ("goToFly", FadeLength, PlayMode.StopAll);
//				}
				if (Flapping) {
					Animation ["fly"].speed = FlapSpeed;
					Animation.CrossFade ("fly", FadeLength, PlayMode.StopAll);
				} else {
					Animation.CrossFade ("glide", FadeLength, PlayMode.StopAll);
					UpdateWings ();
				}

				UpdateTail ();
			} else {
				Animation.CrossFade ("falling", FadeLength, PlayMode.StopAll);
			}
		}
	}

	void ChooseIdle(){
		idle = "idle" + Random.Range (1, 3);
	}

	void UpdateWings(){
		Vector3 leftRotation = defaultLeftWingRotation;
		Vector3 rightRotation = defaultRightWingRotation;
		Vector3 leftScale = defaultLeftWingScale;
		Vector3 rightScale = defaultRightWingScale;

		//if pitch > 0, rotate back and scale down both wings
		if (pitch > 0) {
			leftRotation.y += pitchDownScale * pitch;
			rightRotation.y -= pitchDownScale * pitch;

			leftScale.z -= pitchDownSizeScale * pitch;
			rightScale.z -= pitchDownSizeScale * pitch;

			//if roll > 0, rotate up left wing
			if (roll > 0) {
				leftRotation.z -= rollScale * roll * (1f - pitch);
			}
			//if roll < 0, rotate up right wing
			else if (roll < 0) {
				rightRotation.z += rollScale * roll * (1f - pitch);
			}
		}
		//if pitch < 0, rotate forward and scale up both wings
		else if (pitch <= 0) {
			leftRotation.x -= pitchUpScale * pitch;
			rightRotation.x += pitchUpScale * pitch;

			leftScale.z -= pitchUpSizeScale * pitch;
			rightScale.z -= pitchUpSizeScale * pitch;

			//if roll > 0, rotate up left wing
			if (roll > 0) {
				leftRotation.z -= rollScale * roll;
			}
			//if roll < 0, rotate up right wing
			else if (roll < 0) {
				rightRotation.z += rollScale * roll;
			}
		}

		//set left/right clavicle rotation and scale based on player input
		float lerpAmount = Time.deltaTime * lerpSpeed;
		leftWing.localRotation = Quaternion.Lerp (leftWing.localRotation, Quaternion.Euler (leftRotation), lerpAmount);
		leftWing.localScale = Vector3.Lerp (leftWing.localScale, leftScale, lerpAmount);;
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
			leftTailRotation.z += tailPitchUpScale * roll;
			rightTailRotation.z += tailPitchUpScale * roll;
//		}

//		if (pitch > 0) {
			leftTailRotation.y += tailPitchDownScale * pitch;
			rightTailRotation.y -= tailPitchDownScale * pitch;
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
