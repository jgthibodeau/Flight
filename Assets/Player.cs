using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private Glide glideScript;

	// Use this for initialization
	void Start () {
		glideScript = transform.GetComponent<Glide> ();
	}
	
	// Update is called once per frame
	void Update () {
		glideScript.pitch = Input.GetAxis ("Vertical");
		glideScript.roll = Input.GetAxis ("Horizontal");
		glideScript.forward = Input.GetAxis ("Vertical");
		glideScript.turn = Input.GetAxis ("Horizontal");
		glideScript.flapSpeed = Input.GetAxis ("Flap");
		glideScript.flapDirection = Input.GetAxis ("Vertical Right");
	}
}
