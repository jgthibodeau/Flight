using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    public bool useStaticRotation;
    public Vector3 staticRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (useStaticRotation)
        {
            transform.eulerAngles = staticRotation;
        }
    }
}
