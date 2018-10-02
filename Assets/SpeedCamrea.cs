using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects{
	public class SpeedCamrea : MonoBehaviour {
		public Camera[] cameras;

        public VignetteAndChromaticAberration vignette;
        public MotionBlur motionBlur;
        public Rigidbody targetRigidBody;

        public float minVelocityToTrigger;
        public float maxVelocity;

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

        public bool useMotionBlur;
        public float maxMotionBlur = 0.5f;

        public bool useShake;
        public float shakeScale = 0f;
        public float maxShake = 10f;
        public float shakeChangeSpeed;
        Vector3 cameraOffset = Vector3.zero;

        // Use this for initialization
        void Start () {
			cameras = GetComponentsInChildren<Camera> ();
            vignette.blur = 0;
            vignette.chromaticAberration = 0;
            vignette.intensity = 0;
            motionBlur.blurAmount = 0;
            foreach (Camera camera in cameras)
            {
                camera.fieldOfView = minFov;
            }
        }
		
		// Update is called once per frame
		void LateUpdate () {
            float percent = Util.ConvertScale(minVelocityToTrigger, maxVelocity, 0, 1, targetRigidBody.velocity.magnitude);

			if (useBlur) {
				float newBlur = percent * maxBlur;
				vignette.blur = Mathf.Lerp (vignette.blur, newBlur, Time.deltaTime * effectChangeSpeed);
			} else
            {
                vignette.blur = 0;
            }

			if (useChromatic) {
				float newChromatic = percent * maxChromatic;
				vignette.chromaticAberration = Mathf.Lerp (vignette.chromaticAberration, newChromatic, Time.deltaTime * effectChangeSpeed);
            }
            else
            {
                vignette.chromaticAberration = 0;
            }

            if (useVignette)
            {
                float newIntensity = percent * maxVignette;
                vignette.intensity = Mathf.Lerp(vignette.intensity, newIntensity, Time.deltaTime * effectChangeSpeed);
            }
            else
            {
                vignette.intensity = 0;
            }

            if (useMotionBlur)
            {
                float newBlur = percent * maxMotionBlur;
                motionBlur.blurAmount = Mathf.Lerp(motionBlur.blurAmount, newBlur, Time.deltaTime * effectChangeSpeed);
            }
            else
            {
                motionBlur.blurAmount = 0;
            }

            if (useFov)
            {
                float newFov = Util.ConvertScale(0, 1, minFov, maxFov, percent);
                newFov = Mathf.Lerp(cameras[0].fieldOfView, newFov, Time.deltaTime * effectChangeSpeed);
                foreach (Camera camera in cameras)
                {
                    camera.fieldOfView = newFov;
                }
            }
            else
            {
                foreach (Camera camera in cameras)
                {
                    camera.fieldOfView = minFov;
                }
            }

            if (useShake)
            {
                float newShake = percent * maxShake;
                Vector3 desiredCameraOffset = Random.insideUnitSphere * newShake;
                cameraOffset = Vector3.Slerp(cameraOffset, desiredCameraOffset, Time.deltaTime * shakeChangeSpeed);
                transform.position += cameraOffset;
            }
        }
	}
}
