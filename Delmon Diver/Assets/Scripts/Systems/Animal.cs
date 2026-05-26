using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for all Land Creatures (AfricanBuffalo, Gazelle, Jackal, Lizard, and future animals).
/// Attach to any animal prefab. Configure everything in the Inspector — no code changes needed for new animals.
/// </summary>
public class Animal : MonoBehaviour
{
    [Header("Identity")]
    public string animalName = "Animal";
    public string animalPrefix = "Land Creature";

    [Header("Health")]
public float maxHealth = 50f;
    [HideInInspector] public float currentHealth;

    [Header("Meat Drop")]
    public ItemClass meatItem;
    [Min(0)] public int meatAmount = 2;

    [Header("Instant Heal on Touch")]
    [Min(0)] public float instantHealAmount = 0f;

    [Header("Quest Integration")]
    public string questId = "";
    [Min(1)] public int questProgressAmount = 1;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Visual Effects")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    [Header("UI")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    private bool isDead = false;
    private AudioSource audioSource;
    private Renderer[] renderers;
    private Color[] originalColors;
    private Slider healthSlider;
    private GameObject healthBarInstance;

    private void Awake()
    {
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
            else if (renderers[i].material.HasProperty("_BaseColor"))
                originalColors[i] = renderers[i].material.GetColor("_BaseColor");
        }

        SpawnHealthBar();
    }

    private void SpawnHealthBar()
    {
        if (healthBarPrefab == null) return;

        healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity, transform);
        healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        
        // Face camera logic would go here or in a separate Billboard script
        healthBarInstance.AddComponent<BillboardUI>();
        healthBarInstance.SetActive(false); // Hide initially
    }

    private void OnTriggerEnter(Collider other)
    {
        if (instantHealAmount <= 0f) return;
        if (isDead) return;

        bool isPlayer = other.CompareTag("Player") || other.GetComponentInParent<PlayerManager>() != null;
        if (!isPlayer) return;

        PlayerSurvival survival = other.GetComponentInParent<PlayerSurvival>() ?? other.GetComponent<PlayerSurvival>() ?? FindObjectOfType<PlayerSurvival>();
        if (survival != null)
        {
            survival.Eat(instantHealAmount);
            Debug.Log($"[{animalPrefix}] {animalName} touched — instantly healed player for {instantHealAmount} HP!");
        }

        DieInstant();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (healthSlider != null)
        {
            healthBarInstance.SetActive(true);
            healthSlider.value = currentHealth;
        }

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);

        StartCoroutine(FlashRoutine());

        Debug.Log($"[{animalPrefix}] {animalName} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        SetRenderersColor(flashColor);
        yield return new WaitForSeconds(flashDuration);
        ResetRenderersColor();
    }

    private void SetRenderersColor(Color color)
    {
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
                r.material.color = color;
            else if (r.material.HasProperty("_BaseColor"))
                r.material.SetColor("_BaseColor", color);
        }
    }

    private void ResetRenderersColor()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
            else if (renderers[i].material.HasProperty("_BaseColor"))
                renderers[i].material.SetColor("_BaseColor", originalColors[i]);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        DropMeat();
        ReportQuestProgress();

        if (healthBarInstance != null) Destroy(healthBarInstance);
        Destroy(gameObject, 0.15f);
    }

    private void DieInstant()
    {
        if (isDead) return;
        isDead = true;
        ReportQuestProgress();
        if (healthBarInstance != null) Destroy(healthBarInstance);
        Destroy(gameObject, 0.05f);
    }

    private void DropMeat()
    {
        if (meatItem == null || meatAmount <= 0) return;

        InventoryManager inv = FindObjectOfType<InventoryManager>();
        if (inv != null)
        {
            bool added = inv.Add(meatItem, meatAmount);
            if (added)
                Debug.Log($"[{animalPrefix}] {animalName} dropped {meatAmount}x {meatItem.itemName} into inventory.");
        }
    }

    private void ReportQuestProgress()
    {
        if (string.IsNullOrEmpty(questId)) return;
        if (QuestManager.Instance != null)
            QuestManager.Instance.AddProgress(questId, questProgressAmount);
    }
}

public class BillboardUI : MonoBehaviour
{
    private Transform cam;
    private void Start() { cam = Camera.main.transform; }
    private void LateUpdate() { if (cam != null) transform.LookAt(transform.position + cam.forward); }
}
