using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Controller inputHandler;

    //movement
    public float movespeed = 5f;
    public float facingDirection = 1f;
    public bool isFacingRight = true;

    // Jumping
    [Header("Jumping Mechanics")]
    [Space]
    public float jumpforce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    // Health and Attack
    public float Attack = 10f;
    public float maxHealth = 100f;
    public float currentHealth;

    // Wall climbing
    [Header("Wall Mechanics")]
    [Space]
    public Transform WallCheck;
    public float WallSlideSpeed = 2f;
    public bool isWallSliding;
    public bool isWallJumping;
    public float walljumpTimer;
    public float walljumpTime = 0.5f;
    public float walljumpdirection = 1f;
    public Vector2 wallJumpForce = new Vector2(4f, 8f);

    // Dash
    [Header("Dash Mechanics")]
    [Space]
    public bool canDash = true;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;


    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<Controller>();
    }

    private void Update()
    {
        if (!isWallJumping)
        {
            move();
            Flip();
        }
        jump();
        wallSliding();
        wallJumping();
        FlipCharacter();
        if (inputHandler.dash && canDash)
        {
            StartCoroutine(Dash());
        }

    }

    private void FlipCharacter()
    {
        if ((inputHandler.movement.x > 0 && facingDirection < 0))
        {
            Flip();
        }
        else if ((inputHandler.movement.x < 0 && facingDirection > 0))
        {
            Flip();
        }
    }

    public void move()
    {
        if (isDashing)
        {
            return;
        }
        rb.linearVelocityX = inputHandler.movement.x * movespeed;
    }

    public void Flip()
    {
        if (isFacingRight && inputHandler.movement.x < 0f || !isFacingRight && inputHandler.movement.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }


    public void jump()
    {
        if (isDashing)
        {
            return;
        }

        if (inputHandler.jumpInput && isGrounded())
        {
            rb.linearVelocityY = jumpforce;
        }
    }

    #region Wall Mechanics
    public void wallSliding()
    {
        if (isWall() && !isGrounded() && inputHandler.movement != Vector2.zero)
        {
            rb.linearVelocityY = Mathf.Clamp(rb.linearVelocity.y, -WallSlideSpeed, float.MaxValue);
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    public void wallJumping()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            walljumpdirection = -transform.localScale.x;
            walljumpTimer = walljumpTime;

            CancelInvoke(nameof(stopWallJumping));
        }
        else if (walljumpTimer > 0f)
        {
            walljumpTimer -= Time.deltaTime;
        }

        if (inputHandler.jumpInput && walljumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(walljumpdirection * wallJumpForce.x, wallJumpForce.y);
            walljumpTimer = 0f;

            if (transform.localScale.x != walljumpdirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(stopWallJumping), walljumpTime + 0.1f);
        }

    }

    public void stopWallJumping()
    {
        isWallJumping = false;
    }
    #endregion

    #region Attack, Damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damagable = other.GetComponent<IDamageable>();
        if (damagable != null)
        {
            damagable.TakeDamage(Attack);
        }
    }
    #endregion

    #region Dash Mechanics

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float gscale = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashForce, 0);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = gscale;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion

    public bool isGrounded()
    {
        return Physics2D.Raycast(groundCheck.transform.position, Vector2.down, 0.1f, groundLayer);
    }

    public bool isWall()
    {
        return Physics2D.OverlapCircle(WallCheck.position, 0.1f, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(WallCheck.position, WallCheck.position + Vector3.right * 0.1f);
    }
}
public interface IDamageable
{
    void TakeDamage(float damage);
}