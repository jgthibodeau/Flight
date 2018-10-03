using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyDebug : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
        Debug.Log("OnCollisionEnter");
        Debug.Log(collision.gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
        Debug.Log("OnCollisionStay");
        Debug.Log(collision.gameObject);
    }
}
