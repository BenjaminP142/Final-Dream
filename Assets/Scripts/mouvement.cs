using System;
using Mono.Cecil.Cil;
using UnityEngine;

public class Mouvement : MonoBehaviour
{
    public Rigidbody2D rb;
    public bool isGrounded = false;

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
        isGrounded = false;
        
        if (Input.GetButton("Horizontal"))
        {
            transform.Translate(7f * Input.GetAxis("Horizontal") * Time.deltaTime, 0, 0);
        }
        
        Vector2 point = transform.position + Vector3.down * 0.2f;
        Vector2 size = new Vector2(transform.localScale.x, transform.localScale.y);

        isGrounded = Physics2D.OverlapBox(point, size, 0);
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
        }
    }
}    
