using UnityEngine;

public class Level1Item : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Level1SequenceManager manager = Object.FindAnyObjectByType<Level1SequenceManager>();
            if (manager != null)
            {
                manager.ItemCollected();
                gameObject.SetActive(false);
            }
        }
    }
}
