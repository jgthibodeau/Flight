using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundParticles : MonoBehaviour {
	public bool waterParticle;
	public LayerMask triggerLayers;
	public float triggerDistance;
	public float maxEmission;
	public float minVelocity;
	public float maxVelocity;
	public Rigidbody target;
	public float forwardOffset;
	public float upOffset;

	private ParticleSystem particles;
	private ParticleSystem.EmissionModule emission;

	// Use this for initialization
	void Start () {
		particles = GetComponent<ParticleSystem> ();
		emission = particles.emission;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//if raycast down from center contains layermask
		RaycastHit hit;
		if (target.velocity.magnitude > minVelocity && Physics.Raycast (Util.RigidBodyPosition (target) + Vector3.up, Vector3.down, out hit, triggerDistance, triggerLayers)) {
			//calculate number emission based off distance and speed
			Vector3 velocityDirection = Vector3.ProjectOnPlane (target.velocity, Vector3.up);
			float emissionAmount = maxEmission * (1 - hit.distance / triggerDistance);
			emissionAmount *= Mathf.Clamp (velocityDirection.magnitude / maxVelocity, 0f, 1f);

			//make sure particle emitter is enabled
			emission.rateOverTime = emissionAmount;

			//set position
			Vector3 targetDirection = Vector3.ClampMagnitude (velocityDirection, 1);
			Vector3 forward = forwardOffset * targetDirection;
			Vector3 position = hit.point;
			Vector3 up;
			if (waterParticle) {
				float waterLevel = Util.GetWaterLevel (Util.RigidBodyPosition (target), false, false);
				up = Vector3.up * Mathf.Clamp (upOffset, 0, target.transform.position.y - waterLevel);
				position.y = waterLevel;
			} else {
				float realDistance = Vector3.Distance (Util.RigidBodyPosition (target), hit.point);
				up = Vector3.up * Mathf.Clamp(upOffset, 0, realDistance);
			}
			transform.position = position + forward + up;
		} else {
			//disable particle emitter
			emission.rateOverTime = 0;
		}
	}
}
