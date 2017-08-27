using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class WaterDeformation : MonoBehaviour 
{
	public bool resetMesh = false;

	public static Mesh mesh;            //The water mesh
	public static Transform water;        //The water transform

	public float deformAmount = 1;        //Amount to deform the water
	public float scale = 2.5f;            //How fine the displacement is
	public float speed = 1;                //The speed of waves

	private Vector2 time = Vector2.zero;    //The actual speed offset

	// Use this for initialization
	void Start () 
	{
		//Set the water and mesh variables at start
		GetMesh ();
	}
	void GetMesh () {
		water = transform;
		Mesh originalMesh = GetComponent<MeshFilter>().mesh;

		mesh = new Mesh ();
		mesh.name = gameObject.name;

		List<Vector3> vertices = new List<Vector3> ();
		foreach (Vector3 vert in originalMesh.vertices) {
			vertices.Add (new Vector3(vert.x, vert.y, vert.z));
		}
		mesh.vertices = vertices.ToArray ();

		List<int> triangles = new List<int> ();
		foreach (int tri in originalMesh.triangles) {
			triangles.Add (tri);
		}
		mesh.triangles = triangles.ToArray ();

		List<Vector2> uvs = new List<Vector2> ();
		foreach (Vector2 uv in originalMesh.uv) {
			uvs.Add (new Vector2(uv.x, uv.y));
		}
		mesh.uv = uvs.ToArray ();

//		mesh.colors = originalMesh.colors;

		GetComponent<MeshFilter> ().mesh = mesh;
	}

	// Update is called once per frame
	void Update () 
	{
		if (resetMesh) {
			GetMesh ();
			resetMesh = false;
		}

		time = new Vector2 (Time.time, Time.time) * speed;            //Set up speed offset for deformation

		Vector3[] vertices = mesh.vertices;                //Create a variable for the vertices beforehand

		Vector2 currentTime = time;
		for (int i = 0; i < vertices.Length; i++) {
			vertices[i] = Deform (vertices[i], currentTime);                //For every vertice, deform the Y position
		}

		mesh.vertices = vertices;                      //Re-assign the vertices
		mesh.RecalculateNormals ();                    //Recalculate the normals so the object doesn't look flat
		mesh.RecalculateTangents ();
		GetComponent<MeshFilter>().mesh = mesh;        //Re-assign the mesh to the filter
	}

	Vector3 Deform (Vector3 v, Vector2 currentTime)            //Takes a Vector3
	{
		float xScale = 10*transform.localScale.x;
		float zScale = 10*transform.localScale.z;
		float xPos = transform.position.x;
		float zPos = transform.position.z;

//		v.y = Mathf.PerlinNoise (xPos + xScale * (v.x / scale + currentTime.x), zPos + zScale * (v.z / scale + currentTime.y)) * deformAmount;
		v.y = Mathf.PerlinNoise (xPos + xScale * v.x +10, zPos + zScale * v.z +10) * deformAmount;           //Distort the vertice's Y position based off its X and Z positions + time

		return v;            //Return the offset vertice position
	}
}