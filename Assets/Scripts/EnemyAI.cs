using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float pathUpdateRate = 0.5f;
    private Vector2 currentPathTarget;
    private List<Vector2> pathPoints = new List<Vector2>();
    private int currentPathIndex = 0;
    private float pathUpdateTimer = 0f;
    
    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;
    private float lastAttackTime = 0f;
    
    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    private bool playerInRange = false;
    
    // A simple grid-based pathfinding system for 2D
    private GridPathfinding pathfinder;

    IEnumerator WaitForPlayer()
    {
        while (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
                Debug.Log("Player found!");
                Vector2 centerPoint = player.position;
                float gridSize = 30f; // Adjust based on your level size
                float cellSize = 0.5f; // Smaller for more precise paths

                // Create grid centered on the player
                int gridWidth = Mathf.CeilToInt(gridSize / cellSize);
                int gridHeight = Mathf.CeilToInt(gridSize / cellSize);

                pathfinder = new GridPathfinding(gridWidth, gridHeight, cellSize);
                Debug.Log($"Grid initialized: {gridWidth}x{gridHeight} cells at {cellSize} size");

                UpdatePathfindingGrid();
                yield break; // Stop the coroutine
            }
            yield return new WaitForSeconds(0.5f); // Retry after a short delay
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(WaitForPlayer());
    }

    void Update()
    {
        // Check if player is in detection range
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= detectionRange;

        if (playerInRange)
        {
            // Update path periodically
            pathUpdateTimer -= Time.deltaTime;
            if (pathUpdateTimer <= 0)
            {
                UpdatePath();
                pathUpdateTimer = pathUpdateRate;
            }

            // Check if player is in attack range
            if (distanceToPlayer <= attackRange)
            {
                TryAttack();
            }
            else
            {
                FollowPath();
            }
        }
    }

    void UpdatePathfindingGrid()
    {
        if (pathfinder == null)
        {
            Debug.LogError("Pathfinder is null! Cannot update grid.");
            return;
        }
        // Clear the grid
        pathfinder.ClearObstacles();
        
        // Find all colliders that should be obstacles
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, detectionRange * 1.5f);
        foreach (Collider2D obstacle in obstacles)
        {
            if (obstacle.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                // Add the obstacle to the grid
                Vector2 size = obstacle.bounds.size;
                Vector2 min = obstacle.bounds.min;
                
                for (float x = min.x; x < min.x + size.x; x += 0.5f)
                {
                    for (float y = min.y; y < min.y + size.y; y += 0.5f)
                    {
                        pathfinder.AddObstacle(new Vector2(x, y));
                    }
                }
            }
        }
    }
    
    void UpdatePath()
    {
        // Update the pathfinding grid first
        UpdatePathfindingGrid();
    
        // Find path to player
        List<Vector2> newPath = pathfinder.FindPath(transform.position, player.position);
    
        if (newPath.Count > 0)
        {
            pathPoints = newPath;
            currentPathIndex = 0;
        
            // Debug path information
            Debug.Log($"Found path with {pathPoints.Count} points from {transform.position} to {player.position}");
        
            // Visualize the full path
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                Debug.DrawLine(pathPoints[i], pathPoints[i + 1], Color.blue, pathUpdateRate);
            }
        }
        else
        {
            Debug.LogWarning("No path found to player!");
        
            // If no path is found, try direct movement
            Vector2 directDir = ((Vector3)player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(directDir.x * moveSpeed, Mathf.Clamp(directDir.y * moveSpeed, -moveSpeed, moveSpeed));

        }
    }
    
    void FollowPath()
    {
        if (pathPoints.Count == 0 || currentPathIndex >= pathPoints.Count) 
        {
            Debug.Log("No valid path. Trying to update...");
            UpdatePath();
            return;
        }
    
        // Get the current target point
        currentPathTarget = pathPoints[currentPathIndex];
    
        // Calculate direction to the current target
        Vector2 direction = (currentPathTarget - (Vector2)transform.position).normalized;
        
        if (Mathf.Abs(direction.x) < 0.2f) 
        {
            direction.x = player.position.x > transform.position.x ? 1 : -1;
        }
        direction = direction.normalized;
        // Move the enemy
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, Mathf.Clamp(direction.y * moveSpeed, -moveSpeed, moveSpeed));
        
        Debug.Log($"Moving enemy. Direction: {direction}, Velocity: {rb.linearVelocity}");
    
        // Visualize the path and current target in the scene view
        Debug.DrawLine(transform.position, currentPathTarget, Color.green);
    
        // Check if we've reached the current path point
        float distanceToTarget = Vector2.Distance(transform.position, currentPathTarget);
        if (distanceToTarget < 0.1f)
        {
            // Advance to next point
            currentPathIndex++;
        
            if (currentPathIndex >= pathPoints.Count)
            {
                Debug.Log("Reached end of path. Updating...");
                UpdatePath();
            }
        }
    
        // Flip the sprite based on movement direction
        if (Mathf.Abs(direction.x) > 0.1f)  // Only flip if there's significant horizontal movement
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        }
    }
    
    void TryAttack()
    {
        // Check attack cooldown
        if (Time.time - lastAttackTime < attackCooldown) return;
        
        // Execute the attack
        lastAttackTime = Time.time;
        
        // Face the player
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
        // Stop moving
        rb.linearVelocity = Vector2.zero;
        
        // Deal damage to player
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked player for " + attackDamage + " damage!");
        }
        
        // You could also trigger an attack animation here
    }
    
    // Draw gizmos for debugging
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw path
        if (pathPoints.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
            }
        }
    }
}

// Simple grid-based pathfinding system
public class GridPathfinding
{
    private bool[,] grid;
    private float cellSize;
    private int width, height;
    
    public GridPathfinding(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        grid = new bool[width, height];
    }
    
    public void AddObstacle(Vector2 worldPosition)
    {
        // Convert world position to grid position
        int x = Mathf.FloorToInt(worldPosition.x / cellSize) + width / 2;
        int y = Mathf.FloorToInt(worldPosition.y / cellSize) + height / 2;
        
        // Ensure position is within grid bounds
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            grid[x, y] = true; // Mark as obstacle
        }
    }
    
    public void ClearObstacles()
    {
        grid = new bool[width, height];
    }
    
    public List<Vector2> FindPath(Vector2 startPos, Vector2 endPos)
    {
        // Convert world positions to grid positions
        int startX = Mathf.FloorToInt(startPos.x / cellSize) + width / 2;
        int startY = Mathf.FloorToInt(startPos.y / cellSize) + height / 2;
        int endX = Mathf.FloorToInt(endPos.x / cellSize) + width / 2;
        int endY = Mathf.FloorToInt(endPos.y / cellSize) + height / 2;
        
        // Ensure positions are within grid bounds
        startX = Mathf.Clamp(startX, 0, width - 1);
        startY = Mathf.Clamp(startY, 0, height - 1);
        endX = Mathf.Clamp(endX, 0, width - 1);
        endY = Mathf.Clamp(endY, 0, height - 1);
        
        // A* pathfinding algorithm
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        Node startNode = new Node(startX, startY);
        Node endNode = new Node(endX, endY);
        
        openList.Add(startNode);
        
        while (openList.Count > 0)
        {
            // Find node with lowest F cost
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || 
                   (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
                {
                    currentNode = openList[i];
                }
            }
            
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            
            // Check if we reached the end
            if (currentNode.X == endNode.X && currentNode.Y == endNode.Y)
            {
                return RetracePath(startNode, currentNode);
            }
            
            // Check neighbors
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Skip diagonal movement (optional)
                    if (Mathf.Abs(x) == Mathf.Abs(y)) continue;
                    
                    int checkX = currentNode.X + x;
                    int checkY = currentNode.Y + y;
                    
                    // Skip if out of bounds
                    if (checkX < 0 || checkX >= width || checkY < 0 || checkY >= height) continue;
                    
                    // Skip if obstacle
                    if (grid[checkX, checkY]) continue;
                    
                    // Create neighbor node
                    Node neighbor = new Node(checkX, checkY);
                    
                    // Skip if already in closed list
                    bool inClosedList = false;
                    foreach (Node node in closedList)
                    {
                        if (node.X == neighbor.X && node.Y == neighbor.Y)
                        {
                            inClosedList = true;
                            break;
                        }
                    }
                    if (inClosedList) continue;
                    
                    int moveCost = currentNode.GCost + 1;
                    
                    // Check if in open list
                    bool inOpenList = false;
                    foreach (Node node in openList)
                    {
                        if (node.X == neighbor.X && node.Y == neighbor.Y)
                        {
                            inOpenList = true;
                            
                            // Check if new path is better
                            if (moveCost < node.GCost)
                            {
                                node.GCost = moveCost;
                                node.Parent = currentNode;
                            }
                            break;
                        }
                    }
                    
                    if (!inOpenList)
                    {
                        neighbor.GCost = moveCost;
                        neighbor.HCost = GetDistance(neighbor, endNode);
                        neighbor.Parent = currentNode;
                        openList.Add(neighbor);
                    }
                }
            }
        }
        
        // No path found
        return new List<Vector2>();
    }
    
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.X - nodeB.X);
        int distY = Mathf.Abs(nodeA.Y - nodeB.Y);
        return distX + distY;
    }
    
    private List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;
        
        while (currentNode.X != startNode.X || currentNode.Y != startNode.Y)
        {
            // Convert grid position to world position
            Vector2 worldPos = new Vector2(
                (currentNode.X - width / 2) * cellSize,
                (currentNode.Y - height / 2) * cellSize
            );
            path.Add(worldPos);
            currentNode = currentNode.Parent;
        }
        
        path.Reverse();
        return path;
    }
    
    // Node class for A* pathfinding
    private class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int GCost { get; set; } // Distance from start
        public int HCost { get; set; } // Distance to end (heuristic)
        public int FCost { get { return GCost + HCost; } }
        public Node Parent { get; set; }
        
        public Node(int x, int y)
        {
            X = x;
            Y = y;
            GCost = 0;
            HCost = 0;
            Parent = null;
        }
    }
}