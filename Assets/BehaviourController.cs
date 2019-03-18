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
    void SetSeenPlayer(bool seenPlayer)
    {
        this.seenPlayer = seenPlayer;
        Task.current.Succeed();
    }
    [Task]
    public bool seesPlayer;
    [Task]
    void SetSeesPlayer(bool seesPlayer)
    {
        this.seesPlayer = seesPlayer;
        Task.current.Succeed();
    }
    [Task]
    public bool scared;
    [Task]
    void SetScared(bool scared)
    {
        this.scared = scared;
        Task.current.Succeed();
    }

    private NavMeshAgent navMeshAgent;
    private Rigidbody rigidbody;
    private Animator animator;
    private Animation animation;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        animation = GetComponentInChildren<Animation>();
    }

    void Update()
    {
        DebugExtension.DebugCircle(navMeshAgent.destination, Color.red);
    }

    public void Reset()
    {
        Vector3 position;
        GetNavMeshPoint(transform.position, 10f, out position);
        //transform.position = GetNavMeshPoint(transform.position + Vector3.up, 5f);

        transform.LookAt(position + transform.forward, Vector3.up);
        navMeshAgent.Warp(position);
        navMeshAgent.nextPosition = position;
        navMeshAgent.SetDestination(transform.position);
    }

    public void EnableGravity()
    {
        Stop();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = false;
        }
    }

    public void UpdateAgent(BaseBehaviour behaviour)
    {
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }

        //navMeshAgent.enabled = true;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;

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

    public bool RandomNavPoint(Vector3 center, float range, out Vector3 result)
    {
        float height = 2 * navMeshAgent.height;
        NavMeshPath path = new NavMeshPath();
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, height, NavMesh.AllAreas))
            {
                navMeshAgent.CalculatePath(hit.position, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    result = hit.position;
                    return true;
                }
            }
        }
        result = Vector3.zero;
        return false;
    }

    public bool GetNavMeshPoint(Vector3 point, out Vector3 result)
    {
        float height = 2 * navMeshAgent.height;
        NavMeshHit hit;
        bool success = NavMesh.SamplePosition(point, out hit, height, NavMesh.AllAreas);

        result = hit.position;
        return success;
    }

    public bool GetNavMeshPoint(Vector3 point, float maxDistance, out Vector3 result)
    {
        NavMeshHit hit;
        bool success = NavMesh.SamplePosition(point, out hit, maxDistance, NavMesh.AllAreas);

        result = hit.position;
        return success;
    }

    [Task]
    public void Stop()
    {
        //navMeshAgent.enabled = false;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        Task.current.Succeed();
    }

    [Task]
    public bool AtDestination()
    {
        return (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) && (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
    }

    [Task]
    protected void SetAnimationTrigger(string name)
    {
        animator.SetTrigger(name);
        Task.current.Succeed();
    }

    [Task]
    protected void SetAnimationBool(string name, bool value)
    {
        animator.SetBool(name, value);
        Task.current.Succeed();
    }

    [Task]
    protected void SetAnimationFloat(string name, float value)
    {
        animator.SetFloat(name, value);
        Task.current.Succeed();
    }

    [Task]
    protected void SetAnimationMoveSpeed()
    {
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        Task.current.Succeed();
    }

    [Task]
    protected void PlayAnimation(string animationName)
    {
        //animation.CrossFade(animationName);
        Task.current.Succeed();
    }
}
