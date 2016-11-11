using UnityEngine;
using System.Collections;

public class BirdAnimator : MonoBehaviour {
	public float FadeLength;
	public bool Flapping, Grounded, WingsClosed, Walking, Hopping;
	public float FlapSpeed;
	private string idle = "idle1";
	private Animation Animation;

	// Use this for initialization
	void Start () {
		Animation = GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Grounded) {
			if (Walking) {
				if (!Animation.IsPlaying ("walk")) {
					Animation.CrossFade ("walk", FadeLength, PlayMode.StopAll);
				}
			} else if (Hopping) {
				if (!Animation.IsPlaying ("hop")) {
					Animation.CrossFade ("hop", FadeLength, PlayMode.StopAll);
				}
			} else {
				if (!Animation.IsPlaying (idle)) {
					ChooseIdle ();
					Animation.CrossFade (idle);
				}
			}
		} else {
			if (WingsClosed) {
				Animation.CrossFade ("falling", FadeLength, PlayMode.StopAll);
			} else {
				//if just start to flap
//				if (just start to flap) {
//					Animation.CrossFade ("goToFly", FadeLength, PlayMode.StopAll);
//				}
				if (Flapping) {
					Animation ["fly"].speed = FlapSpeed;
					Animation.CrossFade ("fly", FadeLength, PlayMode.StopAll);
				} else {
					Animation.CrossFade ("glide", FadeLength, PlayMode.StopAll);
				}
			}
		}
	}

	void ChooseIdle(){
		idle = "idle" + Random.Range (1, 3);
	}
}
