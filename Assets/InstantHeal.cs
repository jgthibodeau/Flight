using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantHeal : InstantPickup
{
    public float healAmount;
    public float ammoAmount;

    public override void HandleCollide(GameObject collidedObject)
    {
        Health health = collidedObject.GetComponentInParent<Health>();
        if (health)
        {
            health.Heal(healAmount);
        }

        FlameBreath flameBreath = collidedObject.GetComponentInParent<FlameBreath>();
        if (flameBreath)
        {
            flameBreath.RegainFlame(ammoAmount);
        }
    }
}
