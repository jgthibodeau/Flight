using UnityEngine;
using System.Collections;

public class FishJump : MonoBehaviour {
	public float upJumpForce;
	public float forwardJumpForce;
	public float swimSpeed;
	public float gravity;
	public float deathTime;

	public float minSwimTime;
	public float maxSwimTime;
	public float remainingSwimTime = 0f;

	public float minJumpTime;
	public float maxJumpTime;
	public float remainingJumpTime = 0f;

	private Rigidbody rigidBody;

	public float underwaterDistance;
	float waterLevel;

	public Vector3 forwardDirection;

	public float forwardDistance = 0.1f;

	public bool jumping;
	public bool inWater;
	public bool atDesiredDepth;

	// Use this for initialization
	void Start () {
		waterLevel = MyGameManager.instance.oceanLevel;
		rigidBody = GetComponent<Rigidbody> ();
		if (deathTime > 0) {
			Invoke ("Die", deathTime);
		}
	}
	
	// Update is called once per frame
	void Update () {
//		Vector3 newRotation = transform.rotation.eulerAngles;
//		newRotation.z = 0;
//		transform.rotation = Quaternion.Euler (newRotation);

		//in water
		if (InWater ()) {
			Swim ();
		} else {
			//reset jump timer
			remainingJumpTime = Random.Range (minJumpTime, maxJumpTime);
			jumping = false;
		}

//		if (!AtDesiredDepth ()) {
//			rigidBody.AddForceAtPosition (Vector3.down * gravity, transform.position, ForceMode.Acceleration);
//		}
		if (AtDesiredDepth ()) {
			rigidBody.useGravity = false;
		} else {
			rigidBody.useGravity = true;
		}

//		Vector3 rotation = Quaternion.LookRotation(rigidBody.velocity, transform.up).eulerAngles;
//		transform.rotation = Quaternion.Euler (rotation);
	}

	//TODO
	bool InWater(){
		inWater = transform.position.y <= waterLevel;
		return transform.position.y <= waterLevel;
	}

	bool AtDesiredDepth (){
		atDesiredDepth = transform.position.y < (waterLevel - underwaterDistance);
		return transform.position.y < (waterLevel - underwaterDistance);
	}

	void Swim(){
		if (CloseToShore ()){
			//pick a direction away from shore
		}
		//else if swim time done
		else if (remainingSwimTime <= 0) {
			//pick randomish direction
			float randomDir = Random.Range (-1f, 1f);
			float forward = Random.Range (-1f, 1f);
			forwardDirection = Vector3.forward * forward + Vector3.right*randomDir;

			//reset timer
			remainingSwimTime = Random.Range (minSwimTime, maxSwimTime);
		}
		//else if jump time done
		if (remainingJumpTime <= 0){
			jumping = true;
			Jump ();
		}

		//move forward
		rigidBody.AddForceAtPosition (forwardDirection * swimSpeed, transform.position + transform.forward * forwardDistance);

		remainingSwimTime -= Time.deltaTime;
		remainingJumpTime -= Time.deltaTime;
	}

	//TODO
	bool CloseToShore(){
		//circle cast and return the point
		return false;
	}

	void Jump(){
		rigidBody.AddForceAtPosition (Vector3.up * upJumpForce, transform.position + transform.forward * forwardDistance);
//		rigidBody.AddForceAtPosition (Vector.forward * forwardJumpForce, transform.position + transform.forward * forwardDistance);
	}

	void Die(){
		Destroy (gameObject);
	}
}
