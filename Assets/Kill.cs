using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour {
	public float lifeTimeInSeconds;
	public float remainingLifeTime;

    public ParticleSystem ps;
	public float deathTimeInSeconds;
    public float remainingDeathTime;

    private AudioSource audioSource;
	public float volumeDropoffRate = 1f;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
        remainingLifeTime = lifeTimeInSeconds;
		MyGameManager.instance.AddInstace (this.gameObject);
	}

    public bool IsAlive()
    {
        return !IsDead() && !IsDying();
    }

    public bool IsDying()
    {
        return remainingLifeTime <= 0;
    }

    public bool IsDead()
    {
        return remainingLifeTime <= 0 && remainingDeathTime <= 0;
    }

    // Update is called once per frame
    void Update () {
        if (!IsDying())
        {
            remainingLifeTime -= Time.deltaTime;
            remainingDeathTime = deathTimeInSeconds;
        }

        else if (!IsDead())
        {
            remainingDeathTime -= Time.deltaTime;

            if (ps != null && ps.isPlaying)
            {
                ps.Stop();
            }
            if (audioSource != null && audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime * volumeDropoffRate;
            }
        }

        else
        {
            Die();
        }
	}

	public void Die () {
		MyGameManager.instance.RemoveInstance (this.gameObject);
	}
}
