using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Transform cam;
    
    [SerializeField] private TextMeshProUGUI healthText;

    private float targetFill = 1f;
    private float currentFill = 1f;
    [SerializeField] private float fillSpeed = 5f;


    public void SetHealth(float current, float max)
    {
        current = Mathf.Max(0, current); // Clamp to 0 minimum

        targetFill = Mathf.Clamp01(current / max);

        if (foregroundImage != null)
        {
            foregroundImage.fillAmount = Mathf.Clamp01(current / max);
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
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

        if (foregroundImage != null)
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * fillSpeed);
            foregroundImage.fillAmount = currentFill;
        }
    }
}
