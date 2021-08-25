using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class MixLevels : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerSnapshot playSnapshot;
    [SerializeField] AudioMixerSnapshot pausedSnapshot;


    public void SetMasterVolume(float volume) {
        audioMixer.SetFloat("MasterVolume", 20f * Mathf.Log10(volume));
    }
        
    public void SetMusicVolume(float volume) {
        audioMixer.SetFloat("MusicVolume", 20f * Mathf.Log10(volume));

    }
    public void SetSfxVolume(float volume) {
        audioMixer.SetFloat("SfxVolume", 20f * Mathf.Log10(volume));
    }
    public void SetToPlaySnapshot() {
        //audioMixer.FindSnapshot("Play").TransitionTo(0.5f);
        playSnapshot.TransitionTo(0.5f);
    }
    public void SetToPauseSnapshot() {
        //audioMixer.FindSnapshot("Paused").TransitionTo(0.5f);
        pausedSnapshot.TransitionTo(0.5f);


    }
}
