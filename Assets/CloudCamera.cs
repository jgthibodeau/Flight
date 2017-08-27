using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCamera : MonoBehaviour {
	public ParticleSystem ps;
	public LayerMask cloudLayer;
	public int cloud = 0;

	// Use this for initialization
	void Start () {
	}
	
	void OnCollisionEnter(Collision collision){
		Debug.Log ("collision enter: " + collision);
		if (collision.gameObject.layer == cloudLayer) {
			ps.Play ();
		}
	}

	void OnCollisionExit(Collision collision){
		Debug.Log ("collision exit: " + collision);
		if (collision.gameObject.layer == cloudLayer) {
			ps.Stop ();
			ps.Clear ();
		}
	}

	void OnTriggerEnter(Collider collision){
		if (cloudLayer == (cloudLayer | (1 << collision.gameObject.layer))) {
			Debug.Log ("trigger enter: " + collision);
			if (cloud == 0) {
				ps.Play ();
			}
			cloud++;
//			ps.gameObject.SetActive (true);
		}
	}

	void OnTriggerExit(Collider collision){
		if (cloudLayer == (cloudLayer | (1 << collision.gameObject.layer))) {
			Debug.Log ("trigger exit: " + collision);
			cloud--;
			if (cloud == 0) {
				ps.Stop ();
//				ps.Clear ();
//				ps.gameObject.SetActive (false);
			}
		}
	}
}
