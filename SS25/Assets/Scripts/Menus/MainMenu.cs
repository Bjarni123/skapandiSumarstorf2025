using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadMapScene()
    {
        GameSave.Loaded = $"{Application.dataPath}/Resources/Maps/Example";
        SceneManager.LoadScene("Scenes/Map");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}