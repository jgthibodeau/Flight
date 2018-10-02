using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBar : MonoBehaviour {
	public Image[] images;
	public FlameBreath breath;

	// Update is called once per frame
	void Update () {
        foreach (Image img in images)
        {
            img.fillAmount = breath.Percentage();
        }
	}
}
