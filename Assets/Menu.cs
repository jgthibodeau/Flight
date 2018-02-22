using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public Menu previous;
	public Selectable firstSelected;

	private void Show() {
		this.gameObject.SetActive (true);
		firstSelected.Select ();
	}

	public void Show(Menu current) {
		previous = current;
		if (previous != null) {
			previous.Hide ();
		}
		Show ();
	}

	public void Back() {
		this.gameObject.SetActive (false);
		if (previous != null) {
			previous.Show ();
		}
	}

	public void Hide(){
		this.gameObject.SetActive (false);
	}
}
