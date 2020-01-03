using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraUpdateOverride : MonoBehaviour
{
    public CinemachineBrain.UpdateMethod defaultUpdateMethod;
    public CinemachineBrain brain;

    public List<GameObject> fixedUpdateCameras;
    public List<GameObject> lateUpdateCameras;

    private CinemachineBrain.UpdateMethod desiredUpdateMethod;

    // Start is called before the first frame update
    void Start()
    {
        brain.m_UpdateMethod = defaultUpdateMethod;
    }

    void LateUpdate()
    {
        if (desiredUpdateMethod == CinemachineBrain.UpdateMethod.LateUpdate)
        {
            ChangeUpdateMethod();
        }
    }

    void FixedUpdate()
    {
        if (desiredUpdateMethod == CinemachineBrain.UpdateMethod.FixedUpdate)
        {
            ChangeUpdateMethod();
        }
    }

    void ChangeUpdateMethod()
    {
        if (desiredUpdateMethod != brain.m_UpdateMethod)
        {
            brain.m_UpdateMethod = desiredUpdateMethod;
        }
    }

    public void CameraActivated(ICinemachineCamera camera1, ICinemachineCamera camera2)
    {
        if (camera1 != null)
        {
            GameObject newCamera = camera1.VirtualCameraGameObject;

            if (fixedUpdateCameras.Contains(newCamera))
            {
                desiredUpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
            }
            else if (lateUpdateCameras.Contains(newCamera))
            {
                desiredUpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            }
            else
            {
                desiredUpdateMethod = defaultUpdateMethod;
            }
        }
    }
}
