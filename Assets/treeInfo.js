#pragma strict

var treeType : int; //Couolp be used to lookup a static array of tree types

function OnTriggerEnter(col : Collider){
	Debug.LogError('The tree here is of type '+treeType);    
}