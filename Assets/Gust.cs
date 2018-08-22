using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gust : MonoBehaviour {

	public float currentScale;
    public float minScale;
    public float maxScale;
    public float growSpeed;
    public Vector3 moveSpeed;
	public float gustForce;
    public float gustForceRollOff;

	public List<Collider> collidedObjects = new List<Collider>();

	// Use this for initialization
	void Start () {
		currentScale = minScale;
		transform.localScale = new Vector3 (currentScale, currentScale, currentScale);
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 movementDir = transform.forward * moveSpeed.z + transform.right * moveSpeed.x + transform.up * moveSpeed.y;
        transform.position += movementDir * Time.deltaTime;

        currentScale += growSpeed * Time.deltaTime;
        if (currentScale < maxScale)
        {
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        }
        //else
        //{
        //    GameObject.Destroy(gameObject);
        //}
    }

        void OnTriggerEnter(Collider other) {
		if (!collidedObjects.Contains(other)) {
			collidedObjects.Add (other);
//			Rigidbody otherRb = other.GetComponent<Rigidbody> ();
//			otherRb.AddForce ();
		}
	}
}
