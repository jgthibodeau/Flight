using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioController : MonoBehaviour {
    public Player player;
    public DoubleAudioSource doubleAudioSource;

    public AudioClip groundMusic, airMusic, combatMusic;
    public float airVolumeScale, minHeightDecreaseVolume, maxHeightIncreaseVolume;
    public float minVolumePercent;
    public float defaultVolume = 1;
    public float fadingTime = 1;
    public float fadingDelay = 1;
    public enum AudioState { GROUND, AIR, COMBAT, NONE }
    public AudioState currentAudioState = AudioState.NONE;
    public AudioState desiredAudioState = AudioState.NONE;

    public float musicChangeDelay;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
	// Update is called once per frame
	void Update () {
		if (player.isGrounded)
        {
            ChangeState(AudioState.GROUND);
        } else
        {
            ChangeState(AudioState.AIR);
        }

        if (player.transform.position.y > minHeightDecreaseVolume)
        {
            float volumePercent = Util.ConvertScale(minHeightDecreaseVolume, maxHeightIncreaseVolume, 1, minVolumePercent, player.transform.position.y);
            Debug.Log(volumePercent);
            //doubleAudioSource._source0.volume = Mathf.Min(doubleAudioSource._source0.volume, volumePercent * defaultVolume);
            //doubleAudioSource._source1.volume = Mathf.Min(doubleAudioSource._source1.volume, volumePercent * defaultVolume);
            doubleAudioSource.AdjustVolume(volumePercent * defaultVolume);

        }
    }

    Coroutine stateChangeRoutine;
    public void ChangeState(AudioState newState)
    {
        if (newState != desiredAudioState)
        {
            desiredAudioState = newState;
            if (stateChangeRoutine != null)
            {
                StopCoroutine(stateChangeRoutine);
            }
            stateChangeRoutine = StartCoroutine(DelayedChangeState(newState));
        }
    }

    IEnumerator DelayedChangeState(AudioState newState)
    {
        yield return new WaitForSeconds(musicChangeDelay);

        if (desiredAudioState != currentAudioState)
        {
            currentAudioState = desiredAudioState;

            AudioClip newClip = null;
            float volume = defaultVolume;

            switch (currentAudioState)
            {
                case AudioState.GROUND:
                    newClip = groundMusic;
                    break;
                case AudioState.AIR:
                    newClip = airMusic;
                    break;
                case AudioState.COMBAT:
                    newClip = combatMusic;
                    break;
                case AudioState.NONE:
                    break;
            }

            doubleAudioSource.CrossFade(newClip, volume, fadingTime, fadingDelay);
        }

        stateChangeRoutine = null;
    }
}
