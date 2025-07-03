using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    // This method will be called by the Input System
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}