using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBreath : MonoBehaviour {
	public bool infiniteBreath = false;

	public bool flameOn;

	public ParticleSystem flameParticles;
	public ParticleSystem flameDepletedParticles;
	public ParticleVelocity flameParticleVelocity;
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

	public float maxedOutRateMultiplier;
	public  float maxBreath;
	public  float currentBreath;

	public  float breathUseRate;
	public  float breathUseDelay;
	public  float currentUseDelay;

	public  float breathRegainRate;
	public  float breathRegainDelay;
	public  float currentRegainDelay;

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
		if (currentUseDelay > 0) {
			currentUseDelay -= Time.deltaTime;
		} else {
			currentBreath -= breathUseRate * Time.deltaTime;
			currentBreath = Mathf.Clamp (currentBreath, 0, maxBreath);
		}
		currentRegainDelay = breathRegainDelay;

		if (rateMultiplier == 0) {
			flameParticles.Play ();
			flameAudio.time = 0;
		}

		if (currentBreath > 0 || infiniteBreath) {
			rateMultiplier = Mathf.Clamp01 (rateMultiplier + Time.deltaTime * rampUpSpeed);
			flameParticleVelocity.collisionsEnabled = true;
		} else {
//			rateMultiplier = Mathf.Clamp (rateMultiplier, 0, maxedOutRateMultiplier);
//			rateMultiplier = maxedOutRateMultiplier;
//			flameParticleVelocity.collisionsEnabled = false;
			if (flameParticles.isPlaying) {
				flameParticles.Stop ();
//				ParticleSystem.MainModule mm = flameDepletedParticles.main;
//				mm.prewarm = true;
				flameDepletedParticles.Play ();
			} else if (!flameDepletedParticles.isPlaying){
				flameDepletedParticles.Play ();
			}
		}
	}
	
	public void StopFlame () {
		if (currentRegainDelay > 0 && currentBreath <= 0) {
			currentRegainDelay -= Time.deltaTime;
		} else {
			currentBreath += breathRegainRate * Time.deltaTime;
			currentBreath = Mathf.Clamp (currentBreath, 0, maxBreath);
		}
		currentUseDelay = breathUseDelay;

		rateMultiplier = Mathf.Clamp01 (rateMultiplier - Time.deltaTime * rampDownSpeed);

		if (rateMultiplier == 0) {
			if (flameParticles.isPlaying) {
				flameParticles.Stop ();
			}
			if (flameDepletedParticles.isPlaying) {
//				ParticleSystem.MainModule mm = flameDepletedParticles.main;
//				mm.prewarm = false;
				flameDepletedParticles.Stop ();
			}
		}
	}

	public float Percentage() {
		return currentBreath / maxBreath;
	}
}
