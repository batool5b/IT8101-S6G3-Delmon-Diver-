using UnityEngine;

/// <summary>
/// Master controller. Checks InventoryManager.IsOpen to block movement while
/// the inventory panel is visible.
/// When inventory is open:
///   - Input reading stops (player can't accidentally move)
///   - Rigidbody velocity is zeroed
///   - Animation is set to idle (moveAmount=0) so character doesn't freeze mid-run
///   - Physics is skipped
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private InputManagement     inputManager;
    private PlayerLocomotion    playerLocomotion;
    private AnimatorManager     animatorManager;
    private EnvironmentDetector env;
    private Rigidbody           rb;

    private InventoryManager inventoryManager;

    private void Awake()
    {
        inputManager     = GetComponent<InputManagement>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animatorManager  = GetComponent<AnimatorManager>();
        env              = GetComponent<EnvironmentDetector>();
        rb               = GetComponent<Rigidbody>();
        inventoryManager = GetComponent<InventoryManager>();
        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (inputManager     == null) Debug.LogError("Missing InputManagement!");
        if (playerLocomotion == null) Debug.LogError("Missing PlayerLocomotion!");
        if (animatorManager  == null) Debug.LogError("Missing AnimatorManager!");
        if (env              == null) Debug.LogError("Missing EnvironmentDetector!");
        if (inventoryManager == null) Debug.LogWarning("[PlayerManager] InventoryManager not found — movement will never be blocked.");
    }

    private void Update()
    {
        // Always handle inputs so that toggle flags (Inventory, Chat) are updated correctly.
        if (inputManager != null)
            inputManager.HandleAllInputs();

        bool isUIVisible = UIManager.Instance != null && UIManager.Instance.IsAnyUIVisible();

        if (isUIVisible)
        {
            // Drive animator to idle if UI is visible
            if (animatorManager != null && env != null)
                animatorManager.UpdateAnimations(0f, env);
            return;
        }

        if (env != null && animatorManager != null)
            animatorManager.UpdateAnimations(inputManager.moveAmount, env);
    }

    private void FixedUpdate()
    {
        // Always call HandleAllMovement; it now internally checks UIManager.IsAnyUIVisible
        // to restrict input while preserving gravity/physics.
        if (playerLocomotion != null)
            playerLocomotion.HandleAllMovement();
    }

    private bool CanMove()
    {
        if (inventoryManager == null) return true;
        return !inventoryManager.IsOpen;
    }

    // Call this function whenever the player selects or picks up a tool
public void EquipTool(GameObject toolPrefab, Transform handSlot)
{
    // 1. Spawn the tool at the player's hand position
    GameObject equippedTool = Instantiate(toolPrefab, handSlot.position, handSlot.rotation);
    equippedTool.transform.SetParent(handSlot);

    // 2. Clear out any physical colliders so it doesn't crash your EnvironmentDetector
    if (equippedTool.TryGetComponent<Collider>(out Collider toolCollider))
    {
        toolCollider.enabled = false; 
    }

    // 3. Make sure any child objects of the tool also lose their colliders
    Collider[] childColliders = equippedTool.GetComponentsInChildren<Collider>();
    foreach (Collider childCol in childColliders)
    {
        childCol.enabled = false;
    }
}
}