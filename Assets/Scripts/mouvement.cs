using System;
using UnityEngine;
using Mirror;
using UnityEditor;

public class Mouvement : NetworkBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isLocalPlayer)
            PlayerMovement();
    }

    private void PlayerMovement()
    {
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
