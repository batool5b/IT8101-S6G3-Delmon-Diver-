using UnityEngine;

public class ParrotMovement : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player; 

    public Vector3 followOffset = new Vector3(0f, 2.2f, -1.2f); 
    public float followSpeed = 3f;
    public float catchUpSpeed = 7f;
    public float maxDistance = 5f;

    [Header("Turning Settings")]
    public float turnSpeed = 6f;

    [Header("Animation")]
    public Animator birdAnimator; 
    public string flyStateName = "fly"; 

    void Start()
    {
        PlayFlyAnimation();
    }

    void Update()
    {
        if (player == null) return;

        FollowPlayer();
        TurnWithPlayer();
        PlayFlyAnimation();
    }

    void FollowPlayer()
    {
        //bird follows the player direction, not world direction
        Vector3 targetPosition = player.position + player.TransformDirection(followOffset);

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        float currentSpeed = distanceToTarget > maxDistance ? catchUpSpeed : followSpeed;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            currentSpeed * Time.deltaTime
        );
    }

    void TurnWithPlayer()
    {
        //bird turns with the same direction as the player
        Quaternion targetRotation = player.rotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    void PlayFlyAnimation()
    {
        if (birdAnimator == null) return;

        //set animator parameters
        birdAnimator.SetBool("flying", true);
        birdAnimator.SetBool("perched", false);
        birdAnimator.SetBool("landing", false);

        //force the fly animation if it is not already playing
        AnimatorStateInfo stateInfo = birdAnimator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName(flyStateName))
        {
            birdAnimator.CrossFade(flyStateName, 0.1f);
        }
    }
}