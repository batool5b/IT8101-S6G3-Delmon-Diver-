using UnityEngine;

/// <summary>
/// EnvironmentDetector
/// Detects what environment the player is currently in by using
/// trigger colliders and a ground raycast.
///
/// ── SETUP ──────────────────────────────────────────────────────
/// 1. Add this script to your Player GameObject.
/// 2. In Unity: Edit → Project Settings → Tags and Layers → Layers
///    Create a Layer called "Water" (e.g. Layer 4).
/// 3. Select your Water GameObject and set its Layer to "Water".
/// 4. Make sure the Water GameObject has a Collider with "Is Trigger" = true.
/// 5. In this script’s Inspector, set waterLayerMask to the "Water" layer.
/// 6. Set groundLayerMask to your ground layer.
/// NOTE: No "Water" tag is needed. Detection is layer-based only.
/// ─────────────────────────────────────────────────────────────────────
/// </summary>
public class EnvironmentDetector : MonoBehaviour
{
    // ── Public flags read by PlayerLocomotion and AnimatorManager ─────
    public bool isGrounded       { get; private set; }
    public bool isInWater        { get; private set; }
    public bool isSubmerged      { get; private set; }  // fully underwater
    public bool isAtEdge         { get; private set; }  // about to run off a ledge
    public bool isSwimmingToEdge { get; private set; }  // swimming toward land edge

    // ── Inspector settings ────────────────────────────────────────────
    [Header("Ground Check")]
    [Tooltip("Layers that count as ground. Set this to your Ground layer.")]
    public LayerMask groundLayerMask = 1;  // default layer

    [Tooltip("Raycast length downward to detect ground.")]
    public float groundCheckDistance = 0.5f;

    [Tooltip("Offset from pivot to start the ground ray (keeps it inside the collider).")]
    public float groundCheckOriginY = 0.2f;

    [Header("Edge Detection")]
    [Tooltip("How far forward to check for edge (no ground ahead).")]
    public float edgeCheckDistance = 0.8f;

    [Tooltip("How far down the edge forward ray goes.")]
    public float edgeCheckDepth = 1.2f;

    [Header("Water Detection")]
    [Tooltip("Set this to the 'Water' layer in your project. NO tag needed.")]
    public LayerMask waterLayerMask;

    [Header("Submersion")]
    [Tooltip("How far above the water surface the player must be to count as submerged.")]
    public float submersionOffset = 0.5f;

    // ── Private References ────────────────────────────────────────────
    private PlayerManager _playerManager; // References the master manager class
    private int waterTriggerCount = 0;   // counts overlapping water triggers
    private float waterSurfaceY   = float.MinValue;

    private void Awake()
    {
        // Automatically find and cache the PlayerManager component on the player object
        _playerManager = GetComponent<PlayerManager>();
    }

    // ─────────────────────────────────────────────────────────────────
    private void FixedUpdate()
    {
        CheckGround();
        CheckEdge();
        CheckSubmerged();
        CheckSwimmingToEdge();
    }

    // ── Ground ────────────────────────────────────────────────────────
    private void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.up * groundCheckOriginY;
        isGrounded = Physics.Raycast(origin, Vector3.down,
                                     groundCheckDistance + groundCheckOriginY,
                                     groundLayerMask);
    }

    // ── Edge: ground exists under player but NOT one step ahead ───────
    private void CheckEdge()
    {
        if (!isGrounded) { isAtEdge = false; return; }

        Vector3 forwardOrigin = transform.position
                              + transform.forward * edgeCheckDistance
                              + Vector3.up * groundCheckOriginY;

        bool groundAhead = Physics.Raycast(forwardOrigin, Vector3.down,
                                           edgeCheckDepth, groundLayerMask);
        isAtEdge = !groundAhead;
    }

    // ── Submerged: player Y is below water surface ────────────────────
    private void CheckSubmerged()
    {
        if (!isInWater) { isSubmerged = false; return; }
        isSubmerged = transform.position.y < (waterSurfaceY - submersionOffset);
    }

    // ── Swimming toward edge: in water but ground very close ahead ────
    private void CheckSwimmingToEdge()
    {
        if (!isInWater) { isSwimmingToEdge = false; return; }

        Vector3 forwardOrigin = transform.position
                              + transform.forward * (edgeCheckDistance * 0.5f)
                              + Vector3.up * groundCheckOriginY;

        isSwimmingToEdge = Physics.Raycast(forwardOrigin, Vector3.down,
                                           edgeCheckDepth * 2f, groundLayerMask);
    }

    // ── Water trigger enter/exit ──────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        // Skip colliders that belong to the player itself (tools, weapons, etc.)
        if (other.transform.IsChildOf(transform.root)) return;
        if (other.gameObject.name.Contains("Tool") || other.gameObject.name.Contains("Weapon")) return;

        if (IsWater(other))
        {
            waterTriggerCount++;                    // ← was missing: keep count in sync
            isInWater  = true;
            isGrounded = false;
            waterSurfaceY = other.bounds.max.y;    // record surface height on enter
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsWater(other)) return;

        waterTriggerCount = Mathf.Max(0, waterTriggerCount - 1);
        if (waterTriggerCount == 0)
        {
            isInWater        = false;
            isSubmerged      = false;
            isSwimmingToEdge = false;
            waterSurfaceY    = float.MinValue;
        }
    }

    // ── Water detection ───────────────────────────────────────────────
    // Uses LayerMask ONLY — no CompareTag, so no "Water" tag needed.
    // Just set waterLayerMask in the Inspector to the Water layer.
    private bool IsWater(Collider col)
    {
        return ((waterLayerMask.value & (1 << col.gameObject.layer)) != 0);
    }

    // ── Debug gizmos (visible in Scene view) ─────────────────────────
    private void OnDrawGizmosSelected()
    {
        // Ground check ray — green=hit, red=miss
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 origin = transform.position + Vector3.up * groundCheckOriginY;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance + groundCheckOriginY));

        // Edge check ray — yellow
        Gizmos.color = Color.yellow;
        Vector3 fwd = transform.position + transform.forward * edgeCheckDistance + Vector3.up * groundCheckOriginY;
        Gizmos.DrawLine(fwd, fwd + Vector3.down * edgeCheckDepth);
    }
}