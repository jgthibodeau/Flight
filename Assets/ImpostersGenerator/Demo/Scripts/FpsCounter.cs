using UnityEngine;
using System.Collections;

/// <summary>
/// Displays FPS on screen
/// </summary>
public class FpsCounter : MonoBehaviour {
	
	#region Members
	
	/// <summary>
	/// How often it will be refreshed
	/// </summary>
	[SerializeField]
	private float updateInterval = 0.5F;
	
	// FPS accumulated over the interval
	private float accumulated   = 0; 
	// Frames drawn over the interval
	private int   frames  = 0; 
	// Left time for current interval
	private float timeLeft; 
	// Current FPS
	private float fps = 0;

	public static int FPS = 0;
	
	#endregion
	
	#region Methods
	
	private void Awake() {
		this.useGUILayout = false;
	}
	
	private void OnGUI(){		
		GUI.Label(new Rect(10, Screen.height - 20, 400, 100), "FPS: " + fps.ToString());
	}

	private void Update () {
		
		timeLeft -= Time.deltaTime;
		accumulated += Time.timeScale/Time.deltaTime;
		++frames;
		
		if( timeLeft <= 0.0 ){
			fps = accumulated/frames;
			FPS = Mathf.FloorToInt(fps);
			timeLeft = updateInterval;
			accumulated = 0.0F;
			frames = 0;
		}
	}
	
	#endregion
}
