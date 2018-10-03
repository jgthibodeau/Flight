using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureHoard : MonoBehaviour {
    public int totalHoardValue = 0;

    void OnTriggerEnter(Collider other)
    {
        Treasure treasure = other.GetComponent<Treasure>();
        if (treasure != null)
        {
            totalHoardValue += treasure.value;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Treasure treasure = other.GetComponent<Treasure>();
        if (treasure != null)
        {
            totalHoardValue -= treasure.value;
        }
    }
}
