using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 3f;
    public float currentHealth;
    Animator animator;
    Rigidbody2D rb;
    [Header("Attacks")]
    [SerializeField] float DashDistance;
    [SerializeField] float JumpHeight;
    [SerializeField] float jumpAttackDashDistance;
    [SerializeField] float attackDamage;
    private bool isAttacking;

    private PlayerControl playerControl;
    [SerializeField] private float attackRange;

    [Header("MoveMent")]
    [SerializeField] private Transform wallCheckTrans;
    [SerializeField] private Transform GroundCheckTrans;
    [SerializeField] LayerMask WallAndGroundLayer;
    [SerializeField] private float speed;

    [Header("Stance")]
    [SerializeField] private float MaxStance;
    [SerializeField] private float StancePerSecond = 5f;
    public float Stance;
    private bool isBlocking;
    private bool isStance_Break;

    public int currentCombo;

    // Start is called before the first frame update
    void Start()
    {
        Stance = MaxStance;
        playerControl = FindObjectOfType<PlayerControl>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        Stance = Mathf.Min(Stance + StancePerSecond * Time.deltaTime, MaxStance);

        if (Vector2.Distance(transform.position, playerControl.transform.position) < attackRange)
        {
            if (isAttacking) return;

            if (currentCombo == 0)
            {
                currentCombo = getAttackPattern();
            }
            Attack((int)((float)currentCombo % 10));
            currentCombo /= 10;
        }
        else
        {
            //walk
            Walk();
        }
    }

    public void Walk()
    {
        float X_Distance = playerControl.transform.position.x - transform.position.x;
        float dir = X_Distance / Mathf.Abs(X_Distance);
        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y);
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
    }

    public void Damage(float damageAmount)
    {
        if (isStance_Break)
        {
            Die();
        }

        currentCombo = 0;
        isBlocking = isBLock();
        animator.SetBool("Block", isBlocking);
        if (isBlocking)
        {
            Stance -= damageAmount;

            if(Stance <= 0)
            {
                //staggered
                animator.SetTrigger("Stance_Break");
                isStance_Break = true;
            }
            return;
        }
        currentHealth -= damageAmount;
        animator.SetTrigger("Get_Hit");
        Debug.Log("DAMAGE TAKEN");

        if(currentHealth <= 0)
        {
            //die
            Die();
        }
    }

    public void GotBlocked(float BlockKB)
    {
        rb.velocity = new Vector2(-BlockKB * transform.localScale.x, rb.velocity.y);

        Stance -= attackDamage;

        if (Stance <= 0)
        {
            //staggered
            animator.SetTrigger("Stance_Break");
            isStance_Break = true;
        }
    }

    private bool isBLock() //theres a chance of blocking after getting hit and not
    {
        if(Random.Range(1, 101) > 10) //90 percent to block
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Attack(int AttackMoveSet) //move set start at 1
    {
        rb.velocity = Vector2.zero;
        if(playerControl.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        }

        isAttacking = true;
        //Debug.Log(AttackMoveSet);
        animator.SetInteger("Attack", AttackMoveSet);
    }

    private  void Die()
    {
        Destroy(gameObject);
    }

    private void AttackPattern()
    {
        if(Vector2.Distance(transform.position, playerControl.transform.position) < attackRange)
        {
            if(currentCombo == 0)
            {
                currentCombo = getAttackPattern();
            }
            Attack((int)((float)currentCombo % 10));
            currentCombo /= 10;
        }
        else
        {
            animator.SetInteger("Attack", 0); //stop attack animation
            isAttacking = false;
        }
    }

    private int getAttackPattern()
    {
        int[] patterns = { 1234}; //the attack pattern is actually read from back to front
        int i = Random.Range(0, patterns.Length);
        return patterns[i];
    }

    private void Dash()
    {
        rb.velocity = new Vector2(DashDistance * transform.localScale.x, rb.velocity.y);
    }

    private void StoppedBlocking()
    {
        isBlocking = false;
    }

    private void StoppedStance_Break()
    {
        isStance_Break = false;
    }
}
