using UnityEngine;
using System.Collections;

public class Bug : MonoBehaviour {
	public Transform player;
	public float minDistance;
	public float speed;

	public float reactionTime;
	public float remainingReactionTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (transform.position, player.position) <= minDistance) {
			if (remainingReactionTime <= 0) {
				Vector3 direction = transform.position - player.position;
				direction.y = 0;
				direction = direction.normalized * speed;

				transform.position = Vector3.Slerp (transform.position, transform.position + direction, speed * Time.deltaTime);
			} else {
				remainingReactionTime -= Time.deltaTime;
			}
		} else {
			remainingReactionTime = reactionTime;
		}
	}
}
