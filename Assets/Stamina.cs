using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour {
	public bool infiniteStamina = false;

	public float currentStamina;
	public float maxStamina;
	public float extraStamina;
	public float maxExtraStamina;

	public bool usingStamina;
	public float staminaUseRate;

	public bool delayOnlyWhenMainDepleted;
	public bool delayOnlyWhenAllDepleted;
	public float staminaRegainRate;
	public float staminaRegainDelay;
	public float staminaRegainCurrentDelay;
	
	// Update is called once per frame
	void Update () {
		if (infiniteStamina) {
			return;
		}

		if (usingStamina && HasStamina()) {
//		if (usingStamina) {
//			if (HasStamina ()) {
				UseStamina ();
				staminaRegainCurrentDelay = staminaRegainDelay;
//			}
		} else if (staminaRegainCurrentDelay <= 0 || (delayOnlyWhenMainDepleted && HasMainStamina ()) || (delayOnlyWhenAllDepleted && HasStamina ())) {
			RegainStamina ();
		} else {
			staminaRegainCurrentDelay -= Time.deltaTime;
		}
	}

	public bool HasStamina () {
		return (currentStamina > 0 || extraStamina > 0);
	}

	public bool HasMainStamina () {
		return currentStamina > 0;
	}

	public bool HasExtraStamina () {
		return extraStamina > 0;
	}

	private void UseStamina () {
		float drainedStamina = staminaUseRate * Time.deltaTime;

		if (drainedStamina > currentStamina) {
			drainedStamina -= currentStamina;
			currentStamina = 0;

			if (drainedStamina > extraStamina) {
				extraStamina = 0;
			} else {
				extraStamina -= drainedStamina;
			}
		} else {
			currentStamina -= drainedStamina;
		}
	}

	private void RegainStamina (){
		float regainedStamina = staminaRegainRate * Time.deltaTime;
		currentStamina += regainedStamina;

		if (currentStamina > maxStamina) {
			currentStamina = maxStamina;
		}
	}

	public void AddStamina (float newStamina){
		extraStamina += newStamina;
		if (extraStamina > maxExtraStamina) {
			extraStamina = maxExtraStamina;
		}
	}
}
