using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour {
    public AudioSource source;
    public AudioClip hoverClip, clickClip;
    public float minHoverPitch, maxHoverPitch;
    public float minClickPitch, maxClickPitch;
    public float hoverVolume, clickVolume;

    public void ClickSound()
    {
        source.volume = clickVolume;
        source.pitch = Random.Range(minClickPitch, maxClickPitch);
        source.PlayOneShot(clickClip);
    }

    public void HoverSound()
    {
        //source.volume = hoverVolume;
        //source.pitch = Random.Range(minHoverPitch, maxHoverPitch);
        //source.PlayOneShot(hoverClip);
    }
}
