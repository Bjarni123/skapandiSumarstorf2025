using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuUI;

    [SerializeField]
    private Button RestartButton;

    [SerializeField]
    private Button ContinueButton;

    // [Header("Input Settings")]
    // public KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;

    void Update()
    {
        // Check for pause input
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
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
        pauseMenuUI.SetActive(true); // Show the menu
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the menu
        Time.timeScale = 1f;
        isPaused = false;
        // Optionally hide cursor here
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

    // Call this from the Play button to restart the game
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Use your main menu scene name here
    }
}