using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMusic : MonoBehaviour
{
    private static PlayMusic instance;

    private AudioSource musicSource;
    [SerializeField] AudioClip MainMenu_Music;
    [SerializeField] AudioClip Battle_Music;
    [SerializeField] AudioClip Boss_Music;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
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
        if (scene.name == "Boss")
        {
            musicSource.clip = Boss_Music;
            musicSource.Play();
        }
    }

    public void playMainMusic()
    {
        musicSource.clip = MainMenu_Music;
        musicSource.Play();
    }

    public void playBattleMusic()
    {
        musicSource.clip = Battle_Music;
        musicSource.Play();
    }

    public void playBossMusic()
    {
        musicSource.clip = Boss_Music;
        musicSource.Play();
    }
}
