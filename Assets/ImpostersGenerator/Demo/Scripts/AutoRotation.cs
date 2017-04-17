using UnityEngine;
using System.Collections;

public class AutoRotation : MonoBehaviour {

    [SerializeField]
    private float speed = 10;
    
    void Update() {
        this.transform.Rotate(0, Time.deltaTime * speed, 0, Space.World);
    }
}
