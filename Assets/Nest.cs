using UnityEngine;
using System.Collections;

public class Nest : MonoBehaviour {
	public GameObject[] sticks;
	public int numberSticks;
	public float radius;
	public float yRadius;
	public float scale = 0.05f;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < numberSticks; i++) {
			int j = Random.Range (0, sticks.Length);
			GameObject newStick = GameObject.Instantiate (sticks [j]);

			newStick.transform.localScale = new Vector3 (scale, scale, scale);

			newStick.transform.parent = this.transform;
			newStick.transform.position = this.transform.position;
			float angle = Random.Range (0, 360);
			float distance = Random.Range (radius/2, radius);
			newStick.transform.localPosition = new Vector3(distance*Mathf.Sin (angle), Random.Range (-yRadius, yRadius), distance*Mathf.Cos (angle));

			newStick.transform.rotation = Quaternion.Euler (new Vector3(Random.Range (30, 150), angle + 90, Random.Range (30, 150)));
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
