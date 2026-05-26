using UnityEngine;

public class BoatPassengerFollow : MonoBehaviour
{
    [Header("Boat")]
    public Transform boat;

    [Header("Seat Position")]
    public Vector3 localOffset = new Vector3(0f, 0.8f, -0.9f);

    [Header("Rotation")]
    public bool followBoatRotation = true;

    void LateUpdate()
    {
        if (boat == null) return;

        //follow boat position with local offset
        transform.position = boat.TransformPoint(localOffset);

        //follow boat rotation
        if (followBoatRotation)
        {
            transform.rotation = boat.rotation;
        }
    }
}
