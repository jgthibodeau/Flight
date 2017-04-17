using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	public class WaterFog : MonoBehaviour {
		public bool replace = true;
		public Shader shader;
		public string replacementTag = "RenderType";
		private Camera camera;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			camera = GetComponent<Camera>();
			if (replace) {
				camera.SetReplacementShader (shader, replacementTag);
			} else {
				camera.ResetReplacementShader ();
			}
		}
	}
}