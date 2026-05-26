using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyObject : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    public float floatPower = 15f; // How strong the water pushes up
    public float depthBeforeSubmerged = 1f; // How deep it needs to go to get max push
    
    [Header("Drag Settings")]
    public float waterDrag = 3f; // Slows it down so it doesn't bounce forever
    public float waterAngularDrag = 1f;

    public Transform[] floaters;
    
    [HideInInspector] public bool inWater = false;
    [HideInInspector] public WaterVolume waterVolume;

    private Rigidbody rb;
    private float originalDrag;
    private float originalAngularDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.linearDamping;
        originalAngularDrag = rb.angularDamping;

        if (floaters == null || floaters.Length == 0)
        {
            // fallback if no floaters assigned
            floaters = new Transform[1];
            floaters[0] = transform;
        }
    }

    void FixedUpdate()
    {
        if (inWater && waterVolume != null)
        {
            // Apply water drag so it doesn't act like a trampoline!
            rb.linearDamping = waterDrag;
            rb.angularDamping = waterAngularDrag;

            float forceMultiplier = 1f / floaters.Length;

            for (int i = 0; i < floaters.Length; i++)
            {
                Transform floater = floaters[i];
                float waterVolY = waterVolume.GetWaterLevelAtPos(floater.position);

                // Check if floater is actually underwater
                if (floater.position.y < waterVolY)
                {
                    float displacementOffset = Mathf.Clamp01((waterVolY - floater.position.y) / depthBeforeSubmerged);
                    
                    // Simplified, stable upward force
                    Vector3 upLift = new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementOffset * floatPower, 0f);
                    
                    rb.AddForceAtPosition(upLift * forceMultiplier, floater.position, ForceMode.Force);
                }
            }
        }
        else
        {
            // Set drag back to normal when out of water
            rb.linearDamping = originalDrag;
            rb.angularDamping = originalAngularDrag;
        }
    }
}
