using UnityEngine;

public class BoatProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 25f;
    public int damage = 10;
    public float lifeTime = 4f;

    private Transform target;
    private Vector3 moveDirection;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        moveDirection = transform.forward;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            moveDirection = (target.position - transform.position).normalized;
            Debug.Log("Projectile target set to: " + target.name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile hit: " + other.gameObject.name);

        SeaCreatureHealth creatureHealth = other.GetComponent<SeaCreatureHealth>();

        if (creatureHealth == null)
        {
            creatureHealth = other.GetComponentInParent<SeaCreatureHealth>();
        }

        if (creatureHealth != null)
        {
            creatureHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}