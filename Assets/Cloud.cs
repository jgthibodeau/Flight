using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {
	public GameObject colliders;

	[HideInInspector]
	public float toggleDistance;

//	[HideInInspector]
	public Transform player;
    public Rigidbody playerRb;
    //	[HideInInspector]
    public Vector3 speed;
	[HideInInspector]
	public float maxDistance;
	[HideInInspector]
	public float lastDistance;
	[HideInInspector]
	public float flipDistance;
	[HideInInspector]
	public float checkDistance;
	[HideInInspector]
	Vector2 playerPos;
	[HideInInspector]
	Vector2 cloudPos;
	[HideInInspector]
	bool recentlyFlipped;

	[HideInInspector]
	public float minScale;
	[HideInInspector]
	public float maxScale;
	[HideInInspector]
	public float scale;

	void Start () {
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		toggleDistance = mesh.bounds.size.magnitude/2 * transform.localScale.x;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (playerRb == null && player != null)
        {
            playerRb = player.GetComponentInChildren<Rigidbody>();
        }
    }

	void Update ()
    {
        if (playerRb == null && player != null)
        {
            playerRb = player.GetComponentInChildren<Rigidbody>();
        }

        //move cloud
        transform.position += speed * Time.deltaTime;

        Vector3 playerPos = player.transform.position;
		if (Vector3.Distance (playerPos, transform.position) < toggleDistance) {
			if (!colliders.activeSelf) {
				colliders.gameObject.SetActive (true);
			}
		} else if (colliders.activeSelf) {
			colliders.gameObject.SetActive (true);
		}
	}

	public void CalculateDistance() {
		Vector3 realPlayerPos = playerRb != null ? Util.RigidBodyPosition (playerRb) : player.position;
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

//	void OnTriggerEnter(Collider collision){
//		if (collision.transform.tag == "MainCamera") {
//			Debug.Log ("camera in cloud");
//			foreach (Transform t in transform) {
//				Debug.Log ("enabling ps");
//				t.gameObject.SetActive (true);
//			}
//			ParticleSystem ps = collision.transform.Find ("Cloud Puff").GetComponent<ParticleSystem> ();
//			if (ps != null) {
////				Debug.Log ("bursting");
////				Debug.Break ();
////				ps.Emit (10);
////				ps.Clear ();
////				ps.Simulate (ps.main.duration);
////				ps.Play ();
//				ps.Clear ();
//				ps.Simulate (0f, true, true);
//				ps.Play ();
//			}
//		}
//	}
//
//	void OnTriggerExit(Collider collision){
//		if (collision.transform.tag == "MainCamera") {
//			Debug.Log ("camera out cloud");
//			foreach (Transform t in transform) {
//				Debug.Log ("disabling ps");
//				t.gameObject.SetActive (false);
//			}
//			ParticleSystem ps = collision.transform.Find ("Cloud Puff").GetComponent<ParticleSystem> ();
//			if (ps != null) {
////				Debug.Log ("bursting");
////				Debug.Break ();
////				ps.Emit (10);
////				ps.Clear ();
////				ps.Simulate (ps.main.duration);
////				ps.Play ();
//				ps.Clear ();
//				ps.Simulate (0f, true, true);
//				ps.Play ();
//			}
//		}
//	}
}
