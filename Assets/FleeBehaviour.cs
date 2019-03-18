using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class FleeBehaviour : BaseBehaviour
{
    public float fleeVisionDistance;
    
    public float fleeDistance;

    public LayerMask fleeFrom;
    public LayerMask fleeTo;
    public LayerMask visionIgnore;

    [Task]
    public bool hasTarget_flee;
    public Transform fleeFromTarget;
    public Vector3 fleeFromPosition;

    [Task]
    void GetTarget_Flee()
    {
        fleeFromTarget = GetFleeTarget();
        if (fleeFromTarget != null)
        {
            hasTarget_flee = true;
            fleeFromPosition = fleeFromTarget.position;
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
        
        if (fleeFromTarget != null)
        {
            fleeFromPosition = fleeFromTarget.position;
        }

        Vector3 direction = transform.position - fleeFromPosition;
        Vector3 destination = Vector3.zero;

        //int steps = 2;
        //float step = fleeDistance / steps;
        //for (int i = 0; i <= steps; i++)
        //{
        //    destination = fleeFromPosition + direction.normalized * (fleeDistance * Util.ConvertScale(0, steps, 1, 0, i));

        //    if (behaviourController.GetNavMeshPoint(destination, 2f, out destination))
        //    {
        //        behaviourController.SetDestination(destination);
        //        break;
        //    }
        //}
        destination = transform.position + direction.normalized * 2f;
        if (behaviourController.GetNavMeshPoint(destination, 5f, out destination))
        {
            bool validDirection = true;

            NavMeshHit hit;
            if (NavMesh.Raycast(transform.position, destination, out hit, NavMesh.AllAreas))
            {
                //forward
                destination = transform.position + transform.forward * 2f;
                if (NavMesh.Raycast(transform.position, destination, out hit, NavMesh.AllAreas))
                {
                    //right
                    destination = transform.position + transform.right * 2f;
                    if (NavMesh.Raycast(transform.position, destination, out hit, NavMesh.AllAreas))
                    {
                        //left
                        destination = transform.position - transform.right * 2f;
                        if (NavMesh.Raycast(transform.position, destination, out hit, NavMesh.AllAreas))
                        {
                            //back
                            destination = transform.position - transform.forward * 2f;
                            if (NavMesh.Raycast(transform.position, destination, out hit, NavMesh.AllAreas))
                            {
                                validDirection = false;
                            }
                        }
                    }
                }

                validDirection = behaviourController.GetNavMeshPoint(destination, 5f, out destination);
            }
            
            if (validDirection)
            {
                behaviourController.SetDestination(destination);
            }
        }

        DebugExtension.DebugCircle(destination, Color.yellow, 0.5f);

        //if (behaviourController.GetNavMeshPoint(destination, fleeDistance, out destination))
        //if (behaviourController.RandomNavPoint(destination, 2f, out destination))
        //{
        //    behaviourController.SetDestination(destination);
        //}

        if (Vector3.Distance(transform.position, fleeFromPosition) >= fleeDistance)
        {
            fleeFromTarget = null;
            fleeFromPosition = Vector3.zero;
            hasTarget_flee = false;
        }
        Task.current.Succeed();
    }
}
