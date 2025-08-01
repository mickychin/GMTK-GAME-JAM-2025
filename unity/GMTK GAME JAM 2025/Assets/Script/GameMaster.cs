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
    public Text MainDeathTxt;
    public string[] MainDeathMessages;
    public Text SubDeathTxt; 
    public string[] SubDeathMessages;
   

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

        if (Input.GetKeyDown(KeyCode.P))
        {
            Die();
        }

    }

    public void Die()
    {
        int DMR = Random.Range(0, MainDeathMessages.Length); 
        MainDeathTxt.text= MainDeathMessages[DMR];
        int DSR = Random.Range(0, SubDeathMessages.Length); 
        SubDeathTxt.text= SubDeathMessages[DSR];
        Debug.Log(playerobj.currentHP);
        DeathScreen.SetActive(true);
    }
}
