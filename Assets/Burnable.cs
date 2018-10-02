using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour {
	Health health;

	public float fireDamage;

	public bool onFire = false;
	public float fireOutTime = 5f;
	public float currentFireOutTime = 0f;

	public GameObject fire;
	public GameObject steam;

	// Use this for initialization
	void Start () {
		health = GetComponentInParent<Health> ();
        if (onFire)
        {
            SetOnFire(fireDamage);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (currentFireOutTime > 0) {
			currentFireOutTime -= Time.deltaTime;
			if (currentFireOutTime <= 0) {
				Extinguish ();
			}
		}

		if (onFire) {
			health.Hit (fireDamage * Time.deltaTime, this.gameObject);
		}
	}

	public void SetOnFire(float damage){
        if (!onFire)
        {
            onFire = true;
            currentFireOutTime = fireOutTime;
            fireDamage = damage;

            foreach (ParticleSystem ps in fire.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
        }
        else if (currentFireOutTime < fireOutTime)
        {
            currentFireOutTime = fireOutTime;
        }
	}

    public void FanFlame(float amount)
    {
        if (onFire)
        {
            currentFireOutTime += amount;
        }
    }

    public void Extinguish() {
		onFire = false;
		currentFireOutTime = 0;
        
        foreach (ParticleSystem ps in fire.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }
    }
}
