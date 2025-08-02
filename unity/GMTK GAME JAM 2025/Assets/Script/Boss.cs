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
    [SerializeField] GameObject bulletPrefabs;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 20f;
    private bool isAttacking;
    private float Gravity_Scale;

    private PlayerControl playerControl;
    [SerializeField] private float attackRange;
    [SerializeField] private float speed;

    [Header("Stance")]
    [SerializeField] private float MaxStance;
    [SerializeField] private float StancePerSecond = 5f;
    public float Stance;
    private bool isBlocking;
    private bool isStance_Break;

    public int currentCombo;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

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

        isAttacking = false;
        currentCombo = 0;
        isBlocking = isBLock();
        animator.SetBool("Block", isBlocking);
        animator.SetTrigger("Hit");
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
        //animator.SetTrigger("Get_Hit");
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
        Debug.Log(AttackMoveSet);
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

        if(AttackMoveSet == 3)
        {
            ShootHorizontal();
        }
    }

    private void Shoot()
    {
        /*
        Vector2 lookDir = playerControl.transform.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefabs);
        bullet.transform.position = firePoint.transform.position;
        Rigidbody2D bullet_rb = bullet.GetComponent<Rigidbody2D>();
        bullet_rb.rotation = angle;
        bullet_rb.AddForce(lookDir.normalized * bulletForce, ForceMode2D.Impulse);
        good bye
        */
        GameObject bullet = Instantiate(bulletPrefabs);
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.localEulerAngles = firePoint.transform.localEulerAngles * transform.localScale.x;
        Rigidbody2D bullet_rb = bullet.GetComponent<Rigidbody2D>();
        bullet_rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }

    private void ShootHorizontal()
    {
        Vector2 playerPos = playerControl.transform.position - transform.position;
        Vector2 ShootDirect;
        float bulletAngle;
        if (playerPos.x > 0)
        {
        ShootDirect = Vector2.right;
        bulletAngle = 0f;
        }
        else
        {
        ShootDirect = Vector2.left;
        bulletAngle = 180f;
        }
        GameObject bullet = Instantiate(bulletPrefabs);
        bullet.transform.position = firePoint.transform.position;
        Rigidbody2D bullet_rb = bullet.GetComponent<Rigidbody2D>();
        bullet_rb.rotation = bulletAngle;
        bullet_rb.AddForce(ShootDirect * bulletForce, ForceMode2D.Impulse);
    }

    private  void Die()
    {
        Destroy(gameObject);
    }

    private void AttackPattern()
    {
        int lastDigitN = (int)((float)currentCombo % 10);
        if (Vector2.Distance(transform.position, playerControl.transform.position) < attackRange || lastDigitN == 6 || lastDigitN == 7)
        {
            if(currentCombo == 0)
            {
                currentCombo = getAttackPattern();
            }
            lastDigitN = (int)((float)currentCombo % 10);
            //Debug.Log(currentCombo);
            Attack(lastDigitN);
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
        int[] patterns = {75}; //the attack pattern is actually read from back to front
        int i = Random.Range(0, patterns.Length);
        return patterns[i];
    }

    private void Dash()
    {
        rb.velocity = new Vector2(DashDistance * transform.localScale.x, rb.velocity.y);
    }

    private void Jump()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + JumpHeight);
        Gravity_Scale = rb.gravityScale;
        rb.gravityScale = 0;
    }

    private void StopFlying()
    {
        rb.gravityScale = Gravity_Scale;
    }

    private void StoppedBlocking()
    {
        isBlocking = false;
    }

    private void StoppedStance_Break()
    {
        isStance_Break = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
