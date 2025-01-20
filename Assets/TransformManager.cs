using UnityEngine;

public class TransformManager : MonoBehaviour
{ 
    public GameObject player; // Référence à l'objet joueur
    private Transform playerTransform;

    void Start()
    {
        // Trouver le Transform du joueur
        playerTransform = player.GetComponent<Transform>();

        if (playerTransform != null)
        {
            // Associer le Transform au script d'un autre objet
            PlayerFollow anotherScript = GetComponent<PlayerFollow>();

            if (anotherScript != null)
            {
                anotherScript.target = playerTransform;
            }
            else
            {
                Debug.LogWarning("AnotherScript non trouvé sur cet objet.");
            }
        }
        else
        {
            Debug.LogWarning("Rigidbody non trouvé sur l'objet joueur.");
        }
    }
}

