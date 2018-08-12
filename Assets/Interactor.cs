using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemHolder))]
public class Interactor : MonoBehaviour {
	public ItemHolder itemHolder;

	bool interacting;

	public LayerMask pickupableLayer;
	public LayerMask interactableLayer;
	public LayerMask dispenserLayer;

	public float halfPickupWidth = 0.25f;
	public float halfPickupHeight = 1;
	public float halfPickupDistance = 1;

	public float halfInteractWidth = 0.25f;
	public float halfInteractHeight = 1;
	public float halfInteractDistance = 1;

	private OutlineController outlineController;

	void Start() {
		itemHolder = GetComponent<ItemHolder> ();
		outlineController = GameObject.Find ("OutlineController").GetComponent<OutlineController> ();
	}

	Dispenser interactableDispenser;
	Pickupable interactablePickupable;
	ItemHolder interactableItemHolder;
	Interactable interactableInteractable;

	void Update() {
		if (itemHolder == null) {
			return;
		}

		GameObject highlighter = null;

		interactableDispenser = null;
		interactablePickupable = null;
		interactableItemHolder = null;
		interactableInteractable = null;

		//check if can directly pick up an item
		if (!itemHolder.HasItem ()) {
			interactablePickupable = FindPickupable ();
			if (interactablePickupable != null) {
				highlighter = interactablePickupable.gameObject;
			}
		}
		//then check if can dispense an item or drop an item
		if (interactablePickupable == null) {
			interactableDispenser = FindDispenser ();
			if (interactableDispenser != null) {
				highlighter = interactableDispenser.gameObject;
			}

			//then check if dropping an item
			else if (itemHolder.HasItem ()) {
				interactableItemHolder = FindItemHolder ();
				if (interactableItemHolder != null) {
					highlighter = interactableItemHolder.gameObject;
				}
			}
		}

		interactableInteractable = FindInteractable ();

//		outlineController.SetRenderers (highlighter);
		outlineController.SetPickupable (highlighter);
		if (interactableInteractable != null) {
			outlineController.SetInteractable (interactableInteractable.gameObject);
		} else {
			outlineController.SetInteractable (null);
		}
		outlineController.UpdateHighlight ();
	}

	public void Pickup() {
		Debug.DrawRay (transform.position, transform.forward * 2 * halfPickupDistance, Color.red);

		if (interactablePickupable != null) {
			Debug.Log ("picking up " + interactablePickupable);
			Pickup (interactablePickupable);
		} else if (interactableDispenser != null) {
			Debug.Log ("dispensing from " + interactableDispenser);
			interactableDispenser.Interact (this, itemHolder);
		} else if (interactableItemHolder != null) {
			Debug.Log ("placing on " + interactableItemHolder);
			interactableItemHolder.Pickup (itemHolder.heldItem);
		} else {
			Drop ();
		}
	}

	public void Drop() {
		Debug.Log ("dropping");
		itemHolder.Drop ();
	}

	public void Pickup(Pickupable pickupable) {
		itemHolder.Pickup (pickupable);
	}

	public bool Interact(bool started, bool continued) {
		if (started || continued) {
			Debug.DrawRay (transform.position - transform.up * 0.1f, transform.forward * 2 * halfInteractDistance, Color.blue);
		}

		if (interactableInteractable != null) {
			Debug.Log ("interacting " + interactableInteractable);
			interactableInteractable.Interact (this, itemHolder, started, continued);
			return true;
		}
		return false;
	}

	private Dispenser FindDispenser() {
		Collider[] colliders = Physics.OverlapBox (transform.position + transform.forward * halfPickupDistance,
			new Vector3(halfPickupWidth, halfPickupHeight, halfPickupDistance), transform.rotation, dispenserLayer);
		Dispenser dispenser = null;
		float currentClosest = Mathf.Infinity;

		//todo map to Pickupable components
		foreach (Collider c in colliders) {
			float distance = (c.transform.position - transform.position).sqrMagnitude;
			Dispenser nextDispenser = c.GetComponentInParent<Dispenser> ();
			if (nextDispenser != null && nextDispenser.IsDispensable (this, itemHolder) && distance < currentClosest) {
				currentClosest = distance;
				dispenser = nextDispenser;
			}
		}
		return dispenser;
	}

	private Pickupable FindPickupable() {
		Collider[] colliders = Physics.OverlapBox (transform.position + transform.forward * halfPickupDistance,
			new Vector3 (halfPickupWidth, halfPickupHeight, halfPickupDistance), transform.rotation, pickupableLayer);
		Pickupable pickupable = null;
		float currentClosest = Mathf.Infinity;

		foreach (Collider c in colliders) {
			float distance = (c.transform.position - transform.position).sqrMagnitude;
			Pickupable nextPickupable = c.GetComponentInParent<Pickupable> ();
			if (nextPickupable != null && nextPickupable.IsPickupable () && distance < currentClosest) {
				currentClosest = distance;
				pickupable = nextPickupable;
			}
		}

		return pickupable;
	}

	private ItemHolder FindItemHolder() {
		Collider[] colliders = Physics.OverlapBox (transform.position + transform.forward * halfPickupDistance,
			new Vector3 (halfPickupWidth, halfPickupHeight, halfPickupDistance), transform.rotation);
		ItemHolder holder = null;
		float currentClosest = Mathf.Infinity;

		foreach (Collider c in colliders) {
			ItemHolder[] itemHolders = c.GetComponentsInParent<ItemHolder> ();
			foreach (ItemHolder nextItemHolder in itemHolders) {
				float distance = (nextItemHolder.heldLocation.position - transform.position).sqrMagnitude;
				if (nextItemHolder != null && distance < currentClosest && !nextItemHolder.HasItem ()) {
					currentClosest = distance;
					holder = nextItemHolder;
				}
			}
		}

		return holder;
	}

	private Interactable FindInteractable() {
		Collider[] colliders = Physics.OverlapBox (transform.position + transform.forward * halfInteractDistance,
			new Vector3 (halfInteractWidth, halfInteractHeight, halfInteractDistance), transform.rotation, interactableLayer);
		Interactable interactable = null;
		float currentClosest = Mathf.Infinity;

		foreach (Collider c in colliders) {
			float distance = (c.transform.position - transform.position).sqrMagnitude;
			Interactable nextInteractable = c.GetComponentInParent<Interactable> ();
			if (nextInteractable != null && distance < currentClosest && nextInteractable.IsInteractable (this, itemHolder)) {
				currentClosest = distance;
				interactable = nextInteractable;
			}
		}

		return interactable;
	}
}
