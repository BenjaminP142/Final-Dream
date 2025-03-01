using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    [SerializeField] private KeyCode quitKey = KeyCode.Escape; // Default to Escape key

    void Update()
    {
        // Check if the specified key was pressed this frame
        if (Input.GetKeyDown(quitKey))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR // If running in the editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else // If running in a build
            Application.Quit();
        #endif
        
        Debug.Log("Game is quitting...");
    }
}
