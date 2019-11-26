using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed;
    public float gravity = 10;
    public float damage;
    public bool sticky;
    public float stickDepth = 1f;
    public LayerMask instantKillLayerMask;
    public GameObject spawnOnCollide;
    public ProjectileLauncher source;
    public Vector3 targetPosition;
    public Transform targetObject;
    public bool collided = false;

    private Rigidbody rigidBody;
    private Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        //rigidBody.AddForce(transform.forward * speed, ForceMode.Impulse);
        //rigidBody.velocity = transform.forward * speed;
        lastPos = rigidBody.position;
    }

    void Update()
    {
        if (!collided)
        {
            if (targetObject)
            {
                targetObject.position = targetPosition;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!collided)
        {
            //rigidBody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
            //transform.LookAt(transform.position + rigidBody.velocity);
            float dt = Time.fixedDeltaTime;
            Vector3 accel = -gravity * Vector3.up;

            Vector3 curPos = rigidBody.position;
            Vector3 newPos = curPos + (curPos - lastPos) + transform.forward * speed * dt + accel * dt * dt;
            lastPos = curPos;
            rigidBody.MovePosition(newPos);
            transform.forward = newPos - lastPos;
            //transform.LookAt(transform.position + rigidBody.velocity);

            speed = 0;



            //rigidBody.AddForce(transform.forward * speed);
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //destroy if collided with fire
        if (Util.InLayerMask(collision.gameObject.layer, instantKillLayerMask))
        {
            GetComponent<Kill>().Die();
        }

        //		collision.rigidbody.AddForce (collision.impulse, ForceMode.Impulse);
        if (collided)
        {
            return;
        }
        collided = true;
        GameObject hit = collision.gameObject;
        IHittable hittable = hit.GetComponent<IHittable>();
        targetObject.gameObject.SetActive(false);

        if (hittable != null)
        {
            Debug.Log("hitting " + hittable);
            hittable.Hit(damage, this.gameObject);
            source.SuccessfulHit();
        }

        if (spawnOnCollide)
        {
            //GameObject.Instantiate(spawnOnCollide, transform.position, transform.rotation);
            GameObject.Instantiate(spawnOnCollide, collision.contacts[0].point, Quaternion.identity);
        }

        if (sticky)
        {
            Halt(collision);
        } else
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void Halt(Collision collision)
    {
        Debug.Log("halting");

        //adjust to be within collider a little bit more
        transform.position += transform.forward * stickDepth;

        //remove collider
        GetComponent<Collider>().enabled = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        rigidBody.isKinematic = true;

        //parent to collision
        transform.parent = collision.transform;

        //turn off this script
        this.enabled = false;

        //remove the trail renderer
        //StartCoroutine (SlowTrailDisable (GetComponent<TrailRenderer> ()));
    }


    //IEnumerator SlowTrailDisable (TrailRenderer trail) {
    //	float rate = trail.time / 15f;
    //	while (trail.time > 0) {
    //		trail.time -= rate;
    //		yield return 0;
    //	}
    //}
}
