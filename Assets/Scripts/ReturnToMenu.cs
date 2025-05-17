using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    public GameObject[] environments;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ReturnToMainMenu()
    {
        environments[1].SetActive(false);
        environments[0].SetActive(true);
    }
}
