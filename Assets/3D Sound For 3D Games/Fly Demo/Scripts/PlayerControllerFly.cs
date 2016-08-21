using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControllerFly : MonoBehaviour 
{

    public float speed = 3.0f;

    private Rigidbody rb;

    private float sensitivityX = 15F;
    private float sensitivityY = 15F;

    private float minimumY = -90F;
    private float maximumY = 90F;

	private int frameCounterX = 5;
	private int frameCounterY = 5;

    private float rotationX = 0F;
    private float rotationY = 0F;

	private Quaternion xQuaternion;
	private Quaternion yQuaternion;
	private Quaternion originalRotation;

	private List<float> rotArrayX = new List<float> ();
	private List<float> rotArrayY = new List<float> ();

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rb)
            rb.freezeRotation = true;
        originalRotation = transform.localRotation;
    }
	
	//Method to clamp angle between min and max
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		float moveFly = Input.GetAxis("Mouse ScrollWheel");
		
		Vector3 movement = new Vector3(moveHorizontal, moveFly, moveVertical);


		//Mouse/Camera Movement Smoothing:    
		//Average rotationX for smooth mouselook
		float rotAverageX = 0f;
		rotationX += Input.GetAxis ("Mouse X") * sensitivityX;

		//Add the current rotation to the array, at the last position
		rotArrayX.Add (rotationX);

		//Reached max number of steps?  Remove the oldest rotation from the array
		if (rotArrayX.Count >= frameCounterX) 
		{
			rotArrayX.RemoveAt (0);
		}

		//Add all of these rotations together
		for (int i_counterX = 0; i_counterX < rotArrayX.Count; i_counterX++) 
		{
			//Loop through the array
			rotAverageX += rotArrayX[i_counterX];
		}

		//Now divide by the number of rotations by the number of elements to get the average
		rotAverageX /= rotArrayX.Count;

		//Average rotationY, same process as above
		float rotAverageY = 0;
		rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;
		rotationY = ClampAngle (rotationY, minimumY, maximumY);
		rotArrayY.Add (rotationY);

		if (rotArrayY.Count >= frameCounterY) 
		{
			rotArrayY.RemoveAt (0);
		}

		for (int i_counterY = 0; i_counterY < rotArrayY.Count; i_counterY++) 
		{
			rotAverageY += rotArrayY[i_counterY];
		}
		
		rotAverageY /= rotArrayY.Count;
		
		//Apply and rotate this object
		xQuaternion = Quaternion.AngleAxis (rotAverageX, Vector3.up);
		yQuaternion = Quaternion.AngleAxis (rotAverageY, Vector3.left);
		transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		
		//Move rigid body
		rb.MovePosition((transform.localRotation * movement.normalized * speed * Time.deltaTime) + transform.position);
		rb.velocity = new Vector3(0,0,0);
	}
}
