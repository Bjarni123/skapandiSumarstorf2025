using UnityEngine;

public class DestroyPrefab : MonoBehaviour
{
    public float duration = 0.2f;

    void Start() => Destroy(gameObject, duration);
}
