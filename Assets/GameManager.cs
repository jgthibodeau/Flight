using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;

	public float oceanLevel;
	public Transform ocean;
	public LayerMask oceanLayer;
	public float oceanCheckDistance;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		oceanLevel = ocean != null ? ocean.position.y : oceanLevel;
	}
}

