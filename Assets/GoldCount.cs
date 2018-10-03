using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldCount : MonoBehaviour {
    public TextMeshProUGUI text;
    public TreasureHoard treasureHoard;

    bool updating = false;
    int value = 0;

    public float minSize, maxSize;
    public float growRate;
    //public float growRateRate;

    void Start()
    {
        text.SetText(string.Format("{0:n0}", treasureHoard.totalHoardValue));
        text.fontSize = minSize;
        value = treasureHoard.totalHoardValue;
    }

    // Update is called once per frame
    void Update () {
        if (updating)
        {
            return;
        }
        if (treasureHoard.totalHoardValue > value)
        {
            updating = true;
            value = treasureHoard.totalHoardValue;
            StartCoroutine(UpdateValue(treasureHoard.totalHoardValue));
        }
        else if (treasureHoard.totalHoardValue < value)
        {
            updating = true;
            value = treasureHoard.totalHoardValue;
            StartCoroutine(UpdateValue(treasureHoard.totalHoardValue));
        }
    }

    IEnumerator UpdateValue(int value)
    {
        while (text.fontSize < maxSize)
        {
            text.fontSize += growRate * Time.deltaTime;
            if (text.fontSize > maxSize)
            {
                text.fontSize = maxSize;
            }
            yield return null;
        }

        text.SetText(string.Format("{0:n0}", value));

        while (text.fontSize > minSize)
        {
            text.fontSize -= growRate * Time.deltaTime;
            if (text.fontSize < minSize)
            {
                text.fontSize = minSize;
            }
            yield return null;
        }

        updating = false;
    }
}
