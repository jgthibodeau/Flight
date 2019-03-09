using UnityEngine;
using System.Collections;

public class Util : MonoBehaviour {
	public static Color orange = new Color(1F, 0.5F, 0F);
	
	public static void DrawRigidbodyRay(Rigidbody rigidBody, Vector3 v1, Vector3 v2){
		Debug.DrawRay (v1 + rigidBody.velocity * Time.fixedDeltaTime, v2);
	}
		
	public static Vector3 RigidBodyPosition(Rigidbody rigidBody){
		return rigidBody.transform.position + rigidBody.velocity * Time.fixedDeltaTime;
	}

	public static void DrawRigidbodyRay(Rigidbody rigidBody, Vector3 start, Vector3 dir, Color color){
		Debug.DrawRay (start + rigidBody.velocity * Time.fixedDeltaTime, dir, color);
	}

	public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal) {
		Vector3 perpVector;
		float angle;

		//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
		perpVector = Vector3.Cross(normal, referenceVector);

		//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
		angle = Vector3.Angle(referenceVector, otherVector);
		angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

		return angle;
	}

	private IEnumerable WaitForAnimation(Animation animation){
		do {
			yield return null;
		} while(animation.isPlaying);
	}

	public static float GetWaterLevel(Vector3 position, bool useLPW, bool useRandomness)
	{
		RaycastHit hit;
		if (useLPW) {
			if (Physics.Raycast (position, Vector3.down, out hit, MyGameManager.instance.oceanCheckDistance, MyGameManager.instance.oceanLayer)) {
				LPWAsset.LowPolyWaterScript waterScript = hit.transform.GetComponent<LPWAsset.LowPolyWaterScript> ();
				return waterScript.WaterHeight (position);
			} else if (Physics.Raycast (position, Vector3.up, out hit, MyGameManager.instance.oceanCheckDistance, MyGameManager.instance.oceanLayer)) {
				LPWAsset.LowPolyWaterScript waterScript = hit.transform.GetComponent<LPWAsset.LowPolyWaterScript> ();
				return waterScript.WaterHeight (position);
			} else {
				return MyGameManager.instance.oceanLevel;
			}
		} else if (useRandomness) {
			return MyGameManager.instance.oceanLevel + Random.Range (-0.01f, 0.01f);
		} else {
			return MyGameManager.instance.oceanLevel;
		}
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 direction = point - pivot; //get direction relative to pivot
		direction = Quaternion.Euler (angles) * direction; // rotate
		return direction + pivot; // calculate rotated point
	}

	public static float GetAxis(string axis){
        float val = TeamUtility.IO.InputManager.GetAxis(axis);
        if (val == 0f && TeamUtility.IO.InputManager.GetButton(axis))
        {
            val = 1f;
        }
        return val;
	}
	public static bool GetButton(string button)
    {
        bool val = TeamUtility.IO.InputManager.GetButton(button);
        if (!val && TeamUtility.IO.InputManager.GetAxis(button) != 0)
        {
            val = true;
        }
        return val;
    }
	public static bool GetButtonDown(string button){
		return TeamUtility.IO.InputManager.GetButtonDown (button);
	}
	public static bool GetButtonUp(string button){
		return TeamUtility.IO.InputManager.GetButtonUp (button);
	}

	public static bool InLayerMask(int layer, LayerMask layermask) {
		return layermask == (layermask | (1 << layer));
	}

    public static float ConvertScale(float oldMin, float oldMax, float newMin, float newMax, float value)
    {
        value = Mathf.Clamp(value, oldMin, oldMax);
        return (((value - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
    }

    public static bool CanSpawn(Vector3 position, float radius, float height, LayerMask layer)
    {
        Collider[] colliders = Physics.OverlapCapsule(position - Vector3.up * height, position + Vector3.up * height, radius, layer);
        return colliders.Length == 0;
    }

    public static Vector3 TransformPointUnscaled(Transform transform, Vector3 position)
    {
        var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        return localToWorldMatrix.MultiplyPoint3x4(position);
    }

    public static Vector3 InverseTransformPointUnscaled(Transform transform, Vector3 position)
    {
        var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
        return worldToLocalMatrix.MultiplyPoint3x4(position);
    }

    public static bool Probability(float chance)
    {
        return Random.value <= chance;
    }
}
