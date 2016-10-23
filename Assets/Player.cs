using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private Glide glideScript;
	private Grab grabScript;
	private Perch perchScript;

	private int PerchableLayer;
	private int EnemyLayer;
	private int PreyLayer;

	// Use this for initialization
	void Start () {
		PerchableLayer = LayerMask.NameToLayer ("Perchable");
		EnemyLayer = LayerMask.NameToLayer ("Enemy");
		PreyLayer = LayerMask.NameToLayer ("Prey");

		glideScript = transform.GetComponent<Glide> ();
		grabScript = transform.GetComponent<Grab> ();
		perchScript = transform.GetComponent<Perch> ();
	}
	
	// Update is called once per frame
	void Update () {
		//handle if have grabbed object
		if (grabScript.hasObject) {
			int grabbedLayer = grabScript.grabbedObject.gameObject.layer;
			if (grabbedLayer == PerchableLayer) {
				perchScript.SetPerch (grabScript.grabbedObject, grabScript.grabbedLocation, grabScript.grabbedNormal, glideScript.rigidBody.velocity.magnitude);
			} else if (grabbedLayer == EnemyLayer) {
			} else if (grabbedLayer == PreyLayer) {
			}

			grabScript.ResetGrabbedObject();
		}

		//as long as we aren't perched, do normal controls
		if (perchScript.isPerching) {
			glideScript.pitch = 0;
			glideScript.roll = 0;
			glideScript.forward = 0;
			glideScript.turn = 0;
			glideScript.flapSpeed = 0;
			glideScript.flapDirection = 0;
			glideScript.flapHorizontalDirection = 0;

			grabScript.grab = false;

			glideScript.rigidBody.velocity = Vector3.zero;
			glideScript.rigidBody.constraints = RigidbodyConstraints.FreezePosition;

//			if ((!perchScript.isPerching && Input.GetAxis ("Flap") != 0)) {
//				perchScript.ResetPerch ();
//			}
		}

		if(!perchScript.isPerching){
			glideScript.pitch = Input.GetAxis ("Vertical Right");
			glideScript.roll = Input.GetAxis ("Horizontal Right");
			glideScript.forward = Input.GetAxis ("Vertical");
			glideScript.right = Input.GetAxis ("Horizontal");
			glideScript.turn = Input.GetAxis ("Horizontal Right");
			glideScript.flapSpeed = Input.GetAxis ("Flap");
			glideScript.flapDirection = Input.GetAxis ("Vertical");
			glideScript.flapHorizontalDirection = Input.GetAxis ("Horizontal");

			grabScript.grab = (Input.GetAxis ("Grab") != 0);

			glideScript.rigidBody.constraints = RigidbodyConstraints.None;
		}
	}
}
