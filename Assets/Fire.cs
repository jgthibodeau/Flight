using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
    public LayerMask fireLayer;
    public LayerMask waterLayer;

    public GameObject spreadableFire;
	public Kill killScript;
    public CapsuleCollider cc;
    //public Rigidbody rb;

    public float changeSpeed;

    public float scale = 1;
    [Range(0,1)]
	public float spreadChance;
	public float spreadRadius;

	public float minPitch, maxPitch;

	public float damage;

    public float waterLifeDecreaseRate;


    public float startLifetime;
    public float maxLifetime;

    public float minScale;
    public float maxScale;

    [Range(0, 1)]
    public float minSpreadChance;
    [Range(0, 1)]
    public float maxSpreadChance;

    public float minSpreadRadius;
    public float maxSpreadRadius;

    public float minDamage;
    public float maxDamage;

    //private float aliveTime = 0f;

    void Start () {
        //rb = GetComponentInParent<Rigidbody>();

        killScript.lifeTimeInSeconds = startLifetime;
        killScript.remainingLifeTime = startLifetime;

        StartCoroutine (Spread ());

		AudioSource audioSource = GetComponent<AudioSource> ();
		audioSource.pitch = Random.Range (minPitch, maxPitch);
		audioSource.time = Random.Range (0, audioSource.clip.length);
	}

    void Update ()
    {
        //aliveTime += Time.deltaTime;
        //if (aliveTime > 1 && rb.velocity.magnitude < 0.01f)
        //{
        //    rb.isKinematic = true;
        //    rb.constraints = RigidbodyConstraints.FreezeAll;
        //}

        float remainingLifeTime = killScript.remainingLifeTime;

        if (!killScript.IsDying())
        {
            scale = Mathf.Lerp(scale, Util.ConvertScale(0, maxLifetime, minScale, maxScale, remainingLifeTime), Time.deltaTime * changeSpeed);
            transform.localScale = new Vector3(scale, scale, scale);

            spreadChance = Mathf.Lerp(spreadChance, Util.ConvertScale(0, maxLifetime, minSpreadChance, maxSpreadChance, remainingLifeTime), Time.deltaTime * changeSpeed);
            spreadRadius = Mathf.Lerp(spreadRadius, Util.ConvertScale(0, maxLifetime, minSpreadRadius, maxSpreadRadius, remainingLifeTime), Time.deltaTime * changeSpeed);
            damage = Mathf.Lerp(damage, Util.ConvertScale(0, maxLifetime, minDamage, maxDamage, remainingLifeTime), Time.deltaTime * changeSpeed);

            DebugExtension.DebugCircle(transform.position + transform.up, Vector3.up, Color.red, spreadRadius);
        }
    }

    public void FanFlame(float amount)
    {
        if (!killScript.IsDying())
        {
            killScript.remainingLifeTime += amount;
            if (killScript.remainingLifeTime > maxLifetime)
            {
                killScript.remainingLifeTime = maxLifetime;
            }
        }
    }

	private IEnumerator Spread() {
		while (!killScript.IsDying()) {
			float waitTime = Random.Range (0.25f, 0.5f);
			yield return new WaitForSeconds (waitTime);
			float spread = Random.value;
			if (spread < spreadChance) {
                Vector3 newPosition;
                do {
                    Vector2 direction = Random.insideUnitCircle.normalized * spreadRadius;
                    newPosition = new Vector3(transform.position.x + direction.x, transform.position.y, transform.position.z + direction.y);
                    yield return null;
                } while (!Util.CanSpawn(newPosition, cc.radius, 10f, fireLayer));

                GameObject.Instantiate(spreadableFire, newPosition, Quaternion.identity);
			}
		}
	}

	void OnTriggerStay(Collider collision) {
		Burnable burnable = collision.gameObject.GetComponentInParent<Burnable> ();
		if (burnable != null) {
			burnable.TriggerFire (damage);
		}
        
        if (collision.gameObject.layer == waterLayer)
        {
            killScript.remainingLifeTime -= waterLifeDecreaseRate * Time.deltaTime;
        }
    }
}
