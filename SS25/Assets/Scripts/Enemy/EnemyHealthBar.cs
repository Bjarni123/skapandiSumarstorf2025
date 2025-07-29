using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Components")]
    public Slider healthSlider;
    public Image fillImage;

    [Header("Visual Settings")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public float lowHealthThreshold = 0.3f;

    [Header("Positioning")]
    public float yOffset = 1.5f;
    public bool alwaysFaceCamera = true;

    [Header("Auto-Hide Settings")]
    public bool hideWhenFull = true;
    public float hideDelay = 1.5f;

    // Private variables
    private Camera playerCamera;
    private Transform enemyTransform;
    private Canvas canvas;
    private float hideTimer;
    private bool isVisible = true;

    // Track previous health to detect changes
    private float previousHealth = -1f;
    private float currentMaxHealth = -1f;

    // Enemy script reference
    private SimpleEnemy simpleEnemy;

    void Start()
    {
        // Get components
        canvas = GetComponent<Canvas>();
        playerCamera = Camera.main;

        // Find the enemy this health bar belongs to
        enemyTransform = transform.parent;
        if (enemyTransform == null)
        {
            Debug.LogError("Health bar must be a child of an enemy GameObject!");
            return;
        }

        // Get the SimpleEnemy component
        simpleEnemy = enemyTransform.GetComponent<SimpleEnemy>();
        if (simpleEnemy == null)
        {
            Debug.LogError("Could not find SimpleEnemy component on " + enemyTransform.name);
            return;
        }

        // Initialize health bar
        UpdateHealthDisplay();

        // Initially hide if set to hide when full
        if (hideWhenFull)
            SetVisible(false);
    }

    void Update()
    {
        // Continuously check for health changes
        UpdateHealthDisplay();

        // Handle auto-hide timer
        HandleAutoHide();
    }

    void LateUpdate()
    {
        // Position above enemy
        if (enemyTransform != null)
        {
            Vector3 worldPosition = enemyTransform.position + new Vector3(0, yOffset, 0);
            transform.position = worldPosition;
        }

        // Always face camera
        if (alwaysFaceCamera && playerCamera != null)
        {
            transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward,
                           playerCamera.transform.rotation * Vector3.up);
        }
    }

    void UpdateHealthDisplay()
    {
        if (simpleEnemy == null)
        {
            Debug.LogError("simpleEnemy is null!");
            return;
        }

        if (healthSlider == null)
        {
            Debug.LogError("healthSlider is null! Make sure it's assigned in inspector.");
            return;
        }

        // Use the new public methods instead of reflection
        float currentHealth = simpleEnemy.GetCurrentHealthForUI();
        float maxHealth = simpleEnemy.GetMaxHealthForUI();

        // Debug the raw values
        Debug.Log($"Raw values - Current: {currentHealth}, Max: {maxHealth}");

        // Check if health values are valid
        if (currentHealth < 0 || maxHealth <= 0)
        {
            Debug.LogWarning($"Invalid health values - Current: {currentHealth}, Max: {maxHealth}");
            return;
        }

        // Only update if health has changed (for performance)
        if (currentHealth != previousHealth || maxHealth != currentMaxHealth)
        {
            Debug.Log($"Updating slider - MaxValue: {maxHealth}, Value: {currentHealth}");

            // Update slider
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;

            // Debug slider after update
            Debug.Log($"Slider after update - MaxValue: {healthSlider.maxValue}, Value: {healthSlider.value}");

            // Update color
            float healthPercentage = currentHealth / maxHealth;
            UpdateHealthBarColor(healthPercentage);

            // Show health bar if damage was taken
            if (hideWhenFull && healthPercentage < 1f && !isVisible)
            {
                SetVisible(true);
                hideTimer = 0f;
            }

            // Store previous values
            previousHealth = currentHealth;
            currentMaxHealth = maxHealth;

            // Debug info
            Debug.Log($"Health updated: {currentHealth}/{maxHealth} ({healthPercentage:P0})");
        }
    }

    float GetCurrentHealth()
    {
        if (simpleEnemy == null)
        {
            Debug.LogError("simpleEnemy is null in GetCurrentHealth!");
            return -1f;
        }

        // Use reflection to get the private _currentHealth field
        var field = typeof(SimpleEnemy).GetField("_currentHealth",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            object value = field.GetValue(simpleEnemy);
            Debug.Log($"Reflection found _currentHealth: {value} (Type: {value?.GetType()})");
            return (float)value;
        }
        else
        {
            Debug.LogError("Could not find _currentHealth field in SimpleEnemy using reflection");

            // Let's try to find what fields ARE available
            var allFields = typeof(SimpleEnemy).GetFields(
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            Debug.Log("Available fields in SimpleEnemy:");
            foreach (var f in allFields)
            {
                Debug.Log($"- {f.Name} ({f.FieldType})");
            }

            return -1f;
        }
    }

    void UpdateHealthBarColor(float healthPercentage)
    {
        if (fillImage == null)
        {
            Debug.LogError("fillImage is null! Make sure it's assigned in inspector.");
            return;
        }

        Color newColor;
        if (healthPercentage <= lowHealthThreshold)
        {
            float normalizedHealth = healthPercentage / lowHealthThreshold;
            newColor = Color.Lerp(lowHealthColor, fullHealthColor, normalizedHealth);
        }
        else
        {
            newColor = fullHealthColor;
        }

        fillImage.color = newColor;
        Debug.Log($"Updated health bar color to: {newColor} (Health: {healthPercentage:P0})");
    }

    void HandleAutoHide()
    {
        if (!hideWhenFull || healthSlider == null) return;

        if (healthSlider.value >= healthSlider.maxValue)
        {
            hideTimer += Time.deltaTime;
            if (hideTimer >= hideDelay && isVisible)
            {
                SetVisible(false);
            }
        }
        else
        {
            hideTimer = 0f;
        }
    }

    void SetVisible(bool visible)
    {
        isVisible = visible;
        if (canvas != null)
            canvas.enabled = visible;
    }

    // Public method that can be called manually when enemy takes damage
    public void ForceUpdate()
    {
        previousHealth = -1f; // Force update on next frame
    }
}