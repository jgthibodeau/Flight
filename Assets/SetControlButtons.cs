using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class SetControlButtons : MonoBehaviour {
	public GameObject inputMappingPrefab;
	public GameObject keyboardMenu;
	public GameObject gamepadMenu;

	// Use this for initialization
	void Start () {
		ResetKeyboard ();
		ResetGamepad ();
	}

	void ConfirmSave() {
	}

	void SaveControls() {
	}

	void ResetKeyboard(){
		Debug.Log ("reseting keyboard");
		Transform content = keyboardMenu.transform.Find ("Scroll View/Viewport/Content");
		int childCount = content.childCount;
		for (int i = 0; i < childCount; i++) {
			GameObject.Destroy (content.GetChild (0).gameObject);
		}

		//add an instance of inputMappingPrefab for each axis
		TeamUtility.IO.InputConfiguration keyboard = TeamUtility.IO.InputManager.Instance.inputConfigurations[0];

		for (int i=0; i<keyboard.axes.Count; i++) {
			TeamUtility.IO.AxisConfiguration keyboardAxis = keyboard.axes [i];

			bool isAxis = keyboardAxis.type == TeamUtility.IO.InputType.AnalogAxis ||
			            keyboardAxis.type == TeamUtility.IO.InputType.DigitalAxis ||
			            keyboardAxis.type == TeamUtility.IO.InputType.AnalogAxis ||
			            keyboardAxis.type == TeamUtility.IO.InputType.RemoteAxis;

			if (isAxis) {
				bool doubleAxis = keyboardAxis.negative.ToString().CompareTo ("None") != 0;

				string[] descriptionTokens = keyboardAxis.description.Split('%');

				InstanceKeyMapping (
					content,
					keyboardAxis.name,
					descriptionTokens[0] + descriptionTokens[1],
					keyboardAxis.positive.ToString (),
					true,
					keyboardAxis.altPositive.ToString (),
					true,
					true,
					false,
					false,
					TeamUtility.IO.Examples.RebindInput.RebindType.Keyboard
				);

				if (doubleAxis) {
					InstanceKeyMapping (
						content,
						keyboardAxis.name,
						descriptionTokens [0] + descriptionTokens [2],
						keyboardAxis.negative.ToString (),
						true,
						keyboardAxis.altNegative.ToString (),
						false,
						true,
						false,
						false,
						TeamUtility.IO.Examples.RebindInput.RebindType.Keyboard
					);
				}
			} else {
				InstanceKeyMapping (
					content,
					keyboardAxis.name,
					keyboardAxis.description,
					keyboardAxis.positive.ToString (),
					true,
					keyboardAxis.altPositive.ToString (),
					true,
					true,
					false,
					false,
					TeamUtility.IO.Examples.RebindInput.RebindType.Keyboard
				);
			}
		}
	}

	void ResetGamepad(){
		Debug.Log ("reseting gamepad");
		Transform content = gamepadMenu.transform.Find ("Scroll View/Viewport/Content");
		int childCount = content.childCount;
		for (int i = 0; i < childCount; i++) {
			GameObject.Destroy (content.GetChild (0).gameObject);
		}

		//add an instance of inputMappingPrefab for each axis
		TeamUtility.IO.InputConfiguration gamepad = TeamUtility.IO.InputManager.Instance.inputConfigurations[1];

		for (int i=0; i<gamepad.axes.Count; i++) {
			TeamUtility.IO.AxisConfiguration gamepadAxis = gamepad.axes [i];

			bool isAxis = gamepadAxis.type == TeamUtility.IO.InputType.AnalogAxis ||
				gamepadAxis.type == TeamUtility.IO.InputType.DigitalAxis ||
				gamepadAxis.type == TeamUtility.IO.InputType.AnalogAxis ||
				gamepadAxis.type == TeamUtility.IO.InputType.RemoteAxis ||
				gamepadAxis.type == TeamUtility.IO.InputType.AnalogButton;

			if (isAxis) {
				InstanceKeyMapping (
					content,
					gamepadAxis.name,
					gamepadAxis.description,
					TeamUtility.IO.Examples.RebindInput._axisNames [gamepadAxis.axis],
					false,
					gamepadAxis.altPositive.ToString (),
					true,
					false,
					true,
					gamepadAxis.invert,
					TeamUtility.IO.Examples.RebindInput.RebindType.Keyboard
				);
			} else {
				InstanceKeyMapping (
					content,
					gamepadAxis.name,
					gamepadAxis.description,
					gamepadAxis.positive.ToString (),
					false,
					gamepadAxis.altPositive.ToString (),
					true,
					true,
					false,
					gamepadAxis.invert,
					TeamUtility.IO.Examples.RebindInput.RebindType.Keyboard
				);
			}
		}
	}

	void InstanceKeyMapping(
		Transform parent,
		string name,
		string descriptionText,
		string inputText,
		bool allowAlt,
		string altText,
		bool positiveButton,
		bool allowAnalogButton,
		bool allowInvert,
		bool invert,
		TeamUtility.IO.Examples.RebindInput.RebindType rebindType
	){
		GameObject inputMapping = GameObject.Instantiate (inputMappingPrefab);
		Transform inputMappingTransform = inputMapping.transform;
		inputMappingTransform.parent = parent;

		Transform descriptionTransform = inputMappingTransform.Find ("Name/Name");
		Transform inputTransform = inputMappingTransform.Find ("Input/Input");
		Transform altTransform = inputMappingTransform.Find ("Alt/Alt");
		Transform invertTransform = inputMappingTransform.Find ("Invert/Invert");

		//names and keys
		descriptionTransform.GetComponentInChildren<Text> ().text = descriptionText;
		inputTransform.GetComponentInChildren<Text> ().text = inputText;
		altTransform.GetComponentInChildren<Text> ().text = altText;

		//invert
		if (allowInvert) {
			invertTransform.GetComponent<Toggle> ().isOn = invert;
		} else {
			invertTransform.gameObject.SetActive (false);
		}

		//input rebind variables
		TeamUtility.IO.Examples.RebindInput inputRebind = inputTransform.GetComponentInChildren <TeamUtility.IO.Examples.RebindInput> ();
		inputRebind._rebindType = rebindType;
		inputRebind._axisConfigName = name;
		inputRebind._allowAnalogButton = allowAnalogButton;
		inputRebind._changePositiveKey = positiveButton;

		//alt rebind variables
		if (allowAlt) {
			TeamUtility.IO.Examples.RebindInput altRebind = altTransform.GetComponentInChildren <TeamUtility.IO.Examples.RebindInput> ();
			altRebind._rebindType = rebindType;
			altRebind._axisConfigName = name;
			altRebind._allowAnalogButton = allowAnalogButton;
			altRebind._changePositiveKey = positiveButton;
			altRebind._changeAltKey = true;
		} else {
			altTransform.gameObject.SetActive (false);
		}
	}
}
