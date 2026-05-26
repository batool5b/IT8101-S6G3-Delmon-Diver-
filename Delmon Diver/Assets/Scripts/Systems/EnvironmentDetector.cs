using UnityEngine;

/// <summary>
/// Detects whether the player is on land, in water, at an edge, submerged,
/// or swimming toward land. All results stored as public bools read by
/// AnimatorManager and PlayerLocomotion every frame.
///
/// Setup requirements:
///   - Water mesh: Add Box Collider → Is Trigger = ON → Tag = "Water"
///   - Ground mesh: Set Layer = "Ground" (set same layer in Inspector below)
/// </summary>
public class EnvironmentDetector : MonoBehaviour
{
    [Header("Ground")]
    public LayerMask groundLayer;           // set to "Ground" in Inspector
    public float     groundCheckDistance = 0.25f;  // how far down to raycast

    [Header("Edge (land side)")]
    // how far ahead of the player to look for a drop-off
    public float edgeLookAhead = 0.7f;

    [Header("Swimming To Edge (water side)")]
    // how close to shore before Swimming To Edge animation plays
    public float swimToEdgeDistance = 1.5f;

    // ── PUBLIC FLAGS (read by other scripts every frame) ──────────────
    [HideInInspector] public bool isInWater;        // inside water trigger
    [HideInInspector] public bool isGrounded;       // feet touching ground
    [HideInInspector] public bool isAtEdge;         // on land, cliff ahead
    [HideInInspector] public bool isSubmerged;      // head below water surface
    [HideInInspector] public bool isSwimmingToEdge; // in water, shore is close

    private float waterSurfaceY = float.MinValue;   // Y of water top surface

    // ── UPDATE: runs every frame ──────────────────────────────────────
    private void Update()
    {
        CheckGround();       // is player standing on something?
        CheckLandEdge();     // is there a drop-off ahead?
        CheckSubmerged();    // is head below water?
        CheckSwimmingToEdge(); // is shore close ahead?
    }

    // ── WATER TRIGGER: Unity calls these automatically ────────────────

    // Fires when player's collider overlaps the water trigger collider
    private void OnTriggerEnter(Collider other)
    {
        HandleWaterOverlap(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isInWater) HandleWaterOverlap(other);
    }

    private void HandleWaterOverlap(Collider other)
    {
        if (!other.CompareTag("Water")) return;
        if (!isInWater) Debug.Log($"[EnvironmentDetector] Entered water: {other.name}");
        isInWater     = true;
        // Record where the water surface is (top of water collider)
        waterSurfaceY = other.bounds.max.y;
    }

    // Fires when player's collider leaves the water trigger collider
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Water")) return;
        Debug.Log($"[EnvironmentDetector] Exited water: {other.name}");
        isInWater        = false;
        isSubmerged      = false;
        isSwimmingToEdge = false;
        waterSurfaceY    = float.MinValue;
    }

    // ── GROUND CHECK ─────────────────────────────────────────────────
    // Shoots a short ray downward from just above the player's feet
    // If it hits anything on the "Ground" layer → isGrounded = true
    private void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,  // start slightly above feet
            Vector3.down,                              // direction: straight down
            groundCheckDistance + 0.1f,               // max distance
            groundLayer);                             // only hits "Ground" layer
    }

    // ── EDGE CHECK (land side) ────────────────────────────────────────
    // Shoots a ray downward a short distance AHEAD of the player
    // If no ground is found ahead → player is at a cliff/edge
    private void CheckLandEdge()
    {
        if (isInWater) { isAtEdge = false; return; }

        // Point ahead of player, slightly elevated
        Vector3 ahead    = transform.position
                         + transform.forward * edgeLookAhead
                         + Vector3.up * 0.1f;

        bool groundAhead = Physics.Raycast(ahead, Vector3.down, 1.5f, groundLayer);

        // Edge = standing on ground AND no ground directly ahead
        isAtEdge = isGrounded && !groundAhead;
    }

    // ── SUBMERGED CHECK ───────────────────────────────────────────────
    // Player is submerged if their head (1.8m above feet) is below water
    private void CheckSubmerged()
    {
        if (!isInWater) { isSubmerged = false; return; }
        float headY = transform.position.y + 1.8f;
        isSubmerged = headY < waterSurfaceY;
    }

    // ── SWIMMING TO EDGE CHECK (water side) ───────────────────────────
    // While in water, shoots a horizontal ray forward at water surface level
    // If it hits land → player is approaching shore → play Swimming To Edge
    private void CheckSwimmingToEdge()
    {
        if (!isInWater) { isSwimmingToEdge = false; return; }

        // Origin: at water surface, slightly in front of player
        Vector3 origin = new Vector3(
            transform.position.x,
            waterSurfaceY - 0.1f,   // just below surface
            transform.position.z)
            + transform.forward * 0.3f;

        // If there's land directly ahead within swimToEdgeDistance → approaching shore
        isSwimmingToEdge = Physics.Raycast(
            origin, transform.forward, swimToEdgeDistance, groundLayer);
    }

    // ── GIZMOS: visible in Scene view when object is selected ─────────
    // Green/Red line = ground check ray
    // Yellow/White lines = edge check ray
    // Cyan/Blue line = swim-to-edge check ray
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(
            transform.position + Vector3.up * 0.1f,
            transform.position - Vector3.up * groundCheckDistance);

        Gizmos.color = isAtEdge ? Color.yellow : Color.white;
        Vector3 ahead = transform.position + transform.forward * edgeLookAhead + Vector3.up * 0.1f;
        Gizmos.DrawLine(ahead, ahead - Vector3.up * 1.5f);

        Gizmos.color = isSwimmingToEdge ? Color.cyan : Color.blue;
        Vector3 swimOrigin = new Vector3(
            transform.position.x, waterSurfaceY - 0.1f, transform.position.z)
            + transform.forward * 0.3f;
        Gizmos.DrawLine(swimOrigin, swimOrigin + transform.forward * swimToEdgeDistance);
    }
}