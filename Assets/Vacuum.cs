using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : MonoBehaviour
{
    public float suckForce;

    private VacuumParticles vacuumParticles;
    private Collider collider;
    private bool active;

    void Start()
    {
        vacuumParticles = GetComponentInChildren<VacuumParticles>();
        collider = GetComponentInChildren<Collider>();
        Deactivate();
    }
    
    void Update()
    {
        if (!active && Util.GetButton("Suck"))
        {
            Activate();
        }

        if (active && !Util.GetButton("Suck"))
        {
            Deactivate();
        }
    }

    void Activate()
    {
        active = true;
        vacuumParticles.enabled = true;
        collider.enabled = true;
    }

    void Deactivate()
    {
        active = false;
        vacuumParticles.enabled = false;
        collider.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            rb.AddForce(dir * suckForce);
        }
    }
}
