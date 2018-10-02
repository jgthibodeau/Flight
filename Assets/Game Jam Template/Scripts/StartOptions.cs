using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class StartOptions : MonoBehaviour {
	public int sceneToStart = 1;										//Index number in build settings of scene to load if changeScenes is true
	public bool changeScenes;											//If true, load a new scene when Start is pressed, if false, fade out UI and continue in single scene
	public bool changeMusicOnStart;										//Choose whether to continue playing menu music or start a new music clip

	[HideInInspector] public bool inMainMenu = true;					//If true, pause button disabled in main menu (Cancel in input manager, default escape key)
	[HideInInspector] public Animator animColorFade; 					//Reference to animator which will fade to and from black when starting game.
	[HideInInspector] public Animator animMenuAlpha;					//Reference to animator that will fade out alpha of MenuPanel canvas group
	 public AnimationClip fadeColorAnimationClip;		//Animation clip fading to color (black default) when changing scenes
	[HideInInspector] public AnimationClip fadeAlphaAnimationClip;		//Animation clip fading out UI elements alpha

	private PlayMusic playMusic;										//Reference to PlayMusic script
	private float fastFadeIn = .01f;									//Very short fade time (10 milliseconds) to start playing music immediately without a click/glitch
	private ShowPanels showPanels;										//Reference to ShowPanels script on UI GameObject, to show and hide panels
	private Pause pause;
    private Quality quality;

    public GameObject loadingScreen;
    public TextMeshProUGUI loadingScreenText;

    void Awake() {
		showPanels = GetComponent<ShowPanels> ();
		playMusic = GetComponent<PlayMusic> ();
        pause = GetComponent<Pause>();
        quality = GetComponent<Quality>();
    }


	public void StartButtonClicked() {
        //Pause button now works if escape is pressed since we are no longer in Main menu.
        inMainMenu = false;

        //Hide the main menu UI element
        showPanels.Back();

        //If changeMusicOnStart is true, fade out volume of music group of AudioMixer by calling FadeDown function of PlayMusic, using length of fadeColorAnimationClip as time. 
        //To change fade time, change length of animation "FadeToColor"
        if (changeMusicOnStart) {
			playMusic.FadeDown (fadeColorAnimationClip.length);
		}

		//If changeScenes is true, start fading and change scenes halfway through animation when screen is blocked by FadeImage
		if (changeScenes) {
            //Use invoke to delay calling of LoadDelayed by the length of fadeColorAnimationClip
            Invoke("LoadDelayed", fadeColorAnimationClip.length);

            //Set the trigger of Animator animColorFade to start transition to the FadeToOpaque state.
            animColorFade.SetTrigger ("fade");
		}

		//If changeScenes is false, call StartGameInScene
		else {
			//Call the StartGameInScene function to start game without loading a new scene.
			StartGameInScene ();
		}
	}

    void OnEnable() {
        SceneManager.sceneLoaded += SceneWasLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= SceneWasLoaded;
    }

    //Once the level has loaded, check if we want to call PlayLevelMusic
    void SceneWasLoaded(Scene scene, LoadSceneMode mode) {
		//if changeMusicOnStart is true, call the PlayLevelMusic function of playMusic
		if (changeMusicOnStart) {
			playMusic.PlayLevelMusic ();
		}
    }

    public void EnableLoadingScreen()
    {
        Debug.Log("Loading...");
        loadingScreen.SetActive(true);
    }

    public void DisableLoadingScreen()
    {
        Debug.Log("Loading Done");
        loadingScreen.SetActive(false);
    }

    public void LoadDelayed() {
        //Load the selected scene, by scene index number in build settings
        //SceneManager.LoadScene (sceneToStart);
        EnableLoadingScreen();
        StartCoroutine(LoadNewScene());
    }


    IEnumerator LoadNewScene()
    {
        //// Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        //AsyncOperation async = SceneManager.LoadSceneAsync(sceneToStart);

        //// While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        //while (!async.isDone)
        //{
        //    //yield return new WaitForSeconds(1);
        //    yield return null;
        //}

        //animColorFade.SetTrigger("unfade");
        //DisableLoadingScreen();

        AsyncOperation AO = SceneManager.LoadSceneAsync(sceneToStart);
        AO.allowSceneActivation = false;

        int loadingPercent = (int)(AO.progress * 100f);

        while (AO.progress < 0.9f)
        {
            loadingPercent = (int)(AO.progress * 100f);
            Debug.Log("loading progress without allowSceneActivation: " + loadingPercent);
            loadingScreenText.text = loadingPercent + "%";
            //yield return new WaitForSeconds(1);
            yield return null;
        }

        AO.allowSceneActivation = true;

        while (!AO.isDone)
        {
            loadingPercent = (int)(AO.progress * 100f);
            Debug.Log("loading progress with allowSceneActivation: " + loadingPercent);
            loadingScreenText.text = loadingPercent + "%";
            //yield return new WaitForSeconds(1);
            yield return null;
        }

        //Fade the loading screen out here
        animColorFade.SetTrigger("unfade");
        DisableLoadingScreen();
    }

    public void StartGameInScene() {
		//Pause button now works if escape is pressed since we are no longer in Main menu.
		inMainMenu = false;
		pause.UnPause ();

		//If changeMusicOnStart is true, fade out volume of music group of AudioMixer by calling FadeDown function of PlayMusic, using length of fadeColorAnimationClip as time. 
		//To change fade time, change length of animation "FadeToColor"
		if (changeMusicOnStart) 
		{
			//Wait until game has started, then play new music
			Invoke ("PlayNewMusic", fadeAlphaAnimationClip.length);
		}
		//Set trigger for animator to start animation fading out Menu UI
		animMenuAlpha.SetTrigger ("fade");
		Debug.Log ("Game started in same scene! Put your game starting stuff here.");
	}
		
	public void PlayNewMusic() {
		//Fade up music nearly instantly without a click 
		playMusic.FadeUp (fastFadeIn);
		//Play music clip assigned to mainMusic in PlayMusic script
		playMusic.PlaySelectedMusic (1);
    }
}
