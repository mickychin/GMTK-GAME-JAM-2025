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
    private bool dodgeIFrame;

    private bool isAttacking;
    [Header("Attack")]
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damageAmount = 1f;
    RaycastHit2D[] hits;

    [SerializeField] private int DamageLayer_number;
    [SerializeField] private int maxHP;
    private int currentHP;

    private void Start()
    {
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

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (!isRolling)
        {
            Flip();
            DodgeRollCheck();
        }

        if (Input.GetMouseButtonDown(0) && !isRolling)
        {
            //Debug.Log("FIRE!");
            animator.SetTrigger("Attack");
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (!isRolling && !isAttacking)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
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
        isAttacking = true;
        rb.velocity = new Vector2();
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
            //TAKE DAMAGE
            currentHP -= collision.GetComponent<DamagePlayer>().Damage;
            Debug.Log(currentHP);
            IFrame = IFrameTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
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
}