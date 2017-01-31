using UnityEngine;
using System.Collections;

public class Walk : MonoBehaviour {
	[HideInInspector]
	public BirdAnimator birdAnimator;
	[HideInInspector]
	public Rigidbody rigidBody;
	[HideInInspector]
	public Vector3 groundNormal;
	[HideInInspector]
	public bool isGrounded;

	public float walkSpeed;
	public float minWalkAmount;
	public float walkTurnSpeed;

	//Inputs
	[HideInInspector]
	public float right, forward;

	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate () {
		if (isGrounded) {
			rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
			rigidBody.drag = 1;

			Vector3 inputVector = new Vector3 (right, 0, forward);
			float speed = inputVector.magnitude;
			bool walking = inputVector.magnitude > 0;
			bool running = inputVector.magnitude > 0.5f;

			birdAnimator.Walking = walking && !running;
			birdAnimator.Hopping = running;

			if (walking && speed >= minWalkAmount) {
				Vector3 direction = rigidBody.velocity;
				Vector3 surfaceParallel = direction - groundNormal * Vector3.Dot (direction, groundNormal);
				Quaternion lookDirection = Quaternion.LookRotation (surfaceParallel, groundNormal);
				transform.rotation = Quaternion.Lerp (transform.rotation, lookDirection, Time.fixedDeltaTime * walkTurnSpeed);

				Vector3 velocityChange = CalculateVelocityChange (inputVector);
				Util.DrawRigidbodyRay (rigidBody, transform.position, velocityChange, Color.cyan);

				rigidBody.AddForce (velocityChange, ForceMode.VelocityChange);
			}
		}
	}

	private Vector3 CalculateVelocityChange(Vector3 inputVector)
	{
		// Calculate how fast we should be moving
		Vector3 relativeVelocity = Camera.main.transform.TransformDirection(inputVector) * walkSpeed;

		// Calcualte the delta velocity
		Vector3 currRelativeVelocity = rigidBody.velocity;
		Vector3 velocityChange = relativeVelocity - currRelativeVelocity;
		float maxVelocityChange = 10.0f;
		velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;

		return velocityChange;
	}
}
