using UnityEngine;

public class SeaCreatureAroundBoat : MonoBehaviour
{
    [Header("Boat")]
    public Transform boat;

    [Header("Boat Systems")]
    public SmallBoatController boatController;
    public BoatAttack boatAttack;

    [Header("UI")]
    public DangerUI dangerUI;

    [Header("Detection")]
    public float appearDistance = 60f;
    public float stopDistance = 90f;

    [Header("Circle In Same Area")]
    public float circleRadius = 10f;
    public float circleSpeed = 2f;
    public float moveSpeed = 8f;

    [Header("Height")]
    public bool useStartY = true;
    public float customY = 1.2f;

    [Header("Rotation")]
    public bool rotateWithMovement = false;
    public bool yAxisOnly = true;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 circleCenter;
    private float circleY;
    private float angle = 0f;

    private bool isCircling = false;
    private bool isDefeated = false;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        circleY = useStartY ? startPosition.y : customY;

        circleCenter = new Vector3(
            startPosition.x,
            circleY,
            startPosition.z
        );

        Debug.Log("Sea creature circle center: " + circleCenter);
    }

    void Update()
    {
        if (isDefeated) return;

        if (boat == null)
        {
            Debug.LogError("Boat is not assigned!");
            return;
        }

        float distance = Vector2.Distance(
            new Vector2(circleCenter.x, circleCenter.z),
            new Vector2(boat.position.x, boat.position.z)
        );

        if (!isCircling && distance <= appearDistance)
        {
            StartCombat();
        }

        if (isCircling && distance >= stopDistance)
        {
            StopCombatWithoutDefeat();
        }

        if (isCircling)
        {
            CircleInSamePlace();
        }
    }

    void StartCombat()
    {
        isCircling = true;

        if (dangerUI != null)
        {
            dangerUI.ShowDanger();
        }

        if (boatController != null)
        {
            boatController.StopBoat();
        }

        if (boatAttack != null)
        {
            boatAttack.EnableAttack();
        }

        Debug.Log("Combat started! Creature is circling.");
    }

    void StopCombatWithoutDefeat()
    {
        isCircling = false;

        if (dangerUI != null)
        {
            dangerUI.HideDanger();
        }

        if (boatController != null)
        {
            boatController.AllowBoatMove();
        }

        if (boatAttack != null)
        {
            boatAttack.DisableAttack();
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        Debug.Log("Boat escaped. Combat stopped.");
    }

    void CircleInSamePlace()
    {
        angle += circleSpeed * Time.deltaTime;

        float x = Mathf.Cos(angle) * circleRadius;
        float z = Mathf.Sin(angle) * circleRadius;

        Vector3 targetPosition = new Vector3(
            circleCenter.x + x,
            circleY,
            circleCenter.z + z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (!rotateWithMovement)
        {
            transform.rotation = startRotation;
            return;
        }

        if (yAxisOnly)
        {
            Vector3 direction = targetPosition - transform.position;

            if (direction != Vector3.zero)
            {
                float targetY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                Quaternion targetRotation = Quaternion.Euler(
                    startRotation.eulerAngles.x,
                    targetY,
                    startRotation.eulerAngles.z
                );

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    5f * Time.deltaTime
                );
            }
        }
    }

    public void CreatureDefeated()
    {
        isDefeated = true;
        isCircling = false;

        if (dangerUI != null)
        {
            dangerUI.HideDanger();
        }

        if (boatController != null)
        {
            boatController.AllowBoatMove();
        }

        if (boatAttack != null)
        {
            boatAttack.DisableAttack();
        }

        Debug.Log("Creature defeated. Combat finished.");

        gameObject.SetActive(false);
    }
}