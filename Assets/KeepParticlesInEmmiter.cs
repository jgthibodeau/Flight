using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class KeepParticlesInEmmiter : MonoBehaviour {
	public float distanceToDestroy;
	private ParticleSystem ps;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = transform.position;
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles (particles);
		List<ParticleSystem.Particle> keepParticles = new List<ParticleSystem.Particle> ();
		foreach (ParticleSystem.Particle particle in particles) {
			if (Vector3.Distance (particle.position, position) < distanceToDestroy) {
				keepParticles.Add (particle);
			}
		}
		ps.SetParticles (keepParticles.ToArray (), keepParticles.Count);
	}
}
