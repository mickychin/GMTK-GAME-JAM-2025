using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private float horizontal;
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

    [SerializeField] private Animator animator;
    private bool isRolling;
    [SerializeField] private float rollDistance;
    public bool IFrame;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
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
        }
    }

    private void FixedUpdate()
    {
        if (!isRolling)
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

    private void FinishRolling()
    {
        isRolling = false;
    }

    private void SetIFrameTrue()
    {
        IFrame = true;
    }

    private void SetIFrameFalse()
    {
        IFrame = false;
    }
}