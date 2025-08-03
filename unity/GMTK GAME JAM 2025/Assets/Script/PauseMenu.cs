using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool MenuOpen = false;

    public GameObject MenuUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuOpen) 
            {
                Resume();
            } else {
                Menu();
            }
        }
    }

    void Resume()
    {
        MenuUI.SetActive(false);
        MenuOpen = false;
    }

    void Menu()
    {
        MenuUI.SetActive(true);
        MenuOpen = true;
    }

    public void ReturntoMenu()
    {
        FindObjectOfType<PlayMusic>().playMainMusic();
        SceneManager.LoadScene(0);
    }
}
