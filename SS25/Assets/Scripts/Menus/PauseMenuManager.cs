using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    
    [Header("Input Settings")]
    public KeyCode pauseKey = KeyCode.Escape;
    
    private bool isPaused = false;

    void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    // This method can be called by buttons
    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        
        // Optional: Show cursor when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        
        // Optional: Hide cursor when resumed (common for FPS games)
        // Uncomment these lines if you want to hide the cursor during gameplay
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // Public getter for other scripts to check pause state
    public bool IsPaused()
    {
        return isPaused;
    }

    // Method to pause from other scripts
    public void PauseGame()
    {
        if (!isPaused)
            Pause();
    }

    // Method to resume from other scripts
    public void ResumeGame()
    {
        if (isPaused)
            Resume();
    }
}