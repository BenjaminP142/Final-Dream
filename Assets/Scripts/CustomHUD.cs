using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CustomHUD : MonoBehaviour
{
    public Button buttonHost;
    public Button buttonClient; 

    void Start()
    {
        buttonHost.onClick.AddListener(ButtonHost);
        buttonClient.onClick.AddListener(ButtonClient);
    }

    void ButtonHost()
    {
        NetworkManager.singleton.StartHost();
        buttonHost.gameObject.SetActive(false);
        buttonClient.gameObject.SetActive(false);
    }

    void ButtonClient()
    {
        NetworkManager.singleton.StartClient();
        buttonHost.gameObject.SetActive(false);
        buttonClient.gameObject.SetActive(false);
    }
}