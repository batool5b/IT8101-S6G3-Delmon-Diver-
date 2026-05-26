using System;
using UnityEngine;

/// <summary>
/// Survival system: Health decays slowly over time.
/// The player must eat food (consumables) to restore health.
/// No hunger meter — just health that drains unless you eat.
///
/// Works alongside PlayerStats (which handles oxygen, XP, UI bars).
/// This script handles the slow health drain + eating mechanic.
/// </summary>
public class PlayerSurvival : MonoBehaviour
{
    [Header("Health Decay (Survival Pressure)")]
    [Tooltip("Health lost per second due to not eating. Set to 0 to disable.")]
    [SerializeField] private float healthDecayRate = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip eatingSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;

    // Events for external systems (UI, game over screen, etc.)
    public static event Action<float, float> OnHealthChanged;  // (current, max)
    public static event Action OnPlayerDeath;

    // Cached reference to the existing PlayerStats (handles HP storage + UI)
    private InventoryFramework.PlayerStats playerStats;

    private void Awake()
    {
        // Find the PlayerStats component on the same GameObject
        playerStats = GetComponent<InventoryFramework.PlayerStats>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // 2D sound for player feedback
            }
        }
    }

    private void Start()
    {
        // Fire initial health event for any UI listeners
        if (playerStats != null)
        {
            OnHealthChanged?.Invoke(playerStats.currentHealth, playerStats.maxHealth);
        }
    }

    private void Update()
    {
        if (playerStats == null) return;
        if (playerStats.currentHealth <= 0f) return; // already dead

        // Slowly drain health over time — the player must eat to survive
        if (healthDecayRate > 0f)
        {
            float previousHealth = playerStats.currentHealth;
            playerStats.TakeDamage(healthDecayRate * Time.deltaTime);

            // Only fire event if health actually changed meaningfully
            if (Mathf.Abs(playerStats.currentHealth - previousHealth) > 0.01f)
            {
                OnHealthChanged?.Invoke(playerStats.currentHealth, playerStats.maxHealth);
            }
        }
    }

    /// <summary>
    /// Called when the player eats a consumable item.
    /// Restores health using the existing PlayerStats.Heal() method.
    /// The hungerRestored parameter is kept for backward compatibility but ignored.
    /// </summary>
    public void Eat(float healthRestored, float hungerRestored = 0f)
    {
        if (playerStats == null)
        {
            Debug.LogWarning("[PlayerSurvival] No PlayerStats found — cannot heal!");
            return;
        }

        if (playerStats.currentHealth <= 0f) return; // can't eat while dead

        playerStats.Heal(healthRestored);
        PlayAudio(eatingSound);

        OnHealthChanged?.Invoke(playerStats.currentHealth, playerStats.maxHealth);

        Debug.Log($"[PlayerSurvival] Ate food! Restored {healthRestored} HP. " +
                  $"Health: {playerStats.currentHealth}/{playerStats.maxHealth}");
    }

    /// <summary>
    /// Play a damage sound effect (called externally by combat systems, falls, etc.).
    /// </summary>
    public void PlayDamageSound()
    {
        PlayAudio(damageSound);
    }

    /// <summary>
    /// Play the death sound effect.
    /// </summary>
    public void PlayDeathSound()
    {
        PlayAudio(deathSound);
        OnPlayerDeath?.Invoke();
    }

    private void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
