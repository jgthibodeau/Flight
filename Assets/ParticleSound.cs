using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSound : MonoBehaviour {
	ParticleSystem ps;
	public AudioClip[] clips;
	AudioSource audioSource;
	public int numberParticles = 0;

	public float rate;
	public float minPitch;
	public float maxPitch;
	public float minVolume = 0;
	public float maxVolume = 1;
	public int maxSources = 5;

	private List<AudioSource> sources = new List<AudioSource> ();

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = sources.Count - 1; i >= 0; i--) {
			AudioSource source = sources [i];
			if (! source.isPlaying) {
				sources.Remove (source);
				GameObject.Destroy (source.gameObject);
			}
		}

		int actualParticles = ps.particleCount;
		int particleDiff = actualParticles - numberParticles;
//		if (particleDiff > rate ^ (particleDiff > 0 && !audioSource.isPlaying)) {
//		if (actualParticles > 0 && !audioSource.isPlaying) {
		if (Mathf.Abs(particleDiff) > rate && actualParticles > 0) {
//			audioSource.pitch = Random.Range (minPitch, maxPitch);
//			audioSource.volume = 1f - (ps.main.maxParticles - actualParticles) / ps.main.maxParticles;
//			audioSource.Play ();
			CreateSound (actualParticles);
		}
		numberParticles = actualParticles;
	}

	void CreateSound(float actualParticles) {
		if (sources.Count < maxSources) {
			GameObject newSource = new GameObject ();
			newSource.transform.position = this.transform.position;
			AudioSource source = newSource.AddComponent<AudioSource> ();
			source.pitch = Random.Range (minPitch, maxPitch);
			source.volume = Mathf.Clamp(1f - (ps.main.maxParticles - actualParticles) / ps.main.maxParticles, minVolume, maxVolume);
			source.PlayOneShot (clips[Random.Range (0, clips.Length)]);

			sources.Add (source);
		}
	}
}
