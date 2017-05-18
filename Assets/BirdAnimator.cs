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

	[HideInInspector]
	public Transform leftWing, rightWing;

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

					//set left/right clavicle rotation and scale based on player input
				}
			} else {
				Animation.CrossFade ("falling", FadeLength, PlayMode.StopAll);
			}
		}
	}

	void ChooseIdle(){
		idle = "idle" + Random.Range (1, 3);
	}
}
