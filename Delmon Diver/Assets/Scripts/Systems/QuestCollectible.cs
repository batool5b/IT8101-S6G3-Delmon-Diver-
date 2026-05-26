using UnityEngine;

/// <summary>
/// Attach this to any collectible item in the scene (like coins, pearls, trash).
/// Make sure the GameObject has a Collider set to 'Is Trigger'.
/// </summary>
public class QuestCollectible : MonoBehaviour
{
    [Header("Quest Integration")]
    [Tooltip("The exact Quest Id defined in the QuestManager (e.g., '1')")]
    public string questId;

    [Tooltip("Amount of progress to add when collected")]
    public int amount = 1;

    [Header("Audio (Optional)")]
    [Tooltip("Sound effect to play upon pickup")]
    public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        // Trigger check for player (checks tag on collider, and parent for component safety)
        bool isPlayer = other.CompareTag("Player") || other.GetComponentInParent<PlayerManager>() != null;

        if (isPlayer)
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.AddProgress(questId, amount);
                Debug.Log($"Collected item '{gameObject.name}' for Quest '{questId}' (+{amount} progress)");
            }
            else
            {
                Debug.LogWarning("No QuestManager instance found in the scene!");
            }

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Remove the collected object from the world
            Destroy(gameObject);
        }
    }
}
