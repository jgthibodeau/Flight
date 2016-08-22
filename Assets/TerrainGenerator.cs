using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
	public int numberMountains;
	public float maxMountainDistance;
	public GameObject[] mountains;

	public int numberTrees;
	public float maxTreeDistance;
	public GameObject[] trees;

	// Use this for initialization
	void Start () {
		int treeTypes = trees.Length - 1;
		for(int j = 0; j<numberTrees; j++){
			GameObject newTree = Instantiate (trees [Random.Range (0, treeTypes)]);
			newTree.transform.parent = this.transform;
			newTree.transform.localPosition = new Vector3 (Random.Range (-maxTreeDistance, maxTreeDistance), 0, Random.Range (-maxTreeDistance, maxTreeDistance));
			newTree.transform.Rotate (new Vector3(0, Random.Range (0, 360), 0));
		}
		
		int mountainTypes = mountains.Length - 1;
		for(int j = 0; j<numberMountains; j++){
			GameObject newMountain = Instantiate (mountains [Random.Range (0, mountainTypes)]);
			newMountain.transform.parent = this.transform;
			newMountain.transform.localPosition = new Vector3 (Random.Range (-maxMountainDistance, maxMountainDistance), 0, Random.Range (-maxMountainDistance, maxMountainDistance));
			newMountain.transform.Rotate (new Vector3(0, Random.Range (0, 360), 0));
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
