using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMusic : MonoBehaviour
{
    private AudioSource musicSource;
    [SerializeField] AudioClip MainMenu_Music;
    [SerializeField] AudioClip Battle_Music;

    private void Start()
    {
        if(FindObjectsOfType<AudioSource>().Length >= 2)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        musicSource = GetComponent<AudioSource>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        // Add your logic here to execute after the scene loads
        if (scene.name == "MainMenu1")
        {
            musicSource.clip = MainMenu_Music;
            musicSource.Play();
        }
        if (scene.name == "Lobby")
        {
            musicSource.clip = Battle_Music;
            musicSource.Play();
        }
    }
}
