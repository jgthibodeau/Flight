using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour {
	public Stamina stamina;
	
	public Image mainStaminaImg;

	public GameObject extraStamina;
	public GameObject extraStaminaImg;

	private float staminaPerExtraBar;
	public float staminaPerExtraBarPercent;
	public float additionalExtraOffset;

	public int extraStaminaBarCount;
	public List<GameObject> extraStaminaBars;
	
	// Update is called once per frame
	void Update () {
		staminaPerExtraBar = staminaPerExtraBarPercent * stamina.maxStamina;

		mainStaminaImg.fillAmount = stamina.currentStamina / stamina.maxStamina;

		if (stamina.extraStamina > 0) {
			extraStamina.SetActive (true);
			float extraFill = (stamina.extraStamina % staminaPerExtraBar) / staminaPerExtraBar;
			if (extraFill == 0) {
				extraFill = 1;
			}
			extraStaminaImg.GetComponent<Image> ().fillAmount = extraFill;

			CreateExtraStamina ();
		} else {
			extraStamina.SetActive (false);
			CreateExtraStamina ();
		}
	}

	private void CreateExtraStamina () {
		int desiredExtraStaminaBars = Mathf.CeilToInt (stamina.extraStamina / staminaPerExtraBar) - 1;

		//add stamina bars
		if (desiredExtraStaminaBars > extraStaminaBarCount) {
			for (int i = extraStaminaBarCount; i < desiredExtraStaminaBars; i++) {
				GameObject instance = GameObject.Instantiate (extraStaminaImg);
				Image newImg = instance.GetComponent<Image> ();
				newImg.fillAmount = 1;
				newImg.transform.parent = extraStamina.transform;
				Vector3 newPosition = newImg.rectTransform.position;
				newPosition.x = extraStaminaImg.GetComponent<Image> ().rectTransform.position.x + (i + 1) * additionalExtraOffset;
				newPosition.y = extraStaminaImg.GetComponent<Image> ().rectTransform.position.y;
				newImg.rectTransform.position = newPosition;

				extraStaminaBars.Add (instance);
			}
			extraStaminaBarCount = desiredExtraStaminaBars;
		}
		//remove stamina bars
		else if (desiredExtraStaminaBars < extraStaminaBarCount && extraStaminaBarCount > 0) {
			for (int i = extraStaminaBarCount; i > desiredExtraStaminaBars; i--) {
				GameObject oldImg = extraStaminaBars[i-1];
				extraStaminaBars.RemoveAt (i-1);
				GameObject.DestroyImmediate (oldImg);
			}
			extraStaminaBarCount = desiredExtraStaminaBars;
		}
	}
}
