using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InventoryFramework
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health Settings")]
        public float maxHealth = 100f;
        public float currentHealth;

        [Header("Oxygen (Oxsition) Settings")]
        public float maxOxygen = 100f;
        public float currentOxygen;
        [Tooltip("Oxygen lost per second while fully submerged under water.")]
        public float oxygenDepletionRate = 8f;
        [Tooltip("Oxygen recovered per second when not submerged.")]
        public float oxygenRecoveryRate = 15f;

        [Header("Suffocation Settings")]
        [Tooltip("Damage taken per second when oxygen is completely empty.")]
        public float suffocationDamageRate = 10f;

        [Header("XP & Level Settings")]
        public int currentLevel = 1;
        public float currentXP = 0f;
        public float xpToNextLevel = 100f;

        [Header("UI Health Bars (Assign Slider OR Filled Image)")]
        [Tooltip("Optional: Health Slider component.")]
        public Slider healthSlider;
        [Tooltip("Optional: Health Image component (Fill Method must be Radial/Horizontal/Vertical).")]
        public Image healthImageFill;

        [Header("UI Oxygen Bars (Assign Slider OR Filled Image)")]
        [Tooltip("Optional: Oxygen (Oxsition) Slider component.")]
        public Slider oxygenSlider;
        [Tooltip("Optional: Oxygen (Oxsition) Image component (Fill Method must be Radial/Horizontal/Vertical).")]
        public Image oxygenImageFill;

        [Header("UI XP Components (Assign Slider OR Filled Image)")]
        [Tooltip("Optional: XP Slider component.")]
        public Slider xpSlider;
        [Tooltip("Optional: XP Image component (Fill Method must be Radial/Horizontal/Vertical).")]
        public Image xpImageFill;

        [Header("UI Text Labels")]
        [Tooltip("Optional: TMPro component to display current XP (e.g. '199').")]
        public TextMeshProUGUI xpValueText;
        [Tooltip("Optional: Legacy UI Text component to display current XP.")]
        public Text xpValueTextLegacy;


        // Private components
        private EnvironmentDetector envDetector;
        private bool isDead = false;

        private void Start()
        {
            // Initialize stats
            currentHealth = maxHealth;
            currentOxygen = maxOxygen;

            // Cache EnvironmentDetector on the same object
            envDetector = GetComponent<EnvironmentDetector>();
            if (envDetector == null)
            {
                Debug.LogWarning("[PlayerStats] No EnvironmentDetector component found on the Player! Oxygen depletion will not auto-trigger.");
            }

            // Instantly sync UI at start
            UpdateUI(true);
        }

        private void Update()
        {
            if (isDead) return;

            HandleOxygen();
            HandleSuffocation();
            UpdateUI(false);
        }

        private void HandleOxygen()
        {
            if (envDetector != null && envDetector.isSubmerged)
            {
                // Deplete oxygen when submerged underwater
                currentOxygen -= oxygenDepletionRate * Time.deltaTime;
            }
            else
            {
                // Recover oxygen when on surface or land
                currentOxygen += oxygenRecoveryRate * Time.deltaTime;
            }

            // Clamp oxygen within bounds
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
        }

        private void HandleSuffocation()
        {
            // If oxygen runs out completely, start suffocating (take damage)
            if (currentOxygen <= 0f)
            {
                TakeDamage(suffocationDamageRate * Time.deltaTime);
            }
        }

        public void AddXP(float amount)
        {
            if (isDead) return;

            currentXP += amount;
            Debug.Log($"[PlayerStats] Earned {amount} XP! Total XP is now {currentXP}.");

            UpdateUI(false);
        }

        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            // Trigger Visual Flash
            DamageFlash flash = Object.FindFirstObjectByType<DamageFlash>();
            if (flash != null) flash.Flash();

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (isDead) return;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
            Debug.Log($"[PlayerStats] Healed {amount}! Health is now {currentHealth}/{maxHealth}.");
        }

        public void RefillOxygen(float amount)
        {
            if (isDead) return;

            currentOxygen += amount;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
            Debug.Log($"[PlayerStats] Refilled {amount} Oxygen! Oxygen is now {currentOxygen}/{maxOxygen}.");
        }

        private void Die()
        {
            isDead = true;
            Debug.LogError("[PlayerStats] Player has died!");
            
            // Custom death behavior (e.g. show Game Over screen, reload scene, or respawn)
            Invoke(nameof(Respawn), 2.0f);
        }

        private void Respawn()
        {
            isDead = false;
            currentHealth = maxHealth;
            currentOxygen = maxOxygen;
            
            // Reset player to origin/spawn point if needed
            transform.position = new Vector3(0, 5f, 0); // safe spawn fallback or adjust
            
            Debug.Log("[PlayerStats] Player respawned.");
            UpdateUI(true);
        }

        private void UpdateUI(bool instant)
        {
            float healthRatio = currentHealth / maxHealth;
            float oxygenRatio = currentOxygen / maxOxygen;
            float xpRatio = xpToNextLevel > 0f ? (currentXP / xpToNextLevel) : 0f;

            float speed = instant ? 100f : Time.deltaTime * 8f;

            if (healthSlider != null)
            {
                healthSlider.value = Mathf.Lerp(healthSlider.value, healthRatio, speed);
            }
            if (healthImageFill != null)
            {
                healthImageFill.fillAmount = Mathf.Lerp(healthImageFill.fillAmount, healthRatio, speed);
            }

            if (oxygenSlider != null)
            {
                oxygenSlider.value = Mathf.Lerp(oxygenSlider.value, oxygenRatio, speed);
            }
            if (oxygenImageFill != null)
            {
                oxygenImageFill.fillAmount = Mathf.Lerp(oxygenImageFill.fillAmount, oxygenRatio, speed);
            }

            if (xpSlider != null)
            {
                xpSlider.value = Mathf.Lerp(xpSlider.value, xpRatio, speed);
            }
            if (xpImageFill != null)
            {
                xpImageFill.fillAmount = Mathf.Lerp(xpImageFill.fillAmount, xpRatio, speed);
            }

            string xpString = ((int)currentXP).ToString();
            if (xpValueText != null)
            {
                xpValueText.text = xpString;
            }
            if (xpValueTextLegacy != null)
            {
                xpValueTextLegacy.text = xpString;
            }
        }
    }
}
