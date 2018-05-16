using UnityEngine;

namespace DynamicShadowProjector.Sample {
	public class Rotate : MonoBehaviour {
		public float m_rotateXSpeed = 0f;
		public float m_rotateYSpeed = 0f;
		public float m_rotateZSpeed = 0f;
		void Update()
		{
			transform.rotation = Quaternion.AngleAxis(m_rotateXSpeed*Time.deltaTime, transform.right) * transform.rotation;
			transform.rotation = Quaternion.AngleAxis(m_rotateYSpeed*Time.deltaTime, transform.up) * transform.rotation;
			transform.rotation = Quaternion.AngleAxis(m_rotateZSpeed*Time.deltaTime, transform.forward) * transform.rotation;
		}
	}
}
