using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineTrackPosition : MonoBehaviour
{
    public float defaultFreeY = 0.5f;
    public float defaultFreeX = 0f;
    public float defaultYSpeed = 0.5f;
    public float defaultXSpeed = 0.5f;

    public float cameraResetTime = 0.5f;

    public CinemachineFreeLook freeLook;

    private bool recentering;

    //public CinemachineVirtualCameraBase thisCamera;
    public CinemachineStateDrivenCamera stateCamera;

    // Start is called before the first frame update
    void Start()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        //thisCamera = GetComponent<CinemachineVirtualCameraBase>();
        stateCamera = GetComponentInParent<CinemachineStateDrivenCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Util.GetButtonDown("Center Camera"))
        //{
        //    ResetFreeAxis();
        //}

        //if (Util.GetButtonUp("Center Camera"))
        //{
        //    Invoke("SetFree", cameraResetTime);
        //}

        //if (recentering)
        //{
        //    freeLook.m_XAxis.Value = defaultFreeY;
        //}

        stateCamera.m_AnimatedTarget.SetBool("Recenter", Util.GetButton("Center Camera"));
    }

    public void ResetFreeAxis()
    {
        ResetFreeX();
        ResetFreeY();
        freeLook.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;

        ResetFreeY();
        ResetFreeX();
        freeLook.m_YAxis.m_MaxSpeed = 0;
        freeLook.m_XAxis.m_MaxSpeed = 0;

        recentering = true;
    }

    public void ResetFreeY()
    {
        if (freeLook != null)
        {
            freeLook.m_YAxis.Value = defaultFreeY;
        }
    }

    public void ResetFreeX()
    {
        if (freeLook != null)
        {
            freeLook.m_XAxis.Value = defaultFreeX;
        }
    }

    public void SetFree()
    {
        freeLook.m_YAxis.m_MaxSpeed = defaultYSpeed;
        freeLook.m_XAxis.m_MaxSpeed = defaultXSpeed;
        freeLook.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        recentering = false;
    }
}
