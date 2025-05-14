using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Enemy : NetworkBehaviour
{  
    public Transform player;
    private List<Transform> players = new List<Transform>();
    public float speed = 5f;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Grid grid;
    public float pathUpdateInterval = 0.01f;
    public float trackDistance = 0.1f;

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
        players.Add(playerTransform);
        if (player == null)
        {
            SelectNextPlayer();
        }
    }
    
    /*
    private void RefreshPlayers()
    {
        players.Clear();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(go.transform);
        }
    }
    */
    private void SelectNextPlayer()
    {
        // Nettoie les joueurs qui ont été détruits
        players.RemoveAll(p => p == null);

        if (players.Count > 0)
        {
            player = players[0];
            SelectClosestPlayer(); // On prend le plus proche de l'ennemi
        }
        else
        {
            player = null; // Aucun joueur trouvé
        }
    }

    private void SelectClosestPlayer()
    {
        foreach (Transform p in players)
        {
            if (Vector2.Distance(transform.position, p.position) <
                Vector2.Distance(transform.position, player.position))
            {
                player = p;
            }
        }
    }
    
    void Start()
    {
        if (!isServer)
            return;
        StartCoroutine(FindPlayer());
    }
    
    private void Update() { }

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
            SelectClosestPlayer();
            if (player != null && Vector2.Distance(transform.position, player.position) <= trackDistance)
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
                yield return new WaitForSeconds(1f);
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