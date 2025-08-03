using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroText : MonoBehaviour
{
    [SerializeField] private GameObject IntroTextUIObj;
    [SerializeField] private Text IntroTextUI;
    [SerializeField] private float expandDuration = 0.75f; 
    [SerializeField] private float holdDuration = 2.0f; 
    [SerializeField] private float squishDuration = 0.75f; 
    private Vector3 originalScale; 
    
    void Awake()
    {
        if (IntroTextUI == null)
        {
            IntroTextUI = GetComponent<Text>();
        }

        originalScale = IntroTextUI.transform.localScale;
        IntroTextUI.transform.localScale = new Vector3(0, originalScale.y, originalScale.z);
    }

    void Start()
    {
        IntroTextUIObj.SetActive(true);
        StartBossIntro();

    }

    public void StartBossIntro()
    {
        StopAllCoroutines();
        StartCoroutine(BossTextAnimationSequence());
    }

    private IEnumerator BossTextAnimationSequence()
    {
        float timer = 0f;
        Vector3 startScale = new Vector3(0, originalScale.y, originalScale.z);
        Vector3 targetScale = originalScale;

        while (timer < expandDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / expandDuration;
            IntroTextUI.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        IntroTextUI.transform.localScale = targetScale;

        yield return new WaitForSeconds(holdDuration); 

        timer = 0f;
        startScale = originalScale;
        targetScale = new Vector3(0, originalScale.y, originalScale.z); 

        while (timer < squishDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / squishDuration;
            IntroTextUI.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        IntroTextUI.transform.localScale = targetScale;

        IntroTextUIObj.SetActive(false);
    }
}
