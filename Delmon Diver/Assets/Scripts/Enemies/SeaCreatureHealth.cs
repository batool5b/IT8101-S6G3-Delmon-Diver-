using UnityEngine;

public class SeaCreatureHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 20;
    public int currentHealth;

    [Header("References")]
    public SeaCreatureAroundBoat seaCreatureAI;

    void Start()
    {
        currentHealth = maxHealth;

        if (seaCreatureAI == null)
        {
            seaCreatureAI = GetComponent<SeaCreatureAroundBoat>();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Sea creature hit! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Sea creature defeated after 2 attacks!");

        if (seaCreatureAI != null)
        {
            seaCreatureAI.CreatureDefeated();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}