using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using UnityEditor;

public class SceneManager : MonoBehaviour
{
    public GameObject[] environments;
    public Button buttonHost;
    public Button buttonClient;
    public Button buttonSolo;
    public Button buttonExit;
    public Button buttonSettings;
    void Start()
    {
        buttonHost.onClick.AddListener(ChangeSceneHost);
        buttonClient.onClick.AddListener(ChangeSceneClient);
        buttonSolo.onClick.AddListener(ChangeSceneSolo);
        buttonExit.onClick.AddListener(ExitGame);
        buttonSettings.onClick.AddListener(ChangeSceneSettings);
    }

    void ChangeSceneHost()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopClient();
            NetworkManager.singleton.StopServer();
        }
    
        NetworkManager.singleton.StartHost();
        NetworkManager.singleton.ServerChangeScene("game duo");
    }

    void ChangeSceneClient()
    {
        environments[0].SetActive(false);
        environments[2].SetActive(true);
    }

    void ChangeSceneSolo()
    {
        //start a single host, we don't want any other player
        NetworkManager.singleton.StartHost();
        
        //load the scene for a single player game
        NetworkManager.singleton.ServerChangeScene("game solo");
    }

    void ExitGame()
    {
        #if UNITY_EDITOR // If running in the editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else // If running in a build
            Application.Quit();
        #endif
        
        Debug.Log("Game is quitting...");
    }

    void ChangeSceneSettings()
    {
        environments[0].SetActive(false);
        environments[1].SetActive(true);
    }
}
