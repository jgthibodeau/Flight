using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : Tag {
	public Pickupable heldItem;
	public Transform heldLocation;
	public bool takable = true;

	public bool IsEmpty() {
		return (heldItem == null);
	}

	public bool HasItem() {
		return (heldItem != null);
	}

	public void Pickup(Pickupable item){
		item.Pickup (this);

		this.heldItem = item;
	}

	public void Drop() {
		if (heldItem != null) {
			heldItem.Drop ();

			this.heldItem = null;
		}
	}

	public void DestroyItem() {
		if (heldItem != null) {
			heldItem.DestroyItem ();
			this.heldItem = null;
		}
	}
}
