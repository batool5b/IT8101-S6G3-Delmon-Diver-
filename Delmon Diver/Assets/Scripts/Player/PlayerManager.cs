using UnityEngine;

/// <summary>
/// The master controller. Calls the other scripts in the correct order
/// every frame. This is the only script that has Update/FixedUpdate.
///
/// Order matters:
///   Update      → read input first, then update animations (visual)
///   FixedUpdate → move the Rigidbody (physics runs at fixed timestep)
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private InputManagement     inputManager;     // reads keyboard/gamepad
    private PlayerLocomotion    playerLocomotion; // moves the Rigidbody
    private AnimatorManager     animatorManager;  // drives the Animator
    private EnvironmentDetector env;              // detects land/water/edge

    private void Awake()
    {
        // GetComponent finds each script on this same GameObject
        inputManager     = GetComponent<InputManagement>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animatorManager  = GetComponent<AnimatorManager>();
        env              = GetComponent<EnvironmentDetector>();

        if (inputManager     == null) Debug.LogError("Missing InputManagement!");
        if (playerLocomotion == null) Debug.LogError("Missing PlayerLocomotion!");
        if (animatorManager  == null) Debug.LogError("Missing AnimatorManager!");
        if (env              == null) Debug.LogError("Missing EnvironmentDetector!");
    }

    // Update runs every frame (60fps+)
    private void Update()
    {
        // 1. Read input first so other systems have fresh data
        inputManager.HandleAllInputs();

        // 2. Push data to Animator so animation matches movement
        if (env != null)
            animatorManager.UpdateAnimations(inputManager.moveAmount, env);
    }

    // FixedUpdate runs at fixed intervals (default 50/sec, set in Project Settings)
    // Always move Rigidbodies here — never in Update — for smooth physics
    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
}