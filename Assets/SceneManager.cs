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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Plateformes");
        NetworkManager.singleton.StartHost();
    }

    void ChangeSceneClient()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Plateformes");
        NetworkManager.singleton.StartClient();
    }
}
