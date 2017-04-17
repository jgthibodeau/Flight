using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CreateTreeColliders : MonoBehaviour {
	private string ObjectManager = "ObjectManager";
	private string TreeColliders = "TreeColliders";
	private string Tree = "Tree";
	private int ColliderLayer;
	public float colliderRadius;
	public float colliderHeight;
	public bool createColliders = false;
	public Terrain terrain;

	// Use this for initialization
	void Start () {
//		if (createColliders) {
//			createTrees ();
//		}
	}

	void Update() {
		ColliderLayer = LayerMask.NameToLayer ("ToggleableCollider");
		if (createColliders) {
			createColliders = false;
			destroyTrees ();
			createTrees ();
		}
	}

	void destroyTrees () {
//		foreach (Terrain terrain in GameObject.FindObjectsOfType<Terrain> ()) {
			Transform objectManager = terrain.transform.Find (ObjectManager);
			if (objectManager != null) {
				Debug.Log ("found objectManager");
				Transform treeColliders = objectManager.Find (TreeColliders);
				if (treeColliders != null) {
					Debug.Log ("found treeColliders");
					GameObject.DestroyImmediate (treeColliders.gameObject);
				}
			}
//		}
	}

	void createTrees () {
		Vector3 pos;
		GameObject tree;
		CapsuleCollider ccollider;
		long numberTrees = 0;
//		foreach(Terrain terrain in GameObject.FindObjectsOfType<Terrain> ()){
			Transform objectManager = terrain.transform.Find (ObjectManager);
			if (objectManager == null) {
				objectManager = new GameObject (ObjectManager).transform;
				objectManager.SetParent (terrain.transform);
				objectManager.transform.position = terrain.transform.position;
			}
			Transform treeColliders = objectManager.Find (TreeColliders);
			if (treeColliders == null) {
				treeColliders = new GameObject (TreeColliders).transform;
				treeColliders.SetParent (objectManager);
				treeColliders.transform.position = objectManager.position;
			}

			for (var i=0; i < terrain.terrainData.treeInstances.Length; i++){
				numberTrees++;
				TreeInstance treeInstance = terrain.terrainData.treeInstances [i];
				int treeIndex = treeInstance.prototypeIndex;
				TreePrototype treePrototype = terrain.terrainData.treePrototypes [treeIndex];

				pos = Vector3.Scale(treeInstance.position,terrain.terrainData.size)+terrain.transform.position;
				pos.y += colliderHeight;
				Debug.Log("A tree at world position "+pos+" type: "+ treeIndex);

				tree = new GameObject (Tree);
				tree.transform.position = pos;
				tree.transform.localScale = new Vector3 (treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale);
				tree.tag = Tree;
				tree.layer = ColliderLayer;

				ccollider = tree.AddComponent<CapsuleCollider> ();
				ccollider.center = Vector3.zero;
				ccollider.radius = colliderRadius;
				ccollider.isTrigger = true;

				TreeCollider treeCollider = tree.AddComponent<TreeCollider> ();
				treeCollider.mesh = treePrototype.prefab.GetComponent<MeshFilter> ().sharedMesh;
				treeCollider.index = treeIndex;
				treeCollider.height = colliderHeight;

				tree.transform.SetParent (treeColliders);

				//		treeCollider = Instantiate(treeColliderPrefab, pos, Quaternion.identity);
				//		treeCollider.GetComponent(treeInfo).treeType = Terrain.activeTerrain.terrainData.treeInstances[i].prototypeIndex;
			}
//		}
		Debug.Log ("Trees: "+numberTrees);
	}
}
