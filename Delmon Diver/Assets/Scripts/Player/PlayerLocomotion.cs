using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private InputManagement     inputManager;
    private EnvironmentDetector env;
    private AnimatorManager     animatorManager;
    private Transform           cameraObject;
    private Rigidbody           playerRigidbody;

    [Header("Land Movement")]
    public float walkSpeed     = 4f;
    public float runSpeed      = 8f;
    public float rotationSpeed = 15f;

    [Header("Water Movement")]
    public float swimSpeed    = 3f;
    public float swimRunSpeed = 5f;
    public float waterDrag    = 3f;
    public float normalDrag   = 0f;

    [Header("Dive (run off edge)")]
    public float diveForce    = 6f;

    [Header("Jump / SwimUp")]
    public float jumpForce   = 5f;
    public float swimUpForce = 3f;
    public float swimDownForce = 3f;

    private bool hasDived = false;

    private void Awake()
    {
        inputManager    = GetComponent<InputManagement>();
        env             = GetComponent<EnvironmentDetector>();
        animatorManager = GetComponent<AnimatorManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject    = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        if (env == null) return;

        bool isUIVisible = UIManager.Instance != null && UIManager.Instance.IsAnyUIVisible();
        bool shouldSwim = env.isInWater && (!env.isGrounded || env.isSubmerged);

        UpdateDrag(shouldSwim);

        if (shouldSwim)
        {
            HandleSwimMovement(isUIVisible);
        }
        else
        {
            HandleLandMovement(isUIVisible);
            if (!isUIVisible)
            {
                HandleJump();
                HandleRunOffEdge();
            }
        }

        HandleRotation(isUIVisible);
    }

    private void HandleLandMovement(bool forceStop)
    {
        hasDived = false; 
        float yVel = playerRigidbody.linearVelocity.y;

        if (forceStop || inputManager == null || inputManager.moveAmount <= 0f)
        {
            playerRigidbody.linearVelocity = new Vector3(0f, yVel, 0f);
            return;
        }

        Vector3 dir = GetFlatCameraDirection();
        float spd = inputManager.isRunning ? runSpeed : walkSpeed;
        playerRigidbody.linearVelocity = new Vector3(dir.x * spd, yVel, dir.z * spd);
    }

    private void HandleJump()
    {
        if (inputManager == null || !inputManager.jumpSwimUpInput) return;
        if (!env.isGrounded) return;
        if (animatorManager != null) animatorManager.TriggerJump();
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void HandleSwimMovement(bool forceStop)
    {
        if (forceStop || inputManager == null)
        {
            playerRigidbody.linearVelocity = new Vector3(0f, 0f, 0f);
            return;
        }

        Vector3 dir = GetFlatCameraDirection();
        float xzSpeed = inputManager.isRunning ? swimRunSpeed : swimSpeed;
        float targetYVelocity = 0f;

        if (inputManager.jumpSwimUpInput) targetYVelocity = swimUpForce;
        else if (inputManager.diveInput) targetYVelocity = -swimDownForce;

        float moveX = 0f;
        float moveZ = 0f;

        if (inputManager.moveAmount > 0f)
        {
            moveX = dir.x * xzSpeed;
            moveZ = dir.z * xzSpeed;
        }

        playerRigidbody.linearVelocity = new Vector3(moveX, targetYVelocity, moveZ);
    }

    private void HandleRunOffEdge()
    {
        if (inputManager == null || !env.isAtEdge || !inputManager.isRunning || hasDived) return;
        hasDived = true;
        Vector3 diveDir = (transform.forward + Vector3.down * 0.4f).normalized;
        playerRigidbody.AddForce(diveDir * diveForce, ForceMode.Impulse);
        if (animatorManager != null) animatorManager.TriggerDive();
    }

    private Vector3 GetFlatCameraDirection()
    {
        if (cameraObject == null) return transform.forward;
        Vector3 forward = cameraObject.forward;
        Vector3 right = cameraObject.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        Vector3 dir = forward * inputManager.verticalInput;
        dir += right * inputManager.horizontalInput;
        return dir.normalized;
    }

    private void HandleRotation(bool forceStop)
    {
        if (forceStop) return;
        Vector3 dir = GetFlatCameraDirection();
        if (dir == Vector3.zero) return;
        Quaternion target = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
    }

    private void UpdateDrag(bool shouldSwim)
    {
        playerRigidbody.linearDamping = shouldSwim ? waterDrag : normalDrag;
        playerRigidbody.useGravity = !shouldSwim;
    }
}