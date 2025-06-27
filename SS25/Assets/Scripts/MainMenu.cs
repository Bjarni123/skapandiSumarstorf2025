using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        GameSave.Loaded = $"{Application.dataPath}/Resources/Maps/Example";
        SceneManager.LoadScene("Scenes/Map");
    }
}