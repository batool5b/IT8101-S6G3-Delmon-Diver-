using UnityEngine;

/// <summary>
/// Simple AI for animals. Supports Attack, RunAway, and Passive behaviors.
/// Moves the animal based on player proximity.
/// </summary>
public class SimpleAnimalAI : MonoBehaviour
{
    public enum Behavior { Passive, RunAway, Attack }

    [Header("Behavior Settings")]
    public Behavior behavior = Behavior.Passive;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public float damagePerSecond = 10f;

    [Header("References")]
    private Transform player;
    private InventoryFramework.PlayerStats playerStats;
    private CharacterController controller;
    private Animator animator;

    // Animator Hashes
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

    private float attackTimer = 0f;
    private const float AttackCooldown = 1.5f;

    [Header("Combat")]
    public AudioClip attackSound;
    private AudioSource audioSource;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        // Find player by tag or type
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStats = playerObj.GetComponent<InventoryFramework.PlayerStats>();
        }
    }

    private void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f) isKnockedBack = false;
            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (behavior == Behavior.RunAway)
        {
            if (distance < detectionRange)
            {
                MoveAwayFromPlayer();
            }
            else
            {
                Idle();
            }
        }
        else if (behavior == Behavior.Attack)
        {
            if (distance < detectionRange)
            {
                if (distance <= attackRange)
                {
                    AttackPlayer();
                }
                else
                {
                    MoveTowardsPlayer();
                }
            }
            else
            {
                Idle();
            }
        }
        else
        {
            Idle();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep on ground
        
        Move(direction);
        FaceDirection(direction);
    }

    private void MoveAwayFromPlayer()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        direction.y = 0;
        
        Move(direction);
        FaceDirection(direction);
    }

    private void Move(Vector3 direction)
    {
        if (controller != null)
        {
            controller.SimpleMove(direction * moveSpeed);
        }
        else
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        if (animator != null)
        {
            animator.SetFloat(MoveSpeedHash, moveSpeed);
        }
    }

    private void Idle()
    {
        if (animator != null)
        {
            animator.SetFloat(MoveSpeedHash, 0f);
        }
    }

    private void FaceDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        isKnockedBack = true;
        knockbackTimer = 0.5f;
        if (controller != null)
        {
            // Simplified knockback for CC
            controller.Move(direction * force * Time.deltaTime);
        }
    }

    private void AttackPlayer()
    {
        // Stop moving
        Idle();
        FaceDirection((player.position - transform.position).normalized);

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            if (animator != null)
            {
                animator.SetTrigger(AttackTriggerHash);
            }

            if (attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            if (playerStats != null)
            {
                playerStats.TakeDamage(damagePerSecond); // Simple immediate damage for now
                Debug.Log($"[SimpleAnimalAI] Animal attacked player! Damage: {damagePerSecond}");
            }

            attackTimer = AttackCooldown;
        }
    }
}
