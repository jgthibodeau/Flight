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
	public Vector3 groundNormal;
    public bool isGrounded;
    public bool isFlying;
	public bool isFlaming;
	public bool isRunning;
	public bool isAttacking;

	public float rigidBodyDrag = 1f;
	public float rigidBodyAngularDrag = 20f;
	public float walkSpeed;
	public float flameWalkSpeed;
	public float runSpeed;

	public bool gradualRun = true;
	public float currentSpeedIncreaseDelay;
	public float currentSpeed;
	public float speedIncreaseDelay;
	public float speedIncreaseRate;

	public float maxSpeed = 10f;
	public float hopTransitionSpeed;
	public float animationSpeedScale;
	public float minWalkInput;
	public float minWalkAmount;
	public float walkTurnSpeed;
	public float flameTurnSpeed;
	public float maxDirectTurnVelocity = 1f;

	public ForceMode walkForceMode = ForceMode.Force;

    public float jumpForce;

	//Inputs
	[HideInInspector]
	public float right, forward;
    [HideInInspector]
    public bool jump;

	// Use this for initialization
	void Start () {
	}

    /*
     * if grounded -> do walk/run and jump
     * if flying -> reset walking speeds
     * if !grounded, but !flying -> do gravity
     */
	void FixedUpdate () {
		//if (isGrounded) {
		//	RigidbodyWalk ();
		//} else {
		//	ResetWalkSpeed ();
		//}
		if (isGrounded) {
			RigidbodyWalk ();
		} else if (isFlying) {
			ResetWalkSpeed ();
		} else
        {
            right = 0;
            forward = 0;
            RigidbodyWalk();
        }
	}

	void ResetWalkSpeed() {
		currentSpeedIncreaseDelay = speedIncreaseDelay;
		currentSpeed = walkSpeed;
        dragonAnimator.MoveSpeed = 0;
    }

	void RigidbodyWalk() {
		//			rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		rigidBody.drag = rigidBodyDrag;
		rigidBody.angularDrag = rigidBodyAngularDrag;

		Vector3 inputVector = Vector3.ClampMagnitude(new Vector3 (right, 0, forward), 1);
		float inputSpeed = inputVector.magnitude;
		float speed = rigidBody.velocity.magnitude;

		Quaternion targetRotation;

		AnimatorStateInfo asi = dragonAnimator.animator.GetCurrentAnimatorStateInfo (0);
		asi.IsName ("Attack");

		if (inputSpeed >= minWalkInput && !isAttacking && !asi.IsName ("Attack")) {
//			Debug.Break ();
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
				ResetWalkSpeed ();
			} else if (gradualRun) {
				//if holding down movement, currentSpeed = walkSpeed
				//if holding down movement > .95, start counting down delay
				if (inputSpeed >= 0.95f) {
					if (currentSpeed > walkSpeed || currentSpeedIncreaseDelay <= 0) {
						currentSpeed += speedIncreaseRate * Time.fixedDeltaTime;
					} else {
						currentSpeedIncreaseDelay -= Time.fixedDeltaTime;
					}
				} else {
					currentSpeedIncreaseDelay = speedIncreaseDelay;
					currentSpeed -= speedIncreaseRate * Time.fixedDeltaTime;
				}

				currentSpeed = Mathf.Clamp (currentSpeed, walkSpeed, runSpeed);
				moveDirection *= currentSpeed;
			} else {
				if (isRunning) {
					moveDirection *= runSpeed;
				} else {
					moveDirection *= walkSpeed;
				}
			}

			Util.DrawRigidbodyRay (rigidBody, transform.position + rigidBody.centerOfMass, moveDirection, Color.red);
			Util.DrawRigidbodyRay (rigidBody, transform.position, groundNormal, Color.red);
//			Debug.Break ();
			rigidBody.AddForce (moveDirection, walkForceMode);
//			rigidBody.AddForceAtPosition (moveDirection, transform.position, walkForceMode);

//			Debug.Break ();

			Vector3 lookAt = transform.position;
			//				if (rigidBody.velocity.magnitude > maxDirectTurnVelocity) {
			lookAt += rigidBody.velocity;
			//				} else {
			//					lookAt += moveDirection;
			//				}
			//				lookAt = Vector3.ProjectOnPlane(lookAt, groundNormal);
			//				lookAt = Vector3.Slerp(transform.forward, lookAt, walkTurnSpeed * Time.fixedDeltaTime);

//			Quaternion targetRotation = Quaternion.LookRotation(rigidBody.velocity, groundNormal);
			//			Quaternion targetRotation = Quaternion.LookRotation(rigidBody.velocity);
			if (moveDirection.y > rigidBody.velocity.y) {
				targetRotation = Quaternion.LookRotation (moveDirection, groundNormal);
			} else {
				targetRotation = Quaternion.LookRotation (rigidBody.velocity, groundNormal);
			}
//			Debug.Break ();
			//				Vector3 velocity = rigidBody.velocity;
			//				transform.LookAt (lookAt, groundNormal);

//				rigidBody.velocity = velocity;
		} else {
			Vector3 forward = Vector3.ProjectOnPlane (transform.forward, groundNormal);
			targetRotation = Quaternion.LookRotation(forward, groundNormal);
			ResetWalkSpeed ();
		}

		if (isFlaming) {
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, flameTurnSpeed * Time.fixedDeltaTime);
		} else {
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, walkTurnSpeed * Time.fixedDeltaTime);
		}

        float forwardSpeed = Vector3.Dot(rigidBody.velocity, transform.forward);


        if (forwardSpeed >= minWalkAmount) {
			bool walking = forwardSpeed > 0;
			bool running = forwardSpeed > hopTransitionSpeed;

			birdAnimator.Walking = walking && !running;
			birdAnimator.Hopping = running;
			birdAnimator.MoveSpeed = forwardSpeed * animationSpeedScale;

			dragonAnimator.Walking = walking && !running;
			dragonAnimator.Hopping = running;
			dragonAnimator.MoveSpeed = forwardSpeed;
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
