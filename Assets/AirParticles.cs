using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirParticles : MonoBehaviour {
	public float maxEmission;
	public float minVelocity;
	public float maxVelocity;
	public Rigidbody target;

	private ParticleSystem particles;
	private ParticleSystem.EmissionModule emission;

	// Use this for initialization
	void Start () {
		particles = GetComponent<ParticleSystem> ();
		emission = particles.emission;
	}

	// Update is called once per frame
	void FixedUpdate () {
		float targetVelocity = target.velocity.magnitude;
		float emissionAmount = 0;
		if (targetVelocity > minVelocity) {
			targetVelocity -= minVelocity;
			emissionAmount = maxEmission * Mathf.Clamp (targetVelocity/ maxVelocity, 0f, 1f);
		}

		//make sure particle emitter is enabled
		emission.rateOverTime = emissionAmount;
	}
}
