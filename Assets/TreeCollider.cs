	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(CapsuleCollider))]
public class TreeCollider : MonoBehaviour {
	CapsuleCollider originalCollider;
	MeshFilter meshFilter;
	GameObject collider;

	public int index;
	public Mesh mesh;
	public float height;

	bool treeEnabled = false;

	// Use this for initialization
//	void Start () {
//		meshFilter = GetComponent<MeshFilter> ();
//		mesh = meshFilter.mesh;
//		originalCollider = GetComponent<CapsuleCollider> ();
//	}

////	void OnCollisionEnter(Collision other){
//	public void enableTreeCollider () {
////		if (other.transform.tag == "Player") {
//			//instance an object with meshCollider
//			collider = new GameObject();
//			collider.transform.parent = this.transform;
//			collider.transform.position = Vector3.zero;
//			collider.transform.rotation = Quaternion.Euler (Vector3.zero);
//
//			MeshCollider meshCollider = collider.AddComponent<MeshCollider> ();
//			meshCollider.sharedMesh = meshFilter.mesh;
//			meshCollider.convex = true;
//			originalCollider.enabled = false;
////		}
//	}
//
////	void OnCollisionExit(Collision other){
//	public void disableTreeCollider () {
////		if (other.transform.tag == "Player") {
//			//remove the object with meshCollider
//			GameObject.Destroy (collider);
//			originalCollider.enabled = true;
////		}
//	}
}
