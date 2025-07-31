using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordmenEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 3f;
    Animator animator;
    Rigidbody2D rb;
    [SerializeField] float DashDistance;
    [SerializeField] float JumpHeight;
    [SerializeField] float jumpAttackDashDistance;

    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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

    private void Attack(int AttackMoveSet) //move set start at 1
    {
        Debug.Log(AttackMoveSet);
        animator.SetInteger("Attack", AttackMoveSet);
        if (AttackMoveSet == 3)
        {
            //dash
            rb.velocity = new Vector2(DashDistance, rb.velocity.y);
        }
        else if (AttackMoveSet == 4)
        {
            rb.velocity = new Vector2(jumpAttackDashDistance, JumpHeight);
        }
    }

    private  void Die()
    {
        Destroy(gameObject);
    }

    private void AttackPattern()
    {
        Attack(Random.Range(1, 5));
    }
    
}
