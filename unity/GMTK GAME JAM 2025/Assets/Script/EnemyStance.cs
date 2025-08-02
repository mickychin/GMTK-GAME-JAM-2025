using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStance : MonoBehaviour
{
    [Header("UI Ref")]
    [SerializeField] private RectTransform stanceBarRect;
    [SerializeField] private float barMaxWidth = 100f; 
    [SerializeField] private float barHeight = 10f; 
    [SerializeField] private float SmoothingSpeed = 10f;

    [SerializeField] private Vector3 offset = new Vector3(0, 3.0f, 0); 

    private Transform enemyTransform; 
    private float SmoothedWidth;

    public void SetEnemy(Transform targetEnemy, float inCurrentStance, float inMaxStance)
    {
        enemyTransform = targetEnemy;
        SmoothedWidth = (inCurrentStance / inMaxStance) * barMaxWidth;
        stanceBarRect.sizeDelta = new Vector2(SmoothedWidth,barHeight);
    }

    public void UpdateStanceBar(float currentstance, float maxstance)
    {
        float targetWidth = (currentstance / maxstance) * barMaxWidth;
        SmoothedWidth = targetWidth;
    }

    void LateUpdate()
    {
        if(enemyTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        float currentBarWidth = stanceBarRect.sizeDelta.x;
        float targetBarWidth = ((barMaxWidth - SmoothedWidth) / barMaxWidth) * barMaxWidth;

        float NewSmoothedWidth = Mathf.Lerp(currentBarWidth, targetBarWidth, Time.deltaTime * SmoothingSpeed);
        stanceBarRect.sizeDelta = new Vector2(NewSmoothedWidth,barHeight);

        transform.position = enemyTransform.position + offset;

        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }

    }
}
