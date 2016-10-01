using UnityEngine;
using System.Collections;

public class Prey : MonoBehaviour {
	public Transform player;
	public float minDistance;
	public float speed;

	private CharacterController characterController;

	// Use this for initialization
	void Start () {
		characterController = transform.GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (transform.position, player.position) <= minDistance) {
			Vector3 direction = transform.position - player.position;
			direction = direction.normalized * speed;
			characterController.SimpleMove (direction);
		}
	}
}
