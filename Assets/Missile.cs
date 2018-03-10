using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (Kill))]
public class Missile : MonoBehaviour {
	public int damage = 10;

	public bool accelerate = false;
	public int acceleration = 10;
	public int accelerationTime = 10;
	private float startTime;

	public GameObject explosionPrefab;
	public GameObject explosion;
	public bool createExplosionParticles = true;
	public float explosionRadius = 1f;
	public float explosionForceScale = 1f;
	public ForceMode explosionForceMode = ForceMode.Impulse;

	private NetworkTransform networkTransform;
	private Rigidbody rigidBody;

	private bool collided = false;

	void Start() {
		startTime = Time.time;
		rigidBody = GetComponent<Rigidbody> ();
	}

	void Update() {
		if (accelerate) {
			if (Time.time - startTime < accelerationTime) {
				rigidBody.AddForce (transform.forward * acceleration);
			}
		}

		if (!collided) {
			transform.LookAt (transform.position + rigidBody.velocity);
		}
	}

	void OnCollisionEnter(Collision collision) {
		collision.rigidbody.AddForce (collision.impulse, ForceMode.Impulse);
		if (collided) {
			return;
		}
		collided = true;
		GameObject hit = collision.gameObject;
		IHittable hittable = hit.GetComponent<IHittable> ();

		if (hittable != null) {
			Debug.Log ("hitting " + hittable);
			hittable.Hit (damage, this.gameObject);
			GetComponent<Kill> ().Die ();
		}

//		Debug.Log ("calling explosion " + collision + " " + collision.contacts.Length);
//		CmdExplode (collision.contacts[0].point);
//		CmdExplode (transform.position, hit);
	}

//	[Command]
	private void CmdExplode(Vector3 collisionPoint, GameObject directHit) {
//		Debug.Log ("creating explosion");
		GameObject explosionInst = GameObject.Instantiate (explosionPrefab, collisionPoint, transform.rotation);
		Explosion explosionInstExplosion = explosionInst.GetComponent<Explosion> ();
		explosionInstExplosion.damage = damage;
		explosionInstExplosion.ignoreForDamage.Add (directHit);
		explosionInstExplosion.radius = explosionRadius;
		NetworkServer.Spawn (explosionInst);
//		Debug.Log ("spawned explosion");

		Destroy (gameObject);
	}
}
