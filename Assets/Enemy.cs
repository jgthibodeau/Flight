using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadcrumbAi;

public class Enemy : MonoBehaviour {
	private Ai ai;
	public GameObject arrow;
	public float arrowForce = 10f;
	public float fireOffset = 1.5f;

	public float fireRate;
	private float lastFireTime;

	// Use this for initialization
	void Start () {
		ai = GetComponent<Ai> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ai.attackState == Ai.ATTACK_STATE.CanAttackPlayer) {
			if (Time.time > lastFireTime + fireRate) {
				lastFireTime = Time.time;

				GameObject newArrow = GameObject.Instantiate (arrow);

				Vector3 targetPosition = ai.Player.position;
				Vector3 position = transform.position + transform.forward * fireOffset;

				Vector3 s0, s1;
				BallisticTrajectory.solve_ballistic_arc(position, arrowForce, targetPosition, ai.Player.GetComponent<Rigidbody>().velocity, -Physics.gravity.y, out s0, out s1);

				newArrow.transform.position = position;
				newArrow.transform.rotation = Quaternion.LookRotation (s0, Vector3.up);

				newArrow.GetComponent<Rigidbody> ().AddForce (newArrow.transform.forward * arrowForce, ForceMode.Impulse);
			}
		}
	}
}
