using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformRigidbody : MonoBehaviour {
    public bool useLateUpdate = true;
    public Rigidbody targetBase;
    public Transform target; // The Transform parented to one of the character's bones
    private Vector3 targetPosition; //instead combine base + target to get offset vector, and use this compared to latest base
    private Vector3 targetBasePosition;
    private Vector3 targetOffset;
    private Quaternion targetRotation;

    Vector3 localPosition;
    Quaternion localRotation;

    private Vector3 currentVelocity;
    public float velocityRate;
    Quaternion originalRotation;

    private Rigidbody rb;
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        //LateUpdate();
        originalRotation = transform.rotation;
    }
    void Update()
    {
        //rb.MovePosition(target.position);
        //rb.MoveRotation(target.rotation);

        //rb.position = target.position;
        //rb.rotation = target.rotation;
        //rb.velocity = targetBase.velocity;

        //transform.position = Vector3.Slerp(transform.position, target.position, 100*Time.deltaTime);
        //transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, 100* Time.deltaTime);

        //currentVelocity = Vector3.Slerp(currentVelocity, targetBase.velocity, velocityRate * Time.deltaTime);
        //rb.velocity = currentVelocity;

        //transform.position = target.position + targetBase.velocity * Time.deltaTime;
        //transform.rotation = target.rotation;
        //rb.velocity = targetBase.velocity;


        //rb.velocity = targetBase.velocity;



        transform.position = targetBase.position + targetOffset;
        transform.rotation = targetRotation;

        //transform.position = localPosition;
        //transform.localRotation = localRotation;

    }
    //// Move this ParticleSystem to the target's last frame position before it emits
    void FixedUpdate()
    {
        if (useLateUpdate)
        {
            //rb.MovePosition(targetBase.position + targetOffset + targetBase.velocity * Time.fixedDeltaTime);
            //rb.MovePosition(targetBase.position + targetOffset);
            //rb.MoveRotation(targetRotation.normalized);
        }
        else
        {
            //transform.rotation = target.rotation;
            //transform.position = target.position;
            //rb.velocity = targetBase.velocity;

            //rb.MovePosition(target.position + targetBase.velocity * Time.fixedDeltaTime);
            //rb.MovePosition(target.position);
            //rb.MoveRotation(target.rotation);

            //rb.position = targetBase.position + (targetBase.position - target.position);
            //rb.rotation = target.rotation;
            //rb.velocity = targetBase.velocity;
        }
    }
    //// Read the world space position and rotation of the target after procedural effects have been applied
    //// NB! Make sure this script is set to a higher value than FinalIK components in the Script Execution Order!
    void LateUpdate()
    {
        targetBasePosition = targetBase.position;
        targetPosition = target.position;
        targetOffset = targetPosition - targetBasePosition;
        targetRotation = target.rotation;
        //Vector3 euler = target.eulerAngles;
        //euler.x *= -1;
        //euler.y *= -1;
        //targetRotation = Quaternion.Euler(euler);

        //localPosition = targetBase.transform.InverseTransformPoint(target.position);
        //localRotation = Quaternion.LookRotation(targetBase.transform.InverseTransformDirection(target.forward), transform.up);
    }
}
