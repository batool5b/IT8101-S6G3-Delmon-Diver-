using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads all player input from the Input System (PlayerInputActions asset)
/// and stores it as simple floats and bools that other scripts can read.
/// Other scripts never touch the Input System directly — they read from here.
/// </summary>
public class InputManagement : MonoBehaviour
{
    // ── OUTPUT: read by PlayerLocomotion and AnimatorManager ──────────
    [HideInInspector] public float verticalInput;    // W/S: -1 to 1
    [HideInInspector] public float horizontalInput;  // A/D: -1 to 1
    [HideInInspector] public float moveAmount;       // 0=idle, 0.5=walk, 1=run
    [HideInInspector] public bool isRunning;        // Left Shift held
    [HideInInspector] public bool jumpSwimUpInput;  // Space held
    [HideInInspector] public bool diveInput;        // Left Ctrl held
    [HideInInspector] public bool interactInput;    // E key — pick up / talk
    [HideInInspector] public bool useToolInput;     // F key — use equipped tool
    [HideInInspector] public bool primaryActionInput; // Left Click / F — Attack
    [HideInInspector] public bool toggleInventoryInput; // I key - toggle inventory
    [HideInInspector] public bool toggleChatInput; // T key - toggle chat

    // ── OUTPUT: UI Inputs ────────────────────
[HideInInspector] public Vector2 uiNavigateInput;
    [HideInInspector] public bool uiSubmitInput;
    [HideInInspector] public bool uiCancelInput;
    [HideInInspector] public Vector2 uiPointInput;
    [HideInInspector] public bool uiClickInput;
    [HideInInspector] public Vector2 uiScrollWheelInput;

    // ── OUTPUT: read by ThirdPersonCameraController ────────────────────
    private Vector2 lookInput;
    [HideInInspector] public bool isLookInputFromMouse = true;

    // ── INTERNAL ─────────
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction diveAction;
    private InputAction interactAction;
    private InputAction useToolAction;
    private InputAction primaryAction;
    private InputAction lookAction;
    private InputAction toggleInventoryAction;
    private InputAction toggleChatAction;

    // UI Actions
private InputAction uiNavigateAction;
    private InputAction uiSubmitAction;
    private InputAction uiCancelAction;
    private InputAction uiPointAction;
    private InputAction uiClickAction;
    private InputAction uiScrollWheelAction;

    private void Awake()
    {
        InitializeActions();
    }

    private void InitializeActions()
    {
        // Cache actions from the project-wide asset
        var actions = InputSystem.actions;
        if (actions != null)
        {
            moveAction = actions.FindAction("Player Movement/Move") ?? actions.FindAction("Move");
            runAction = actions.FindAction("Player Movement/Run") ?? actions.FindAction("Run");
            jumpAction = actions.FindAction("Player Movement/Jump-SwimUp") ?? actions.FindAction("Jump-SwimUp");
            diveAction = actions.FindAction("Player Movement/Dive") ?? actions.FindAction("Dive");
            interactAction = actions.FindAction("Player Movement/Interact") ?? actions.FindAction("Interact");
            useToolAction = actions.FindAction("Player Movement/UseTool") ?? actions.FindAction("UseTool");
            primaryAction = actions.FindAction("Player Movement/PrimaryAction") ?? actions.FindAction("PrimaryAction");
            lookAction = actions.FindAction("Player Movement/Look") ?? actions.FindAction("Look");
            toggleInventoryAction = actions.FindAction("Player Movement/ToggleInventory") ?? actions.FindAction("ToggleInventory");
            toggleChatAction = actions.FindAction("Player Movement/ToggleChat") ?? actions.FindAction("ToggleChat");

            uiNavigateAction = actions.FindAction("UI/Navigate") ?? actions.FindAction("Navigate");
            uiSubmitAction = actions.FindAction("UI/Submit") ?? actions.FindAction("Submit");
            uiCancelAction = actions.FindAction("UI/Cancel") ?? actions.FindAction("Cancel");
            uiPointAction = actions.FindAction("UI/Point") ?? actions.FindAction("Point");
            uiClickAction = actions.FindAction("UI/Click") ?? actions.FindAction("Click");
            uiScrollWheelAction = actions.FindAction("UI/ScrollWheel") ?? actions.FindAction("ScrollWheel");

            // Ensure actions and maps are enabled
            actions.FindActionMap("Player Movement")?.Enable();
            actions.FindActionMap("UI")?.Enable();

            moveAction?.Enable();
runAction?.Enable();
            jumpAction?.Enable();
            diveAction?.Enable();
            interactAction?.Enable();
            useToolAction?.Enable();
            primaryAction?.Enable();
            lookAction?.Enable();
            toggleInventoryAction?.Enable();
            toggleChatAction?.Enable();

            uiNavigateAction?.Enable();
            uiSubmitAction?.Enable();
            uiCancelAction?.Enable();
            uiPointAction?.Enable();
            uiClickAction?.Enable();
            uiScrollWheelAction?.Enable();
        }
        else
        {
            Debug.LogError("InputManagement: No project-wide Input Actions asset found in InputSystem.actions!");
        }
    }

    public void HandleAllInputs()
    {
        if (moveAction == null) 
        {
            InitializeActions();
            if (moveAction == null) return;
        }

        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;

        isRunning = runAction != null && runAction.IsPressed();
        jumpSwimUpInput = jumpAction != null && jumpAction.IsPressed();
        diveInput = diveAction != null && diveAction.IsPressed();
        interactInput = interactAction != null && interactAction.IsPressed();
        useToolInput = useToolAction != null && useToolAction.IsPressed();
        primaryActionInput = primaryAction != null && primaryAction.IsPressed();
        toggleInventoryInput = toggleInventoryAction != null && toggleInventoryAction.WasPressedThisFrame();
        toggleChatInput = toggleChatAction != null && toggleChatAction.WasPressedThisFrame();

        if (lookAction != null)
{
            lookInput = lookAction.ReadValue<Vector2>();
            isLookInputFromMouse = true; 
        }

        // Handle UI Inputs
        if (uiNavigateAction != null) uiNavigateInput = uiNavigateAction.ReadValue<Vector2>();
        uiSubmitInput = uiSubmitAction != null && uiSubmitAction.WasPressedThisFrame();
        uiCancelInput = uiCancelAction != null && uiCancelAction.WasPressedThisFrame();
        if (uiPointAction != null) uiPointInput = uiPointAction.ReadValue<Vector2>();
        uiClickInput = uiClickAction != null && uiClickAction.WasPressedThisFrame();
        if (uiScrollWheelAction != null) uiScrollWheelInput = uiScrollWheelAction.ReadValue<Vector2>();

        bool hasInput = movementInput.sqrMagnitude > 0.01f;

        if (!hasInput) moveAmount = 0f;
        else if (isRunning) moveAmount = 1f;
        else moveAmount = 0.5f;
    }

    public Vector2 GetLookInput() => lookInput;

    public void ResetLookInput()
    {
        lookInput = Vector2.zero;
    }
}
