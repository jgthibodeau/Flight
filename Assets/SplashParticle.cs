using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashParticle : MonoBehaviour {
	public ParticleSystem ps;
	private ParticleSystem.EmissionModule em;
	private ParticleSystem.VelocityOverLifetimeModule vm;
	public float yOffset;
	public float maxSplashHeight;
	public int maxSplashParticles;
	public float maxSplashVelocity;

	void Start () {
		vm = ps.velocityOverLifetime;
		em = ps.emission;
	}

	void OnTriggerEnter(Collider other){
		Vector3 position = other.transform.position;
		position.y = transform.position.y + yOffset;
		ps.transform.position = position;

		float splashAmount = Mathf.Min (Mathf.Abs (other.attachedRigidbody.velocity.y), maxSplashVelocity) / maxSplashVelocity;
		vm.y = new ParticleSystem.MinMaxCurve (0, maxSplashHeight * splashAmount);

		ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
		em.GetBursts (bursts);
		bursts [0].minCount = (short)(splashAmount * maxSplashParticles);
		bursts [0].maxCount = (short)(splashAmount * maxSplashParticles);
		em.SetBursts (bursts);

		ps.Simulate (0);
		ps.Play ();
	}
}
