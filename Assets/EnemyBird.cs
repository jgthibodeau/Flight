using UnityEngine;
using System.Collections;

public class EnemyBird : MonoBehaviour {
	public Transform target;
	public float minFlightHeight;
	public float minLookAngle;
	public float pitchScale;
	public float rollScale;

	private float distanceToGround;
	private Glide glideScript;

	// Use this for initialization
	void Start () {
		glideScript = transform.GetComponent<Glide> ();
	}
	
	// Update is called once per frame
	void Update () {
		//if can see target
		if (true) {
			bool aboveGround = StayOffGround ();
			if (aboveGround) {
//				bool lookingAtTarget = LookAtTarget ();
//				if (lookingAtTarget) {
					FlyTowardsTarget ();
//				}
			}
		}
	}

	//returns true if high enough from ground
	bool StayOffGround(){
		//get high enough from the ground
		RaycastHit hit;
		if (Physics.Raycast (transform.position, Vector3.down, out hit, minFlightHeight, glideScript.layerMaskForGround)) {
			glideScript.flapDirection = 0f;
			glideScript.flapSpeed = 1f;
			return false;
		}
		glideScript.flapSpeed = 0f;
		return true;
	}

	bool LookAtTarget(){
		Vector3 desiredDirection = target.position - transform.position;
		if (Vector3.Angle (desiredDirection, transform.forward) > minLookAngle) {
			//roll and pitch enough to turn towards target
			return false;
		}
		return true;
	}

	void FlyTowardsTarget(){
		//horizontal alignment to target
		//roll till target in line with forward-up plane
		Vector3 desiredDirection = target.position - transform.position;
		if (Vector3.Angle (desiredDirection, transform.forward) > minLookAngle) {
			//roll and pitch enough to turn towards target
		}

		//vertical alignment to target
		//pitch till target in forward-right plane
		float desiredPitch = (transform.position.y - target.position.y) * pitchScale;
		glideScript.pitch = Mathf.Clamp (desiredPitch , -1f, 1f);

		//adjust flap angle as speed increases
		//flap more forward as downward velocity decreases
		glideScript.flapDirection = .5f;
		glideScript.flapSpeed = 1f;
	}
}
