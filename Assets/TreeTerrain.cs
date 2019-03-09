using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTerrain : MonoBehaviour {
	private Terrain terrain;
	//public GameObject deadTree;
	//public bool replaceTrees;
    public TreeInstance[] _originalTrees;

    public float treeBillboardDistance;

    public GameObject[] treePrefabs;

    //void Start () {
    //	terrain = GetComponent<Terrain>();
    //       if (terrain.terrainData.treePrototypes.Length != treePrefabs.Length)
    //       {
    //           throw new System.Exception("Tree prefab count does not match terrain tree prototype count");
    //       }

    //       // backup original terrain trees
    //       _originalTrees = terrain.terrainData.treeInstances;

    //       Debug.Log("Instancing " + terrain.terrainData.treeInstanceCount + " trees");
    //	// replace each tree with a collidable tree
    //	for (int i = 0; i < terrain.terrainData.treeInstanceCount; i++) {
    //           //TreeInstance treeInstance = terrain.terrainData.treeInstances[i];
    //           //TreePrototype treePrototype = terrain.terrainData.treePrototypes [treeInstance.prototypeIndex];
    //           //         GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);

    //           //         CapsuleCollider capsuleCollider = capsule.GetComponent<Collider>() as CapsuleCollider;
    //           //float height = treePrototype.prefab.GetComponent<MeshRenderer> ().bounds.size.y * treeInstance.heightScale + 0.5f;
    //           //capsuleCollider.height = height;
    //           //capsuleCollider.center = new Vector3 (0, height / 2 - 0.5f, 0);

    //           //DestroyableTree tree = capsule.AddComponent<DestroyableTree>();
    //           //tree.terrainIndex = i;
    //           //tree.deadTree = deadTree;
    //           //tree.replaceTree = replaceTrees;

    //           //capsule.transform.parent = terrain.transform;
    //           //capsule.transform.localPosition = Vector3.Scale(treeInstance.position, terrain.terrainData.size);
    //           //capsule.tag = "Tree";
    //           //capsule.GetComponent<MeshRenderer> ().enabled = false;

    //           TreeInstance terrainTreeInstance = terrain.terrainData.treeInstances[i];
    //           //TreePrototype treePrototype = terrain.terrainData.treePrototypes[terrainTreeInstance.prototypeIndex];
    //           GameObject treeInst = GameObject.Instantiate(treePrefabs[terrainTreeInstance.prototypeIndex]);


    //           //float height = treePrototype.prefab.GetComponent<MeshRenderer>().bounds.size.y * treeInstance.heightScale + 0.5f;
    //           //capsuleCollider.height = height;
    //           //capsuleCollider.center = new Vector3(0, height / 2 - 0.5f, 0);

    //           treeInst.transform.parent = terrain.transform;
    //           treeInst.transform.localScale *= terrainTreeInstance.heightScale;
    //           //treeInst.transform.localPosition = Vector3.Scale(terrainTreeInstance.position, terrain.terrainData.size) + terrain.transform.position;
    //           treeInst.transform.localPosition = Vector3.Scale(terrainTreeInstance.position, terrain.terrainData.size);

    //           treeInst.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

    //           treeInst.tag = "Tree";
    //           //capsule.GetComponent<MeshRenderer>().enabled = false;
    //       }

    //       terrain.terrainData.treeInstances = new TreeInstance[0];
    //   }

    void Start()
    {
        terrain = GetComponent<Terrain>();

        terrain.treeDistance = treeBillboardDistance;

        // backup original terrain trees
        _originalTrees = terrain.terrainData.treeInstances;

        // create capsule collider for every terrain tree
        for (int i = 0; i < terrain.terrainData.treeInstances.Length; i++)
        {
            TreeInstance treeInstance = terrain.terrainData.treeInstances[i];
            TreePrototype treePrototype = terrain.terrainData.treePrototypes[treeInstance.prototypeIndex];
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            //			GameObject capsule = new GameObject();

            CapsuleCollider capsuleCollider = capsule.GetComponent<Collider>() as CapsuleCollider;
            float height = treePrototype.prefab.GetComponent<MeshRenderer>().bounds.size.y * treeInstance.heightScale + 0.5f;
            capsuleCollider.height = height;
            capsuleCollider.radius = treePrototype.prefab.GetComponent<MeshRenderer>().bounds.size.x * treeInstance.widthScale * 0.5f + 0.5f;
            capsuleCollider.center = new Vector3(0, height / 2 - 0.5f, 0);

            DestroyableTree tree = capsule.AddComponent<DestroyableTree>();
            tree.terrainIndex = i;
            tree.treePrefab = treePrefabs[treeInstance.prototypeIndex];
            tree.collider = capsuleCollider;
            //tree.deadTree = deadTree;
            //tree.replaceTree = replaceTrees;

            capsule.transform.parent = terrain.transform;
            capsule.transform.localPosition = Vector3.Scale(treeInstance.position, terrain.terrainData.size);
            capsule.gameObject.layer = LayerMask.NameToLayer("Tree");
            capsule.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void SetTreeDistance(float newDistance)
    {
        if (terrain != null)
        {
            treeBillboardDistance = newDistance;
            terrain.treeDistance = treeBillboardDistance;
        }
    }

    void OnApplicationQuit() {
        // restore original trees
        if (_originalTrees != null && _originalTrees.Length > 0)
        {
            terrain.terrainData.treeInstances = _originalTrees;
        }
	}
}