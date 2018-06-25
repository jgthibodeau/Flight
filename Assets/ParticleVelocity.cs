using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVelocity : MonoBehaviour {
	public GameObject particleSpawned;
	public float damage;
	public bool collisionsEnabled = true;
	public bool customInheritVelocity = true;
	public float scale = 2f;

	private ParticleSystem ps;
	private ParticleSystem.MainModule mm;
	private ParticleSystem.CollisionModule cm;
	private ParticleSystem.MinMaxCurve particleSpeed;
	private float originalSpeed;
	private Rigidbody rb;

	List<ParticleCollisionEvent> collisionEvents;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		mm = ps.main;
		cm = ps.collision;
//		particleSpeed = mm.startSpeed;
		originalSpeed = mm.startSpeed.constant;

		rb = GetComponentInParent<Rigidbody> ();

		collisionEvents = new List<ParticleCollisionEvent> ();
	}

	void Update () {
//		ps.main = mm;

//		Debug.Log (ps+" particle speed: " + particleSpeed.constant);
		cm.enabled = collisionsEnabled;
	}

	void LateUpdate() {
		if (customInheritVelocity) {
			float forwardVelocity = Vector3.Dot (rb.velocity, transform.forward) * scale;
//			float forwardVelocity = Vector3.Dot (rb.velocity, rb.transform.forward) * scale;
//			particleSpeed.constant = originalSpeed + forwardVelocity;
//			mm.startSpeed = particleSpeed;
			mm.startSpeed = originalSpeed + forwardVelocity;
		}
	}
	
	void OnParticleCollision(GameObject other) {
		int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents (ps, other, collisionEvents);

		int i = 0;
		while (i < numCollisionEvents) {
//			if (i % 20 == 0) {
				ParticleCollisionEvent pevent = collisionEvents [i];
				GameObject.Instantiate (particleSpawned, pevent.intersection, Quaternion.identity);
//			}
			i++;
		}

		Burnable burnable = other.GetComponentInParent<Burnable> ();
		if (burnable != null) {
			burnable.SetOnFire (damage);
		}
	}
}
