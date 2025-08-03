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

    public int Enemies_N;

    void OnTriggerEnter2D(Collider2D collideee)
    {
        if(collideee.gameObject.layer == PlayerLayer)
        {
            randomSceneIndex = UnityEngine.Random.Range(minSceneIndex, maxSceneIndex);
            gameMaster.level = gameMaster.level + 1f;
            SceneManager.LoadScene(randomSceneIndex);
        }
    }

    private void Start()
    {
        Enemies_N += FindObjectsOfType<GunEnemy>().Length;
        Enemies_N += FindObjectsOfType<SwordmenEnemy>().Length;
    }

    public void Enemy_Die()
    {
        Enemies_N--;

        if(Enemies_N <= 0)
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
