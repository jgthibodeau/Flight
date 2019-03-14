using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class WanderBehaviour : BaseBehaviour
{
    public float maxWanderDistance;

    public bool stayCloseToAnchor;
    public Transform anchor;
    //public float maxAnchorDistance;

    [Task]
    bool hasDestination_Wander;

    [Task]
    void SetDestination_Wander()
    {
        if (!hasDestination_Wander)
        {
            Debug.Log("determining wander target");
            Vector3 origin = stayCloseToAnchor ? anchor.position : transform.position;
            Vector3 destination = behaviourController.RandomNavSphere(origin, maxWanderDistance);
            Debug.Log(destination);
            behaviourController.SetDestination(destination);
            hasDestination_Wander = true;
        }
        Task.current.Succeed();
    }

    [Task]
    void MoveToDestination_Wander()
    {
        UpdateAgent();
        if (behaviourController.AtDestination())
        {
            hasDestination_Wander = false;
        }
        Task.current.Succeed();
    }
}
