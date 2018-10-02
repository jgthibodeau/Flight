using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner: MonoBehaviour {
    public List<GameObject> spawnables;
    public bool inheritVelocity;

    public bool useCoolDown;
    public float coolDown;
    public float currentCoolDown;

    private Rigidbody rb;

    public Transform spawnOrigin;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        if (spawnOrigin == null)
        {
            spawnOrigin = transform;
        }
    }

    void Update()
    {
        if (useCoolDown && currentCoolDown > 0)
        {
            currentCoolDown -= Time.deltaTime;
        }
    }

    public bool CanSpawn()
    {
        return !useCoolDown || currentCoolDown <= 0;
    }

    public bool Spawn(int index)
    {
        if (!CanSpawn())
        {
            return false;
        }

        if (index >= spawnables.Count)
        {
            index = spawnables.Count - 1;
        }
        else if (index < 0)
        {
            index = 0;
        }

        GameObject inst = GameObject.Instantiate(spawnables[index]);
        inst.transform.position = spawnOrigin.position;
        inst.transform.rotation = spawnOrigin.rotation;

        if (inheritVelocity && inst.GetComponent<Rigidbody>() != null && rb != null)
        {
            inst.GetComponent<Rigidbody>().velocity = rb.velocity;
        }

        currentCoolDown = coolDown;

        return true;
    }

    public bool Spawn()
    {
        int index = Random.Range(0, spawnables.Count - 1);
        return Spawn(index);
    }
}
