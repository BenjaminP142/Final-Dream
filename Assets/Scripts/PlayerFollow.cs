using UnityEngine;
using Mirror;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float smoothSpeed = 5f;
    
    private Transform target;
    
    void Update()
    {
        // If we don't have a target, try to find the local player
        if (target == null && NetworkClient.localPlayer != null)
        {
            target = NetworkClient.localPlayer.gameObject.transform;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        
        // Make camera look at player
        transform.LookAt(target);
    }
}
