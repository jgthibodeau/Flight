using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedParticles : MonoBehaviour {
	Rigidbody parentRigidBody;
	ParticleSystem.EmissionModule emissionModule;
	ParticleSystem particles;
	public float emissionScale;
	public float emissionPower;
	public float maxEmissionRate;
	public float emissionRate;

	// Use this for initialization
	void Start () {
		particles = GetComponent<ParticleSystem> ();
		emissionModule = particles.emission;
		parentRigidBody = this.transform.parent.GetComponentInChildren<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		float velocity = parentRigidBody.velocity.magnitude;
		emissionRate = Mathf.Clamp (Mathf.Pow (velocity * emissionScale, emissionPower), 0, maxEmissionRate);
		emissionModule.rateOverTime = emissionRate;
	}
}
