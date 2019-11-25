using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformLate : MonoBehaviour {
	public bool useLateUpdate = true;
	public Transform targetBase;
	public Transform target; // The Transform parented to one of the character's bones
    public bool worldRotation;

	private Vector3 targetPosition;	//instead combine base + target to get offset vector, and use this compared to latest base
	private Vector3 targetOffset;
	private Quaternion targetRotation;

	void Start() {
		LateUpdate();
	}
	// Move this ParticleSystem to the target's last frame position before it emits
	void Update() {
//		if (useLateUpdate) {
////			transform.position = targetPosition;
//			transform.position = targetBase.position + targetOffset;
//			transform.rotation = targetRotation;
//		} else {
//			transform.position = target.position;
//			transform.rotation = target.rotation;
//		}
	}
	// Read the world space position and rotation of the target after procedural effects have been applied
	// NB! Make sure this script is set to a higher value than FinalIK components in the Script Execution Order!
	void LateUpdate() {
		targetOffset = target.position - targetBase.position;
		targetPosition = target.position;
		targetRotation = target.rotation;


        //transform.position = targetBase.position + targetOffset;
        //transform.rotation = targetRotation;

        transform.position = target.position;
        if (worldRotation)
        {
            transform.rotation = targetRotation;
        } else
        {
            transform.localRotation = Quaternion.LookRotation(targetBase.transform.InverseTransformDirection(target.forward), targetBase.transform.up);
        }
    }
}
