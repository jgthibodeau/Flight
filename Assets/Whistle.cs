using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Whistle : MonoBehaviour {
	public AudioClip low;
	public AudioClip midLow;
	public AudioClip middle;
	public AudioClip high;

	public AudioSource whistleSource;
	public AudioSource lowSource;
	public AudioSource midlowSource;
	public AudioSource midSource;
	public AudioSource highSource;

	private List<AudioClip> notes = new List<AudioClip> ();

	private List<AudioSource> lowQueue = new List<AudioSource> ();
	private List<AudioSource> midlowQueue = new List<AudioSource> ();
	private List<AudioSource> midQueue = new List<AudioSource> ();
	private List<AudioSource> highQueue = new List<AudioSource> ();


	// Update is called once per frame
	void Update () {
		if (Util.GetButtonDown ("Low Whistle")) {
//			notes.Add (low);
//			lowQueue.Add (lowSource);
			lowSource.Play ();
		}
		if (Util.GetButtonDown ("Middle Low Whistle")) {
//			notes.Add (midLow);
//			midlowQueue.Add (midlowSource);
			midlowSource.Play ();
		}
		if (Util.GetButtonDown ("Middle Whistle")) {
//			notes.Add (middle);
//			midQueue.Add (midSource);
			midSource.Play ();
		}
		if (Util.GetButtonDown ("High Whistle")) {
//			notes.Add (high);
//			highQueue.Add (highSource);
			highSource.Play ();
		}

//		if (!whistleSource.isPlaying && notes.Count > 0) {
//			whistleSource.clip = notes [0];
//			whistleSource.Play ();
//			notes.RemoveAt (0);
//		}

		if (!lowSource.isPlaying && lowQueue.Count > 0) {
			lowSource.Play ();
			lowQueue.RemoveAt (0);
		}
		if (!midlowSource.isPlaying && midlowQueue.Count > 0) {
			midlowSource.Play ();
			midlowQueue.RemoveAt (0);
		}
		if (!midSource.isPlaying && midQueue.Count > 0) {
			midSource.Play ();
			midQueue.RemoveAt (0);
		}
		if (!highSource.isPlaying && highQueue.Count > 0) {
			highSource.Play ();
			highQueue.RemoveAt (0);
		}
	}
}
