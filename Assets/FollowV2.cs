using UnityEngine;
using System.Collections;

namespace ThirdPersonCamera
{
	public class FollowV2 : MonoBehaviour
	{
		public bool rotateWithTarget = false;
		public bool follow = true;
		public bool alignOnSlopes = true;

		public float minSpeedToFollow = 0.05f;

		public float rotationSpeed = 1.0f;
		public float rotationSpeedSlopes = 0.5f;
		public bool lookBackwards = false;
		public bool lookLeft = false;
		public bool lookRight = false;

		public bool checkMotionForBackwards = true;
		public float backwardsMotionThreshold = 0.05f;
		public float angleThreshold = 170.0f;

		public Vector3 tiltVector;
		public LayerMask layerMask;

		private Vector3 prevPosition;

		CameraController cc;
		Player player;
		Rigidbody targetRb;

		Vector3 lastForward = new Vector3();
		Vector3 lastRight = new Vector3();
		Vector3 lastUp = new Vector3();
		Vector3 up;

		void Start()
		{
			cc = GetComponent<CameraController>();
			player = cc.target.GetComponent<Player> ();
			targetRb = player.GetComponent<Rigidbody> ();
		}

		void Update()
		{
			alignOnSlopes = player.inWater;

			if (follow)
				//			if (follow && Vector3.Distance (transform.position, cc.target.position) > cc.desiredDistance+0.1f)
			{
				RaycastHit raycastHit;
				Vector3 upVector = Vector3.up;

				//                if (checkMotionForBackwards)
				//                {
				//                    Vector3 motionVector = cc.target.transform.position - prevPosition;
				//
				//                    if (motionVector.magnitude > backwardsMotionThreshold)
				//                    {
				//                        float angle = Vector3.Angle(motionVector, cc.target.transform.forward);
				//
				//                        if (angle > angleThreshold)
				//                        {
				//                            lookBackwards = true;
				//                        }
				//                        else
				//                            lookBackwards = false;
				//                    }
				//
				//                    prevPosition = cc.target.transform.position;
				//                }


				Vector3 desiredRotation;
				Vector3 up;

				bool left = Util.GetButton ("Camera Left");
				bool right = Util.GetButton ("Camera Right");
				bool back = Util.GetButton ("Camera Back");

				//if player moving fast enough or player moving away from camera
				float movementAngle = Vector3.Angle(transform.forward, targetRb.velocity);
				if (targetRb.velocity.magnitude > minSpeedToFollow || movementAngle < angleThreshold) {
//					if (back || (left && right)) {
//						desiredRotation = -cc.target.transform.forward + tiltVector;
//					} else if (left) {
//						desiredRotation = -cc.target.transform.right + tiltVector;
//					} else if (right) {
//						desiredRotation = cc.target.transform.right + tiltVector;
//					} else {
//						desiredRotation = cc.target.transform.forward + tiltVector;
//					}
					desiredRotation = cc.target.transform.position - transform.position + tiltVector;
					if (rotateWithTarget) {
						up = cc.target.transform.up;
					} else {
						up = Vector3.up;
					}
					lastForward = transform.forward;
					lastRight = transform.right;
					lastUp = transform.up;
				} else {
					if (back || (left && right)) {
						desiredRotation = -lastForward + tiltVector;
					} else if (left) {
						desiredRotation = -lastRight;
					} else if (right) {
						desiredRotation = lastRight;
					} else {
						desiredRotation = lastForward;
					}
					if (rotateWithTarget) {
						up = lastUp;
					} else {
						up = Vector3.up;
					}
				}

				Quaternion toRotation = Quaternion.LookRotation(desiredRotation, up);

				if (alignOnSlopes)
				{
					if (Physics.Raycast(cc.target.transform.position, Vector3.down, out raycastHit, 25.0f, layerMask)) // if the range of 15.0 is not enough, increase the value
					{
						upVector = raycastHit.normal;
					}

					float angle = AngleSigned(Vector3.up, upVector, cc.target.transform.right);

					toRotation = Quaternion.Slerp(toRotation, toRotation * Quaternion.AngleAxis(angle, Vector3.right), Time.deltaTime * rotationSpeedSlopes);
				}

				cc.transform.rotation = Quaternion.Slerp(cc.transform.rotation, toRotation, Time.deltaTime * rotationSpeed);

				//				Quaternion toRotation = Quaternion.LookRotation((cc.target.transform.position - transform.position + tiltVector), Vector3.up);
				//                cc.transform.rotation = Quaternion.Slerp(cc.transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
			}
		}

		public float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
		{
			return Mathf.Atan2(
				Vector3.Dot(n, Vector3.Cross(v1, v2)),
				Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
		}
	}
}