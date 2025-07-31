using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordmenEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 3f;
    Animator animator;

    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        Attack(1);
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

    private void Attack(int AttackMoveSet) //move set start at 1
    {
        animator.SetInteger("Attack", AttackMoveSet);
        if (AttackMoveSet == 1)
        {
            //overhead swing
        }
    }

    private  void Die()
    {
        Destroy(gameObject);
    }
}
