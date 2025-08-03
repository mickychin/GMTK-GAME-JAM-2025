using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    [Header("Level")]
    public float level = 0f;    
    
    [Header("Health Bar")]
    public PlayerControl playerobj;
    [SerializeField] private float Health, MaxHealth, Width = 500f, Height = 50f;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private float healthBarSmoothingSpeed = 10f; 

    [Header("Stance Bar")]
    [SerializeField] private float Stance, MaxStance, Swidth = 800f, Sheight = 50f;
    [SerializeField] private RectTransform stanceBar;
    [SerializeField] private float stanceBarSmoothingSpeed = 10f;
    [SerializeField] private GameObject SBARObj;

    [Header("Transitions")]
    public Animator transition;

    [Header("Death Screen")]
    public GameObject DeathScreen;
    public Text MainDeathTxt;
    public string[] MainDeathMessages;
    public Text SubDeathTxt; 
    public string[] SubDeathMessages;
    private bool IsDead = false;

    [Header("BossKill Screen")]
    public GameObject WinScreen;
   

    void Start()
    {
        DeathScreen.SetActive(false);
        IsDead = false;

        if (playerobj != null)
        {
            healthBar.sizeDelta = new Vector2((playerobj.currentHP / playerobj.maxHP) * Width, Height);
            stanceBar.sizeDelta = new Vector2(((playerobj.MaxStance - playerobj.Stance) / playerobj.MaxStance) * Swidth, Sheight);
        }
    }

    void Update()
    {
        MaxHealth = playerobj.maxHP;

        Health = playerobj.currentHP;

        MaxStance = playerobj.MaxStance;

        Stance = playerobj.Stance;

        float HPBarTargetIdk = (Health / MaxHealth) * Width;
        float HPBarRn = healthBar.sizeDelta.x;
        float SHealthWidth = Mathf.Lerp(HPBarRn, HPBarTargetIdk, Time.deltaTime * healthBarSmoothingSpeed);
        healthBar.sizeDelta = new Vector2(SHealthWidth, Height);

        float SBARTargetIDk = ((MaxStance - Stance) / MaxStance) * Swidth;
        float SBarRN = stanceBar.sizeDelta.x;
        float SStanceWidth = Mathf.Lerp(SBarRN, SBARTargetIDk, Time.deltaTime * stanceBarSmoothingSpeed);
        stanceBar.sizeDelta = new Vector2(SStanceWidth, Sheight);

        if(Stance < 100)
        {
            SBARObj.SetActive(true);
        } else {
            SBARObj.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Die();
        }

    }

    public void Die()
    {
        FindObjectOfType<PlayMusic>().playMainMusic();
        FindObjectOfType<GameManager>().Player_HP = (int)MaxHealth;
        FindObjectOfType<GameManager>().CurrentLV = 0;

        if (IsDead == false)
        {
            IsDead = true;
            int DMR = Random.Range(0, MainDeathMessages.Length); 
            MainDeathTxt.text= MainDeathMessages[DMR];
            int DSR = Random.Range(0, SubDeathMessages.Length); 
            SubDeathTxt.text= SubDeathMessages[DSR];
            Debug.Log(playerobj.currentHP);
            DeathScreen.SetActive(true);
            StartCoroutine(GotoMainMenu());
        }
    }

    public void BossSlain()
    {
        StartCoroutine(waitAbit());
    }

    IEnumerator GotoMainMenu()
    {
        yield return new WaitForSeconds(2);

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(0);
    }

    IEnumerator waitAbit()
    {
        yield return new WaitForSeconds(3);
        WinScreen.SetActive(true);
        StartCoroutine(GotoMainMenu());
    }
}
