using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordmenEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 3f;

    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("DAMAGE TAKEN");

        if(currentHealth <= 0)
        {
            //die
            Die();
        }
    }

    private  void Die()
    {
        Destroy(gameObject);
    }
}
