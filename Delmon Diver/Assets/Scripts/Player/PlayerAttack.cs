using UnityEngine;

/// <summary>
/// Handles player attack input, animations, and damaging animals.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 20f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1f;
    public float knockbackForce = 5f;
    public LayerMask animalLayer;

    [Header("VFX")]
    public GameObject hitEffectPrefab;

    [Header("Audio")]
    public AudioClip attackSound;
    private AudioSource audioSource;

    private InputManagement inputManager;
    private Animator animator;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        inputManager = GetComponent<InputManagement>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (inputManager == null) return;

        if (inputManager.primaryActionInput && Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void PerformAttack()
    {
        Debug.Log("[PlayerAttack] Performing Attack!");
        
        // Randomize attack animation
        if (animator != null)
        {
            int attackIndex = Random.Range(0, 3); // 0, 1, or 2
            animator.SetInteger("AttackIndex", attackIndex);
            animator.SetTrigger("Attack");
        }

        // Play Sound
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Detect Animals in front and around the player
        Vector3 detectionCenter = transform.position + transform.forward * 1.0f + Vector3.up * 1.0f;
        Collider[] hitObjects = Physics.OverlapSphere(detectionCenter, attackRange, animalLayer);

        foreach (Collider col in hitObjects)
        {
            Animal animal = col.GetComponent<Animal>() ?? col.GetComponentInParent<Animal>();
            if (animal != null)
            {
                animal.TakeDamage(attackDamage);
                
                // VFX
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, col.bounds.center, Quaternion.identity);
                }

                // Apply Knockback
                SimpleAnimalAI ai = col.GetComponent<SimpleAnimalAI>() ?? col.GetComponentInParent<SimpleAnimalAI>();
                Vector3 knockbackDir = (col.transform.position - transform.position).normalized;

                if (ai != null)
                {
                    ai.ApplyKnockback(knockbackDir, knockbackForce);
                }
                else
                {
                    Rigidbody animalRb = col.GetComponent<Rigidbody>();
                    if (animalRb != null)
                    {
                        knockbackDir.y = 0.5f;
                        animalRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
                    }
                    else
                    {
                        col.transform.position += knockbackDir * 0.5f;
                    }
                }

                Debug.Log($"[PlayerAttack] Hit {animal.animalName} for {attackDamage} damage.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1.0f + Vector3.up * 1.0f, attackRange);
    }
}
