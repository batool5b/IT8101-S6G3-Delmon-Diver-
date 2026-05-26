using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HazardVolume : MonoBehaviour
{
    public float damageAmount = 10f;
    public float damageTickRate = 1f;

    private float nextDamageTime = 0f;

    void OnTriggerStay(Collider other)
    {
        if (Time.time >= nextDamageTime)
        {
            if (other.CompareTag("Player"))
            {
                // Assuming simple PlayerHealth component - adjust to match your exact health script name
                // PlayerHealth health = other.GetComponent<PlayerHealth>();
                // if (health != null) health.TakeDamage(damageAmount);
                
                Debug.Log("Player is taking fire/smoke damage! Implement health reduction here.");
                nextDamageTime = Time.time + damageTickRate;
            }
        }
    }
}
