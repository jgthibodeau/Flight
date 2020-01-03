using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantPickup : MonoBehaviour
{
    public GameObject collideEffect;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            GameObject.Instantiate(collideEffect, transform.position, Quaternion.identity);

            Kill k = GetComponent<Kill>();
            if (k)
            {
                k.Die();
            }
            else
            {
                GameObject.Destroy(gameObject);
            }

            HandleCollide(collider.gameObject);
        }
    }

    public virtual void HandleCollide(GameObject collidedObject)
    {
        Debug.LogError("HandleCollide not implemented!!!");
    }
}
