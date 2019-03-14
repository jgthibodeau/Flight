using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class FleeBehaviour : BaseBehaviour
{
    public float fleeVisionDistance;
    
    public float fleeDistance;

    public LayerMask fleeFrom;
    public LayerMask fleeTo;
    public LayerMask visionIgnore;

    [Task]
    bool hasTarget_flee;
    private Vector3 fleeFromTarget;

    [Task]
    void GetTarget_Flee()
    {
        Transform target = GetFleeTarget();
        if (target != null)
        {
            hasTarget_flee = true;
            fleeFromTarget = target.position;
        }
        Task.current.Succeed();
    }

    private Transform GetFleeTarget()
    {
        DebugExtension.DebugWireSphere(transform.position, Color.yellow, fleeVisionDistance);
        Collider[] closeColliders = Physics.OverlapSphere(transform.position, fleeVisionDistance, fleeFrom);

        Transform target = null;
        float distance = Mathf.Infinity;
        foreach (Collider c in closeColliders)
        {
            Vector3 point = c.ClosestPoint(transform.position);
            if (!Physics.Linecast(transform.position, point, ~visionIgnore))
            {
                float newDistance = Vector3.SqrMagnitude(point - transform.position);
                if (newDistance < distance)
                {
                    target = c.transform;
                }
            }
        }
        return target;
    }

    [Task]
    void Flee()
    {
        UpdateAgent();

        Transform newTarget = GetFleeTarget();
        if (newTarget != null)
        {
            fleeFromTarget = newTarget.position;
        }

        Vector3 direction = transform.position - fleeFromTarget;
        Vector3 destination = fleeFromTarget + direction.normalized * fleeDistance;
        destination = behaviourController.GetNavMeshPoint(destination, fleeDistance);

        behaviourController.SetDestination(destination);

        if (behaviourController.AtDestination())
        {
            hasTarget_flee = false;
        }
        Task.current.Succeed();
    }
}
