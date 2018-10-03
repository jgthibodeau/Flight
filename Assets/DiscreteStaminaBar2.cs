using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscreteStaminaBar2 : MonoBehaviour {
    public DiscreteStamina stamina;

    public Image[] staminaImgs;
    public Image[] staminaRechargeImgs;


    // Update is called once per frame
    void Update()
    {
        float regularFill = stamina.GetCurrentStaminaPercent();
        float rechargeFill = regularFill + stamina.GetCurrentStaminaRechargePercent() / stamina.maxStamina;

        //Debug.Log(regularFill + " " + rechargeFill);

        foreach (Image img in staminaImgs)
        {
            img.fillAmount = regularFill;
        }
        foreach (Image img in staminaRechargeImgs)
        {
            img.fillAmount = rechargeFill;
        }
    }
}
