using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBar : MonoBehaviour {
	public Image image;
	public FlameBreath breath;

	// Update is called once per frame
	void Update () {
		image.fillAmount = breath.Percentage ();
	}
}
