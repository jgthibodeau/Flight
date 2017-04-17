#pragma strict


/*
You will need to assign a prefab with the script treeInfo.js and basic trigger collider
*/

//var treeColliderPrefab : GameObject;

function Start() {
	var pos : Vector3;
	var treeCollider : GameObject;

	for (var i=0; i < Terrain.activeTerrain.terrainData.treeInstances.Length; i++){
		pos = Vector3.Scale(Terrain.activeTerrain.terrainData.treeInstances[i].position,Terrain.activeTerrain.terrainData.size)+ Terrain.activeTerrain.transform.position;
		Debug.Log('A tree at world position '+pos+' type: '+ Terrain.activeTerrain.terrainData.treeInstances[i].prototypeIndex);

//		treeCollider = Instantiate(treeColliderPrefab, pos, Quaternion.identity);
//		treeCollider.GetComponent(treeInfo).treeType = Terrain.activeTerrain.terrainData.treeInstances[i].prototypeIndex;

		//You might want to parent all colliders to something so they dont clutter the scene
		//treeCollider.transform.parent = //sometransform
	}
} 