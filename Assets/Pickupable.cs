using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Tag {
	public bool removeRigidbody;
	public bool destroyable = true;
	public Vector3 heldLocalRotation;
	public ItemHolder itemHolder;

	public Transform pickupSpot;

	private float slopeLimit, stepOffset, skinWidth, minMoveDistance, radius, height;
	private Vector3 center;

	private Rigidbody originalRb;
	private RigidbodyConstraints originalConstraints;
	private float mass, drag, angularDrag;
	private bool useGravity, isKinematic;
	private RigidbodyInterpolation interpolation;
	private CollisionDetectionMode collisionDetectionMode;
	public Collider[] colliders;

	private bool isRigidbodyObj;

	private float pickupSpeed = 15.0f;
	private float pickupRotateSpeed = 10f;

	private int defaultLayer;
	void Awake () {
		originalRb = GetComponentInParent<Rigidbody> ();
		isRigidbodyObj = (originalRb != null);
		int layer = LayerMask.NameToLayer ("Pickupable");
		defaultLayer = LayerMask.NameToLayer ("Default");
		MoveToLayer (transform, layer);
		if (itemHolder != null) {
			itemHolder.Pickup (this);
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

	void Update() {
		if (IsHeld ()) {
			Freeze ();
		} else {
			UnFreeze ();
		}
	}

	public void Pickup (ItemHolder itemHolder) {
		if (this.itemHolder != null) {
			this.itemHolder.Drop ();
		}

		this.itemHolder = itemHolder;
		transform.parent = itemHolder.heldLocation;

		if (isRigidbodyObj) {
			if (removeRigidbody) {
				Rigidbody rb = GetComponent<Rigidbody> ();
				originalConstraints = rb.constraints;
				mass = rb.mass;
				drag = rb.drag;
				angularDrag = rb.angularDrag;
				useGravity = rb.useGravity;
				isKinematic = rb.isKinematic;
				interpolation = rb.interpolation;
				collisionDetectionMode = rb.collisionDetectionMode;
				Destroy (rb);
			} else {
				originalConstraints = originalRb.constraints;
				originalRb.constraints = RigidbodyConstraints.FreezeAll;
				colliders = GetComponentsInChildren <Collider> ();
				for (int i = 0; i < colliders.Length; i++) {
					Collider c = colliders [i];
					if (c.isTrigger) {
						colliders [i] = null;
					} else {
						c.isTrigger = true;
					}
				}
			}
		} else {
			CharacterController cc = GetComponent<CharacterController> ();
			slopeLimit = cc.slopeLimit;
			stepOffset = cc.stepOffset;
			skinWidth = cc.skinWidth;
			minMoveDistance = cc.minMoveDistance;
			radius = cc.radius;
			height = cc.height;
			center = cc.center;
			Destroy (cc);
		}
	}

	public void Drop() {
		transform.parent = null;
		itemHolder = null;

		if (isRigidbodyObj) {
			if (removeRigidbody) {
				Rigidbody rb = gameObject.AddComponent<Rigidbody> ();
				rb.constraints = originalConstraints;
				rb.mass = mass;
				rb.drag = drag;
				rb.angularDrag = angularDrag;
				rb.useGravity = useGravity;
				rb.isKinematic = isKinematic;
				rb.interpolation = interpolation;
				rb.collisionDetectionMode = collisionDetectionMode;
			} else {
				originalRb.constraints = originalConstraints;
				for (int i = 0; i < colliders.Length; i++) {
					Collider c = colliders [i];
					if (c != null) {
						c.isTrigger = false;
					}
				}
			}
		} else {
			CharacterController cc = gameObject.AddComponent<CharacterController> ();
			cc.slopeLimit = slopeLimit;
			cc.stepOffset = stepOffset;
			cc.skinWidth = skinWidth;
			cc.minMoveDistance = minMoveDistance;
			cc.radius = radius;
			cc.height = height;
			cc.center = center;
		}
	}

	void Freeze() {
		Vector3 desiredLocalPosition = itemHolder.heldLocation.position;
		if (pickupSpot != null) {
			desiredLocalPosition += transform.position - pickupSpot.position;
		}

		if (Vector3.SqrMagnitude (transform.position - desiredLocalPosition) > 0.1f) {
			transform.position = Vector3.Slerp (transform.position, desiredLocalPosition, Time.deltaTime * pickupSpeed);
		} else {
			transform.position = desiredLocalPosition;
		}


		Quaternion desiredRotation = Quaternion.identity;
		if (pickupSpot != null) {
			desiredRotation = pickupSpot.localRotation;
		}

		if (Quaternion.Angle (transform.localRotation, desiredRotation) > 5f) {
			transform.localRotation = Quaternion.Slerp (transform.localRotation, desiredRotation, Time.deltaTime * pickupRotateSpeed);
		} else {
			transform.localRotation = desiredRotation;
		}
	}

	void UnFreeze() {
	}

	public void DestroyItem() {
		Drop ();
		if (destroyable) {
			GameObject.Destroy (gameObject);
		}
	}

	public bool IsHeld() {
		return (itemHolder != null);
	}

	public bool IsPickupable() {
		return itemHolder == null || itemHolder.takable;
	}
}
