using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour {
	public float lifeTimeInSeconds;
	public float startTime;

	private ParticleSystem ps;
	public float stopParticlesTimeInSeconds;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (ps != null && ps.isPlaying && Time.time >= startTime + stopParticlesTimeInSeconds) {
			ps.Stop ();
		}

		if (Time.time >= startTime + lifeTimeInSeconds) {
			GameObject.Destroy (this.gameObject);
		}
	}
}
