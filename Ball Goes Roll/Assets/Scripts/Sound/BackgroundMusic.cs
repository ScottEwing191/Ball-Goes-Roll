using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] AudioClip[] backgroundMusic;    // each element in the array is a different level in the game
    AudioSource musicSource;
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex < backgroundMusic.Length) {
            musicSource.clip = backgroundMusic[buildIndex];
            musicSource.Play();
        }
        else {
            Debug.LogError("The scen build index exedes the number of track in the music array");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
