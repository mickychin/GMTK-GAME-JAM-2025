using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    public Animator transition;

    public float transitiontime = 1f;

    public void StartGame()
    {
        StartCoroutine(LoadLevel(2));
        //SceneManager.LoadScene(2);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitiontime);

        SceneManager.LoadScene(levelIndex);

    }

    public void GotoBossFight()
    {
        StartCoroutine(LoadLevel(7));
        //SceneManager.LoadScene(2);
    }


} 
