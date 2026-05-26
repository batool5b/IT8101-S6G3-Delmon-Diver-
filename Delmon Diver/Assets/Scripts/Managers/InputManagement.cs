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
    [HideInInspector] public bool toggleInventoryInput; // I key — toggle inventory
    [HideInInspector] public bool toggleChatInput;      // T key — toggle chat
    [HideInInspector] public bool primaryActionInput;   // Mouse 0 / Space — primary action
    [HideInInspector] public bool uiCancelInput;       // Escape — cancel/back
    [HideInInspector] public Vector2 uiPointInput;     // Mouse position
    [HideInInspector] public bool uiClickInput;        // Mouse 0 (click)
    [HideInInspector] public Vector2 uiScrollWheelInput; // Scroll wheel

    // ── OUTPUT: read by ThirdPersonCameraController ────────────────────
private Vector2 lookInput;
    [HideInInspector] public bool isLookInputFromMouse = true;

    // ── INTERNAL ──────────────────────────────────────────────────────
    private PlayerInputActions playerInputActions;  // auto-generated from .inputactions file
    private Vector2 movementInput;        // raw WASD vector

    private void Awake()
    {
        InitInputActions();
    }

    private void InitInputActions()
    {
        if (playerInputActions == null)
            playerInputActions = new PlayerInputActions();
    }

    // OnEnable/OnDisable: subscribe/unsubscribe to input events
    // This is the correct pattern — never subscribe in Awake or Start
    private void OnEnable()
    {
        InitInputActions();
        playerInputActions.PlayerMovement.Enable();
        playerInputActions.UI.Enable();

        // performed = key pressed or axis moved
        // canceled  = key released or axis returned to zero
        playerInputActions.PlayerMovement.Move.performed += OnMove;
        playerInputActions.PlayerMovement.Move.canceled += OnMove;
        playerInputActions.PlayerMovement.Run.performed += OnRun;
        playerInputActions.PlayerMovement.Run.canceled += OnRun;
        playerInputActions.PlayerMovement.JumpSwimUp.performed += OnJumpSwimUp;
        playerInputActions.PlayerMovement.JumpSwimUp.canceled += OnJumpSwimUp;
        playerInputActions.PlayerMovement.Dive.performed += OnDive;
        playerInputActions.PlayerMovement.Dive.canceled += OnDive;

        playerInputActions.PlayerMovement.Interact.performed += OnInteract;
        playerInputActions.PlayerMovement.Interact.canceled += OnInteract;
        playerInputActions.PlayerMovement.UseTool.performed += OnUseTool;
        playerInputActions.PlayerMovement.UseTool.canceled += OnUseTool;
        playerInputActions.PlayerMovement.Look.performed += OnLook;
        playerInputActions.PlayerMovement.Look.canceled += OnLook;

        playerInputActions.PlayerMovement.ToggleInventory.performed += OnToggleInventory;
        playerInputActions.PlayerMovement.ToggleInventory.canceled += OnToggleInventory;
        playerInputActions.PlayerMovement.ToggleChat.performed += OnToggleChat;
        playerInputActions.PlayerMovement.ToggleChat.canceled += OnToggleChat;
        playerInputActions.PlayerMovement.PrimaryAction.performed += OnPrimaryAction;
        playerInputActions.PlayerMovement.PrimaryAction.canceled += OnPrimaryAction;

        playerInputActions.UI.Cancel.performed += OnUICancel;
        playerInputActions.UI.Cancel.canceled += OnUICancel;
        playerInputActions.UI.Point.performed += OnUIPoint;
        playerInputActions.UI.Point.canceled += OnUIPoint;
        playerInputActions.UI.Click.performed += OnUIClick;
        playerInputActions.UI.Click.canceled += OnUIClick;
        playerInputActions.UI.ScrollWheel.performed += OnUIScrollWheel;
        playerInputActions.UI.ScrollWheel.canceled += OnUIScrollWheel;
    }

    private void OnDisable()
    {
        if (playerInputActions != null)
        {
            // Always unsubscribe to avoid memory leaks
            playerInputActions.PlayerMovement.Move.performed -= OnMove;
            playerInputActions.PlayerMovement.Move.canceled -= OnMove;
            playerInputActions.PlayerMovement.Run.performed -= OnRun;
            playerInputActions.PlayerMovement.Run.canceled -= OnRun;
            playerInputActions.PlayerMovement.JumpSwimUp.performed -= OnJumpSwimUp;
            playerInputActions.PlayerMovement.JumpSwimUp.canceled -= OnJumpSwimUp;
            playerInputActions.PlayerMovement.Dive.performed -= OnDive;
            playerInputActions.PlayerMovement.Dive.canceled -= OnDive;

            playerInputActions.PlayerMovement.Interact.performed -= OnInteract;
            playerInputActions.PlayerMovement.Interact.canceled -= OnInteract;
            playerInputActions.PlayerMovement.UseTool.performed -= OnUseTool;
            playerInputActions.PlayerMovement.UseTool.canceled -= OnUseTool;
            playerInputActions.PlayerMovement.Look.performed -= OnLook;
            playerInputActions.PlayerMovement.Look.canceled -= OnLook;

            playerInputActions.PlayerMovement.ToggleInventory.performed -= OnToggleInventory;
            playerInputActions.PlayerMovement.ToggleInventory.canceled -= OnToggleInventory;
            playerInputActions.PlayerMovement.ToggleChat.performed -= OnToggleChat;
            playerInputActions.PlayerMovement.ToggleChat.canceled -= OnToggleChat;
            playerInputActions.PlayerMovement.PrimaryAction.performed -= OnPrimaryAction;
            playerInputActions.PlayerMovement.PrimaryAction.canceled -= OnPrimaryAction;

            playerInputActions.UI.Cancel.performed -= OnUICancel;
            playerInputActions.UI.Cancel.canceled -= OnUICancel;
            playerInputActions.UI.Point.performed -= OnUIPoint;
            playerInputActions.UI.Point.canceled -= OnUIPoint;
            playerInputActions.UI.Click.performed -= OnUIClick;
            playerInputActions.UI.Click.canceled -= OnUIClick;
            playerInputActions.UI.ScrollWheel.performed -= OnUIScrollWheel;
            playerInputActions.UI.ScrollWheel.canceled -= OnUIScrollWheel;

            playerInputActions.PlayerMovement.Disable();
playerInputActions.UI.Disable();
        }
}

    // These are called automatically by the Input System when keys change
    // => is shorthand for a one-line function (lambda expression)
    private void OnMove(InputAction.CallbackContext ctx)
        => movementInput = ctx.ReadValue<Vector2>();  // reads WASD as X/Y vector

    private void OnRun(InputAction.CallbackContext ctx)
        => isRunning = ctx.ReadValueAsButton();        // true while Shift is held

    private void OnJumpSwimUp(InputAction.CallbackContext ctx)
        => jumpSwimUpInput = ctx.ReadValueAsButton();  // true while Space is held

    private void OnDive(InputAction.CallbackContext ctx)
        => diveInput = ctx.ReadValueAsButton();        // true while LCtrl is held

    private void OnInteract(InputAction.CallbackContext ctx)
        => interactInput = ctx.ReadValueAsButton();  // true while E is held

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
        isLookInputFromMouse = ctx.control.device is Mouse;
    }

    private void OnUseTool(InputAction.CallbackContext ctx)
        => useToolInput = ctx.ReadValueAsButton();   // true while F is held

    private void OnToggleInventory(InputAction.CallbackContext ctx)
        => toggleInventoryInput = ctx.ReadValueAsButton();

    private void OnToggleChat(InputAction.CallbackContext ctx)
        => toggleChatInput = ctx.ReadValueAsButton();

    private void OnPrimaryAction(InputAction.CallbackContext ctx)
        => primaryActionInput = ctx.ReadValueAsButton();

    private void OnUICancel(InputAction.CallbackContext ctx)
        => uiCancelInput = ctx.ReadValueAsButton();

    private void OnUIPoint(InputAction.CallbackContext ctx)
        => uiPointInput = ctx.ReadValue<Vector2>();

    private void OnUIClick(InputAction.CallbackContext ctx)
        => uiClickInput = ctx.ReadValueAsButton();

    private void OnUIScrollWheel(InputAction.CallbackContext ctx)
        => uiScrollWheelInput = ctx.ReadValue<Vector2>();

    /// <summary>
/// Get the camera look input (left/right rotation).
    /// Called by ThirdPersonCameraController.
    /// </summary>
    public Vector2 GetLookInput() => lookInput;

    public void ResetLookInput()
    {
        lookInput = Vector2.zero;
    }

    public void HandleAllInputs()
    {
        // Add a small deadzone to prevent drift from gamepads or virtual drivers
        if (movementInput.sqrMagnitude < 0.01f)
        {
            horizontalInput = 0f;
            verticalInput = 0f;
            moveAmount = 0f;
            return;
        }

        horizontalInput = movementInput.x;  // A = -1, D = +1
        verticalInput = movementInput.y;  // S = -1, W = +1

        // Build moveAmount: single number representing movement state
        // 0.0 = Idle (no input)
        // 0.5 = Walk (input but no shift)
        // 1.0 = Run  (input + shift)
        if (isRunning) moveAmount = 1f;
        else moveAmount = 0.5f;
    }
}