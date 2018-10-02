using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDetector : MonoBehaviour {
	//Dictionary<TreeCollider, GameObject> enabledTrees = new Dictionary<TreeCollider, GameObject> ();
	Collider[] colliders;

    public int updateInterval = 3;
    public float radius;
    public float revertRadius;
    //public bool useTreeMesh;
    //Mesh[] meshes;
    public LayerMask treeMask;

	// Use this for initialization
	void Start () {
        if (revertRadius < radius)
        {
            revertRadius = radius;
        }
	}

    void Update ()
    {
        DebugExtension.DebugWireSphere(transform.position, Color.green, radius);
        if (Time.frameCount % updateInterval == 0)
        {
            foreach (Collider c in Physics.OverlapSphere(transform.position, radius, treeMask))
            {
                //Debug.Log("collided with tree" + c);
                DestroyableTree tree = c.GetComponent<DestroyableTree>();
                if (tree != null && !tree.treeIsActive)
                {
                    //Debug.Log("activating tree");
                    tree.Replace(revertRadius, this.transform);
                }
            }
        }
    }
	
//	// Update is called once per frame
//	void FixedUpdate () {
//		List<TreeCollider> disableableTrees = new List<TreeCollider> ();
//		foreach (TreeCollider t in enabledTrees.Keys) {
//			disableableTrees.Add (t);
//		}

////		foreach (RaycastHit hit in Physics.SphereCastAll (transform.position - transform.forward*0.1f, 1f, transform.forward*0.2f, radius, treeMask)) {
//		foreach (Collider hit in Physics.OverlapSphere (transform.position, radius, treeMask)) {
//			if (hit.transform.tag == "Tree") {
////				Debug.Log ("hit tree");
//				TreeCollider tree = hit.transform.GetComponent<TreeCollider> ();

//				if (!enabledTrees.ContainsKey (tree)) {
//					//make a new gameObject with a meshCollider
//					GameObject newTreeCollider = new GameObject ();

//					Vector3 colliderPosition = tree.transform.position;
//					colliderPosition.y -= tree.height;

//					newTreeCollider.transform.position = colliderPosition;
//					newTreeCollider.transform.rotation = tree.transform.rotation;
//					newTreeCollider.transform.localScale = tree.transform.localScale;
//					newTreeCollider.transform.SetParent (tree.transform);
//					newTreeCollider.layer = Perchable;

//					MeshCollider meshCollider = newTreeCollider.AddComponent<MeshCollider> ();
//					if (useTreeMesh) {
//						meshCollider.sharedMesh = tree.mesh;
//					} else {
//						meshCollider.sharedMesh = meshes [tree.index];
//					}

//					enabledTrees [tree] = newTreeCollider;
//				}

//				//remove chunk from the list of unloadable chunks
//				if (disableableTrees.Contains (tree)) {
//					disableableTrees.Remove (tree);
//				}
//			}
//		}

//		//disable all chunks in the list of unloadable chunks and remove them from the loaded chunks list
//		foreach (TreeCollider tree in disableableTrees) {
//			GameObject.Destroy (enabledTrees[tree]);
//			enabledTrees.Remove (tree);
//		}
//	}

//	void OnCollisionEnter (Collision other) {
//		Debug.Log ("collided");
//		if (other.transform.tag == "Tree") {
//			Debug.Log ("tree");
//			TreeCollider treeCollider = other.transform.GetComponent<TreeCollider> ();
//			treeCollider.enableTreeCollider ();
//
//			CapsuleCollider ccollider = other.transform.GetComponent<CapsuleCollider> ();
//			ccollider.isTrigger = true;
//
//			foreach (Collider c in colliders) {
//				Physics.IgnoreCollision (ccollider, c);
//			}
//		}
//	}
//
//	void OnCollisionExit(Collision other) {
//		Debug.Log ("uncollided");
//		if (other.transform.tag == "Tree") {
//			Debug.Log ("tree");
//			TreeCollider treeCollider = other.transform.GetComponent<TreeCollider> ();
//			treeCollider.disableTreeCollider ();
//
//			CapsuleCollider ccollider = other.transform.GetComponent<CapsuleCollider> ();
//			ccollider.isTrigger = true;
//		}
//	}
}
