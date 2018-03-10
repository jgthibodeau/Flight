using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IHittable {
	public float maxHealth = 100;
	public float currentHealth;

//	private Respawnable respawnable;

	void Start() {
		Reset ();
//		respawnable = GetComponent<Respawnable> ();
	}

	public void Hit(float damage, GameObject hitter) {
		RpcTakeDamage (damage);
	}

//	[ClientRpc]
	public void RpcTakeDamage(float damage) {
//		Debug.Log ("Already dead? " + IsDead ());
		if (IsDead ()) {
			return;
		}

		Debug.Log ("Taking damage " + damage + " " + currentHealth + " " + gameObject);
		currentHealth -= damage;
		Debug.Log ("Took damage " + damage + " " + currentHealth + " " + gameObject);

//		Debug.Log ("Now dead? " + IsDead ());
		if (IsDead ()) {
			Kill ();
		}
	}

//	[ClientRpc]
//	void RpcRespawn() {
//		if (isLocalPlayer) {
//			Vector3 spawnPoint = Vector3.zero;
//			if (originalSpawn != null) {
//				spawnPoint = originalSpawn;
//			} else if (spawnPoints != null && spawnPoints.Length > 0) {
//				spawnPoint = spawnPoints [Random.Range (0, spawnPoints.Length)].transform.position;
//			}
//			transform.position = spawnPoint;
//		}
//	}

	public bool IsDead() {
		return currentHealth <= 0;
	}

	public void Kill() {
		Debug.Log ("Killed " + gameObject);
//		if (respawnable != null) {
//			Debug.Log ("Respawning " + gameObject);
//			respawnable.Respawn ();
//		} else {
			GameObject.Destroy (gameObject);
//		}
	}

	public void Reset() {
		currentHealth = maxHealth;
	}
}
