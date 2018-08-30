using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IHittable {
    public bool invincible = false;
    public float maxHealth = 100;
	public float currentHealth;

    public GameObject deathParticles;
    public GameObject spawnOnDeath;
    public Vector3 minSpawnForce = new Vector3 (-1, 1, -1);
    public Vector3 maxSpawnForce = new Vector3(1, 2, 1);

    void Start() {
//		Reset ();
//		respawnable = GetComponent<Respawnable> ();
	}

	public void Hit(float damage, GameObject hitter) {
        TakeDamage(damage);
	}

	private float previouslyHealedHealth;
	public bool Heal(float amount) {
		bool healing = (currentHealth < maxHealth) && (previouslyHealedHealth <= currentHealth);

		if (healing) {
			currentHealth = Mathf.Clamp (currentHealth + amount, 0, maxHealth);
		}

		previouslyHealedHealth = currentHealth;

		return healing;
	}
    
	public void TakeDamage(float damage) {
        if (invincible)
        {
            return;
        }

		if (IsDead ()) {
			return;
		}

		Debug.Log ("Taking damage " + damage + " " + currentHealth + " " + gameObject);
		currentHealth -= damage;
		Debug.Log ("Took damage " + damage + " " + currentHealth + " " + gameObject);
        
		if (IsDead ()) {
			Kill ();
		}
	}

	public bool IsDead() {
		return currentHealth <= 0;
	}

	public void Kill() {
		Debug.Log ("Killed " + gameObject);

        SpawnDeathObject();

        GameObject.Destroy (gameObject);
	}

    public void SpawnDeathObject()
    {
        GameObject inst = GameObject.Instantiate(spawnOnDeath, transform.position + Vector3.up * 0.1f, transform.rotation);
        Rigidbody[] rbs = inst.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            Vector3 force = new Vector3(
                Random.Range(minSpawnForce.x, maxSpawnForce.x),
                Random.Range(minSpawnForce.y, maxSpawnForce.y),
                Random.Range(minSpawnForce.z, maxSpawnForce.z)
                );
            rb.AddForce(force);
        }
    }

	public void Reset() {
		currentHealth = maxHealth;
	}

	public float Percentage() {
		return currentHealth / maxHealth;
	}
}
