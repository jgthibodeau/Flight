using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class FallBehaviour : BaseBehaviour
{
    public LayerMask layerMaskForGround;
    public float groundDistance = 0.1f;
    public float groundCheckRadius = 0.5f;

    private bool isGrounded;

    [Task]
    void SetGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
        if (!isGrounded)
        {
            behaviourController.EnableGravity();
        }
        Task.current.Succeed();
    }

    [Task]
    bool IsGrounded()
    {//if flapping, not grounded
     //if (glideV2Script.IsFlapping()) {
     //isGrounded = false;
     //} else {
        float groundCheckDistance = 0;
        float groundCapsuleHeight = 0.5f;

        Vector3 capsuleStart = transform.position + groundCapsuleHeight * transform.forward + groundCheckDistance * Vector3.down;
        Vector3 capsuleEnd = transform.position - groundCapsuleHeight * transform.forward + groundCheckDistance * Vector3.down;

        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckCapsule(
            capsuleStart,
            capsuleEnd,
            groundCheckRadius,
            layerMaskForGround.value
        );

        if (!wasGrounded && isGrounded)
        {
            //Debug.Log("wasnt grounded but now is grounded");
            behaviourController.Reset();
        }

        return isGrounded;
    }

    [Task]
    void Fall()
    {
        behaviourController.EnableGravity();
        Task.current.Succeed();
    }
}
