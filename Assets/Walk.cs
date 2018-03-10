using UnityEngine;
using System.Collections;

public class Walk : MonoBehaviour {
	[HideInInspector]
	public BirdAnimator birdAnimator;
	[HideInInspector]
	public DragonAnimator dragonAnimator;
	[HideInInspector]
	public Rigidbody rigidBody;
	[HideInInspector]
	public CharacterController characterController;
	[HideInInspector]
	public Vector3 groundNormal;
	[HideInInspector]
	public bool isGrounded;
	[HideInInspector]
	public bool isFlaming;

	public bool useRigidbody = true;
	public float rigidBodyDrag = 1f;
	public float walkSpeed;
	public float flameWalkSpeed;
	public float maxSpeed = 10f;
	public float hopTransitionSpeed;
	public float animationSpeedScale;
	public float minWalkInput;
	public float minWalkAmount;
	public float walkTurnSpeed;
	public float flameTurnSpeed;
	public float maxDirectTurnVelocity = 1f;

	public ForceMode walkForceMode = ForceMode.Force;

	//Inputs
	[HideInInspector]
	public float right, forward;

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController> ();
	}

	void FixedUpdate () {
		if (isGrounded) {
			if (useRigidbody) {
				rigidBody.isKinematic = false;
				characterController.enabled = false;
				RigidbodyWalk ();
			} else {
				rigidBody.isKinematic = true;
				characterController.enabled = true;
				CharacterControllerWalk ();

				rigidBody.velocity = currentMoveDirection;
			}
		} else {
			rigidBody.isKinematic = false;
			characterController.enabled = false;
			currentMoveDirection = Vector3.ClampMagnitude(rigidBody.velocity, 0) * walkSpeed;
		}
	}

	Vector3 currentMoveDirection = Vector3.zero;
	Vector3 lastMoveDirection = Vector3.zero;
	void CharacterControllerWalk() {
		Vector3 inputVector = new Vector3 (right, 0, forward).normalized;
		float inputSpeed = inputVector.magnitude;

		Vector3 moveDirection = CalculateDesiredMovementDirection (inputVector);
		moveDirection = Vector3.ProjectOnPlane (moveDirection, groundNormal).normalized * inputSpeed;
		moveDirection *= walkSpeed;

		currentMoveDirection = Vector3.Slerp (currentMoveDirection, moveDirection, walkTurnSpeed * Time.fixedDeltaTime);
		if (currentMoveDirection.magnitude < 0.01f) {
			currentMoveDirection = Vector3.zero;
		}

		Vector3 moveWithGravity = currentMoveDirection;
		if (!characterController.isGrounded) {
			moveWithGravity.y = -10f * Time.fixedDeltaTime;
		}
//		transform.up = groundNormal;
		characterController.Move (moveWithGravity);

		Quaternion targetRotation;
		if (inputSpeed > 0) {
			targetRotation = Quaternion.LookRotation (moveDirection, groundNormal);
		} else {
			targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, groundNormal));
		}
		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, walkTurnSpeed * Time.fixedDeltaTime);

//		birdAnimator.Walking = walking && !running;
//		birdAnimator.Hopping = running;
		birdAnimator.MoveSpeed = currentMoveDirection.magnitude * animationSpeedScale;

//		dragonAnimator.Walking = walking && !running;
//		dragonAnimator.Hopping = running;
		dragonAnimator.MoveSpeed = currentMoveDirection.magnitude;
		dragonAnimator.Grounded = characterController.isGrounded;
		dragonAnimator.Walking = currentMoveDirection.magnitude > 0;
	}

	void RigidbodyWalk() {
		//			rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		rigidBody.drag = rigidBodyDrag;

		Vector3 inputVector = new Vector3 (right, 0, forward).normalized;
		float inputSpeed = inputVector.magnitude;
		float speed = rigidBody.velocity.magnitude;

		if (inputSpeed >= minWalkInput) {
			//				Vector3 direction = rigidBody.velocity;
			//				Vector3 surfaceParallel = direction - groundNormal * Vector3.Dot (direction, groundNormal);
			//				Quaternion lookDirection = Quaternion.LookRotation (surfaceParallel, groundNormal);
			//				transform.rotation = Quaternion.Lerp (transform.rotation, lookDirection, Time.fixedDeltaTime * walkTurnSpeed);
			//
			//				Vector3 velocityChange = CalculateVelocityChange (inputVector);
			//				Util.DrawRigidbodyRay (rigidBody, transform.position, velocityChange, Color.cyan);
			//
			//				rigidBody.AddForce (velocityChange, ForceMode.VelocityChange);

			Vector3 moveDirection = CalculateDesiredMovementDirection (inputVector);
			moveDirection = Vector3.ProjectOnPlane (moveDirection, groundNormal).normalized * inputSpeed;
			if (isFlaming) {
				moveDirection *= flameWalkSpeed;
			} else {
				moveDirection *= walkSpeed;
			}

			Util.DrawRigidbodyRay (rigidBody, transform.position, moveDirection, Color.red);
			Util.DrawRigidbodyRay (rigidBody, transform.position, groundNormal, Color.red);
			//				rigidBody.AddForce (moveDirection, walkForceMode);
			rigidBody.AddForceAtPosition (moveDirection, transform.position, walkForceMode);


			Vector3 lookAt = transform.position;
			//				if (rigidBody.velocity.magnitude > maxDirectTurnVelocity) {
			lookAt += rigidBody.velocity;
			//				} else {
			//					lookAt += moveDirection;
			//				}
			//				lookAt = Vector3.ProjectOnPlane(lookAt, groundNormal);
			//				lookAt = Vector3.Slerp(transform.forward, lookAt, walkTurnSpeed * Time.fixedDeltaTime);

			//				Quaternion targetRotation = Quaternion.LookRotation(rigidBody.velocity, groundNormal);
			Quaternion targetRotation = Quaternion.LookRotation(rigidBody.velocity);

			//				Vector3 velocity = rigidBody.velocity;
			//				transform.LookAt (lookAt, groundNormal);
			if (isFlaming) {
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, flameTurnSpeed * Time.fixedDeltaTime);
			} else {
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, walkTurnSpeed * Time.fixedDeltaTime);
			}
			//				rigidBody.velocity = velocity;
		}


		if (speed >= minWalkAmount) {
			bool walking = speed > 0;
			bool running = speed > hopTransitionSpeed;

			birdAnimator.Walking = walking && !running;
			birdAnimator.Hopping = running;
			birdAnimator.MoveSpeed = speed * animationSpeedScale;

			dragonAnimator.Walking = walking && !running;
			dragonAnimator.Hopping = running;
			dragonAnimator.MoveSpeed = speed;
		} else {
			birdAnimator.Walking = false;
			birdAnimator.Hopping = false;
			birdAnimator.MoveSpeed = 1;

			dragonAnimator.Walking = false;
			dragonAnimator.Hopping = false;
			dragonAnimator.MoveSpeed = 0;
		}
	}

	private Vector3 CalculateVelocityChange(Vector3 inputVector)
	{
		// Calculate how fast we should be moving
		Vector3 relativeVelocity = Camera.main.transform.TransformDirection(inputVector) * walkSpeed;

		// Calcualte the delta velocity
		Vector3 currRelativeVelocity = rigidBody.velocity;
		Vector3 velocityChange = relativeVelocity - currRelativeVelocity;
		float maxVelocityChange = maxSpeed;
		velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;

		return velocityChange;
	}

	private Vector3 CalculateDesiredMovementDirection(Vector3 input) {
//		Vector3 cameraForward = Camera.main.transform.TransformDirection(Vector3.forward);
		Vector3 cameraForward = Camera.main.transform.forward;
		cameraForward.y = 0f;
		cameraForward = cameraForward.normalized;

//		Vector3 cameraRight = new Vector3(cameraForward.z, 0.0f, -cameraForward.x);
		Vector3 cameraRight = Camera.main.transform.right;
		cameraRight.y = 0f;
		cameraRight = cameraRight.normalized;

		Vector3 desiredDirection = input.x * cameraRight + input.z * cameraForward;

		return desiredDirection;
	}
}
