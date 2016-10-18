using UnityEngine;
using System.Collections;

public class EnemyBird : MonoBehaviour {
	public Transform target;
	public float minUpAngle;
	public float minFlightHeight;
	public float minPitchAlignment;
	public float minRollAlignment;
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
				FlyTowardsTarget ();
			}
		}
	}

	//returns true if high enough from ground
	bool StayOffGround(){
		//get high enough from the ground
		RaycastHit hit;
		if (Physics.Raycast (transform.position, Vector3.down, out hit, minFlightHeight, glideScript.layerMaskForGround)) {
			//do nothing until pointed close to up
			Debug.Log (Mathf.Abs (transform.rotation.eulerAngles.z));
			if (transform.rotation.eulerAngles.z > minUpAngle &&  transform.rotation.eulerAngles.z < 360 - minUpAngle) {
				glideScript.flapSpeed = 0f;
			} else {
				glideScript.flapSpeed = 1f;
			}
			glideScript.flapDirection = 0f;
			return false;
		}
		glideScript.flapSpeed = 0f;
		return true;
	}

	void FlyTowardsTarget(){
		Vector3 desiredDirection = target.position - transform.position;
		bool pointedAtTarget = true;

		//horizontal alignment to target
		//roll till target in line with forward-up plane
		Plane forwardUpPlane = new Plane (Vector3.zero, transform.up, transform.forward);
		float horizontalDistance = forwardUpPlane.GetDistanceToPoint (desiredDirection);

		if (Mathf.Abs (horizontalDistance) > minRollAlignment) {
			glideScript.roll = Mathf.Clamp (horizontalDistance * rollScale, -1, 1);
			pointedAtTarget = false;
		} 
		//TODO otherwise, roll to be perpendicular to the ground
		else {
			
		}

		//vertical alignment to target
		//pitch till target in front of up-right plane
		Plane upRightPlane = new Plane (Vector3.zero, transform.right, transform.up);
		float forwardDistance = upRightPlane.GetDistanceToPoint (desiredDirection);

		Plane forwardRightPlane = new Plane (Vector3.zero, transform.forward, transform.right);
		float verticalDistance = forwardRightPlane.GetDistanceToPoint (desiredDirection);

		if (forwardDistance <= 0) {
			if (verticalDistance < 0) {
				glideScript.pitch = 1f;
			} else {
				glideScript.pitch = -1f;
			}
			pointedAtTarget = false;
		}
		//pitch till target in forward-right plane 
		else if (Mathf.Abs (verticalDistance) > minPitchAlignment) {
			glideScript.pitch = Mathf.Clamp (-verticalDistance * pitchScale, -1, 1);
			pointedAtTarget = false;
		}

		//adjust flap angle to stay moving forward
		//flap more forward as velocity aproaches transform.forward
		//flap more upward as velocity goes away from transform.forward
		if (pointedAtTarget) {
			glideScript.flapDirection = 1f;
		} else {
			glideScript.flapDirection = .5f;
		}
		glideScript.flapSpeed = 1f;
	}
}
