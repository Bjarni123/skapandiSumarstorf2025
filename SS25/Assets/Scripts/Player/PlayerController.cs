using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Prefab;

    public void Start()
    {
        GamePlayer.Load(GameSave.Loaded);
        GameSave.Subscribe(OnSave);

        GamePlayer.Player = Instantiate(Prefab, transform);
        GamePlayer.Player.transform.position = new Vector3(GamePlayer.Position.x, GamePlayer.Position.y, -1);
    }

    public void Update()
    {
        GamePlayer.Update();
    }

    public void OnSave(string path)
    {
        Debug.LogWarning("TODO");
    }
}