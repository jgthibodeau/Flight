using UnityEngine;
using System.Collections;

public class MyTerrainGenerator : MonoBehaviour {
	public int numberMountains;
	public float maxMountainDistance;
	public GameObject[] mountains;

	public int numberTrees;
	public float maxTreeDistance;
	public GameObject[] trees;

	public int numberGrasses;
	public float maxGrassDistance;
	public GameObject[] grasses;


	// Use this for initialization
	void Start () {
		TreeGen ();
		MountainGen ();
		GrassGen ();
	}

	// Update is called once per frame
	void Update () {
	}

	void TreeGen(){
		int types = trees.Length - 1;
		for(int j = 0; j<numberTrees; j++){
			GameObject newTree = Instantiate (trees [Random.Range (0, types)]);
			newTree.transform.parent = this.transform;
			newTree.transform.localPosition = new Vector3 (Random.Range (-maxTreeDistance, maxTreeDistance), 0, Random.Range (-maxTreeDistance, maxTreeDistance));
			newTree.transform.Rotate (new Vector3(0, Random.Range (0, 360), 0));
			newTree.isStatic = true;
		}
	}

	void MountainGen(){
		int types = mountains.Length - 1;
		for(int j = 0; j<numberMountains; j++){
			GameObject newMountain = Instantiate (mountains [Random.Range (0, types)]);
			newMountain.transform.parent = this.transform;
			newMountain.transform.localPosition = new Vector3 (Random.Range (-maxMountainDistance, maxMountainDistance), 0, Random.Range (-maxMountainDistance, maxMountainDistance));
			newMountain.transform.Rotate (new Vector3(0, Random.Range (0, 360), 0));
		}
	}

	void GrassGen(){
		int types = grasses.Length - 1;
		for(int j = 0; j<numberGrasses; j++){
			GameObject newGrass = Instantiate (grasses [Random.Range (0, types)]);
			newGrass.transform.parent = this.transform;
			newGrass.transform.localPosition = new Vector3 (Random.Range (-maxGrassDistance, maxGrassDistance), 0, Random.Range (-maxGrassDistance, maxGrassDistance));
			newGrass.transform.Rotate (new Vector3(0, Random.Range (0, 360), 0));
		}
	}
}
