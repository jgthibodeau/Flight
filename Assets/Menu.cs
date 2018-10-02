using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public Menu previous;
	public Selectable firstSelected;
    public bool hideOnStart = true;

    void Start()
    {
        //if (hideOnStart)
        //{
        //    Hide();
        //}
    }

	private void Show() {
		this.gameObject.SetActive (true);
        if (firstSelected != null)
        {
            firstSelected.Select();
        }
	}

	public void Show(Menu current) {
        Debug.Log("Showing " + this.gameObject.name);
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

	public void Hide()
    {
        Debug.Log("Hiding " + this.gameObject.name);
        this.gameObject.SetActive (false);
	}
}
