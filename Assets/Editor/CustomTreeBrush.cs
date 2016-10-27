using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CustomTreeBrush : EditorWindow
{
	private bool randomRotation = true;
	private bool randomScale = false;
	private float scaleMinValue = 0.9f;
	private float scaleMaxValue = 1.1f;

	[MenuItem("Tools/Custom/Terrain")]
	static void Init()
	{
		CustomTreeBrush window = (CustomTreeBrush)GetWindow(typeof(CustomTreeBrush));
	}

	void OnGUI()
	{
		if (GUILayout.Button("Convert to objects"))
		{
			Convert();
		}
		//  if(GUILayout.Button("Debug"))
		//  {
		//  }
	}

	public void Convert()
	{
		TerrainData terrain = Terrain.activeTerrain.terrainData;
		TreeInstance[] treeInstances = terrain.treeInstances;
		List<TreeInstance> TreeInstances = new List<TreeInstance>();
		float treeRot = 0;
		GameObject go = new GameObject("Trees");
		GameObject[] gos = new GameObject[terrain.treePrototypes.Length];

		for (int i = 0; i < terrain.treePrototypes.Length; i++)
		{
			gos[i] = new GameObject(terrain.treePrototypes[i].prefab.name);
			gos[i].transform.parent = go.transform;
		}

		for (int i = 0; i < treeInstances.Length; i++)
		{
			TreeInstance myTree = treeInstances[i];
			Vector3 treePos = new Vector3(myTree.position.x * terrain.size.x, myTree.position.y * terrain.size.y, myTree.position.z * terrain.size.z);

			if (randomRotation)
				treeRot = UnityEngine.Random.Range(0.0f, 360.0f);

			GameObject tempTree = PrefabUtility.InstantiatePrefab(terrain.treePrototypes[myTree.prototypeIndex].prefab) as GameObject;
			Transform nt = tempTree.transform;
			nt.rotation = Quaternion.Euler(new Vector3(0, treeRot, 0));
			nt.position = treePos;

			if (randomScale)
				tempTree.transform.localScale *= UnityEngine.Random.Range(scaleMinValue, scaleMaxValue);

			tempTree.transform.parent = gos[myTree.prototypeIndex].transform;
		}
		terrain.treeInstances = TreeInstances.ToArray();
	}

}