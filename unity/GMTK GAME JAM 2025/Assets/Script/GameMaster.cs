using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public float level = 0f;    
    public PlayerControl playerobj;
    private float Health, MaxHealth, Width = 500f, Height = 50f;
    public GameObject DeathScreen;
    [SerializeField] private RectTransform healthBar;

    void Start()
    {
        DeathScreen.SetActive(false);
    }

    void Update()
    {
        MaxHealth = playerobj.maxHP;

        Health = playerobj.currentHP;

        float newWidth = (Health / MaxHealth) * Width;

        healthBar.sizeDelta = new Vector2(newWidth, Height);

    }

    public void Die()
    {
        Debug.Log(playerobj.currentHP);
            DeathScreen.SetActive(true);
    }
}
