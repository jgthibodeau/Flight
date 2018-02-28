using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVelocity : MonoBehaviour {
	public Rigidbody rb;

	private ParticleSystem ps;
	private ParticleSystem.VelocityOverLifetimeModule vm;

	Vector3 initialVelocity;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		vm = ps.velocityOverLifetime;
		initialVelocity = new Vector3 (vm.xMultiplier, vm.yMultiplier, vm.zMultiplier);
	}
	
	// Update is called once per frame
	void Update () {
		float velocityUp = Vector3.Dot(rb.velocity, rb.transform.up);
		float velocityRight = Vector3.Dot(rb.velocity, rb.transform.right);
		float velocityForward = Vector3.Dot(rb.velocity, rb.transform.forward);

		vm.xMultiplier = initialVelocity.x + velocityRight;
		vm.yMultiplier = initialVelocity.y + velocityUp;
		vm.zMultiplier = initialVelocity.z + velocityForward;
	}
//	public Rigidbody target = null;
//	private ParticleSystem theEmitter;
//	public bool inheritParentVelocity = true;
//
//	void Start()
//	{
//		theEmitter = transform.GetComponent<ParticleSystem> ().emission;
//	}    void Update()
//	{
//		if (inheritParentVelocity)
//		{
//			if (target != null)
//			{
//				theEmitter.worldVelocity = target.velocity;
//			}
//			else
//			{
//				theEmitter.worldVelocity = transform.root.GetComponent<Rigidbody>().velocity;
//			}
//		}
//		else
//			theEmitter.worldVelocity = Vector3.zero;
//	}
}
