using UnityEngine;
using System.Collections;
using EcholocationLtd3DFor3DUnity4;

public class AudioControlFly : MonoBehaviour 
{
	private GameObject goListener, goSoundSource;
	private GameObject[] goFly;
	private _3DSoundFor3DGames soundSource;
	private int environmentLayerMask = 1; //See Unity documentation on Layer Masks
	public GameObject fly; //to assign in Inspector
	public int flyCount = 30; //Start with 30 flies in the room

	
	void Start()
	{
		//Find and assign game objects
		goListener = GameObject.Find("Player Fly");
		goSoundSource = GameObject.Find("Sound Source");
		goFly = new GameObject[flyCount]; //Create array of fly game objects
		
		//Clone fly prefab depending on number set in inspector and set at a random position within the room
		for (int j = 0; j < flyCount; j++)
		{
			goFly[j] = (GameObject)Instantiate(fly, new Vector3((UnityEngine.Random.value * 4f) - 2f, (UnityEngine.Random.value * 4f) - 2f, (UnityEngine.Random.value * 4f) - 2f), Quaternion.identity);
		}
		
		//Get instance of plugin and pass layer mask
		soundSource = goSoundSource.GetComponent<_3DSoundFor3DGames>();
		soundSource.Setup3D(environmentLayerMask);
	}
	
	void Update ()
	{
		if (goListener != null) 
		{
			//Set listener/player position
			soundSource.SetListenerPosition(goListener);
		}

		if (goFly[0] != null) 
		{
			//start playing/update 3D sound position of each fly, playing each at half volume
			for(int k = 0; k < flyCount; k++)
			{
				soundSource.Play3D(goFly[k], 0.5f); //May need to reduce volume further for more flies to prevent clipping
			}
		}
		
		//Quit if 'Escape' button is pressed
		if (Input.GetKey("escape"))	Application.Quit();
	}
}
