using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
public class SceneManager : MonoBehaviour
{
    public Button buttonHost;
    public Button buttonClient;
    public Button buttonSolo;
    void Start()
    {
        buttonHost.onClick.AddListener(ChangeSceneHost);
        buttonClient.onClick.AddListener(ChangeSceneClient);
        buttonSolo.onClick.AddListener(ChangeSceneSolo);
    }

    void ChangeSceneHost()
    {
        DontDestroyOnLoad(NetworkManager.singleton.gameObject);
        
        // First start the network
        NetworkManager.singleton.StartHost();

        // Then load scene through NetworkManager
        NetworkManager.singleton.ServerChangeScene("game duo");
    }

    void ChangeSceneClient()
    {
        DontDestroyOnLoad(NetworkManager.singleton.gameObject);
        
        // start the client
        NetworkManager.singleton.StartClient();

        // Then load scene through NetworkManager
        NetworkManager.singleton.ServerChangeScene("game duo");
    }

    void ChangeSceneSolo()
    {
        //start a single host, we don't want any other player
        NetworkManager.singleton.StartHost();
        
        //load the scene for a single player game
        NetworkManager.singleton.ServerChangeScene("game solo");
    }
}
