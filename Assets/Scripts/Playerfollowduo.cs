using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class PlayerFollowduo : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomLimiter = 50f;
    [SerializeField] private Vector3 offset = new Vector3(0, 10, -10);

    private Vector3 velocity;
    private Camera cam;
    private List<NetworkIdentity> players = new List<NetworkIdentity>();

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        // Get all player objects in the scene
        players = NetworkClient.spawned.Values
            .Where(identity => identity.CompareTag("Player"))  // Assuming players have the "Player" tag
            .ToList();

        // Only follow if we have exactly two players
        if (players.Count == 2)
        {
            Move();
            Zoom();
        }
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 targetPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void Zoom()
    {
        float distance = GetDistance();
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime);
    }

    private Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(players[0].transform.position, Vector3.zero);
        bounds.Encapsulate(players[1].transform.position);
        return bounds.center;
    }

    private float GetDistance()
    {
        var bounds = new Bounds(players[0].transform.position, Vector3.zero);
        bounds.Encapsulate(players[1].transform.position);
        return bounds.size.x;
    }
}

