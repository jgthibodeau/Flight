using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour {
	public float currentStamina;
	public float maxStamina;
	public float extraStamina;

	public bool usingStamina;
	public float staminaUseRate;

	public bool delayOnlyWhenDepleated;
	public float staminaRegainRate;
	public float staminaRegainDelay;
	public float staminaRegainCurrentDelay;
	
	// Update is called once per frame
	void Update () {
		if (usingStamina && HasStamina()) {
//		if (usingStamina) {
//			if (HasStamina ()) {
				UseStamina ();
				staminaRegainCurrentDelay = staminaRegainDelay;
//			}
		} else if (staminaRegainCurrentDelay <= 0 || (delayOnlyWhenDepleated && HasStamina ())) {
			RegainStamina ();
		} else {
			staminaRegainCurrentDelay -= Time.deltaTime;
		}
	}

	public bool HasStamina (){
		return (currentStamina > 0 || extraStamina > 0);
	}

	private void UseStamina (){
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
}
