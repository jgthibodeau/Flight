using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
	public ParticleSystem ps;
	private bool justInteracted, isInteracting;

	// Use this for initialization
	private int defaultLayer;
	void Start () {
		int layer = LayerMask.NameToLayer ("Interactable");
		defaultLayer = LayerMask.NameToLayer ("Default");
		MoveToLayer (transform, layer);
	}

	protected virtual void Update(){
		if (justInteracted || isInteracting) {
			if (!ps.isPlaying) {
				ps.Play ();
			}
			if (justInteracted) {
				isInteracting = true;
				justInteracted = false;
			} else {
				isInteracting = false;
			}
		} else {
			ps.Stop ();
		}
	}

	void MoveToLayer (Transform root, int layer) {
		if (root.gameObject.GetComponent<Collider> () != null && root.gameObject.layer == defaultLayer) {
			root.gameObject.layer = layer;
		}
		foreach (Transform t in root) {
			MoveToLayer (t, layer);
		}
	}

	public virtual bool IsInteractable (Interactor interactor, ItemHolder itemHolder) {
		return true;
	}

	public void Interact (Interactor interactor, ItemHolder itemHolder, bool started, bool continued) {
		if (IsInteractable (interactor, itemHolder)) {
			justInteracted = true;
			DoInteract (interactor, itemHolder, started, continued);
		}
	}

	public virtual void DoInteract (Interactor interactor, ItemHolder itemHolder, bool started, bool continued) {}
}
