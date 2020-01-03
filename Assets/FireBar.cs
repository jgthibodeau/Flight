using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBar : MonoBehaviour {
	public Image[] images;
	public FlameBreath breath;
    public bool invert;

	// Update is called once per frame
	void Update () {
        foreach (Image img in images)
        {
            if (invert)
            {
                img.fillAmount = 1 - breath.Percentage();
            } else
            {
                img.fillAmount = breath.Percentage();
            }
        }
	}
}
