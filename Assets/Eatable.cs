using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eatable : MonoBehaviour {
    public float healAmount;
    public GameObject eatEffects;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponentInChildren<Health>();
            Eat(health, transform.position);
        }
    }

    public void Eat(Health health, Vector3 position)
    {
        health.Heal(healAmount);
        GameObject.Instantiate(eatEffects, position, Quaternion.identity);

        Kill k = GetComponent<Kill>();
        if (k != null)
        {
            k.Die();
        } else
        {
            GameObject.Destroy(gameObject);
        }
    }
}
