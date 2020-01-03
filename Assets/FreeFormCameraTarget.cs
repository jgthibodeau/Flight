using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFormCameraTarget : MonoBehaviour
{
    public bool allowFreeForm = true;
    public bool alignOffsetCameraToWorldUp;
    public Camera camera;
    public Player player;
    public bool freeFormActive;
    public bool enableX = true, enableY = true;
    public float offsetScaleX, offsetScaleY, freeFormScaleX, freeFormScaleY, resetScale;
    public float maxYAngle = 60;
    public float minYAngle = -60;
    public float maxYOffsetAngle = 60;
    public float minYOffsetAngle = -60;
    public Rigidbody rb;
    public float minVelocityToReset;

    Quaternion desiredRotation = Quaternion.identity;

    void Start()
    {
        Debug.Assert(camera != null, "Need to attach a camera");
        rb = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<Player>();
    }

	void FixedUpdate ()
    {
        if (!allowFreeForm)
        {
            transform.localRotation = Quaternion.identity;
            return;
        }
        
        if (Util.GetButton("Center Camera"))
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            float x = enableX ? Util.GetAxis("Horizontal Right") : 0;
            float y = enableY ? -Util.GetAxis("Vertical Right") : 0;

            float velocity = rb.velocity.magnitude;

            bool freeForm360 = (player.isGrounded);// || velocity < minVelocityToReset);

            if ((x != 0f) || (y != 0f))
            {
                if (!freeFormActive)
                {
                    Vector3 desiredEuler = camera.transform.eulerAngles;
                    desiredEuler.z = 0;
                    desiredRotation = Quaternion.Euler(desiredEuler);
                }
                freeFormActive = true;

                if (freeForm360)
                {
                    AdjustFreeForm360(x, y);
                }
                else
                {
                    AdjustOffset(x, y);
                }
            }
            else if (!freeForm360 || velocity > minVelocityToReset)
            {
                desiredRotation = Quaternion.Slerp(desiredRotation, transform.parent.rotation, Time.fixedDeltaTime * resetScale);
                freeFormActive = false;
            }
        }

        transform.rotation = desiredRotation;
    }

    void AdjustFreeForm360(float x, float y)
    {
        Quaternion xRot = Quaternion.Euler(Vector3.up * x * Time.fixedDeltaTime * freeFormScaleX);
        Quaternion yRot = Quaternion.Euler(Vector3.right * y * Time.fixedDeltaTime * freeFormScaleY);
        desiredRotation = desiredRotation * xRot * yRot;

        desiredRotation = ClampCameraAngle(desiredRotation, maxYAngle, minYAngle);
    }

    void AdjustOffset(float x, float y)
    {
        Quaternion xRot = Quaternion.Euler(Vector3.up * x * offsetScaleX);
        Quaternion yRot = Quaternion.Euler(Vector3.right * y * offsetScaleY);

        Vector3 parentForward;
        if (alignOffsetCameraToWorldUp)
        {
            parentForward = Vector3.ProjectOnPlane(transform.parent.forward, Vector3.up);
        }
        else
        {
            parentForward = transform.parent.forward;
        }
        Util.DrawRigidbodyRay(GetComponentInParent<Rigidbody>(), transform.position, parentForward * 20, Color.gray);
        Quaternion forward = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.parent.forward, Vector3.up), Vector3.up);


        Quaternion newDesiredRotation = forward * xRot * yRot;

        desiredRotation = ClampCameraAngle(desiredRotation, maxYOffsetAngle, minYOffsetAngle);

        desiredRotation = Quaternion.Slerp(desiredRotation, newDesiredRotation, Time.fixedDeltaTime * offsetScaleX);
    }

    Quaternion ClampCameraAngle(Quaternion rotation, float maxY, float minY)
    {
        Vector3 euler = rotation.eulerAngles;
        euler.z = 0;

        if (euler.x > maxY && euler.x < 180)
        {
            Debug.Log(euler + "reverting x to " + maxY);
            euler.x = maxY;
        }
        else if (euler.x < (360 + minY) && euler.x > 180)
        {
            Debug.Log(euler + "reverting x to " + minY);
            euler.x = minY;
        }

        return Quaternion.Euler(euler);
    }

    public void AllowFreeForm()
    {
        if (!allowFreeForm)
        {
            allowFreeForm = true;
            desiredRotation = camera.transform.rotation;
        }
    }

    public void DisallowFreeForm()
    {
        if (allowFreeForm)
        {
            allowFreeForm = false;
        }
    }
}
