using UnityEngine;

public class TeleportOnTouch : MonoBehaviour
{

    public Vector3 teleportDestination = new Vector3(10f, 0f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ggbg"))
        {
            transform.position = teleportDestination;
            Debug.Log("TeleportOnTouch");
        }
    }
}

