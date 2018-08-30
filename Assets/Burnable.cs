using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
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
		health = GetComponent<Health> ();
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
		onFire = true;
		currentFireOutTime = fireOutTime;
		fireDamage = damage;

        fire.GetComponent<ParticleSystem>().Play();
	}

	public void Extinguish() {
		onFire = false;
		currentFireOutTime = 0;
        
        fire.GetComponent<ParticleSystem>().Stop();
    }
}
