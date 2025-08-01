using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordmenEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 3f;
    public float currentHealth;
    Animator animator;
    Rigidbody2D rb;
    [Header("Attacks")]
    [SerializeField] float DashDistance;
    [SerializeField] float JumpHeight;
    [SerializeField] float jumpAttackDashDistance;
    private bool isAttacking;

    [Header("Player Detection")]
    [SerializeField] int playersLayer;
    [SerializeField] float EyeSightRange = 5f;
    [SerializeField] LayerMask ThingEnemyCanSee;
    RaycastHit2D hits;
    private bool canSeePlayer;
    private PlayerControl playerControl;

    [Header("MoveMent")]
    [SerializeField] private Transform wallCheckTrans;
    [SerializeField] private Transform GroundCheckTrans;
    [SerializeField] LayerMask WallAndGroundLayer;
    [SerializeField] private float speed;

    [Header("Stance")]
    [SerializeField] private float MaxStance;
    public float Stance;
    private bool isBlocking;
    private bool isStance_Break;

    private int currentCombo;

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
        SeePlayerCheck();

        Patrol();
    }

    public void Damage(float damageAmount)
    {
        if (isStance_Break)
        {
            Die();
        }

        currentCombo = 0;
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

        isBlocking = isBLock();
        animator.SetBool("Block", isBlocking);
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
        if (AttackMoveSet == 3)
        {
            //dash
            //rb.velocity = new Vector2(DashDistance * transform.localScale.x, rb.velocity.y);
        }
        else if (AttackMoveSet == 4)
        {
            rb.velocity = new Vector2(jumpAttackDashDistance * transform.localScale.x, JumpHeight);
        }
    }

    private  void Die()
    {
        Destroy(gameObject);
    }

    private void AttackPattern()
    {
        if(canSeePlayer == true)
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
    
    private void SeePlayerCheck()
    {
        Vector2 dir = playerControl.transform.position - transform.position;
        hits = Physics2D.Raycast(transform.position, dir, EyeSightRange, ThingEnemyCanSee);
        //Debug.Log(hits.collider.name);
        if (hits && hits.collider.GetComponent<PlayerControl>())
        {
            canSeePlayer = true;
            if (isAttacking == false)
            {
                Attack(Random.Range(1, 5));
            }
        }
        else
        {
            canSeePlayer = false;
        }
    }

    private void Patrol()
    {
        if (!canSeePlayer)
        {
            rb.velocity = new Vector2(speed * transform.localScale.x, rb.velocity.y);
        }

        float wallCheckRadius = 0.1f;
        hits = Physics2D.CircleCast(wallCheckTrans.position, wallCheckRadius, transform.right, 0f, WallAndGroundLayer); //check wall
        if (hits && !canSeePlayer)
        {
            //flip!
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
        hits = Physics2D.CircleCast(GroundCheckTrans.position, wallCheckRadius, transform.right, 0f, WallAndGroundLayer); //check ground infront
        if (!hits && !canSeePlayer)
        {
            //flip!
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }

    private int getAttackPattern()
    {
        int[] patterns = { 331, 333, 313, 133, 333, 333 }; //the attack pattern is actually read from back to front
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
