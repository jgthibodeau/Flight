using UnityEngine;
using System.Collections;

public class ShowPanels : MonoBehaviour {
	public Menu current = null;

	public bool NoMenu() {
		return current == null;
	}

	public void Show(Menu menu) {
		menu.Show (current);
		current = menu;
	}

	public void Back() {
		current.Back ();
		current = current.previous;
	}
}
