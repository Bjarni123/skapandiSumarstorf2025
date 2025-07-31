using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;

    private PlayerHealth playerHealth;

    [SerializeField]
    private TextMeshProUGUI healthText;

    private float currentFill = 1f;

    private void Update()
    {
        float targetFill = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * 10f);
        fillImage.fillAmount = currentFill;
    }
    public void Initialize(PlayerHealth health)
    {
        playerHealth = health;
        playerHealth.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar();
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        float percent = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        fillImage.fillAmount = percent;

        if (healthText != null)
            healthText.text = $"{playerHealth.CurrentHealth} / {playerHealth.MaxHealth}";
    }
}
