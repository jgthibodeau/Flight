using UnityEngine;
using System.Collections;

public class CloudGenerator : MonoBehaviour {
	public GameObject cloud;
	public GameObject cloudSphere;
	public int numberClouds;
	public float maxDistance;
	public int minCount;
	public int maxCount;
	public float minRadius;
	public float maxRadius;

	public GameObject[] clouds;

	// Use this for initialization
	void Start () {
		int cloudTypes = clouds.Length - 1;
		for(int j = 0; j<numberClouds; j++){
			GameObject newCloud = Instantiate (clouds [Random.Range (0, cloudTypes)]);
			newCloud.transform.parent = this.transform;
			newCloud.transform.localPosition = new Vector3 (Random.Range (-maxDistance, maxDistance), Random.Range (-maxDistance, maxDistance), Random.Range (-maxDistance, maxDistance));
				
//			GameObject newCloud = Instantiate (cloud);
//			newCloud.transform.parent = this.transform;
//			newCloud.transform.localPosition = new Vector3 (Random.Range (-maxDistance, maxDistance), Random.Range (-maxDistance, maxDistance), Random.Range (-maxDistance, maxDistance));
//
//			int count = Random.Range (minCount, maxCount);
//			for (int i = 0; i < count; i++) {
//				GameObject newCloudSphere = Instantiate (cloudSphere);
//				newCloudSphere.transform.parent = newCloud.transform;
//
//				float scale = Random.Range (minRadius, maxRadius);
//				newCloudSphere.transform.localScale = new Vector3(scale, scale, scale);
//
//				float distance = Mathf.Sqrt (count)/4;
//				newCloudSphere.transform.localPosition = new Vector3 (Random.Range (-distance, distance), Random.Range (-distance, distance), Random.Range (-distance, distance));
//			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
