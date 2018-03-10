using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBreath : MonoBehaviour {
	public bool flameOn;

	public ParticleSystem flameParticles;
	public enum FlameState
	{
		Starting, Playing, Stopping, Stopped
	}
	public FlameState flameState = FlameState.Stopped;
	public AudioSource flameAudio;
	public AudioClip flameStartClip;
	public AudioClip flameContinueClip;
	public AudioClip flameEndClip;

	public float rampUpSpeed = 2;
	public float rampDownSpeed = 2f;

	private ParticleSystem.EmissionModule em;
	private ParticleSystem.MinMaxCurve rateOverTime;
	public float originalRate;
	public float rateMultiplier;

	void Start() {
		em = flameParticles.emission;
		rateOverTime = em.rateOverTime;
		originalRate = rateOverTime.constant;

		rateMultiplier = 0;

		flameAudio.loop = true;
		flameAudio.clip = flameContinueClip;
		flameAudio.Play ();

		flameAudio.volume = rateMultiplier;
	}

	void Update() {
		if (flameOn) {
			StartFlame ();
		} else {
			StopFlame ();
		}
		rateOverTime.constant = originalRate * rateMultiplier;
		em.rateOverTime = rateOverTime;
		flameAudio.volume = rateMultiplier;
	}

	public void StartFlame () {
		if (rateMultiplier == 0) {
			flameParticles.Play ();
			flameAudio.time = 0;
		}
		rateMultiplier = Mathf.Clamp01 (rateMultiplier + Time.deltaTime * rampUpSpeed);
	}
	
	public void StopFlame () {
		rateMultiplier = Mathf.Clamp01 (rateMultiplier - Time.deltaTime * rampDownSpeed);

		if (rateMultiplier == 0) {
			if (flameParticles.isPlaying) {
				flameParticles.Stop ();
			}
		}
	}
}
