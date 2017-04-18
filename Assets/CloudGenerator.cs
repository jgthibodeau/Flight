using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CloudGenerator : MonoBehaviour {
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

	public GameObject[] clouds;
	public bool createClouds = false;
	string cloudTag = "Cloud";

	MeshParticleEmitter originalEmmiter;
	ParticleAnimator originalAnimator;
	ParticleRenderer originalRenderer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (createClouds) {
			GameObject[] oldClouds = GameObject.FindGameObjectsWithTag (cloudTag);
			foreach (GameObject cloud in oldClouds) {
				GameObject.DestroyImmediate (cloud);
			}

			createClouds = false;
			CreateClouds ();
		}
	}

	void CreateClouds() {
		int cloudLayer = LayerMask.NameToLayer ("Cloud");
		//		originalEmmiter = cloudSphere.GetComponent<MeshParticleEmitter> ();
		//		originalAnimator = cloudSphere.GetComponent<ParticleAnimator> ();
		//		originalRenderer = cloudSphere.GetComponent<ParticleRenderer> ();

		int cloudTypes = clouds.Length - 1;
		for(int j = 0; j<numberClouds; j++){
			GameObject newCloud = Instantiate (clouds [Random.Range (0, cloudTypes)]);
			newCloud.tag = cloudTag;
			SetLayerRecursively (newCloud, cloudLayer);
			newCloud.transform.parent = this.transform;
			newCloud.transform.localPosition = new Vector3 (Random.Range (-maxDistance, maxDistance), Random.Range (minHeight, maxHeight), Random.Range (-maxDistance, maxDistance));
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
