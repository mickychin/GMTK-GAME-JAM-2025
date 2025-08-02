using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Random;

public class Gateway : MonoBehaviour
{
    public GameMaster gameMaster;
    public int PlayerLayer = 8;

    private int randomSceneIndex; 

    public int minSceneIndex = 0;
    public int maxSceneIndex = 3;
    
    public int Wowwyveryrandomfloatverycoolfrfrfrfr;

    void OnTriggerEnter2D(Collider2D collideee)
    {
        if(collideee.gameObject.layer == PlayerLayer)
        {
            randomSceneIndex = UnityEngine.Random.Range(minSceneIndex, maxSceneIndex);
            gameMaster.level = gameMaster.level + 1f;
            SceneManager.LoadScene(randomSceneIndex);
        }
    }

}
