using UnityEngine;

/// <summary>
/// Plays movement sound effects (footsteps, running, swimming, jumping).
/// Attach to the Player GameObject alongside InputManagement and EnvironmentDetector.
///
/// Uses interval-based playback so footsteps fire at a natural rhythm
/// rather than looping continuously.
/// </summary>
public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Footstep Sounds (Land)")]
    [Tooltip("Array of footstep clips — a random one is picked each step for variety.")]
    [SerializeField] private AudioClip[] walkFootsteps;
    [SerializeField] private AudioClip[] runFootsteps;

    [Header("Swim Sounds (Water)")]
    [SerializeField] private AudioClip[] swimStrokes;

    [Header("Jump Sound")]
    [SerializeField] private AudioClip jumpSound;

    [Header("Timing (seconds between steps/strokes)")]
    [SerializeField] private float walkStepInterval  = 0.5f;
    [SerializeField] private float runStepInterval   = 0.3f;
    [SerializeField] private float swimStrokeInterval = 0.7f;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float footstepVolume = 0.6f;
    [Range(0f, 1f)]
    [SerializeField] private float swimVolume     = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float jumpVolume     = 0.8f;

    // Cached references
    private InputManagement     inputManager;
    private EnvironmentDetector env;

    // Internal state
    private float stepTimer = 0f;
    private bool  wasJumping = false;

    private void Awake()
    {
        inputManager = GetComponent<InputManagement>();
        if (inputManager == null)
            inputManager = GetComponentInParent<InputManagement>();

        env = GetComponent<EnvironmentDetector>();
        if (env == null)
            env = GetComponentInParent<EnvironmentDetector>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1.0f; // 3D positional audio
            }
        }
    }

    private void Update()
    {
        if (inputManager == null || env == null) return;

        HandleMovementSounds();
        HandleJumpSound();
    }

    private void HandleMovementSounds()
    {
        // Only play sounds when player is actually moving
        bool isMoving = inputManager.moveAmount > 0.1f;
        if (!isMoving)
        {
            stepTimer = 0f; // reset so first step plays immediately on next move
            return;
        }

        // Count down the step timer
        stepTimer -= Time.deltaTime;
        if (stepTimer > 0f) return;

        // Determine which sound set and interval to use
        if (env.isInWater)
        {
            // Swimming sounds
            PlayRandomClip(swimStrokes, swimVolume);
            stepTimer = swimStrokeInterval;
        }
        else if (env.isGrounded)
        {
            if (inputManager.isRunning)
            {
                // Running footsteps
                if (runFootsteps != null && runFootsteps.Length > 0)
                {
                    PlayRandomClip(runFootsteps, footstepVolume);
                    stepTimer = runStepInterval;
                }
                else
                {
                    Debug.LogWarning("[PlayerAudio] Run footstep clips missing!");
                    stepTimer = 1f; // Avoid spam
                }
            }
            else
            {
                // Walking footsteps
                if (walkFootsteps != null && walkFootsteps.Length > 0)
                {
                    PlayRandomClip(walkFootsteps, footstepVolume);
                    stepTimer = walkStepInterval;
                }
                else
                {
                    Debug.LogWarning("[PlayerAudio] Walk footstep clips missing!");
                    stepTimer = 1f;
                }
            }
        }
        else
        {
            // Debug: Why no sound?
            // Debug.Log("[PlayerAudio] Not in water and not grounded - no movement sounds.");
        }
    }

    private void HandleJumpSound()
    {
        // Detect jump: was grounded last frame, now Space is pressed and grounded
        bool jumpPressed = inputManager.jumpSwimUpInput;

        if (!env.isInWater && env.isGrounded && jumpPressed && !wasJumping)
        {
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound, jumpVolume);
            }
        }

        wasJumping = jumpPressed;
    }

    private void PlayRandomClip(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0) return;
        if (audioSource == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
