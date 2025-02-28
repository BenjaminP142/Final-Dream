using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Grid grid;

    IEnumerator WaitForPlayer()
    {
        while (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
                yield break; // Stop the coroutine
            }
            yield return new WaitForSeconds(0.5f); // Retry after a short delay
        }
    }
    void Start()
    {
        StartCoroutine(WaitForPlayer());
    }

    void Update()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        List<Node> path = pathfinding.FindPath(transform.position, player.position);

        if (path != null)
        {
            Vector3[] waypoints = SimplifyPath(path);
            StartCoroutine(FollowPath(waypoints));
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
        foreach (Vector3 waypoint in waypoints)
        {
            while (Vector3.Distance(transform.position, waypoint) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
