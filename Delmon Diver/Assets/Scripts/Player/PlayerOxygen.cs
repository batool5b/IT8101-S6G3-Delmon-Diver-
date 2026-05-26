using UnityEngine;
using UnityEngine.Events;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float depletionRate = 2f; // Oxygen units per second
    public float surfaceRecoveryRate = 10f;

    [Header("Drowning Settings")]
    public float drowningDamage = 10f;
    public float damageInterval = 1f;
    private float nextDamageTime;

    [Header("Events")]
    public UnityEvent OnDrowned;
    public UnityEvent<float> OnOxygenChanged;

    private EnvironmentDetector env;
    private bool isDead = false;

    private void Awake()
    {
        env = GetComponent<EnvironmentDetector>();
        currentOxygen = maxOxygen;
    }

    private void Update()
    {
        if (isDead) return;

        bool isUnderwater = env != null && env.isInWater && env.isSubmerged;

        if (isUnderwater)
        {
            DepleteOxygen();
        }
        else if (env != null && !env.isSubmerged)
        {
            RecoverOxygen();
        }

        if (currentOxygen <= 0)
        {
            HandleDrowning();
        }
        
        OnOxygenChanged?.Invoke(currentOxygen / maxOxygen);
    }

    private void DepleteOxygen()
    {
        currentOxygen -= depletionRate * Time.deltaTime;
        currentOxygen = Mathf.Max(currentOxygen, 0f);
    }

    private void RecoverOxygen()
    {
        currentOxygen += surfaceRecoveryRate * Time.deltaTime;
        currentOxygen = Mathf.Min(currentOxygen, maxOxygen);
    }

    private void HandleDrowning()
    {
        if (Time.time >= nextDamageTime)
        {
            Debug.Log("Player is drowning!");
            nextDamageTime = Time.time + damageInterval;
            
            // For now, if oxygen is 0, we can just trigger "OnDrowned" if we want a game over
            // Or implement a health system. Let's start with a simple Game Over if oxygen hits 0 for too long.
            // If we have a Health system, we would do: health.TakeDamage(drowningDamage);
        }
    }
    
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDrowned?.Invoke();
        Debug.Log("Player has drowned.");
    }
}
