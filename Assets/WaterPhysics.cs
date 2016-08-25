using UnityEngine;
using System.Collections;

public class WaterPhysics : MonoBehaviour {
	public float bouyancy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionStay(Collision collisionInfo) {
		if (collisionInfo.gameObject.CompareTag ("Player")) {
			collisionInfo.gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.up*bouyancy, ForceMode.Force);
		}
	}
}
