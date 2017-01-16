using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private Glide glideScript;
	private GlideV2 glideV2Script;
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
		glideV2Script = transform.GetComponent<GlideV2> ();
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

			glideScript.rigidBody.velocity = Vector3.zero;
			glideScript.rigidBody.constraints = RigidbodyConstraints.FreezePosition;


			glideV2Script.pitch = 0;
			glideV2Script.yaw = 0;
			glideV2Script.roll = 0;
			glideV2Script.forward = 0;
			glideV2Script.turn = 0;
			glideV2Script.flapSpeed = 0;
			glideV2Script.flapDirection = 0;
			glideV2Script.flapHorizontalDirection = 0;

			glideV2Script.rigidBody.velocity = Vector3.zero;
			glideV2Script.rigidBody.constraints = RigidbodyConstraints.FreezePosition;


			grabScript.grab = false;

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

			if (glideScript.flapSpeed == 0) {
				glideScript.wingsOut = Input.GetButtonDown ("Close Wings") ^ glideScript.wingsOut;
			} else {
				glideScript.wingsOut = true;
			}

			glideScript.rigidBody.constraints = RigidbodyConstraints.None;



			glideV2Script.pitch = Input.GetAxis ("Vertical");
			glideV2Script.roll = Input.GetAxis ("Horizontal");

			glideV2Script.yaw = Input.GetAxis ("Horizontal Right");
//			glideV2Script.tailPitch = (-1)*Input.GetAxis ("Vertical Right");

			glideV2Script.forward = Input.GetAxis ("Vertical");
			glideV2Script.right = Input.GetAxis ("Horizontal");
			glideV2Script.turn = Input.GetAxis ("Horizontal Right");

			glideV2Script.flapSpeed = Input.GetAxis ("Flap");
			glideV2Script.flapDirection = Input.GetAxis ("Vertical Right");
//			glideV2Script.flapHorizontalDirection = Input.GetAxis ("Horizontal Right");

			if (glideV2Script.flapSpeed == 0) {
				glideV2Script.wingsOut = Input.GetButtonDown ("Close Wings") ^ glideScript.wingsOut;
			} else {
				glideV2Script.wingsOut = true;
			}

			glideV2Script.rigidBody.constraints = RigidbodyConstraints.None;


			grabScript.grab = (Input.GetAxis ("Grab") != 0);
		}
	}
}
