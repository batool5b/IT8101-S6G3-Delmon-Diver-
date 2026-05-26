using System;
using UnityEngine;

[System.Serializable]
public struct UpgradeRequirement
{
    [Tooltip("The item required for this upgrade step.")]
    public ItemClass item;
    [Tooltip("The quantity of this item needed.")]
    public int requiredAmount;
}

[System.Serializable]
public struct UpgradeStage
{
    public string stageName;
    [Tooltip("The materials required to complete this stage.")]
    public UpgradeRequirement[] requirements;
    [Tooltip("The GameObject representing this visual stage (enabled when this stage is active/reached).")]
    public GameObject visualObject;
}

public class ShipUpgrader : MonoBehaviour, IInteractable
{
    [Header("Ship Upgrade Progression")]
    [SerializeField] private UpgradeStage[] upgradeStages;
    [SerializeField] private int currentStageIndex = 0;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip upgradeSuccessSound;
    [SerializeField] private AudioClip upgradeFailSound;
    [SerializeField] private AudioClip finalQuestCompleteSound;

    // Events for UI, Quests, or GameManagers to subscribe to
    public static event Action<int> OnShipUpgraded;
    public static event Action OnShipFullyUpgraded;

    // Public Getters
    public int CurrentStageIndex => currentStageIndex;
    public int TotalStages => upgradeStages.Length;
    public bool IsFullyUpgraded => currentStageIndex >= upgradeStages.Length;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1.0f; // 3D spatial sound
            }
        }
    }

    private void Start()
    {
        UpdateVisuals();
    }

    // IInteractable Implementation
    public string InteractionPrompt
    {
        get
        {
            if (IsFullyUpgraded)
            {
                return "Ship is fully upgraded! Ready to sail.";
            }

            UpgradeStage stage = upgradeStages[currentStageIndex];
            string prompt = $"Repair Ship ({stage.stageName}): Needs ";
            
            for (int i = 0; i < stage.requirements.Length; i++)
            {
                var req = stage.requirements[i];
                prompt += $"{req.requiredAmount}x {req.item.itemName}";
                if (i < stage.requirements.Length - 1) prompt += ", ";
            }
            
            prompt += " (Interact with E)";
            return prompt;
        }
    }

    public bool Interact(Interactor interactor)
    {
        if (IsFullyUpgraded)
        {
            interactor.OpenBigPopup("The ship is already fully upgraded and seaworthy! You are ready to set sail!");
            return false;
        }

        // 1. Locate InventoryManager from the interactor
        InventoryManager inventory = null;
        simpleplayercontroller controller = interactor.GetComponent<simpleplayercontroller>() ?? interactor.GetComponentInParent<simpleplayercontroller>();
        if (controller != null)
        {
            inventory = controller.inventory;
        }

        if (inventory == null)
        {
            inventory = interactor.GetComponentInChildren<InventoryManager>() ?? 
                        interactor.GetComponentInParent<InventoryManager>() ?? 
                        FindObjectOfType<InventoryManager>();
        }

        if (inventory == null)
        {
            Debug.LogError("[ShipUpgrader] InventoryManager not found! Cannot process ship upgrade.");
            return false;
        }

        // 2. Verify all requirements for the current stage
        UpgradeStage activeStage = upgradeStages[currentStageIndex];
        bool hasAllMaterials = true;
        string missingPromptText = $"Missing Materials for {activeStage.stageName}:\n\n";

        foreach (var req in activeStage.requirements)
        {
            int currentInventoryCount = GetItemCountInInventory(inventory, req.item);
            if (currentInventoryCount < req.requiredAmount)
            {
                hasAllMaterials = false;
                int needed = req.requiredAmount - currentInventoryCount;
                missingPromptText += $"- {req.item.itemName}: Need {needed} more (Have: {currentInventoryCount}/{req.requiredAmount})\n";
            }
        }

        if (!hasAllMaterials)
        {
            // Play error sound and show details in the player's big popup panel
            PlayAudio(upgradeFailSound);
            interactor.OpenBigPopup(missingPromptText);
            return false;
        }

        // 3. Deduct materials from inventory
        foreach (var req in activeStage.requirements)
        {
            inventory.Remove(req.item, req.requiredAmount);
        }

        // 4. Advance Stage
        currentStageIndex++;
        OnShipUpgraded?.Invoke(currentStageIndex);

        // Update Quest Progress
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddProgress("upgrade_ship", 1);
        }

        // 5. Update Visual Assets in the Scene
        UpdateVisuals();

        // 6. Give feedback
        if (IsFullyUpgraded)
        {
            PlayAudio(finalQuestCompleteSound);
            interactor.OpenBigPopup("CONGRATULATIONS!\n\nThe ship has been fully repaired, upgraded, and is ready for deep sea diving!\n\nQuest Complete: Escape Delmon Island!");
            OnShipFullyUpgraded?.Invoke();
        }
        else
        {
            PlayAudio(upgradeSuccessSound);
            interactor.OpenBigPopup($"UPGRADE SUCCESSFUL!\n\nYou successfully contributed materials and advanced to: {upgradeStages[currentStageIndex].stageName}!");
        }

        return true;
    }

    private int GetItemCountInInventory(InventoryManager inventory, ItemClass targetItem)
    {
        if (inventory.items == null) return 0;

        int totalCount = 0;
        foreach (var slot in inventory.items)
        {
            if (slot != null && slot.GetItem() == targetItem)
            {
                totalCount += slot.getQuantity();
            }
        }
        return totalCount;
    }

    private void UpdateVisuals()
    {
        if (upgradeStages == null || upgradeStages.Length == 0) return;

        // Visuals can be designed in two ways:
        // Option 1: Only the active stage model is enabled.
        // Option 2: All completed stages stay enabled, and the current stage is enabled.
        // Let's implement Option 1 (clean transition) but design it to be flexible.
        for (int i = 0; i < upgradeStages.Length; i++)
        {
            if (upgradeStages[i].visualObject != null)
            {
                // If ship is fully upgraded, keep only the final stage active.
                // Otherwise, keep only the active current stage visual active.
                bool shouldBeActive = false;
                
                if (IsFullyUpgraded)
                {
                    shouldBeActive = (i == upgradeStages.Length - 1);
                }
                else
                {
                    shouldBeActive = (i == currentStageIndex);
                }

                upgradeStages[i].visualObject.SetActive(shouldBeActive);
            }
        }
    }

    private void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // ==========================================
    // Context Menu for Debugging Quest Progress
    // ==========================================
    [ContextMenu("Force Upgrade Stage")]
    private void TestUpgrade()
    {
        if (IsFullyUpgraded)
        {
            Debug.Log("[ShipUpgrader] Already fully upgraded!");
            return;
        }

        currentStageIndex++;
        UpdateVisuals();
        OnShipUpgraded?.Invoke(currentStageIndex);
        
        if (IsFullyUpgraded)
        {
            OnShipFullyUpgraded?.Invoke();
            Debug.Log("[ShipUpgrader] Forced upgrade to maximum stage!");
        }
        else
        {
            Debug.Log($"[ShipUpgrader] Forced upgrade to stage {currentStageIndex}: {upgradeStages[currentStageIndex].stageName}");
        }
    }

    [ContextMenu("Reset Upgrades")]
    private void TestReset()
    {
        currentStageIndex = 0;
        UpdateVisuals();
        Debug.Log("[ShipUpgrader] Quest progress reset to Stage 0.");
    }
}
