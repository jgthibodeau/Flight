using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable {
	void Hit(float damage, GameObject hitter);
}
