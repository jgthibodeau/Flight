using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineTrackPosition : MonoBehaviour
{
    public float defaultFreeY = 0.5f;
    public float defaultFreeX = 0f;

    public CinemachineFreeLook freeLook;

    //public CinemachineVirtualCameraBase thisCamera;
    //public CinemachineStateDrivenCamera stateCamera;

    // Start is called before the first frame update
    void Start()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        //thisCamera = GetComponent<CinemachineVirtualCameraBase>();
        //stateCamera = GetComponentInParent<CinemachineStateDrivenCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(thisCamera.VirtualCameraGameObject != stateCamera.LiveChild.VirtualCameraGameObject && !stateCamera.IsBlending)
        //{
        //    thisCamera.VirtualCameraGameObject.transform.position = stateCamera.LiveChild.VirtualCameraGameObject.transform.position;
        //    thisCamera.VirtualCameraGameObject.transform.rotation = stateCamera.LiveChild.VirtualCameraGameObject.transform.rotation;
        //    thisCamera.UpdateCameraState(Vector3.up, Time.deltaTime);
        //}
        if (Util.GetButton("Center Camera"))
        {
            ResetFreeAxis();
        }
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

    public void ResetFreeAxis()
    {
        ResetFreeX();
        ResetFreeY();
    }
}
