using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Transform cam;

    public void SetHealth(float current, float max)
    {
        if (foregroundImage != null)
        {
            foregroundImage.fillAmount = Mathf.Clamp01(current / max);
        }
    }
    private void Awake()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // Always face the camera
        transform.LookAt(transform.position + cam.forward);
    }
}
