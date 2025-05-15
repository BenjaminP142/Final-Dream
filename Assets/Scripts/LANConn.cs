using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LANConnection : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public Button connectButton;

    void Start()
    {
        connectButton.onClick.AddListener(JoinServer);
    }

    public void JoinServer()
    {
        string ipAddress = ipInputField.text;

        if (string.IsNullOrEmpty(ipAddress))
        {
            Debug.LogWarning("Veuillez entrer une adresse IP valide.");
            return;
        }

        // Modifier l'adresse du NetworkManager et démarrer le client
        NetworkManager.singleton.networkAddress = ipAddress;
        if (!NetworkManager.singleton.isNetworkActive)
        {
            // start the client
            NetworkManager.singleton.StartClient();
        }
        else
        {
            NetworkManager.singleton.StopClient();
            
            NetworkManager.singleton.StartClient();
        }

        Debug.Log("Tentative de connexion au serveur: " + ipAddress);
    }
}