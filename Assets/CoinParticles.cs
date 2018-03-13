using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinParticles : MonoBehaviour {
	ParticleSystem ps;

	void Start() {
		ps = GetComponent<ParticleSystem> ();
	}

	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag == "Gold") {
			ps.Play ();
		}
	}

	void OnTriggerExit(Collider c) {
		if (c.gameObject.tag == "Gold") {
			ps.Stop ();
		}
	}
}
