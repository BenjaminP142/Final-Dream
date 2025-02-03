using UnityEngine;

public class Mouvement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public float Runspeed = 40f;
    private float horizontalMove = 0f;
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool crouch = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMovement();
        UpdateJumpAnimation();
    }

    private void PlayerMovement()
    {
        horizontalMove = Input.GetAxis("Horizontal") * Runspeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButton("Horizontal"))
        {
            transform.Translate(7f * Input.GetAxis("Horizontal") * Time.deltaTime, 0, 0);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouch = true;
            animator.SetBool("IsCrouching", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouch = false;
            animator.SetBool("IsCrouching", false);
        }
        
        if (horizontalMove > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalMove < 0 && facingRight)
        {
            Flip();
        }
    }

    private void UpdateJumpAnimation()
    {
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 2f) // Montée
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("JumpMid", false);
                animator.SetBool("JumpFall", false);
            }
            else if (rb.linearVelocity.y <= 2f && rb.linearVelocity.y >= -2f) // Sommet du saut
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("JumpMid", true);
                animator.SetBool("JumpFall", false);
            }
            else if (rb.linearVelocity.y < -2f) // Descente
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("JumpMid", false);
                animator.SetBool("JumpFall", true);
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("JumpMid", false);
        if (isGrounded)
        {
            animator.SetBool("JumpFall", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            isGrounded = true;
            OnLanding();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            isGrounded = false;
        }
    }
}
