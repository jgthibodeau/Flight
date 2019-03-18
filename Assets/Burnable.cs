using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour {
	Health health;

	public float fireDamage;

    public float timeToCatchFire;
    public float currentTimeToCatchFire;
    private bool fireTriggered = false;
    
    public bool onFire = false;
	public float fireOutTime = 5f;
	public float currentFireOutTime = 0f;

	public GameObject fire;

	// Use this for initialization
	void Start () {
		health = GetComponentInParent<Health> ();
        if (onFire)
        {
            SetOnFire();
        } else
        {
            Extinguish();
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
            if (fireTriggered)
            {
                SetOnFire();
            }
		} else
        {
            if (fireTriggered)
            {
                currentTimeToCatchFire += Time.deltaTime;
            } else
            {
                currentTimeToCatchFire -= Time.deltaTime;
            }

            if (currentTimeToCatchFire >= timeToCatchFire)
            {
                SetOnFire();
            } else if (currentTimeToCatchFire < 0)
            {
                currentTimeToCatchFire = 0;
            }
        }

        fireTriggered = false;
	}

    public void TriggerFire(float damage)
    {
        fireTriggered = true;
        fireDamage = damage;
    }

	private void SetOnFire(){
        if (!onFire)
        {
            Debug.Log("Set on fire");

            onFire = true;
            currentFireOutTime = fireOutTime;

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
