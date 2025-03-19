using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Grid grid;
    public float pathUpdateInterval = 0.5f;

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
                    /*
                    // Visualize the path (for debugging)
                    for (int i = 0; i < waypoints.Length - 1; i++)
                    {
                        Debug.DrawLine(waypoints[i], waypoints[i + 1], Color.green, 1f);
                    }
                    */
                    yield return StartCoroutine(FollowPath(waypoints));
                }
                else
                {
                    Debug.Log("No path found.");
                }
            }
        }
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            waypoints.Add(path[i].worldPosition);
        }
        return waypoints.ToArray();
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            while (Vector3.Distance(transform.position, waypoints[i]) > 0.05f)
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
