using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
public class SceneManager : MonoBehaviour
{
    public Button buttonHost;
    public Button buttonClient;
    void Start()
    {
        buttonHost.onClick.AddListener(ChangeSceneHost);
        buttonClient.onClick.AddListener(ChangeSceneClient);
    }

    void ChangeSceneHost()
    {
        DontDestroyOnLoad(NetworkManager.singleton.gameObject);
        
        // First start the network
        NetworkManager.singleton.StartHost();

        // Then load scene through NetworkManager
        NetworkManager.singleton.ServerChangeScene("Plateformes");
    }

    void ChangeSceneClient()
    {
        DontDestroyOnLoad(NetworkManager.singleton.gameObject);
        
        // start the client
        NetworkManager.singleton.StartClient();

        // Then load scene through NetworkManager
        NetworkManager.singleton.ServerChangeScene("Plateformes");
    }
}
