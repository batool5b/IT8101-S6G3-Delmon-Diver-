using UnityEngine;
using UnityEngine.InputSystem;

public class BoatAttack : MonoBehaviour
{
    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float cooldown = 0.8f;

    [Header("Animation")]
    public AnimatorManager characterAnimatorManager;

    [Header("Combat State")]
    public bool canAttack = false;

    private float nextAttackTime = 0f;

    void Update()
    {
        if (!canAttack) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + cooldown;
        }
    }

    void Attack()
    {
        Debug.Log("Space pressed. Attack started.");

        if (characterAnimatorManager != null)
        {
            characterAnimatorManager.TriggerThrowCreature();
            Debug.Log("Throw animation triggered.");
        }
        else
        {
            Debug.LogWarning("Character AnimatorManager is not assigned.");
        }

        Shoot();
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing projectile prefab or fire point!");
            return;
        }

        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Debug.Log("Boat attack fired!");
    }

    public void EnableAttack()
    {
        canAttack = true;
        Debug.Log("Boat attack enabled.");
    }

    public void DisableAttack()
    {
        canAttack = false;
        Debug.Log("Boat attack disabled.");
    }
}