using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edible : MonoBehaviour {
	public float staminaRegain;
	public bool eaten;
	public float deathTime;
	public GameObject deathObject;

	public float Eat() {
		if (!eaten) {
			eaten = true;
			StartCoroutine (Die ());

			return staminaRegain;
		} else {
			return 0f;
		}
	}

	IEnumerator Die() {
		foreach(Renderer renderer in gameObject.GetComponentsInChildren<Renderer> ()) {
			renderer.enabled = false;
		}
		foreach(Collider collider in gameObject.GetComponentsInChildren<Collider> ()) {
			collider.enabled = false;
		}
		foreach(CharacterController controller in gameObject.GetComponentsInChildren<CharacterController> ()) {
			controller.enabled = false;
		}

		GameObject newObj = GameObject.Instantiate (deathObject);
		newObj.transform.parent = this.transform;
		newObj.transform.localPosition = Vector3.zero;

		yield return new WaitForSecondsRealtime(deathTime);
		GameObject.Destroy (this.gameObject);
	}
}
