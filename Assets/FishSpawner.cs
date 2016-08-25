using UnityEngine;
using System.Collections;

public class FishSpawner : MonoBehaviour {
	public GameObject fish;
	public float spawnRate;
	public Vector3 position;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("SpawnFish", spawnRate, spawnRate);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void SpawnFish(){
		//pick a random spot and rotation, instantiate fish at that spot
		GameObject newFish = Instantiate (fish);
		fish.transform.position = position;
	}
}
