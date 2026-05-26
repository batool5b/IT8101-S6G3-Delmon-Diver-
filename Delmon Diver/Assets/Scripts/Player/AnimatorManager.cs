using UnityEngine;

/// <summary>
/// Drives the Animator Controller parameters every frame.
/// The Animator Controller reads these parameters and decides
/// which animation clip to play based on the transitions you set up.
/// </summary>
public class AnimatorManager : MonoBehaviour
{
    private Animator animator;  // Unity's built-in Animator component

    // StringToHash converts a string name to an integer ID
    // This is faster than passing strings every frame
    // These names must EXACTLY match what you typed in the Animator Controller Parameters panel
    private static readonly int MoveSpeedHash        = Animator.StringToHash("MoveSpeed");
    private static readonly int IsInWaterHash        = Animator.StringToHash("IsInWater");
    private static readonly int IsSubmergedHash      = Animator.StringToHash("IsSubmerged");
    private static readonly int IsAtEdgeHash         = Animator.StringToHash("IsAtEdge");
    private static readonly int IsGroundedHash       = Animator.StringToHash("IsGrounded");
    private static readonly int IsSwimmingToEdgeHash = Animator.StringToHash("IsSwimmingToEdge");
    private static readonly int DiveTriggerHash      = Animator.StringToHash("Dive");
    private static readonly int ThrowCreatureHash    = Animator.StringToHash("ThrowCreature");

    private void Awake()
{
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("AnimatorManager: No Animator component found on " + gameObject.name);
    }

    /// <summary>
    /// Called every frame from PlayerManager.
    /// moveAmount: 0 = idle, 0.5 = walk/swim, 1 = run/swimfast
    /// env: all the detection flags from EnvironmentDetector
    /// </summary>
    public void UpdateAnimations(float moveAmount, EnvironmentDetector env)
    {
        if (env == null) return;

        // Mathf.Lerp smoothly blends from current value toward target
        // Time.deltaTime * 10f means "reach target in about 0.1 seconds"
        // This prevents jarring instant switches between idle/walk/run
        float current  = animator.GetFloat(MoveSpeedHash);
        float smoothed = Mathf.Lerp(current, moveAmount, Time.deltaTime * 10f);
        animator.SetFloat(MoveSpeedHash, smoothed);

        // Booleans are set directly — no smoothing needed
        // The Animator Controller transitions use these to decide state changes:
        //   IsInWater=true  → switch to Swim states
        //   IsAtEdge=true   → trigger edge/dive behaviour
        //   IsGrounded=true → allow jump, exit water states
        animator.SetBool(IsInWaterHash,        env.isInWater);
        animator.SetBool(IsSubmergedHash,      env.isSubmerged);
        animator.SetBool(IsAtEdgeHash,         env.isAtEdge);
        animator.SetBool(IsGroundedHash,       env.isGrounded);
        animator.SetBool(IsSwimmingToEdgeHash, env.isSwimmingToEdge);
    }

    /// <summary>
    /// Called ONCE from PlayerLocomotion when player runs off edge.
    /// Triggers are consumed immediately by the Animator (fire-and-forget).
    /// </summary>
    public void TriggerDive()
    {
        animator.SetTrigger(DiveTriggerHash);
    }

    /// <summary>
    /// Called from BoatAttack when player throws a creature.
    /// </summary>
    public void TriggerThrowCreature()
    {
        animator.SetTrigger(ThrowCreatureHash);
    }
}