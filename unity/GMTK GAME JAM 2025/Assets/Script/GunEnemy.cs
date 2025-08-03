using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GunEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 3f;
    public float currentHealth;
    Animator animator;
    Rigidbody2D rb;

    public GameObject Corspe;

    [Header("Attacks")]
    [SerializeField] float DashDistance;
    [SerializeField] float JumpHeight;
    [SerializeField] float jumpAttackDashDistance;
    [SerializeField] float attackDamage;
    [SerializeField] GameObject bulletPrefabs;
    [SerializeField] float bulletForce;
    [SerializeField] Transform firePoint;
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
    [SerializeField] private float StancePerSecond = 5f;
    public float Stance;
    private bool isBlocking;
    private bool isStance_Break;

    public int currentCombo;

    [Header("Health Bar UI")]
    [SerializeField] private GameObject HealthBarUI;
    private EnemyHP instantiatedHealthBar;

    [Header("Stance Bar UI")]
    [SerializeField] private GameObject StanceBarUI;
    private EnemyStance instantiatedStanceBar;

    // Start is called before the first frame update
    void Start()
    {
        Stance = MaxStance;
        playerControl = FindObjectOfType<PlayerControl>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        InstantiateHealthBar();
        InstantiateStanceBar();
    }

    private void Update()
    {
        SeePlayerCheck();

        Patrol();

        Stance = Mathf.Min(Stance + StancePerSecond * Time.deltaTime, MaxStance);
        if (instantiatedStanceBar != null)
        {
            instantiatedStanceBar.UpdateStanceBar(Stance, MaxStance);
        }
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
        //animator.SetTrigger("Get_Hit");
        if (isBlocking)
        {
            Stance -= damageAmount;

            if (instantiatedStanceBar != null)
            {
                instantiatedStanceBar.UpdateStanceBar(Stance, MaxStance);
            }

            if (Stance <= 0)
            {
                //staggered
                Debug.Log("BREAK");
                animator.SetTrigger("Stance_Break");
                isStance_Break = true;
            }
            else
            {
                animator.SetTrigger("Get_Hit");
            }
            return;
        }
        currentHealth -= damageAmount;
        animator.SetTrigger("Get_Hit");
        Debug.Log("DAMAGE TAKEN");

        if (instantiatedHealthBar != null)
        {
            instantiatedHealthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            //die
            Die();
        }
    }

    public void GotBlocked(float BlockKB)
    {
        rb.velocity = new Vector2(-BlockKB * transform.localScale.x, rb.velocity.y);

        Stance -= attackDamage;

        if (instantiatedStanceBar != null)
        {
            instantiatedStanceBar.UpdateStanceBar(Stance, MaxStance);
        }

        if (Stance <= 0)
        {
            //staggered
            animator.SetTrigger("Stance_Break");
            isStance_Break = true;
        }
    }

    private bool isBLock() //theres a chance of blocking after getting hit and not
    {
        if (Random.Range(1, 101) > 10) //90 percent to block
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
        if (playerControl.transform.position.x > transform.position.x)
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

    private void Die()
    {
        FindObjectOfType<Gateway>().Enemy_Die();

        if (instantiatedHealthBar != null)
        {
            Destroy(instantiatedHealthBar.gameObject);
        }
        if (instantiatedStanceBar != null)
        {
            Destroy(instantiatedStanceBar.gameObject);
        }
        if (Corspe != null)
        {
            GameObject EnemyCorspe = Instantiate(Corspe, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void AttackPattern()
    {
        if (canSeePlayer == true)
        {
            if (currentCombo == 0)
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
                if (canSeePlayer == true)
                {
                    if (currentCombo == 0)
                    {
                        currentCombo = getAttackPattern();
                    }
                    Attack((int)((float)currentCombo % 10));
                    currentCombo /= 10;
                }
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

    private void InstantiateHealthBar()
    {
        if (HealthBarUI != null)
        {
            GameObject healthBarObj = Instantiate(HealthBarUI);
            instantiatedHealthBar = healthBarObj.GetComponent<EnemyHP>();

            if (instantiatedHealthBar != null)
            {
                instantiatedHealthBar.SetEnemy(this.transform, currentHealth, maxHealth);
            }
        }

    }

    private void InstantiateStanceBar()
    {
        if (StanceBarUI != null)
        {
            GameObject stanceBarObj = Instantiate(StanceBarUI);
            instantiatedStanceBar = stanceBarObj.GetComponent<EnemyStance>();

            if (instantiatedStanceBar != null)
            {
                instantiatedStanceBar.SetEnemy(this.transform, Stance, MaxStance);
            }
        }

    }

    void OnDestroy()
    {
        if (instantiatedHealthBar != null)
        {
            Destroy(instantiatedHealthBar.gameObject);
        }
        if (instantiatedStanceBar != null)
        {
            Destroy(instantiatedStanceBar.gameObject);
        }
    }

    private int getAttackPattern()
    {
        int[] patterns = { 1 }; //the attack pattern is actually read from back to front
        int i = Random.Range(0, patterns.Length);
        return patterns[i];
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
        bullet_rb.rotation = bulletAngle - 90f;
        bullet_rb.AddForce(ShootDirect * bulletForce, ForceMode2D.Impulse);
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
