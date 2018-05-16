using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadcrumbAi;

public class Enemy : MonoBehaviour {
	private Ai ai;
	public GameObject arrow;
	public float arrowForce = 10f;
	public Transform firePosition;

	public float fireRate;
	private float lastFireTime;

	private Rigidbody rigidBody;
	public Animator animator;

	void Start () {
		ai = GetComponent<Ai> ();
		rigidBody = GetComponent<Rigidbody> ();
		animator = GetComponentInChildren<Animator> ();
	}

	void Update () {
		animator.SetFloat ("Speed", ai.currentSpeed);
//		Debug.Log (rigidBody.velocity+" "+rigidBody.velocity.magnitude);

		if (ai.attackState == Ai.ATTACK_STATE.CanAttackPlayer) {
			if (Time.time > lastFireTime + fireRate) {
				Vector3 position = firePosition.position;
				Vector3 targetPosition = ai.Player.GetComponent<Rigidbody>().position;

				Vector3 s0, s1;
				BallisticTrajectory.solve_ballistic_arc(position, arrowForce, targetPosition, ai.Player.GetComponent<Rigidbody>().velocity, -Physics.gravity.y, out s0, out s1);

				Vector3 trajectory;
				if (s0.y < s1.y) {
					trajectory = s0;
				} else {
					trajectory = s1;
				}

				if (trajectory != Vector3.zero) {
					Fire (position, trajectory);
				}
			}
		}
	}

	void Fire(Vector3 position, Vector3 trajectory) {
		Quaternion rotation = Quaternion.LookRotation (trajectory);
		lastFireTime = Time.time;
		GameObject newArrow = GameObject.Instantiate (arrow, position, rotation);
		newArrow.GetComponent<Rigidbody> ().AddForce (newArrow.transform.forward * arrowForce, ForceMode.VelocityChange);
	}
}
