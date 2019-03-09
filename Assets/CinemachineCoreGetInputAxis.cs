using Cinemachine;
using UnityEngine;

public class CinemachineCoreGetInputAxis : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
    }

    float HandleAxisInputDelegate(string axisName)
    {
        return Util.GetAxis(axisName);
    }
}
