using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectSpawner : MonoBehaviour {
    public List<GameObject> spawnableObjects;
    public List<GameObject> spawnedObjects;

    public int preSpawnCount;

    public int maxSpawnedObjects;
    public LayerMask nonSpawnableLayers;

    public float minSpawnRadius;
    public float maxSpawnRadius;
    public float spawnWaitTime;
    public float currentSpawnWaitTime;

    private bool spawning = false;
    
	void Start () {
        currentSpawnWaitTime = spawnWaitTime;

        for (int i=0; i<preSpawnCount; i++) {
            StartCoroutine(Spawn());
        }
	}
	
	void Update ()
    {
        DebugExtension.DebugCircle(transform.position, Color.red, minSpawnRadius);
        DebugExtension.DebugCircle(transform.position, Color.red, maxSpawnRadius);

        TrimDeadObjects();

        if (spawnedObjects.Count < maxSpawnedObjects - 1)
        {
            SpawnObject();
        } else
        {
            currentSpawnWaitTime = spawnWaitTime;
        }
	}

    void SpawnObject()
    {
        if (currentSpawnWaitTime > 0)
        {
            currentSpawnWaitTime -= Time.deltaTime;
        }

        if (currentSpawnWaitTime <= 0 && !spawning)
        {
            spawning = true;
            StartCoroutine(Spawn());
        }
    }

    void TrimDeadObjects()
    {
        spawnedObjects.RemoveAll(go => go == null);
    }

    IEnumerator Spawn()
    {
        Vector3 newPosition;
        do
        {
            Vector2 direction = Random.insideUnitCircle;
            direction = direction.normalized * Util.ConvertScale(0, 1, minSpawnRadius, maxSpawnRadius, direction.magnitude);

            newPosition = new Vector3(transform.position.x + direction.x, transform.position.y, transform.position.z + direction.y);
            yield return null;
        } while (!Util.CanSpawn(newPosition, 1f, 10f, nonSpawnableLayers));

        Vector3 newRot = new Vector3(0, Random.RandomRange(0, 360f), 0);
        GameObject go = GameObject.Instantiate(spawnableObjects[Random.Range(0, spawnableObjects.Count - 1)], newPosition, Quaternion.Euler(newRot));

        currentSpawnWaitTime = spawnWaitTime;
        spawnedObjects.Add(go);
        spawning = false;
    }
}
