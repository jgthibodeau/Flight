using UnityEngine;
using System.Collections;

namespace ThirdPersonCamera
{
    public class Follow : MonoBehaviour
    {
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

        void Start()
        {
            cc = GetComponent<CameraController>();
			player = cc.target.GetComponent<Player> ();
        }

        void Update()
        {
			alignOnSlopes = player.inWater;

			if (follow)
//			if (follow && Vector3.Distance (transform.position, cc.target.position) > cc.desiredDistance+0.1f)
            {
                RaycastHit raycastHit;
                Vector3 upVector = Vector3.up;

                if (checkMotionForBackwards)
                {
                    Vector3 motionVector = cc.target.transform.position - prevPosition;

                    if (motionVector.magnitude > backwardsMotionThreshold)
                    {
                        float angle = Vector3.Angle(motionVector, cc.target.transform.forward);

                        if (angle > angleThreshold)
                        {
                            lookBackwards = true;
                        }
                        else
                            lookBackwards = false;
                    }

                    prevPosition = cc.target.transform.position;
                }

				Vector3 desiredRotation;
				Vector3 up;


				bool left = Input.GetButton ("Camera Left");
				bool right = Input.GetButton ("Camera Right");
				bool back = Input.GetButton ("Camera Back");

				if (back || (left && right)) {
					desiredRotation = -(cc.target.transform.forward + tiltVector);
					up = Vector3.up;
				} else if (left) {
					desiredRotation = -(cc.target.transform.right + tiltVector);
					up = cc.target.transform.up;
				} else if (right) {
					desiredRotation = cc.target.transform.right + tiltVector;
					up = cc.target.transform.up;
				} else {
					desiredRotation = cc.target.transform.forward + tiltVector;
					up = Vector3.up;
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