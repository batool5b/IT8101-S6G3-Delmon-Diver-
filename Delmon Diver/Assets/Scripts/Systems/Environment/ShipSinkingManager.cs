using UnityEngine;

public class ShipSinkingManager : MonoBehaviour
{
    [Header("Sinking Settings")]
    public float sinkSpeed = 0.05f;
    public float tiltSpeed = 0.5f;
    public Vector3 targetRotation = new Vector3(15f, 0f, 30f);
    public float maxDepth = -15f;

    [Header("Control")]
    public bool isSinking = false;

    private float currentSinkDepth;

    void Start()
    {
        currentSinkDepth = transform.position.y;
    }

    void Update()
    {
        if (isSinking)
        {
            // Lower the ship down slowly
            if (transform.position.y > maxDepth)
            {
                currentSinkDepth -= sinkSpeed * Time.deltaTime;
                Vector3 newPos = transform.position;
                newPos.y = currentSinkDepth;
                transform.position = newPos;
            }

            // Tilt the ship
            Quaternion targetQuat = Quaternion.Euler(targetRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetQuat, tiltSpeed * Time.deltaTime * 0.01f);
        }
    }
    
    // Call this to trigger dramatic sinking moments based on game progress
    public void AcceleratedSinking(float extraDepth)
    {
        currentSinkDepth -= extraDepth;
        sinkSpeed += 0.02f;
        tiltSpeed += 0.2f;
    }
}
