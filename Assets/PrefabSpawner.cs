using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner: MonoBehaviour {
    public List<GameObject> spawnables;
    public float coolDown;
    public float currentCoolDown;

    void Update()
    {
        if (currentCoolDown > 0)
        {
            currentCoolDown -= Time.deltaTime;
        }
    }

    public bool CanSpawn()
    {
        return currentCoolDown <= 0;
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
        inst.transform.position = transform.position;
        inst.transform.rotation = transform.rotation;

        currentCoolDown = coolDown;

        return true;
    }

    public bool Spawn()
    {
        int index = Random.Range(0, spawnables.Count - 1);
        return Spawn(index);
    }
}
