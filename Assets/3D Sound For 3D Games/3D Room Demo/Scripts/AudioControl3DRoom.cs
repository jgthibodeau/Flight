using UnityEngine;
using System.Collections;
using EcholocationLtd3DFor3DUnity4;

public class AudioControl3DRoom : MonoBehaviour 
{
	private GameObject goListener, goSource1, goSource2, goSource3, goSource4, goSource5, goSource6;
	private _3DSoundFor3DGames soundSource1, soundSource2, soundSource3, soundSource4, soundSource5, soundSource6;
	private int environmentLayerMask = 1;
	
	void Start()
	{
		//Locate and assign game objects
		goListener = GameObject.Find("Player Room");
		goSource1 = GameObject.Find("Sound 1");
		goSource2 = GameObject.Find("Sound 2");
		goSource3 = GameObject.Find("Sound 3");
		goSource4 = GameObject.Find("Sound 4");
		goSource5 = GameObject.Find("Sound 5");
		goSource6 = GameObject.Find("Sound 6");
		
		//Get instances of plugin and pass layer masks for each sound source
		if (goSource1 != null)
		{
			soundSource1 = goSource1.GetComponent<_3DSoundFor3DGames>();
			soundSource1.Setup3D(environmentLayerMask);
		}
		if (goSource2 != null)
		{
			soundSource2 = goSource2.GetComponent<_3DSoundFor3DGames>();
			soundSource2.Setup3D(environmentLayerMask);
		}
		if (goSource3 != null)
		{
			soundSource3 = goSource3.GetComponent<_3DSoundFor3DGames>();
			soundSource3.Setup3D(environmentLayerMask);
		}
		if (goSource4 != null)
		{
			soundSource4 = goSource4.GetComponent<_3DSoundFor3DGames>();
			soundSource4.Setup3D(environmentLayerMask);
		}
		if (goSource5 != null)
		{
			soundSource5 = goSource5.GetComponent<_3DSoundFor3DGames>();
			soundSource5.Setup3D(environmentLayerMask);
		}
		if (goSource6 != null)
		{
			soundSource6 = goSource6.GetComponent<_3DSoundFor3DGames>();
			soundSource6.Setup3D(environmentLayerMask);
		}
	}
	
	void Update ()
	{
		//Set listeenr position for each sound source
		if (goListener != null) 
		{
			soundSource1.SetListenerPosition(goListener);
			soundSource2.SetListenerPosition(goListener);
			soundSource3.SetListenerPosition(goListener);
			soundSource4.SetListenerPosition(goListener);
			soundSource5.SetListenerPosition(goListener);
			soundSource6.SetListenerPosition(goListener);
		}
		
		//Set listeenr position for each sound source and start playback
		if (goSource1 != null) soundSource1.Play3D(goSource1);
		if (goSource2 != null) soundSource2.Play3D(goSource2);
		if (goSource3 != null) soundSource3.Play3D(goSource3);
		if (goSource4 != null) soundSource4.Play3D(goSource4);
		if (goSource5 != null) soundSource5.Play3D(goSource5);
		if (goSource6 != null) soundSource6.Play3D(goSource6);
		
		//Quit if 'Escape' button is pressed
		if (Input.GetKey("escape"))	Application.Quit();
	}
}