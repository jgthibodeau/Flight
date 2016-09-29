//using UnityEngine;
//using System.Collections;
//
//public class CameraFollowAngled : MonoBehaviour {
//	public Transform target;
//	public float height;
//
//	void LateUpdate(){
//		float targetRotation = target.rotation.eulerAngles.y;
//		float targetHeight = target.position.y + height;
//	}
//}

using UnityEngine;
using System.Collections;

public class CameraFollowAngled : MonoBehaviour {
	public Transform target;
	public float distance;
	public float height;
	public float lookOffset;
	public float accuracy;
	public float translationSmoothness;
	public float rotationSmoothness;

	public bool lookAt;
	public bool useTargetForward;

	private Camera camera;
	private Rigidbody targetRigidbody;

	// Use this for initialization
	void Start () {
		camera = this.GetComponent<Camera> ();
		targetRigidbody = target.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 newPos = target.position - target.forward * distance + target.up * height;

//		transform.position = transform.position * accuracy + newPos * (1f - accuracy);

		transform.position = Vector3.Slerp (transform.position, newPos, Time.deltaTime * translationSmoothness);

//		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles * accuracy + target.rotation.eulerAngles * (1f - accuracy));

//		transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationSmoothness);

		if (lookAt) {
			Vector3 lookRotation;
			if (useTargetForward) {
				Vector3 targetPosition = target.position + target.forward * lookOffset;
				lookRotation = Quaternion.LookRotation(targetPosition - transform.position).eulerAngles;
			} else {
				Vector3 targetPosition = target.position + targetRigidbody.velocity.normalized * lookOffset;
				lookRotation = Quaternion.LookRotation(targetPosition - transform.position).eulerAngles;
			}

			lookRotation.z = target.rotation.eulerAngles.z;
			Quaternion newRotation = Quaternion.Euler (lookRotation);
			transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * rotationSmoothness);
		} else {
			transform.rotation = Quaternion.Slerp (transform.rotation, target.rotation, Time.deltaTime * rotationSmoothness);
		}
	}
}
