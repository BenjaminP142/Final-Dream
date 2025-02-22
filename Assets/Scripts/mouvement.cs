using UnityEngine;
using Mirror;
public class Mouvement : NetworkBehaviour
{
    public Rigidbody2D rb;
    public NetworkAnimator animator;
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
        if (!isLocalPlayer) return;
        
        PlayerMovement();
        UpdateJumpAnimation();
    }

    private void PlayerMovement()
    {
        horizontalMove = Input.GetAxis("Horizontal") * Runspeed;
        animator.animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButton("Horizontal"))
        {
            transform.Translate(7f * Input.GetAxis("Horizontal") * Time.deltaTime, 0, 0);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            isGrounded = false;
            animator.animator.SetBool("IsJumping", true);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouch = true;
            animator.animator.SetBool("IsCrouching", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouch = false;
            animator.animator.SetBool("IsCrouching", false);
        }
        
        if (horizontalMove > 0 && !facingRight)
        {
            if (isServer) RpcFlip();
            else CmdRequestFlip();
        }
        else if (horizontalMove < 0 && facingRight)
        {
            if (isServer) RpcFlip();
            else CmdRequestFlip();
        }
    }

    private void UpdateJumpAnimation()
    {
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 2f) // Montée
            {
                animator.animator.SetBool("IsJumping", true);
                animator.animator.SetBool("JumpMid", false);
                animator.animator.SetBool("JumpFall", false);
            }
            else if (rb.linearVelocity.y <= 2f && rb.linearVelocity.y >= -2f) // Sommet du saut
            {
                animator.animator.SetBool("IsJumping", false);
                animator.animator.SetBool("JumpMid", true);
                animator.animator.SetBool("JumpFall", false);
            }
            else if (rb.linearVelocity.y < -2f) // Descente
            {
                animator.animator.SetBool("IsJumping", false);
                animator.animator.SetBool("JumpMid", false);
                animator.animator.SetBool("JumpFall", true);
            }
        }
    }

    [ClientRpc]
    private void RpcFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    private void OnLanding()
    {
        animator.animator.SetBool("IsJumping", false);
        animator.animator.SetBool("JumpMid", false);
        if (isGrounded)
        {
            animator.animator.SetBool("JumpFall", false);
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
    
    [Command]
    private void CmdRequestFlip()
    {
        RpcFlip();
    }
}
