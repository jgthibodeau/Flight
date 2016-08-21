using UnityEngine;
using System.Collections;

public class RandomPath : MonoBehaviour 
{
	private Vector3 targetPosition;
	public float speed = 3.0f;
	public float myTimerRead = 0.3f;
	private float myTimer;

	void Start () 
	{
		myTimer = myTimerRead;
	}
	
	void FixedUpdate () 
	{
		//Decrement timer if above zero
		if(myTimer > 0)
		{
			myTimer -= Time.deltaTime;
		}
		
		//Once timer hits zero set a new random position for fly to aim for, then reset timer
		if(myTimer <= 0)
		{
			targetPosition = new Vector3((Random.value * 8) - 4, (Random.value * 8) - 4, (Random.value * 8) - 4);
			myTimer = myTimerRead;
		}
		
		//Move fly towards new position at specified speed
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
	}
}