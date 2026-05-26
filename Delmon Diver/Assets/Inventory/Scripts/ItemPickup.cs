using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Pickup Settings")]
    [SerializeField] private ItemClass item;
    [SerializeField] private int amount = 1;
    [Tooltip("If true, the item is collected immediately when the player walks over it. If false, the player must press 'E'.")]
    [SerializeField] private bool autoPickup = false;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    // IInteractable Implementation
    public string InteractionPrompt => item != null ? $"Pick up {item.itemName} (x{amount})" : "Pick up item";

    public bool Interact(Interactor interactor)
    {
        return TryCollect(interactor.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If auto pickup is enabled, try to collect when the player touches the trigger
        if (autoPickup)
        {
            // Check if the colliding object is the player
            if (other.CompareTag("Player") || other.GetComponentInChildren<Interactor>() != null || other.GetComponent<simpleplayercontroller>() != null)
            {
                TryCollect(other.gameObject);
            }
        }
    }

    private bool TryCollect(GameObject playerObject)
    {
        if (item == null)
        {
            Debug.LogWarning("[ItemPickup] No item assigned to this pickup!");
            return false;
        }

        // 1. Locate InventoryManager
        InventoryManager inventory = null;
        
        // Search on the player object itself
        simpleplayercontroller controller = playerObject.GetComponent<simpleplayercontroller>() ?? playerObject.GetComponentInParent<simpleplayercontroller>();
        if (controller != null)
        {
            inventory = controller.inventory;
        }

        // Fallback search in hierarchy or scene
        if (inventory == null)
        {
            inventory = playerObject.GetComponentInChildren<InventoryManager>() ?? 
                        playerObject.GetComponentInParent<InventoryManager>() ?? 
                        FindObjectOfType<InventoryManager>();
        }

        if (inventory != null)
        {
            // 2. Add item to inventory
            bool added = inventory.Add(item, amount);
            if (added)
            {
                // Play audio feedback at pickup location (survives object destruction)
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1.0f);
                }

                Debug.Log($"[ItemPickup] Successfully collected {amount}x {item.itemName}.");
                Destroy(gameObject);
                return true;
            }
            else
            {
                Debug.LogWarning("[ItemPickup] Could not add item. Inventory might be full!");
                return false;
            }
        }
        else
        {
            Debug.LogError("[ItemPickup] InventoryManager not found! Cannot add item to inventory.");
            return false;
        }
    }
}
