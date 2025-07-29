using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth;

    public GameObject healthBarPrefab;

    public event System.Action OnHealthChanged;

    [Header("Flash Settings")]
    [SerializeField] float flashDuration = 0.1f;
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);
        }
        originalColor = sr.color;
    }

    void Start()
    {
        CurrentHealth = MaxHealth;

        if (healthBarPrefab != null)
        {
            GameObject canvas = Instantiate(healthBarPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            canvas.transform.SetParent(transform); // optional: make it follow
            var ui = canvas.GetComponent<HealthBarUI>();
            if (ui != null)
                ui.Initialize(this); // <-- this connects the two scripts
        }

        OnHealthChanged?.Invoke(); // fire once to init
    }
    
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        Flash();
        Debug.Log("Player takes " + damage + " damage! Current health: " + CurrentHealth);

        if (CurrentHealth <= 5)
        {
            Debug.Log("Players heals 50 health");
            CurrentHealth += 50;
        }
        if (CurrentHealth <= 0)
        {
        
        }
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            Debug.Log("Player has died!");
            // Here you can add logic for player death, like respawning or ending the game
        }
        OnHealthChanged?.Invoke();
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }


    private IEnumerator FlashRoutine()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    public void AddHealth(int heal)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + heal, MaxHealth);
        Debug.Log($"Player heals {heal} HP! Current health: {CurrentHealth}");
        OnHealthChanged?.Invoke();
    }
}

