using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour {
	public GameObject dispensedObject;
	public bool limited = false;
	public int remaining;
	public bool replace = false;
//	[TagSelector]
	public List<string> replacableTags = new List<string> ();

	private int defaultLayer;
	void Awake () {
		int layer = LayerMask.NameToLayer ("Dispenser");
		defaultLayer = LayerMask.NameToLayer ("Default");
		MoveToLayer (transform, layer);
	}

	void MoveToLayer (Transform root, int layer) {
		if (root.gameObject.GetComponent<Collider> () != null && root.gameObject.layer == defaultLayer) {
			root.gameObject.layer = layer;
		}
		foreach (Transform t in root) {
			MoveToLayer (t, layer);
		}
	}

	public void Interact (Interactor interactor, ItemHolder itemHolder) {
		if (!AnyLeft ()) {
			return;
		}

		bool dispense = false;
		Vector3 oldItemPosition = Vector3.zero;
		Quaternion oldItemRotation = Quaternion.identity;
		if (IsReplaceable (itemHolder)) {
			oldItemPosition = itemHolder.heldItem.transform.localPosition;
			oldItemRotation = itemHolder.heldItem.transform.localRotation;
			itemHolder.DestroyItem ();
			dispense = true;
		} else if (IsPickupable (itemHolder)) {
			dispense = true;
		}
		if (dispense) {
			DispenseItem (itemHolder, oldItemPosition, oldItemRotation);
		}
	}

	public bool IsDispensable(Interactor interactor, ItemHolder itemHolder) {
		return AnyLeft() && IsReplaceable (itemHolder) || IsPickupable (itemHolder);
	}

	public bool AnyLeft() {
		return !limited || remaining > 0;
	}
	
	public bool IsReplaceable(ItemHolder itemHolder) {
		return replace && itemHolder.HasItem() && (replacableTags.Count == 0 || replacableTags.Contains (itemHolder.heldItem.gameObject.tag));
	}

	public bool IsPickupable(ItemHolder itemHolder) {
		return !replace && itemHolder.IsEmpty ();
	}

	private void DispenseItem(ItemHolder itemHolder, Vector3 oldItemPosition, Quaternion oldItemRotation) {
		if (dispensedObject != null) {
			GameObject newItem = GameObject.Instantiate (dispensedObject);
			itemHolder.Pickup (newItem.GetComponent<Pickupable> ());
			newItem.transform.localPosition = oldItemPosition;
			newItem.transform.localRotation = oldItemRotation;

			if (limited) {
				remaining--;
			}
		}
	}
}
