using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {
	public Transform player;
	public Vector3 speed;
	public float maxDistance;
	public float lastDistance;
	public float flipDistance;
	public float checkDistance;
	Vector2 playerPos;
	Vector2 cloudPos;
	bool recentlyFlipped;

	public float minScale;
	public float maxScale;
	public float scale;

	void Start () {
	}

	void Update () {
		//move cloud
		transform.position += speed * Time.deltaTime;
	}

	public void CalculateDistance() {
		Vector3 realPlayerPos = Util.RigidBodyPosition (player.GetComponent<Rigidbody> ());
		playerPos = new Vector2 (realPlayerPos.x, realPlayerPos.z);
		cloudPos = new Vector2 (transform.position.x, transform.position.z);
		lastDistance = Vector2.Distance (playerPos, cloudPos);
	}

	public void Flip() {
		if (recentlyFlipped) {
			recentlyFlipped = false;
			return;
		}

		lastDistance /= 2;

//		Debug.Log ("lastDistance "+lastDistance);
		Vector3 newCloudPos = transform.position;
		float xDif = 2 * (playerPos.x - cloudPos.x);
		newCloudPos.x += xDif;
		cloudPos.x = newCloudPos.x;

		float yDif = 2 * (playerPos.y - cloudPos.y);
		newCloudPos.z += yDif;
		cloudPos.y = newCloudPos.z;

		//as long as the new position doesn't put us too far again, set it
		//				if (Vector2.Distance (cloud.playerPos, cloud.cloudPos) <= flipDistance) {
		transform.position = newCloudPos;
		//				}
		recentlyFlipped = true;
	}

	void OnTriggerEnter(Collider collision){
		if (collision.transform.tag == "MainCamera") {
			Debug.Log ("camera in cloud");
			foreach (Transform t in transform) {
				Debug.Log ("enabling ps");
				t.gameObject.SetActive (true);
			}
			ParticleSystem ps = collision.transform.Find ("Cloud Puff").GetComponent<ParticleSystem> ();
			if (ps != null) {
//				Debug.Log ("bursting");
//				Debug.Break ();
//				ps.Emit (10);
//				ps.Clear ();
//				ps.Simulate (ps.main.duration);
//				ps.Play ();
				ps.Clear ();
				ps.Simulate (0f, true, true);
				ps.Play ();
			}
		}
	}

	void OnTriggerExit(Collider collision){
		if (collision.transform.tag == "MainCamera") {
			Debug.Log ("camera out cloud");
			foreach (Transform t in transform) {
				Debug.Log ("disabling ps");
				t.gameObject.SetActive (false);
			}
			ParticleSystem ps = collision.transform.Find ("Cloud Puff").GetComponent<ParticleSystem> ();
			if (ps != null) {
//				Debug.Log ("bursting");
//				Debug.Break ();
//				ps.Emit (10);
//				ps.Clear ();
//				ps.Simulate (ps.main.duration);
//				ps.Play ();
				ps.Clear ();
				ps.Simulate (0f, true, true);
				ps.Play ();
			}
		}
	}
}
