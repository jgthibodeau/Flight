using UnityEngine;
using System.Collections;

public class Bug : MonoBehaviour {
	public Transform player;
	public float reactionDistance;
	public float desiredDistance;
	public float speed;

	public float randomness;

	private bool noticed = false;
	private Rigidbody rigidBody;

	public float reactionTime;
	public float remainingReactionTime;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		//if player is close enough
		if (Vector3.Distance (transform.position, player.position) <= reactionDistance) {
			//decrease reaction time until noticed player
			if (remainingReactionTime <= 0) {
				noticed = true;
			} else {
				remainingReactionTime -= Time.deltaTime;
			}
		} else {
			remainingReactionTime = reactionTime;
		}
			
		//if far enough away from player, stop moving
		if (Vector3.Distance (transform.position, player.position) >= desiredDistance) {
			noticed = false;
		}
	}

	void FixedUpdate(){
		//if player has been detected
		if (noticed) {
			//move away from player
			Vector3 direction = transform.position - player.position;
			direction = direction.normalized + new Vector3 (Random.Range (-randomness, randomness), Random.Range (-randomness, randomness), Random.Range (-randomness, randomness));
			direction *= speed;

			//			transform.position = Vector3.Slerp (transform.position, transform.position + direction, speed * Time.deltaTime);
			rigidBody.AddForce (direction * speed);
		}
	}
}
