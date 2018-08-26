using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Kill))]
public class Gust : MonoBehaviour {
    private Kill killScript;

	public float currentScale;
    public float minScale;
    public float maxScale;
    public float growSpeed;
    public Vector3 moveSpeed;

    public float fanFlameAmount;

    public float minGustForce;
    public float maxGustForce;

	public List<GameObject> collidedObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
        killScript = GetComponent<Kill>();
		currentScale = minScale;
		transform.localScale = new Vector3 (currentScale, currentScale, currentScale);
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 movementDir = transform.forward * moveSpeed.z + transform.right * moveSpeed.x + transform.up * moveSpeed.y;
        transform.position += movementDir * Time.deltaTime;

        currentScale += growSpeed * Time.deltaTime;
        if (currentScale < maxScale)
        {
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        }
        //else
        //{
        //    GameObject.Destroy(gameObject);
        //}
    }

    void OnTriggerStay(Collider other)
    {
        if (killScript.IsDying())
        {
            return;
        }

        if (isFire(other.gameObject))
        {
            if (canCollide(other.gameObject))
            {
                other.gameObject.GetComponent<Fire>().FanFlame(fanFlameAmount);
            }
        }
        else
        {
            Rigidbody otherRb = getCollidableRigidbody(other.gameObject);
            if (otherRb != null)
            {
                ApplyForce(otherRb);
            }
        }

        if (!collidedObjects.Contains(other.gameObject))
        {
            
        }
	}

    private bool isFire(GameObject go)
    {
        return (go.tag == "Fire");
    }

    private bool canCollide(GameObject go)
    {
        if (!collidedObjects.Contains(go))
        {
            collidedObjects.Add(go);
            return true;
        }
        return false;
    }

    private Rigidbody getCollidableRigidbody(GameObject go)
    {
        Rigidbody otherRb = go.GetComponentInParent<Rigidbody>();
        if (!collidedObjects.Contains(go))
        {
            collidedObjects.Add(go);
            return otherRb;
        }
        return null;
    }

    private void ApplyForce(Rigidbody otherRb)
    {
        Vector3 forceDir = otherRb.transform.position - transform.position;
        Vector3 force = forceDir.normalized * CalculateForce();
        otherRb.AddForce(force);
    }

    private float CalculateForce()
    {
        return Util.ConvertScale(0, killScript.lifeTimeInSeconds, minGustForce, maxGustForce, killScript.remainingLifeTime);
    }
}
