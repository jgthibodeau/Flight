using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects{
	public class SpeedCamrea : MonoBehaviour {
		public Camera[] cameras;

		public VignetteAndChromaticAberration vignette;
		public Rigidbody targetRigidBody;

		public float effectChangeSpeed = 1f;

		public bool useBlur;
		public float blurScale = 0f;
		public float maxBlur = 0.75f;

		public bool useChromatic;
		public float chromaticScale = 0f;
		public float maxChromatic = 15f;

		public bool useVignette;
		public float vignetteScale = 0f;
		public float maxVignette = 0.75f;

		public bool useFov;
		public float fovScale = 0f;
		public float minFov = 90f;
		public float maxFov = 120;

		// Use this for initialization
		void Start () {
			cameras = GetComponentsInChildren<Camera> ();
		}
		
		// Update is called once per frame
		void Update () {
			float velocity = targetRigidBody.velocity.magnitude;
			if (useBlur) {
				float newBlur = Mathf.Clamp (velocity * blurScale, 0, maxBlur);
				vignette.blur = Mathf.Lerp (vignette.blur, newBlur, Time.deltaTime * effectChangeSpeed);
			}

			if (useChromatic) {
				float newChromatic = Mathf.Clamp (velocity * chromaticScale, 0, maxChromatic);
				vignette.chromaticAberration = Mathf.Lerp (vignette.chromaticAberration, newChromatic, Time.deltaTime * effectChangeSpeed);
			}

			if (useVignette) {
				float newIntensity = Mathf.Clamp (velocity * vignetteScale, 0, maxVignette);
				vignette.intensity = Mathf.Lerp (vignette.intensity, newIntensity, Time.deltaTime * effectChangeSpeed);
			}

			if (useFov) {
				float newFov = Mathf.Clamp (minFov + velocity * fovScale, minFov, maxFov);
				newFov = Mathf.Lerp (cameras[0].fieldOfView, newFov, Time.deltaTime * effectChangeSpeed);
				foreach (Camera camera in cameras) {
					camera.fieldOfView = newFov;
				}
			}
		}
	}
}
