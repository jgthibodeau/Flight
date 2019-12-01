using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public float fireRate;
    public float timeSinceLastFire = 0;
    public float minRange, maxRange;
    public float minAngle = 30;
    public float minHeightOffset, maxHeightOffset;
    public float minSpeed, maxSpeed;
    public bool leadTarget;
    [Range(0, 1)]
    public float accuracyGainRate; //gain this amount of accuracy for every shot
    public float currentAccuracy; //reset to 0 on a successful hit
    public float minRandomness, maxRandomness;
    public bool useLateralVelocity;
    public bool useShorterSolution = true;
    public float gravityScale = 2;
    public Transform firePosition;
    public GameObject projectilePrefab;

    public Transform targetTransform;

    private Projectile projectile;

    void Start()
    {
        projectile = projectilePrefab.GetComponent<Projectile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeSinceLastFire > fireRate)
        {
            timeSinceLastFire = fireRate;
            Fire(targetTransform);
        }

        timeSinceLastFire += Time.deltaTime;

        DebugExtension.DebugWireSphere(transform.position, Color.yellow, minRange);
        DebugExtension.DebugWireSphere(transform.position, Color.yellow, maxRange);
    }

    public bool TargetInRange(float distance)
    {
        return minRange < distance && distance < maxRange;
    }

    Vector3 CalculateTargetPosition(Transform target, Rigidbody targetRb)
    {
        Vector3 targetPosition;
        if (targetRb)
        {
            targetPosition = targetRb.position;
        }
        else
        {
            targetPosition = target.position;
        }

        if (currentAccuracy < 1)
        {
            Vector2 random = Random.insideUnitCircle * Util.ConvertScale(0, 1, maxRandomness, minRandomness, currentAccuracy);
            targetPosition.x += random.x;
            targetPosition.z += random.y;
        }

        return targetPosition;
    }

    Vector3 CalculateTargetVelocity(Transform target, Rigidbody targetRb)
    {
        if (leadTarget && targetRb)
        {
            return targetRb.velocity;
        }
        return Vector3.zero;
    }

    void Fire(Transform target)
    {
        float angle = Vector3.Angle(Vector3.up, target.position - transform.position);

        float distance = Vector3.Distance(target.position, transform.position);

        float horizDistance = Vector2.Distance(new Vector2(target.position.x, target.position.z), new Vector2(transform.position.x, transform.position.z));
        float vertDistance = Mathf.Abs(target.position.y - transform.position.y);

        if (angle > minAngle && TargetInRange(distance))
        {
            Vector3 position = firePosition.position;
            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            Vector3 targetPosition = CalculateTargetPosition(target, targetRb);
            Vector3 targetVelocity = CalculateTargetVelocity(target, targetRb);

            if (useLateralVelocity)
            {
                Vector3 fireVelocity, impactPoint;
                float gravity;

                //float height = Mathf.Min(minHeightOffset, maxRange - distance);
                float height = Util.ConvertScale(minRange, maxRange, minHeightOffset, maxHeightOffset, horizDistance);
                //float speed = Util.ConvertScale(minRange, maxRange, minSpeed, maxSpeed, distance);
                float speed = Util.ConvertScale(0, maxRange, minSpeed, maxSpeed, horizDistance);

                if (BallisticTrajectory.solve_ballistic_arc_lateral(position, speed, targetPosition, targetVelocity, height, out fireVelocity, out gravity, out impactPoint))
                {
                    SpawnProjectile(position, fireVelocity, gravity, impactPoint);
                    currentAccuracy = Mathf.Clamp(currentAccuracy + accuracyGainRate, 0, 1);
                    timeSinceLastFire = 0;
                }
            }
            else
            {
                Vector3 s0, s1;
                Debug.Log(position + " " + projectile.speed + " " + targetPosition + " " + targetVelocity + " " + projectile.gravity);
                BallisticTrajectory.solve_ballistic_arc(position, projectile.speed, targetPosition, targetVelocity, projectile.gravity, out s0, out s1);

                Vector3 trajectory;
                if (s0.y < s1.y)
                {
                    if (useShorterSolution)
                    {
                        trajectory = s0;
                    }
                    else
                    {
                        trajectory = s1;
                    }
                }
                else
                {
                    if (useShorterSolution)
                    {
                        trajectory = s1;
                    }
                    else
                    {
                        trajectory = s0;
                    }
                }

                if (trajectory != Vector3.zero)
                {
                    SpawnProjectile(position, trajectory, targetPosition);
                    currentAccuracy = Mathf.Clamp(currentAccuracy + accuracyGainRate, 0, 1);
                    timeSinceLastFire = 0;
                }
            }
        }
    }

    void SpawnProjectile(Vector3 position, Vector3 trajectory, Vector3 impactPoint)
    {
        Quaternion rotation = Quaternion.LookRotation(trajectory);
        GameObject instance = GameObject.Instantiate(projectilePrefab, position, rotation);
        Projectile instanceProjectile = instance.GetComponent<Projectile>();
        instanceProjectile.source = this;
        instanceProjectile.targetPosition = impactPoint;
    }

    void SpawnProjectile(Vector3 position, Vector3 velocity, float gravity, Vector3 impactPoint)
    {
        Quaternion rotation = Quaternion.LookRotation(velocity.normalized);
        GameObject instance = GameObject.Instantiate(projectilePrefab, position, rotation);
        Projectile instanceProjectile = instance.GetComponent<Projectile>();
        instanceProjectile.source = this;
        instanceProjectile.gravity = gravityScale * gravity;
        instanceProjectile.speed = velocity.magnitude;
        instanceProjectile.targetPosition = impactPoint;
    }

    public void SuccessfulHit()
    {
        currentAccuracy = 0;
    }
}
