using UnityEngine;
using UnityEngine.InputSystem;

public class SmallBoatController : MonoBehaviour
{
    [Header("Boat Movement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    [Header("Boat Floating")]
    public float waterHeight = 0f;          //water surface height
    public float boatSinkAmount = 0.15f;    //how much boat goes under water
    public float floatSmoothness = 5f;

    [Header("Wave Motion")]
    public float waveHeight = 0.15f;        //up and down wave movement
    public float waveSpeed = 1.5f;          //wave speed
    public float tiltAmount = 3f;           //boat tilt strength

    [Header("Boat State")]
    public bool canMove = true;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("SmallBoatController: Missing Rigidbody!");
        }
    }

    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        FloatOnWater();

        if (canMove)
        {
            MoveBoat();
        }
        else
        {
            StopMovementInput();
        }
    }

    void ReadInput()
    {
        moveInput = 0f;
        turnInput = 0f;

        if (!canMove) return;

        if (Keyboard.current == null) return;

        //forward: W or Up Arrow
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            moveInput = 1f;
        }

        //backward: S or Down Arrow
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            moveInput = -1f;
        }

        //turn left: A or Left Arrow
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            turnInput = -1f;
        }

        //turn right: D or Right Arrow
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            turnInput = 1f;
        }
    }

    void MoveBoat()
    {
        if (rb == null) return;

        //move forward/backward
        Vector3 move = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        //turn left/right
        Quaternion turn = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }

    void FloatOnWater()
    {
        if (rb == null) return;

        //wave bobbing
        float waveOffset = Mathf.Sin(Time.time * waveSpeed) * waveHeight;

        //boat slightly under water
        float targetY = waterHeight - boatSinkAmount + waveOffset;

        Vector3 targetPosition = new Vector3(
            rb.position.x,
            targetY,
            rb.position.z
        );

        Vector3 smoothPosition = Vector3.Lerp(
            rb.position,
            targetPosition,
            floatSmoothness * Time.fixedDeltaTime
        );

        rb.MovePosition(smoothPosition);

        //gentle boat rocking
        float tiltX = Mathf.Sin(Time.time * waveSpeed) * tiltAmount;
        float tiltZ = Mathf.Cos(Time.time * waveSpeed * 0.8f) * tiltAmount;

        //keep the boat's Y rotation, only add small X/Z tilt
        Quaternion targetRotation = Quaternion.Euler(
            tiltX,
            rb.rotation.eulerAngles.y,
            tiltZ
        );

        rb.MoveRotation(Quaternion.Lerp(
            rb.rotation,
            targetRotation,
            Time.fixedDeltaTime * 0.5f
        ));
    }

    void StopMovementInput()
    {
        moveInput = 0f;
        turnInput = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void StopBoat()
    {
        canMove = false;
        StopMovementInput();

        Debug.Log("Boat stopped for combat.");
    }

    public void AllowBoatMove()
    {
        canMove = true;

        Debug.Log("Boat movement allowed again.");
    }
}