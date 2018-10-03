using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRigidbodyFollow : MonoBehaviour {
    Rigidbody rb;
    private Vector3 currentVelocity;
    public float velocityRate;
    public Rigidbody parent;
    
    // Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        currentVelocity = parent.velocity;
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

        currentVelocity = Vector3.Slerp(currentVelocity, parent.velocity, velocityRate * Time.deltaTime);
        rb.velocity = currentVelocity;
	}
}
