using System;
using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private float horizontal;
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;

    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private Animator animator;
    private bool isRolling;
    [SerializeField] private float rollDistance;
    [SerializeField] private float IFrameTime;
    public float IFrame;
    public bool dodgeIFrame;

    [SerializeField] float friction;

    private bool isAttacking;
    [Header("Attack")]
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damageAmount = 1f;
    RaycastHit2D[] hits;

    [SerializeField] private int DamageLayer_number;
    public int maxHP;
    public int currentHP;

    public float MaxStance;
    public float Stance;
    [SerializeField] float StanceLosePerBlock = 10f;
    [SerializeField] float StancePerSecond;
    [Header("Parry")]
    [SerializeField] private float ParryCD; //parry cooldown
    [SerializeField] private float CanParry; //if more than 0 can parry
    [SerializeField] private float CanReleaseToParryTime;
    [SerializeField] private float CanReleaseToParry; //if more than 0 can release right click to use parry
    [SerializeField] private float ParryTime; //max duration of attack coming after pressing parry.
    [SerializeField] private float Parry; //if more than 0 mean it is successful parrying
    private bool IsParrying;
    [SerializeField] private float BlockKB;
    [SerializeField] private Transform ParryPos_1;
    [SerializeField] private Transform ParryPos_2;
    [SerializeField] private LayerMask AttacksLayer;
    private bool isStanceBreak;
    [SerializeField] private GameObject ParryEffect;

    private void Start()
    {
        Stance = MaxStance;
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        IFrame -= Time.deltaTime;
        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButton("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0f && !isStanceBreak && !isAttacking)
        {
            
            animator.SetTrigger("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && !isStanceBreak && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (!isRolling && !IsParrying && !isStanceBreak)
        {
            Flip();
            DodgeRollCheck();
        }

        if (Input.GetMouseButtonDown(0) && !isRolling && !isAttacking && !IsParrying && !isStanceBreak)
        {
            //Debug.Log("FIRE!");
            isAttacking = true;
            animator.SetTrigger("Attack");
            //Attack();
        }

        CanReleaseToParry -= Time.deltaTime;
        Parry -= Time.deltaTime;
        CanParry += Time.deltaTime;

        if (Input.GetMouseButtonDown(1) && !isAttacking && !isRolling && !isStanceBreak)
        {
            //right click
            rb.velocity = Vector2.zero;
            IsParrying = true;
            animator.SetBool("Parry", true);
            CanReleaseToParry = CanReleaseToParryTime;
            Stance -= StanceLosePerBlock;
            CheckStanceBreak();
        }
        if (Input.GetMouseButtonUp(1) && CanReleaseToParry > 0 && CanParry > 0)
        {
            //parry
            Parry = ParryTime;
        }
        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("Parry", false);
            IsParrying = false;
            CanParry = ParryCD * -1;
        }

        Stance = Mathf.Min(Stance + StancePerSecond * Time.deltaTime, MaxStance);

        float minSpeedForRunAnim = 0.1f;
        if (MathF.Abs(rb.velocity.x) > minSpeedForRunAnim)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        if(rb.velocity.y > 0f && !isAttacking && !IsParrying && !isRolling)
        {
            animator.SetBool("Jump", true);
        }
        else
        {
            animator.SetBool("Jump", false);
        }
    }

    private void FixedUpdate()
    {
        if (!isRolling && !IsParrying && !isStanceBreak && !isAttacking)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        if (isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x * friction, rb.velocity.y * friction);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void DodgeRollCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //dodge roll
            animator.SetTrigger("Dodge");
            isRolling = true;
            if (isFacingRight)
            {
                rb.velocity = new Vector2(rollDistance, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rollDistance * -1, rb.velocity.y);
            }
        }
    }

    private void Attack()
    {
        //rb.velocity = new Vector2();
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

        for(int i = 0; i < hits.Length; i++)
        {
            IDamagable idamagable = hits[i].collider.gameObject.GetComponent<IDamagable>();

            if(idamagable != null)
            {
                // its an enemy, the enemy take damage!
                idamagable.Damage(damageAmount);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == DamageLayer_number && IFrame <= 0 && !dodgeIFrame)
        {
            FinishAttack();
            Debug.Log("HIt");
            if (isStanceBreak)
            {
                Die();
            }

            if (IsParryContact() && Parry > 0)
            {
                //PARRY
                CancelEveryAnim();
                animator.SetTrigger("Parry_fr");
                Debug.Log("PARRY");
                CanParry = 1f;
                Stance = Stance + 15f;
                GameObject ParryVFX = Instantiate(ParryEffect, transform.position, Quaternion.identity);
                if (!collision.CompareTag("Bullet"))
                {
                    collision.gameObject.GetComponentInParent<IDamagable>().GotBlocked(BlockKB);
                }
                bulletDetection(collision.gameObject);
                return;
            }

            if (IsParrying)
            {
                //block and lose stance
                Stance -= collision.GetComponent<DamagePlayer>().Damage;
                CheckStanceBreak();
                bulletDetection(collision.gameObject);
                return;
            }

            //TAKE DAMAGE
            currentHP -= collision.GetComponent<DamagePlayer>().Damage;
            animator.SetTrigger("Damage");
            CancelEveryAnim();
            //animator.SetTrigger("Damage");
            //Debug.Log(currentHP);
            IFrame = IFrameTime;
            if(currentHP <= 0)
            {
                //die
                Die();
            }
            bulletDetection(collision.gameObject);
        }
    }

    private void bulletDetection(GameObject isBullet)
    {
        if (isBullet.CompareTag("Bullet"))
        {
            Destroy(isBullet);
        }
    }

    private void CheckStanceBreak()
    {
        if (Stance <= 0f)
        {
            CancelEveryAnim();
            isStanceBreak = true;
            animator.SetTrigger("Stance_Break");
            rb.velocity = Vector2.zero;
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        FindObjectOfType<GameMaster>().Die();
        isStanceBreak = true; //so u cannot move
        isAttacking = true; // same as above
    }

    public bool IsParryContact()
    {
        Vector2 dir = ParryPos_2.position - ParryPos_1.position;
        RaycastHit2D hit = Physics2D.Raycast(ParryPos_1.position, dir, Vector2.Distance(ParryPos_1.position, ParryPos_2.position), AttacksLayer);
        if (hit)
        {
            Debug.Log(hit.collider.name);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 dir = ParryPos_2.position - ParryPos_1.position;
        Gizmos.DrawRay(ParryPos_1.position, dir);
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }

    private void FinishRolling()
    {
        isRolling = false;
    }

    private void SetIFrameTrue()
    {
        dodgeIFrame = true;
    }

    private void SetIFrameFalse()
    {
        dodgeIFrame = false;
    }

    private void FinishAttack()
    {
        isAttacking = false;
    }

    private void FinishStanceBreak()
    {
        isStanceBreak = false;
    }

    private void CancelEveryAnim()
    {
        isAttacking = false;
        dodgeIFrame = false;
        isRolling = false;
        IsParrying = false;
    }
}