using UnityEngine;

public class Mouvement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public float speed = 40f;
    [SerializeField] private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal") * speed));
        if (Input.GetButton("Horizontal"))
        {
            transform.Translate(7f * Input.GetAxis("Horizontal") * Time.deltaTime, 0, 0);
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("floor") && !isGrounded)
        {
            isGrounded = true;
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


