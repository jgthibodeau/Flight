using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects{
	public class SpeedCamrea : MonoBehaviour {
		public VignetteAndChromaticAberration vignette;
		public Rigidbody targetRigidBody;

		public bool useBlur;
		public float blurScale = 0f;
		public float maxBlur = 0.75f;

		public bool useChromatic;
		public float chromaticScale = 0f;
		public float maxChromatic = 15f;

		public bool useVignette;
		public float vignetteScale = 0f;
		public float maxVignette = 0.75f;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			float velocity = targetRigidBody.velocity.magnitude;
			if (useBlur) {
				vignette.blur = Mathf.Clamp (velocity * blurScale, 0, maxBlur);
			}

			if (useChromatic) {
				vignette.chromaticAberration = Mathf.Clamp (velocity * chromaticScale, 0, maxChromatic);
			}

			if (useVignette) {
				vignette.intensity = Mathf.Clamp (velocity * vignetteScale, 0, maxVignette);
			}
		}
	}
}
