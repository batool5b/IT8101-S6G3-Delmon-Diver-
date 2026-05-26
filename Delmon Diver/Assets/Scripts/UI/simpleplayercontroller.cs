using UnityEngine;
using UnityEngine.InputSystem;

public class simpleplayercontroller : MonoBehaviour
{

    public InventoryManager inventory;

    public Interactor interactor;

    private InputManagement inputManager;

    void Start()
    {
        inputManager = Object.FindAnyObjectByType<InputManagement>();
    }

    void Update()
    {
        if (inputManager == null) return;

        if (inputManager.interactInput && interactor._numFound == 0)
        {
            if (inventory.selectedItem != null)
            {
                inventory.selectedItem.Use(this);
            }
            else
            {
                Debug.Log("No item");
            }
        }
    }
}
