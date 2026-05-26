using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────
    public static ThirdPersonCameraController Instance { get; private set; }

    // ── Inspector fields ──────────────────────────────────────────────────
    [Header("References")]
    [Tooltip("The empty GameObject that Cinemachine tracks (CameraFollow).")]
    public Transform cameraFollowTarget;

    [Tooltip("InputManagement on the player — used for look inputs.")]
    public InputManagement inputManager;

    [Header("Mouse Sensitivity")]
    [Tooltip("How fast the camera turns left/right with the mouse.")]
    public float horizontalSensitivity = 200f;

    [Tooltip("How fast the camera tilts up/down with the mouse.")]
    public float verticalSensitivity   = 150f;

    [Header("Gamepad Sensitivity")]
    [Tooltip("How fast the camera turns left/right with the right stick.")]
    public float gamepadHorizontalSensitivity = 100f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 1.5f;

    [Header("Gamepad Settings")]
    public float gamepadSensitivity = 100f;

    [Header("Pitch Constraints")]
    public float minPitch = -40f;
    public float maxPitch = 75f;

    [Header("Underwater Options")]
    public float underwaterRollAngle = 90f;
    public float rollLerpSpeed = 5f;

    [Header("Options")]
    public bool invertY = false;
    public bool lockCursor = true;

    // ── Private Fields ────────────────────────────────────────────────────
    private float _yaw;
    private float _pitch;
    private float _roll;
    private bool _uiMode = false;

    private void Awake()
    {
        Instance = this;

        // Auto-find InputManagement if not assigned in Inspector
        if (inputManager == null)
            inputManager = FindFirstObjectByType<InputManagement>();

        if (cameraFollowTarget == null)
            Debug.LogError("[ThirdPersonCameraController] cameraFollowTarget is not assigned!");
    }

    private void Start()
    {
        LockCursor();

        // Initialise yaw so the camera starts facing the same way as the target
        if (cameraFollowTarget != null)
        {
            _yaw   = cameraFollowTarget.eulerAngles.y;
            _pitch = cameraFollowTarget.eulerAngles.x;
            _roll  = cameraFollowTarget.eulerAngles.z;
        }
    }

    private void LateUpdate()
    {
        // Update UI mode based on UIManager
        if (UIManager.Instance != null)
        {
            _uiMode = UIManager.Instance.IsAnyUIVisible();
        }

        HandleCursorToggle();

        if (!_uiMode)
        {
            HandleCameraRotation();   // look left/right/up/down
            HandleUnderwaterRoll();   // tilt when submerged
        }
        else
        {
            EnableCursor(); // Ensure cursor is free when UI is visible
        }
    }

    // ── Camera rotation ───────────────────────────────────────────────────

    private void HandleCameraRotation()
    {
        // Hard guard: if cursor is free (e.g. notebook UI) don't rotate
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (cameraFollowTarget == null) return;

        Vector2 look = inputManager != null ? inputManager.GetLookInput() : Vector2.zero;
        
        // Use a delta threshold or input system tracking to check for mouse
        bool usingMouse = inputManager != null ? inputManager.isLookInputFromMouse : true;

        float yawDelta = 0f;
        float pitchDelta = 0f;

        if (usingMouse)
        {
            // Mouse delta is a frame-based displacement. Do NOT multiply by Time.deltaTime.
            yawDelta = look.x * 0.005f * horizontalSensitivity * mouseSensitivity;
            pitchDelta = look.y * 0.005f * verticalSensitivity * mouseSensitivity;

            if (inputManager != null)
            {
                inputManager.ResetLookInput();
            }
        }
        else
        {
            // Gamepad right-stick: values range -1…1, needs deltaTime
            yawDelta = look.x * gamepadHorizontalSensitivity * gamepadSensitivity * Time.deltaTime;
            pitchDelta = look.y * gamepadHorizontalSensitivity * gamepadSensitivity * Time.deltaTime;
        }

        _yaw  += yawDelta;
        _pitch = 0f; // Force vertical angle to stay flat (left/right only)

        // Apply Pitch (Vertical) with inversion setting and clamping
        _pitch += invertY ? pitchDelta : -pitchDelta;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        // Write rotation to the follow target (Roll is updated in HandleUnderwaterRoll)
        cameraFollowTarget.rotation = Quaternion.Euler(_pitch, _yaw, _roll);
    }

    // ── Underwater roll ───────────────────────────────────────────────────

    private void HandleUnderwaterRoll()
    {
        // TODO: Connect this to your player's actual water/swimming state state logic
        bool submerged = false; 
        float targetRoll = submerged ? underwaterRollAngle : 0f;

        // Smoothly interpolate toward the target roll angle
        _roll = Mathf.LerpAngle(_roll, targetRoll, rollLerpSpeed * Time.deltaTime);

        // Apply back to target
        if (cameraFollowTarget != null)
            cameraFollowTarget.rotation = Quaternion.Euler(_pitch, _yaw, _roll);
    }

    // ── Cursor toggle (Escape) ────────────────────────────────────────────

    private void HandleCursorToggle()
    {
        if (inputManager != null && inputManager.uiCancelInput)
        {
            if (_uiMode) ExitUIMode();
            else         EnterUIMode();
        }
    }

    // ── Public API (call from UI buttons) ─────────────────────────────────

    /// <summary>Free the cursor and stop camera from rotating (open menus).</summary>
    public void EnterUIMode()
    {
        _uiMode = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>Lock the cursor back and resume camera control (close menus).</summary>
    public void ExitUIMode()
    {
        _uiMode = false;
        LockCursor();
    }

    /// <summary>Alias for EnterUIMode — frees cursor for UI interaction.</summary>
    public void EnableCursor()  => EnterUIMode();

    /// <summary>Alias for ExitUIMode — locks cursor for gameplay.</summary>
    public void DisableCursor() => ExitUIMode();

    // ── Helpers ───────────────────────────────────────────────────────────

    private void LockCursor()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}