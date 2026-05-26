using UnityEngine;

public class ItemDrop : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string _prompt =
        "Press E to Pick Up";

    public string InteractionPrompt => _prompt;

    [Header("Inventory Item")]
    [SerializeField]
    private ItemClass itemClass;

    [SerializeField]
    private int amount = 1;

    private InventoryManager inventory;

    private void Start()
    {
        inventory =
            FindFirstObjectByType<InventoryManager>();
    }

    public bool Interact(Interactor interactor)
    {
        inventory.Add(itemClass, amount);

        Destroy(gameObject);

        return true;
    }
}