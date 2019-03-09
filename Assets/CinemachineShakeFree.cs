using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineShakeFree : MonoBehaviour
{
    public float minVelocity, maxVelocity, minShake, maxShake;

    public Rigidbody rb;
    CinemachineFreeLook vcam;
    CinemachineBasicMultiChannelPerlin[] noise = new CinemachineBasicMultiChannelPerlin[3];

    void Start()
    {
        vcam = GetComponent<CinemachineFreeLook>();
        noise[0] = vcam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise[1] = vcam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise[2] = vcam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        float velocity = rb.velocity.magnitude;
        if (velocity > minVelocity)
        {
            Noise(Util.ConvertScale(minVelocity, maxVelocity, minShake, maxShake, velocity), 1);
        } else
        {
            Noise(0, 0);
        }
    }

    public void Noise(float amplitudeGain, float frequencyGain)
    {
        foreach(CinemachineBasicMultiChannelPerlin n in noise) {
            n.m_AmplitudeGain = amplitudeGain;
            n.m_FrequencyGain = frequencyGain;
        }
    }
}
