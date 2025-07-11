using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Reset Settings")]
    [Tooltip("Enable game reset with R key")]
    public bool allowReset = true;

    void Update()
    {
        if (allowReset && Keyboard.current != null)
        {
            // reset game -> R
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                ResetGame();
            }
        }
    }

    public void ResetGame()
    {
        Debug.Log("Resetting game...");
        
        // Get current scene + reload 
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
