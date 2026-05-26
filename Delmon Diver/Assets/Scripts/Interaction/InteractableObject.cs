using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public string promptMessage = "Interact";
    public UnityEvent onInteract;
    
    // Call this method from your player interaction Raycast script
    public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
        onInteract.Invoke();
    }
}
