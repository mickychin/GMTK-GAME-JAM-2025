using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Random;

public class Gateway : MonoBehaviour
{
    public GameMaster gameMaster;
    public int PlayerLayer = 8;
    
    public int Wowwyveryrandomfloatverycoolfrfrfrfr;

    void OnTriggerEnter2D(Collider2D collideee)
    {
        if(collideee.gameObject.layer == PlayerLayer)
        {
            Wowwyveryrandomfloatverycoolfrfrfrfr = UnityEngine.Random.Range(0, 3);
            Debug.Log(Wowwyveryrandomfloatverycoolfrfrfrfr);
            gameMaster.level = gameMaster.level + 1f;
            SceneManager.LoadScene(1);
        }
    }

}
