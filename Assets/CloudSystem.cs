using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CloudSystem : MonoBehaviour {
	public int numberClouds;
	public float maxDistance,minHeight,maxHeight;
	public int minCount;
	public int maxCount;
	public float minRadius;
	public float maxRadius;

	public float minScale;
	public float maxScale;

	public bool randomRotation;

	public bool move;
	public Vector3 cloudSpeed;

	public bool setParticleScale;
	public float particleScale = 15f;

	public GameObject[] cloudPrefabs;
	public float[] cloudProbabilities;
	public bool createClouds = false;
	public bool adjustParticles;
	public float maxThickness;
	public float minThickness;
	public int minEmission;
	public int maxEmission;
	public int lifetime;

	string cloudTag = "Cloud";
	string playerTag = "Player";

	public List<Cloud> instancedClouds;
	public Transform player;

	MeshParticleEmitter originalEmmiter;
	ParticleAnimator originalAnimator;
	ParticleRenderer originalRenderer;

	int cloudLayer;

	public int updateChunkScale = 1;
	private int currentChunkIndex = 0;

	// Use this for initialization
	void Start () {
		cloudLayer = LayerMask.NameToLayer ("Cloud");
		//spawn initial clouds
//		instancedClouds = new List<Cloud> (numberClouds);
		//		player = GameObject.FindGameObjectWithTag (playerTag).transform;
	}

	// Update is called once per frame
	void Update () {
		if (createClouds) {
			instancedClouds = new List<Cloud> (numberClouds);
			GameObject[] oldClouds = GameObject.FindGameObjectsWithTag (cloudTag);
			foreach (GameObject cloud in oldClouds) {
				GameObject.DestroyImmediate (cloud);
			}

			createClouds = false;
			CreateClouds ();
		}

		float flipDistance = maxDistance + 10;
		float checkDistance = maxDistance;
		foreach (Cloud cloud in instancedClouds) {
//		for (int i = currentChunkIndex; i < instancedClouds.Count; i += updateChunkScale) {
			//			Cloud cloud = instancedClouds [i];
//			Vector3 realPlayerPos = Util.RigidBodyPosition (player.GetComponent<Rigidbody> ());
//			Vector2 playerPos = new Vector2 (realPlayerPos.x, realPlayerPos.z);
//			Vector2 cloudPos = new Vector2 (cloud.transform.position.x, cloud.transform.position.z);

			if (cloud.lastDistance < checkDistance) {
				cloud.lastDistance += (cloudSpeed + player.GetComponent<Rigidbody> ().velocity).magnitude * Time.deltaTime;
			}

			if (cloud.lastDistance >= checkDistance) {
				cloud.CalculateDistance ();
			}

			//move any clouds that have moved too far away to opposite side
			if (cloud.lastDistance > flipDistance) {
				cloud.Flip ();
			}
		}
//		currentChunkIndex++;
//		if (currentChunkIndex >= updateChunkScale) {
//			currentChunkIndex = 0;
//		}

		//add clouds at edge of players forward in horizontal plane until we are back to full amount
//		for (int i = instancedClouds.Count; i < numberClouds; i++) {
//			CreateCloud ();
//		}
	}

	void CreateClouds() {
		//		originalEmmiter = cloudSphere.GetComponent<MeshParticleEmitter> ();
		//		originalAnimator = cloudSphere.GetComponent<ParticleAnimator> ();
		//		originalRenderer = cloudSphere.GetComponent<ParticleRenderer> ();


		for(int j = 0; j<numberClouds; j++){
			CreateCloud ();
		}
	}

	int GetCloudIndex () {
		float total = 0;
		foreach (float prob in cloudProbabilities) {
			total += prob;
		}
		float randomPoint = Random.value * total;
		for (int i = 0; i < cloudProbabilities.Length; i++) {
			if (randomPoint < cloudProbabilities [i]) {
				return i;
			} else {
				randomPoint -= cloudProbabilities [i];
			}
		}
		return cloudProbabilities.Length - 1;
	}

	void CreateCloud() {
		int cloudTypes = cloudPrefabs.Length;

//		int cloudIndex = Random.Range (0, cloudTypes);
		int cloudIndex = GetCloudIndex ();

		GameObject newCloud = Instantiate (cloudPrefabs [cloudIndex]);
		Transform cloudTransform = newCloud.transform;

		newCloud.tag = cloudTag;
		SetLayerRecursively (newCloud, cloudLayer);
		cloudTransform.parent = this.transform;
		//		Vector3 position = new Vector3 (Random.Range (-maxDistance, maxDistance), Random.Range (minHeight, maxHeight), Random.Range (-maxDistance, maxDistance));
		Vector2 randomPosition = Random.insideUnitCircle * (maxDistance);
		Vector3 position = new Vector3 (randomPosition.x + player.position.x, Random.Range (minHeight, maxHeight), randomPosition.y + player.position.z);
		cloudTransform.position = position;

		if (randomRotation) {
			Vector3 rotation = new Vector3 (0, Random.Range (0f, 360f), 0);
			cloudTransform.rotation = Quaternion.Euler (rotation);
		}

		float scale = Random.Range (minScale, maxScale);
		Vector3 localScale = Vector3.one * scale;

//		newCloud.transform.sc = localScale;

//		foreach (Transform t in cloudTransform) {
			cloudTransform.transform.localScale = localScale;
//		}

		float thickPercent = 1f - (scale - minScale) / (maxScale - minScale);
		float thickness = (thickPercent * (maxThickness - minThickness)) + minThickness;
		foreach (ParticleSystem ps in cloudTransform.GetComponentsInChildren<ParticleSystem> ()) {
			if (adjustParticles) {
				ParticleSystem.MainModule main = ps.main;
				if (setParticleScale) {
					main.startSize = particleScale;
				} else {
					main.startSizeMultiplier = 20*scale;
				}
				main.prewarm = true;
				ParticleSystem.EmissionModule em = ps.emission;
//				em.rateOverTime = Random.Range (minEmission, maxEmission);//(thickPercent * (maxEmission - minEmission)) + minEmission;
//				main.startLifetime = lifetime;
				ParticleSystem.ShapeModule shape = ps.shape;
				shape.meshScale = scale;

//				ps.GetComponent<ParticleSystemRenderer> ().material.SetFloat ("_Thickness", thickness);
				ps.gameObject.SetActive (false);
			}
//			ps.Simulate (0, false, true);
		}

//		if (newCloud.GetComponent<Collider> () != null) {
//			newCloud.GetComponent<Collider> ().isTrigger = true;
//		}

		Cloud cloudScript = newCloud.GetComponent <Cloud> ();
		cloudScript.maxDistance = maxDistance;
		cloudScript.speed = cloudSpeed;
		cloudScript.flipDistance = maxDistance + 10;
		cloudScript.checkDistance = maxDistance - 20;
		cloudScript.lastDistance = maxDistance;
		cloudScript.player = player;
		cloudScript.minScale = minScale;
		cloudScript.maxScale = maxScale;
		cloudScript.scale = scale;

//		newCloud.GetComponent<Renderer> ().material.SetFloat("_MeshRadius", scale);

		instancedClouds.Add (cloudScript);
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