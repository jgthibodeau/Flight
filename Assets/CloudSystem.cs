using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CloudSystem : MonoBehaviour {
//	public GameObject cloud;
//	public GameObject cloudSphere;

	public int numberClouds;
	public float maxDistance,minHeight,maxHeight;
	public int minCount;
	public int maxCount;
	public float minRadius;
	public float maxRadius;

	public float minScale;
	public float maxScale;

	public Vector3 cloudSpeed;

	public GameObject[] clouds;
	public bool createClouds = false;
	string cloudTag = "Cloud";
	string playerTag = "Player";

	public List<GameObject> instancedClouds;
	public Transform player;

	MeshParticleEmitter originalEmmiter;
	ParticleAnimator originalAnimator;
	ParticleRenderer originalRenderer;

	int cloudLayer;

	// Use this for initialization
	void Start () {
		cloudLayer = LayerMask.NameToLayer ("Cloud");
		//spawn initial clouds
		instancedClouds = new List<GameObject> ();
//		player = GameObject.FindGameObjectWithTag (playerTag).transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (createClouds) {
			instancedClouds = new List<GameObject> ();
			GameObject[] oldClouds = GameObject.FindGameObjectsWithTag (cloudTag);
			foreach (GameObject cloud in oldClouds) {
				GameObject.DestroyImmediate (cloud);
			}

			createClouds = false;
			CreateClouds ();
		}

		//iterate over instanced clouds
		List<GameObject> removableClouds = new List<GameObject> ();
		float flipDistance = maxDistance + 10;
		foreach (GameObject cloud in instancedClouds) {
			Vector2 playerPos = new Vector2 (player.transform.position.x, player.transform.position.z);
			Vector2 cloudPos = new Vector2 (cloud.transform.position.x, cloud.transform.position.z);

			//move any clouds that have moved too far away to opposite side
			if (Vector2.Distance (playerPos, cloudPos) > flipDistance) {
				Vector3 newCloudPos = cloud.transform.position;
				float xDif = 2 * Mathf.Abs (playerPos.x - cloudPos.x);
				if (cloudPos.x > playerPos.x) {
					xDif *= -1;
				}
				newCloudPos.x += xDif;
				cloudPos.x = newCloudPos.x;

				float yDif = 2 * Mathf.Abs (playerPos.y - cloudPos.y);
				if (cloudPos.y > playerPos.y) {
					yDif *= -1;
				}
				newCloudPos.z += yDif;
				cloudPos.y = newCloudPos.z;

				//as long as the new position doesn't put us too far again, set it
				if (Vector2.Distance (playerPos, cloudPos) <= flipDistance) {
					cloud.transform.position = newCloudPos;
				}
			}
			//move cloud
			gameObject.transform.position += cloudSpeed * Time.deltaTime;
		}
//		foreach (GameObject cloud in removableClouds) {
//			instancedClouds.Remove (cloud);
//			GameObject.DestroyImmediate (cloud);
//		}

		//add clouds at edge of players forward in horizontal plane until we are back to full amount
		for (int i = instancedClouds.Count; i < numberClouds; i++) {
			CreateCloud ();
		}
	}

	void CreateClouds() {
		//		originalEmmiter = cloudSphere.GetComponent<MeshParticleEmitter> ();
		//		originalAnimator = cloudSphere.GetComponent<ParticleAnimator> ();
		//		originalRenderer = cloudSphere.GetComponent<ParticleRenderer> ();


		for(int j = 0; j<numberClouds; j++){
			CreateCloud ();
		}
	}

	void CreateCloud() {
		int cloudTypes = clouds.Length - 1;
		GameObject newCloud = Instantiate (clouds [Random.Range (0, cloudTypes)]);
		instancedClouds.Add (newCloud);

		newCloud.tag = cloudTag;
		SetLayerRecursively (newCloud, cloudLayer);
		newCloud.transform.parent = this.transform;
//		Vector3 position = new Vector3 (Random.Range (-maxDistance, maxDistance), Random.Range (minHeight, maxHeight), Random.Range (-maxDistance, maxDistance));
		Vector2 randomPosition = Random.insideUnitCircle * (maxDistance-1f);
		Vector3 position = new Vector3 (randomPosition.x + player.position.x, Random.Range (minHeight, maxHeight), randomPosition.y + player.position.z);

		newCloud.transform.position = position;
		Vector3 scale = Vector3.one * Random.Range (minScale, maxScale);
		newCloud.transform.localScale = scale;

		foreach (ParticleSystem ps in newCloud.transform.GetComponentsInChildren<ParticleSystem> ()) {
			//				ps.main.startSize = scale.magnitude;
			ps.transform.localScale = scale;
		}

		if (newCloud.GetComponent<Collider> () != null) {
			newCloud.GetComponent<Collider> ().isTrigger = true;
		}
		//			SetParticles (newCloud);

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

	void SetLayerRecursively(GameObject obj, int newLayer){
		if (null == obj){
			return;
		}

		obj.layer = newLayer;

		foreach (Transform child in obj.transform){
			if (null == child){
				continue;
			}
			SetLayerRecursively(child.gameObject, newLayer);
		}
	}

	void SetParticles(GameObject go){
		foreach (Transform transform in go.transform) {
			GameObject child = transform.gameObject;
			MeshParticleEmitter emmiter = child.AddComponent<MeshParticleEmitter> ();
			emmiter.minSize = originalEmmiter.minSize;
			emmiter.maxSize = originalEmmiter.maxSize;
			emmiter.minEnergy = originalEmmiter.minEnergy;
			emmiter.maxEnergy = originalEmmiter.maxEnergy;
			emmiter.minEmission = originalEmmiter.minEmission;
			emmiter.maxEmission = originalEmmiter.maxEmission;

			ParticleAnimator animator = child.AddComponent<ParticleAnimator> ();

			ParticleRenderer renderer = child.AddComponent<ParticleRenderer> ();
			renderer.materials = originalRenderer.materials;
			renderer.maxParticleSize = originalRenderer.maxParticleSize;

			child.GetComponent<MeshRenderer> ().enabled = false;
		}
	}
}