using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashParticle : MonoBehaviour {
	public GameObject particleSystemObj;
//	public ParticleSystem ps;
//	private ParticleSystem.EmissionModule em;
//	private ParticleSystem.VelocityOverLifetimeModule vm;
//	public AudioSource audioSource;
	public float yOffset;
	public float maxSplashHeight;
	public int maxSplashParticles;
	public float maxSplashVelocity;
	public float minSplashPitch = 0.9f;
	public float maxSplashPitch = 1.1f;
	public float minSplashVolume = 0.25f;
	public float maxSplashVolume = 1f;

	void Start () {
//		vm = ps.velocityOverLifetime;
//		em = ps.emission;
//		audioSource = GetComponent<AudioSource> ();
	}

	void OnTriggerEnter(Collider other){
		GameObject psInstance = GameObject.Instantiate (particleSystemObj);
		AudioSource audioSource = psInstance.GetComponent<AudioSource> ();
		ParticleSystem ps = psInstance.GetComponent<ParticleSystem> ();
		ParticleSystem.VelocityOverLifetimeModule vm = ps.velocityOverLifetime;
		ParticleSystem.EmissionModule em = ps.emission;

		Vector3 position = other.transform.position;
		position.y = transform.position.y + yOffset;
		ps.transform.position = position;

		float velocity = other.attachedRigidbody.velocity.y;

		float splashAmount = Mathf.Min (Mathf.Abs (velocity), maxSplashVelocity) / maxSplashVelocity;
		vm.y = new ParticleSystem.MinMaxCurve (0, maxSplashHeight * splashAmount);

		ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
		em.GetBursts (bursts);
		bursts [0].minCount = (short)(splashAmount * maxSplashParticles);
		bursts [0].maxCount = (short)(splashAmount * maxSplashParticles);
		em.SetBursts (bursts);

		ps.Stop ();
		ps.Simulate (0);
		ps.Play ();

		PlaySound (audioSource, splashAmount);

		Destroy (psInstance, 1f);
	}

	void PlaySound(AudioSource audioSource, float splashAmount) {
		audioSource.pitch = Random.Range (minSplashPitch, maxSplashPitch);
		audioSource.volume = minSplashVolume + (maxSplashVolume - minSplashVolume) * splashAmount;
		if (!audioSource.isPlaying) {
			audioSource.Play ();
		}
	}
}
