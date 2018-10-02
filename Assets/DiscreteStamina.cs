using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteStamina : MonoBehaviour {
	public bool infiniteStamina = false;

	public int currentStamina;
	public int maxStamina;

	public float staminaRegainDelay;
	public float staminaRegainCurrentDelay;

	// Update is called once per frame
	void Update () {
		if (infiniteStamina) {
			return;
		}

		if (staminaRegainCurrentDelay < staminaRegainDelay) {
			staminaRegainCurrentDelay += Time.deltaTime;
		} else {
			RegainStamina ();
		}
	}

	public bool HasStamina () {
		return currentStamina > 0;
	}

	public void UseStamina () {
		if (HasStamina ()) {
			currentStamina -= 1;
			staminaRegainCurrentDelay = 0;
		}
	}

	private void RegainStamina () {
		if (currentStamina < maxStamina) {
			currentStamina += 1;
			staminaRegainCurrentDelay = 0;
		}
    }

    public float GetCurrentStaminaRechargePercent()
    {
        return Mathf.Clamp(staminaRegainCurrentDelay / staminaRegainDelay, 0, 1f);
    }

    public float GetCurrentStaminaPercent()
    {
        return Mathf.Clamp(((float)currentStamina) / maxStamina, 0, 1f);
    }
}
