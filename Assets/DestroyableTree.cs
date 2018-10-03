using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTree : MonoBehaviour {
	public int terrainIndex;
    public GameObject treePrefab;
    public Collider collider;
    public float treeLifeCheckTime = 1;

    //public GameObject deadTree;
    //public bool replaceTree;

    public float distanceToRevert;
    public Transform treeDetector;
    
    public bool treeIsActive = false;
    public GameObject treeInstance;
    public Burnable burnable;

	public void Replace(float distance, Transform detector)
    {
        collider.enabled = false;
        treeIsActive = true;
        treeDetector = detector;
        distanceToRevert = distance;
        treeInstance = GameObject.Instantiate(treePrefab, transform.position, Quaternion.identity);
        treeInstance.transform.parent = transform;
        burnable = treeInstance.GetComponentInChildren<Burnable>();
        StartCoroutine(TryToRevert());
    }

    public void Revert()
    {
        treeIsActive = false;
        collider.enabled = true;
        if (treeInstance != null)
        {
            GameObject.Destroy(treeInstance);
        }
    }

    public bool CanDelete()
    {
        return treeInstance == null;
    }

    public bool CanRevert()
    {
        bool distanceOk = Vector3.Distance(transform.position, treeDetector.position) > distanceToRevert;
        return distanceOk && burnable != null && !burnable.onFire;
    }

    IEnumerator TryToRevert()
    {
        yield return new WaitForSeconds(1);
        bool tryingToRevert = true;
        while (tryingToRevert)
        {
            //if far from player and not on fire
            if (CanDelete())
            {
                //Debug.Log("deleting");
                tryingToRevert = false;
                Delete();
            }
            else if (CanRevert())
            {
                //Debug.Log("reverting");
                tryingToRevert = false;
                Revert();
            }
            //Debug.Log("tree lives for another " + treeLifeCheckTime + " seconds");
            yield return new WaitForSeconds(treeLifeCheckTime);
        }
    }

	public void Delete() {
		Terrain terrain = Terrain.activeTerrain;

		List<TreeInstance> treesInstances = new List<TreeInstance>(terrain.terrainData.treeInstances);
		//if (replaceTree) {
		//	TreeInstance treeInstance = treesInstances [terrainIndex];
		//	treeInstance.prototypeIndex = 1;
		//	treesInstances [terrainIndex] = treeInstance;
		//} else {
			treesInstances [terrainIndex] = new TreeInstance ();
		//}


		terrain.terrainData.treeInstances = treesInstances.ToArray();


		//GameObject deadTreeInst = GameObject.Instantiate (deadTree);
		//deadTreeInst.transform.parent = terrain.transform;
		//Vector3 position = transform.position;
		//deadTreeInst.transform.position = position;


		Destroy(gameObject);
	}
}