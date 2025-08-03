using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    public Animator transition;

    public float transitiontime = 1f;

    public void StartGame()
    {
        FindObjectOfType<PlayMusic>().playBattleMusic();

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
        FindObjectOfType<PlayMusic>().playBossMusic();

        StartCoroutine(LoadLevel(8));
        //SceneManager.LoadScene(2);
    }


} 
