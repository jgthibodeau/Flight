using UnityEngine;
using System.Collections;

public class BirdAnimator : MonoBehaviour {
	public bool Flapping, Grounded, WingsClosed, Walking, Hopping;
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
					Animation.CrossFade ("walk");
				}
			} else if (Hopping) {
				if (!Animation.IsPlaying ("hop")) {
					Animation.CrossFade ("hop");
				}
			} else {
				if (!Animation.IsPlaying (idle)) {
					ChooseIdle ();
					Animation.CrossFade (idle);
				}
			}
		} else {
			if (WingsClosed) {
				Animation.CrossFade ("falling");
			} else {
				if (Flapping) {
					Animation.CrossFade ("fly");
				} else {
					Animation.CrossFade ("glide");
				}
			}
		}
	}

	void ChooseIdle(){
		idle = "idle" + Random.Range (1, 3);
	}
}
