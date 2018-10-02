using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVelocity : MonoBehaviour {
	public GameObject particleSpawned;
	public float damage;
	public bool collisionsEnabled = true;
	public bool customInheritVelocity = true;
	public float scale = 2f;
    public LayerMask fireLayer;

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
	
	void OnParticleCollisionOld(GameObject other) {
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

    private ParticleCollisionEvent[] CollisionEvents = new ParticleCollisionEvent[8];
    public void OnParticleCollision(GameObject other)
    {
        int collCount = ps.GetSafeCollisionEventSize();

        if (collCount > CollisionEvents.Length)
        {
            CollisionEvents = new ParticleCollisionEvent[collCount];
        }

        int eventCount = ps.GetCollisionEvents(other, CollisionEvents);
        
        //for (int i = 0; i < eventCount; i++)
        //{
            int i = Random.Range(0, eventCount - 1);
            ParticleCollisionEvent pevent = CollisionEvents[i];
            Vector3 position = pevent.intersection;
            if (Util.CanSpawn(position, 0.5f, 10f, fireLayer))
            {
                GameObject.Instantiate(particleSpawned, position, Quaternion.identity);
            }
        //}

        DestroyableTree destroyableTree = other.GetComponentInParent<DestroyableTree>();
        if (destroyableTree != null && !destroyableTree.treeIsActive)
        {
            float distance = Vector3.Distance(other.transform.position, transform.position) + 10f;
            destroyableTree.Replace(distance, transform);
        }

        Burnable burnable = other.GetComponentInParent<Burnable>();
        if (burnable != null)
        {
            burnable.SetOnFire(damage);
        }
        
        burnable = other.GetComponentInChildren<Burnable>();
        if (burnable != null)
        {
            burnable.SetOnFire(damage);
        }
    }
}
