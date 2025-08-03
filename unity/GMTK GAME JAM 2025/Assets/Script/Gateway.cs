using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Random;

public class Gateway : MonoBehaviour
{
    private GameManager gameMaster;
    public int PlayerLayer = 8;

    private int randomSceneIndex; 

    public int minSceneIndex = 0;
    public int maxSceneIndex = 3;
    
    public int Wowwyveryrandomfloatverycoolfrfrfrfr;

    public int Enemies_N;

    public int BossRoom;

    public int HealAmount;

    void OnTriggerEnter2D(Collider2D collideee)
    {

        FindObjectOfType<PlayerControl>().Heal(HealAmount);
        
        if(collideee.gameObject.layer == PlayerLayer)
        {
            randomSceneIndex = UnityEngine.Random.Range(minSceneIndex, maxSceneIndex);
            gameMaster.CurrentLV = gameMaster.CurrentLV + 1;
            SceneManager.LoadScene(randomSceneIndex);
        }

        if(gameMaster.CurrentLV > 5)
        {
            SceneManager.LoadScene(BossRoom);
        }
    }

    private void Start()
    {
        gameMaster = FindObjectOfType<GameManager>();
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
