using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCamera : MonoBehaviour {
	public ParticleSystem ps;
	public LayerMask cloudLayer;
	public int cloud = 0;
	public float radius;
	HashSet<GameObject> cloudObjects;

	// Use this for initialization
	void Start () {
	}

//	void Update() {
//		if (Physics.CheckSphere (transform.position, radius, cloudLayer)) {
//			if (ps.isStopped) {
//				ps.Play ();
//			}
//		} else if (ps.isPlaying) {
//			ps.Stop ();
//		}
//	}

	void OnCollisionEnter(Collision collision){
		if (cloudLayer == (cloudLayer | (1 << collision.gameObject.layer))) {
			Debug.Log ("collision enter: " + collision);
			if (cloud == 0) {
				ps.Play ();
			}
			cloud++;
		}
	}

	void OnCollisionExit(Collision collision){
		if (cloudLayer == (cloudLayer | (1 << collision.gameObject.layer))) {
			Debug.Log ("collision exit: " + collision);
			cloud--;
			if (cloud == 0) {
				ps.Stop ();
			}
		}
	}

	void OnTriggerEnter(Collider collision){
		if (cloudLayer == (cloudLayer | (1 << collision.gameObject.layer))) {
			Debug.Log ("trigger enter: " + collision+" "+cloud);
			if (cloud == 0) {
				ps.Play ();
			}
			cloud++;
		}
	}

	void OnTriggerExit(Collider collision){
		if (cloudLayer == (cloudLayer | (1 << collision.gameObject.layer))) {
			Debug.Log ("trigger exit: " + collision+" "+cloud);
			cloud--;
			if (cloud == 0) {
				ps.Stop ();
			}
		}
	}

//	bool inCloud;
//	void Update(){
//		if (!inCloud && ps.isPlaying) {
//			ps.Stop ();
//		} else if (inCloud) {
//			inCloud = false;
//
//			if (ps.isStopped) {
//				ps.Play ();
//			}
//		}
//	}
//
//	void OnTriggerStay(Collider collision){
//		if (cloudLayer == (cloudLayer | (1 << collision.gameObject.layer))) {
//			inCloud = true;
//		}
//	}
}
