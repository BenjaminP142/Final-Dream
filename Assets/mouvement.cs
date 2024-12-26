using System;
using Mono.Cecil.Cil;
using UnityEngine;

public class mouvement : MonoBehaviour
{
    public Rigidbody2D rb;
    bool isgrounded =false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey("d"))
        {
            rb.AddForce(Vector2.right *5f );
        }

        if (Input.GetKey("a"))
        {
            rb.AddForce(Vector2.left *5f);
        }

        if (Input.GetKey("space") && isgrounded)
        {
            rb.velocity=new Vector2(rb.velocity.x,10f);
            isgrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag=="floor") 
        {
            isgrounded = true;
        }
        else
        {
            isgrounded = false;
        }
    }
    
}
