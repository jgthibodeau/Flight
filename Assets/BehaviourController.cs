using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class BehaviourController : MonoBehaviour
{
    [Task]
    public bool seenPlayer;
    [Task]
    public bool seesPlayer;
    [Task]
    public bool scared;

    public LayerMask navMeshLayers;

    protected NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        DebugExtension.DebugCircle(navMeshAgent.destination, Color.red);
    }

    public void UpdateAgent(BaseBehaviour behaviour)
    {
        navMeshAgent.speed = behaviour.speed;
        navMeshAgent.angularSpeed = behaviour.angularSpeed;
        navMeshAgent.acceleration = behaviour.acceleration;
        //navMeshAgent.stoppingDistance = behaviour.stoppingDistance;
        //navMeshAgent.autoBraking = behaviour.autoBraking;
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    public Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = origin + Random.insideUnitSphere * distance;
        return GetNavMeshPoint(randomDirection, distance);
    }

    public Vector3 GetNavMeshPoint(Vector3 point, float maxDistance)
    {
        NavMeshHit navHit;
        NavMesh.SamplePosition(point, out navHit, maxDistance, navMeshLayers);

        return navHit.position;
    }

    [Task]
    public bool AtDestination()
    {
        return (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) && (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
    }
}
