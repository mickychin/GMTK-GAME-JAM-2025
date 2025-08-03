using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int Player_HP;

    public int CurrentLV;

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length >= 2)
        {
            Destroy(gameObject);
        }
        Player_HP = FindObjectOfType<PlayerControl>().maxHP;
        DontDestroyOnLoad(gameObject);
    }
}
