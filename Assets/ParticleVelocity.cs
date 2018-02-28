using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVelocity : MonoBehaviour {
	public GameObject particleSpawned;

	private ParticleSystem ps;

	List<ParticleCollisionEvent> collisionEvents;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		collisionEvents = new List<ParticleCollisionEvent> ();
	}
	
	void OnParticleCollision(GameObject other) {
		int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents (ps, other, collisionEvents);

		int i = 0;
		while (i < numCollisionEvents) {
			if (i % 20 == 0) {
				ParticleCollisionEvent pevent = collisionEvents [i];
				GameObject.Instantiate (particleSpawned, pevent.intersection, Quaternion.identity);
			}
			i++;
		}

	}
}
