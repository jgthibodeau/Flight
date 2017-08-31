using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Control{
	PITCH_UP = "Pitch Up",
	PITCH_DOWN = "Pitch Down",
	ROLL_LEFT = "Roll Left",
	ROLL_RIGHT = "Roll Right",
	WINGS_IN = "Wings In",
	WINGS_OUT = "Wings Out",
	FLAP = "Flap",
	GRAB = "Grab",
	CAMERA_LEFT = "Camera Left",
	CAMERA_RIGHT = "Camera Right",
	CAMERA_BACK = "Camera Back",
	PAUSE = "Pause",

	UI_UP = "UI Up",
	UI_DOWN = "UI Down",
	UI_LEFT = "UI Left",
	UI_RIGHT = "UI Right",

	UI_SELECT = "UI Select",
	UI_CANCEL = "UI Cancel"
}

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

	void AddControl(Control control, string button){
		//TODO
	}
	void AddControl(Control control){
		//TODO
	}
	void RemoveControl(Control control, string button){
		//TODO
	}
	void RemoveAllControls(Control control){
		//TODO
	}
}
