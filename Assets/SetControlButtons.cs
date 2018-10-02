using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//[ExecuteInEditMode]
[RequireComponent(typeof(GamepadButtons))]
public class SetControlButtons : MonoBehaviour {
	public GameObject inputMappingPrefab;
    public GameObject keyboardMenu;
    public Transform keyboardContent;
    public GameObject gamepadMenu;
    public Transform gamepadContent;

    private GamepadButtons gamepadButtons;

	// Use this for initialization
	void Start () {
		gamepadButtons = transform.GetComponent<GamepadButtons> ();
		ResetKeyboard ();
		ResetGamepad ();
	}

	void ConfirmSave() {
	}

	void SaveControls() {
	}

	void ResetKeyboard(){
		Debug.Log ("reseting keyboard");
		int childCount = keyboardContent.childCount;
		for (int i = 0; i < childCount; i++) {
			GameObject.Destroy (keyboardContent.GetChild (0).gameObject);
		}

		//add an instance of inputMappingPrefab for each axis
		TeamUtility.IO.InputConfiguration keyboard = TeamUtility.IO.InputManager.Instance.inputConfigurations[0];

		for (int i=0; i<keyboard.axes.Count; i++) {
			TeamUtility.IO.AxisConfiguration keyboardAxis = keyboard.axes [i];

            if (!keyboardAxis.rebindable)
            {
                continue;
            }

            bool isAxis = keyboardAxis.type == TeamUtility.IO.InputType.AnalogAxis ||
			            keyboardAxis.type == TeamUtility.IO.InputType.DigitalAxis ||
			            keyboardAxis.type == TeamUtility.IO.InputType.AnalogAxis ||
			            keyboardAxis.type == TeamUtility.IO.InputType.RemoteAxis;

			if (isAxis) {
				bool doubleAxis = keyboardAxis.negative.ToString().CompareTo ("None") != 0;

				string[] descriptionTokens = keyboardAxis.description.Split('%');

				InstanceKeyMapping (
                    keyboardContent,
					keyboard.name,
					keyboardAxis.name,
					descriptionTokens [0] + descriptionTokens [1],
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
                        keyboardContent,
						keyboard.name,
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
                    keyboardContent,
					keyboard.name,
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
		int childCount = gamepadContent.childCount;
		for (int i = 0; i < childCount; i++) {
			GameObject.Destroy (gamepadContent.GetChild (0).gameObject);
		}

		//add an instance of inputMappingPrefab for each axis
		TeamUtility.IO.InputConfiguration gamepad = TeamUtility.IO.InputManager.Instance.inputConfigurations[1];

		for (int i=0; i<gamepad.axes.Count; i++) {
			TeamUtility.IO.AxisConfiguration gamepadAxis = gamepad.axes [i];

            if (!gamepadAxis.rebindable)
            {
                continue;
            }

			bool isAxis = gamepadAxis.type == TeamUtility.IO.InputType.AnalogAxis ||
				gamepadAxis.type == TeamUtility.IO.InputType.DigitalAxis ||
				gamepadAxis.type == TeamUtility.IO.InputType.AnalogAxis ||
				gamepadAxis.type == TeamUtility.IO.InputType.RemoteAxis ||
				gamepadAxis.type == TeamUtility.IO.InputType.AnalogButton;

			if (isAxis) {
				InstanceKeyMapping (
                    gamepadContent,
					gamepad.name,
					gamepadAxis.name,
					gamepadAxis.description,
					gamepadButtons.GetMapping (gamepadAxis.axis.ToString ()),
					false,
					null,
					true,
					false,
					true,
					gamepadAxis.invert,
					TeamUtility.IO.Examples.RebindInput.RebindType.GamepadAxis
				);
			} else {
				InstanceKeyMapping (
                    gamepadContent,
					gamepad.name,
					gamepadAxis.name,
					gamepadAxis.description,
					gamepadButtons.GetMapping (gamepadAxis.positive.ToString ()),
					false,
					null,
					true,
					true,
					false,
					gamepadAxis.invert,
					TeamUtility.IO.Examples.RebindInput.RebindType.GamepadButton
				);
			}
		}
	}

	void InstanceKeyMapping(
		Transform parent,
		string configName,
		string axisName,
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
		GameObject inputMapping = GameObject.Instantiate (inputMappingPrefab, parent);
        inputMapping.name = "Input - " + configName + " " + axisName;
		Transform inputMappingTransform = inputMapping.transform;

		Transform descriptionTransform = inputMappingTransform.Find ("Name/Name");
		Transform inputTransform = inputMappingTransform.Find ("Input/Input");
		Transform altTransform = inputMappingTransform.Find ("Alt/Alt");
		Transform invertTransform = inputMappingTransform.Find ("Invert/Invert");

		//names and keys
		descriptionTransform.GetComponentInChildren<TextMeshProUGUI> ().text = descriptionText;
		inputTransform.GetComponentInChildren<TextMeshProUGUI> ().text = inputText;
		altTransform.GetComponentInChildren<TextMeshProUGUI> ().text = altText;

		//invert
		if (allowInvert) {
			invertTransform.GetComponent<Toggle> ().isOn = invert;
		} else {
			invertTransform.gameObject.SetActive (false);
		}

		//input rebind variables
		TeamUtility.IO.Examples.RebindInput inputRebind = inputTransform.GetComponentInChildren <TeamUtility.IO.Examples.RebindInput> ();
		inputRebind._rebindType = rebindType;
		inputRebind._inputConfigName = configName;
		inputRebind._axisConfigName = axisName;
		inputRebind._allowAnalogButton = allowAnalogButton;
		inputRebind._changePositiveKey = positiveButton;
		inputRebind.gamepadButtons = gamepadButtons;
		inputRebind.InitializeAxisConfig ();

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
