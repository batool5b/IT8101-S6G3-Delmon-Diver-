using UnityEngine;

/// <summary>
/// Controls all Animator parameters and triggers.
/// Other scripts should NEVER talk directly to the Animator.
/// They should only call functions in this class.
/// </summary>
public class AnimatorManager : MonoBehaviour
{
    private Animator animator;  // Unity's built-in Animator component

    private static readonly int IsSwimmingToEdgeHash =
        Animator.StringToHash("IsSwimmingToEdge");

    private static readonly int DiveTriggerHash =
        Animator.StringToHash("Dive");

    private static readonly int PunchTriggerHash =
        Animator.StringToHash("Punch");
    
    private static readonly int ThrowCreatureHash = 
        Animator.StringToHash("ThrowCreature");
    
    private static readonly int JumpTriggerHash      = 
        Animator.StringToHash("Jump");
    
    private static readonly int IsJumpingHash  =
        Animator.StringToHash("IsJumping");

    private static readonly int MoveSpeedHash = 
        Animator.StringToHash("MoveSpeed");

    private static readonly int IsInWaterHash = 
        Animator.StringToHash("IsInWater");

    private static readonly int IsSubmergedHash = 
        Animator.StringToHash("IsSubmerged");

    private static readonly int IsAtEdgeHash = 
        Animator.StringToHash("IsAtEdge");

    private static readonly int IsGroundedHash = 
        Animator.StringToHash("IsGrounded");
    // ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError(
                "AnimatorManager: No Animator found on " + gameObject.name);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // MOVEMENT / STATE UPDATES
    // Called every frame from PlayerManager
    // ─────────────────────────────────────────────────────────────

    public void UpdateAnimations(
        float moveAmount,
        EnvironmentDetector env)
    {
        if (animator == null || env == null)
            return;

        // Smooth movement blending
        float current =
            animator.GetFloat(MoveSpeedHash);

        float smoothed =
            Mathf.Lerp(
                current,
                moveAmount,
                Time.deltaTime * 10f);

        animator.SetFloat(MoveSpeedHash, smoothed);

        // Environment states
        animator.SetBool(IsInWaterHash, env.isInWater);
        animator.SetBool(IsSubmergedHash, env.isSubmerged);
        animator.SetBool(IsAtEdgeHash, env.isAtEdge);
        animator.SetBool(IsGroundedHash, env.isGrounded);
        animator.SetBool(
            IsSwimmingToEdgeHash,
            env.isSwimmingToEdge);
    }

    // ─────────────────────────────────────────────────────────────
    // TRIGGERS
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Trigger dive animation.
    /// Called when running off edge into water.
    /// </summary>
    public void TriggerDive()
    {
        if (animator == null)
            return;

        animator.SetTrigger(DiveTriggerHash);
    }


    public void TriggerPunch()
    {
        Debug.Log("Punch triggered");
        if (animator == null)
            return;

        animator.SetTrigger(PunchTriggerHash);
    }

    public void TriggerThrowCreature()
    {
        animator.SetTrigger(ThrowCreatureHash);
    }

    /// <summary>
    /// Called from PlayerLocomotion when the player jumps on land.
    /// Fires the "Jump" trigger and sets "IsJumping" bool.
    /// </summary>
    public void TriggerJump()
    {
        animator.SetTrigger(JumpTriggerHash);
        animator.SetBool(IsJumpingHash, true);
    }

    /// <summary>
    /// Called when the player lands on the ground to exit jump animation.
    /// </summary>
    public void SetGrounded(bool grounded)
    {
        if (grounded)
        {
            animator.SetBool(IsJumpingHash, false);
        }
    }
}