using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    public float minVelocity, maxVelocity, minShake, maxShake;

    public Rigidbody rb;
    CinemachineVirtualCamera vcam;
    CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }
}
