using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BehaviourController))]
public class BaseBehaviour : MonoBehaviour
{
    protected BehaviourController behaviourController;

    public float speed = 3.5f;
    public float angularSpeed = 120;
    public float acceleration = 8;
    //public float stoppingDistance = 0;
    //public bool autoBraking = false;

    void Start()
    {
        behaviourController = GetComponent<BehaviourController>();
    }

    protected void UpdateAgent()
    {
        behaviourController.UpdateAgent(this);
    }
}
