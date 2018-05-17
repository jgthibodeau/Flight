using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	public Image image;
	public Health health;

	// Update is called once per frame
	void Update () {
		image.fillAmount = health.Percentage ();
	}
}
