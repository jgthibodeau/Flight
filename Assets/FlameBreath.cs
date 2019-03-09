using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBreath : MonoBehaviour {
	public bool infiniteBreath = false;

	public bool flameOn;

	public ParticleSystem flameParticles;
    //	public ParticleSystem flameDepletedParticles;
    public ParticleVelocity flameParticleVelocity;
    public Transform flameStart;

    public float flameParticleMaxEmission = 100;
	public float flameParticleMaxLifetime = 2;
	public float flameParticleMaxSpeed = 20;
	public float flameParticleMaxScale = 1;

    public bool customInheritVelocity;

//	public float flameParticleMinEmission = 50;
//	public float flameParticleMinLifetime = 1;
//	public float flameParticleMinSpeed = 10;
//	public float flameParticleMinScale = 20;
	public float flameParticleMinPercent = 0.25f;

	public float breathPercentStartsDiminishing = 0.25f;

	public enum FlameState
	{
		Starting, Playing, Stopping, Stopped
	}
	public FlameState flameState = FlameState.Stopped;
	public AudioSource flameAudio;
	public AudioClip flameStartClip;
	public AudioClip flameContinueClip;
	public AudioClip flameEndClip;

	public float flameVolumeScale = 0.1f;
	public float maxFlameVolume = 0.5f;

	public float rampUpSpeed = 2;
	public float rampDownSpeed = 2f;

	private ParticleSystem.MainModule mm;
    private ParticleSystem.EmissionModule em;
    private ParticleSystem.ShapeModule sm;
    private ParticleSystem.MinMaxCurve rateOverTime;
	private ParticleSystem.MinMaxCurve sizeOverTime;
	public float originalRate;
	public float rateMultiplier;

	public float maxedOutRateMultiplier;
	public float maxBreath;
	public float currentBreath;

	public float breathUseRate;
	public float breathUseDelay;
	public float currentUseDelay;

	public float breathRegainRate;
    public float breathRegainDelay;
    public float breathMaxedOutRegainDelay;
    public float currentRegainDelay;

    private Rigidbody rb;

	void Start() {
        rb = GetComponent<Rigidbody>();

        mm = flameParticles.main;
        sm = flameParticles.shape;
		em = flameParticles.emission;
		rateOverTime = em.rateOverTime;

		rateMultiplier = 0;

		flameAudio.loop = true;
		flameAudio.clip = flameContinueClip;
		flameAudio.Play ();

		flameAudio.volume = 0;
	}

	void Update() {
		if (flameOn) {
			StartFlame ();
			flameState = FlameState.Starting;
		} else {
			StopFlame ();
			flameState = FlameState.Stopping;
		}

		float diminishedPercent = DiminishedPercent ();
		rateOverTime.constant = flameParticleMaxEmission * diminishedPercent * rateMultiplier;
		em.rateOverTime = rateOverTime;

		float volume = rateMultiplier * flameVolumeScale;
		flameAudio.volume = Mathf.Clamp(volume, 0, maxFlameVolume);

		mm.startLifetime = flameParticleMaxLifetime * diminishedPercent * rateMultiplier;
		mm.startSize = rateMultiplier * diminishedPercent;

        float startSpeed = flameParticleMaxSpeed *diminishedPercent * rateMultiplier;
        if (customInheritVelocity)
        {
            float forwardVelocity = Vector3.Dot(rb.velocity, flameParticles.transform.forward);
            //			float forwardVelocity = Vector3.Dot (rb.velocity, rb.transform.forward) * scale;
            //			particleSpeed.constant = originalSpeed + forwardVelocity;
            //			mm.startSpeed = particleSpeed;
            startSpeed += forwardVelocity;
        }
        mm.startSpeed = startSpeed;

        flameParticleVelocity.collisionsEnabled = currentBreath > 0;
    }

    void LateUpdate()
    {
        //Vector3 offset = transform.InverseTransformPoint(flameStart.position);
        //sm.position = offset;
        ////sm.rotation = flameStart.localEulerAngles;
        //Vector3 forward = transform.InverseTransformDirection(flameStart.forward);
        //sm.rotation = Quaternion.LookRotation(forward, transform.up).eulerAngles;
    }

    public void StartFlame () {
		if (currentUseDelay > 0) {
			currentUseDelay -= Time.deltaTime;
		} else {
			currentBreath -= breathUseRate * Time.deltaTime;
			currentBreath = Mathf.Clamp (currentBreath, 0, maxBreath);
		}

        currentRegainDelay = currentBreath > 0 ? breathRegainDelay : breathMaxedOutRegainDelay;

		if (rateMultiplier == 0) {
			flameParticles.Play ();
			flameAudio.time = 0;
		}

		rateMultiplier = Mathf.Clamp01 (rateMultiplier + Time.deltaTime * rampUpSpeed);
	}
	
	public void StopFlame () {
		if (currentRegainDelay > 0) {
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
		}
	}

	public float DiminishedPercent() {
		float percentage = Percentage ();
		if (percentage > breathPercentStartsDiminishing || infiniteBreath) {
			return 1f;
		}

		float diminishedPercent = percentage / breathPercentStartsDiminishing;

		return diminishedPercent * (1 - flameParticleMinPercent) + flameParticleMinPercent;
	}

	public float Percentage() {
		return currentBreath / maxBreath;
	}
}
