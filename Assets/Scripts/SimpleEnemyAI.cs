using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    // References
    private Transform player;
    private Rigidbody2D rb;
    
    // Movement settings
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 10f;
    
    // Attack settings
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;
    private float lastAttackTime = 0f;
    
    // Debugging
    [SerializeField] private bool showDebugGizmos = true;
    
    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        
        // If no Rigidbody2D found, add one
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // Disable gravity for top-down movement
            rb.freezeRotation = true; // Prevent rotation
        }
        
        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Player found successfully");
        }
        else
        {
            Debug.LogError("No GameObject with 'Player' tag found! Make sure your player has the Player tag.");
        }
    }
    
    void Update()
    {
        // Skip if no player found
        if (player == null) return;
        
        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Debug distance information
        if (showDebugGizmos)
        {
            Debug.DrawLine(transform.position, player.position, Color.yellow);
            Debug.Log($"Distance to player: {distanceToPlayer}");
        }
        
        // Only activate AI when player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            // If in attack range, try to attack
            if (distanceToPlayer <= attackRange)
            {
                TryAttack();
                // Stop moving when attacking
                rb.linearVelocity = Vector2.zero;
            }
            // Otherwise, move toward the player
            else
            {
                MoveTowardPlayer();
            }
        }
        else
        {
            // Player out of range, stop moving
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    void MoveTowardPlayer()
    {
        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Apply movement
        rb.linearVelocity = direction * moveSpeed;
        
        // Debug velocity
        Debug.Log($"Moving with velocity: {rb.linearVelocity}");
        
        // Flip sprite based on movement direction
        if (direction.x > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    
    void TryAttack()
    {
        // Check if cooldown has elapsed
        if (Time.time - lastAttackTime < attackCooldown) return;
        
        // Perform attack
        lastAttackTime = Time.time;
        
        Debug.Log("Enemy attacking player!");
        
        // Face the player
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
        // Deal damage to player if they have a health component
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Dealt {attackDamage} damage to player");
        }
    }
    
    // Visualize detection and attack ranges in the editor
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;
        
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}