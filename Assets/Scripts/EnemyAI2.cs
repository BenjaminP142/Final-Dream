using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Grid grid;
    public float pathUpdateInterval = 0.01f;

    void OnEnable()
    {
        // Subscribe to the player spawn event
        PlayerController.OnPlayerSpawned += OnPlayerSpawned;
    }

    void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        PlayerController.OnPlayerSpawned -= OnPlayerSpawned;
    }

    private void OnPlayerSpawned(Transform playerTransform)
    {
        player = playerTransform;
        Debug.Log("Player spawned! Starting to track...");
    }

    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        StartCoroutine(TrackPlayer());
    }
    
    IEnumerator TrackPlayer()
    { 
        while (true)
        {
            if (player != null)
            {
                List<Node> path = pathfinding.FindPath(transform.position, player.position);

                if (path != null)
                {
                    Debug.Log("Path found with " + path.Count + " nodes.");
                    Vector3[] waypoints = SimplifyPath(path);
                    
                    // Visualize the path (for debugging)
                    for (int i = 0; i < waypoints.Length - 1; i++)
                    {
                        Debug.DrawLine(waypoints[i], waypoints[i + 1], Color.green, 1f);
                    }
                    
                    yield return StartCoroutine(FollowPath(waypoints));
                }
                else
                {
                    Debug.Log("No path found. Waiting...");
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            while (Vector3.Distance(transform.position, waypoints[i]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoints[i], speed * Time.deltaTime);
                yield return null;
            }
            Debug.Log("Reached waypoint: " + waypoints[i]);
        }
        Debug.Log("Reached the end of the path.");
        yield return null;
    }
}
