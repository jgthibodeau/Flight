using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscreteStaminaBar : MonoBehaviour {
	public DiscreteStamina stamina;

	public GameObject staminaImgContainer;

	public float additionalImgOffset;

	public int staminaBarCount;
	public List<GameObject> staminaBars;

	// Update is called once per frame
	void Update () {
		CreateStaminaBars ();
		UpdateStaminaBars ();
	}

	private void UpdateStaminaBars () {
		int i = 0;
		foreach (GameObject go in staminaBars) {
			DiscreteStaminaIcon icon = go.GetComponent<DiscreteStaminaIcon> ();

			if (i < stamina.currentStamina) {
				icon.mainImg.fillAmount = 1f;
			} else if (i == stamina.currentStamina) {
				icon.rechargeImg.fillAmount = stamina.GetCurrentStaminaRechargePercent ();
				icon.mainImg.fillAmount = 0f;
			} else {
				icon.rechargeImg.fillAmount = 0f;
				icon.mainImg.fillAmount = 0f;
			}

			i++;
		}
	}

	private void CreateStaminaBars () {
		int desiredExtraStaminaBars = stamina.maxStamina;

		//add stamina bars
		if (desiredExtraStaminaBars > staminaBarCount) {
			for (int i = staminaBarCount; i < desiredExtraStaminaBars; i++) {
				GameObject instance = GameObject.Instantiate (staminaImgContainer);
				RectTransform newImg = instance.GetComponent<RectTransform> ();
				newImg.transform.parent = this.transform;
				Vector3 newPosition = newImg.position;
				newPosition.x = i * additionalImgOffset;
				newPosition.y = 0;
				newImg.anchoredPosition = newPosition;
				newImg.localScale = Vector3.one;

				staminaBars.Add (instance);
			}
			staminaBarCount = staminaBars.Count;
		}
		//remove stamina bars
		else if (desiredExtraStaminaBars < staminaBarCount && staminaBarCount > 0) {
			for (int i = staminaBarCount; i > desiredExtraStaminaBars; i--) {
				GameObject oldImg = staminaBars[i-1];
				staminaBars.RemoveAt (i-1);
				GameObject.DestroyImmediate (oldImg);
			}
			staminaBarCount = staminaBars.Count;
		}
	}
}
