
using UnityEngine;

[CreateAssetMenu(fileName = "newCraftingReceipe", menuName = "Crafting/Receipe")]
public class Crafting : ScriptableObject
{
    [Header("Crafting Receipe")]

    public SlotClass[] inputItems;
    public SlotClass outputItem;

public bool CanCraft(InventoryManager inventory)
{
    for (int i = 0; i < inputItems.Length; i++)
    {
        if (!inventory.Contains(
            inputItems[i].GetItem(),
            inputItems[i].getQuantity()))
        {
            return false;
        }
    }

    ItemClass output = outputItem.GetItem();

    SlotClass existingSlot =
        inventory.Contains(output);

    if (existingSlot != null &&
        output.isStackable)
    {
        return true;
    }

    for (int i = 0; i < inventory.items.Length; i++)
    {
        if (inventory.items[i].GetItem() == null)
        {
            return true;
        }
    }

    return false;
}

    public void Craft(InventoryManager inventory)
    {
        for (int i = 0; i < inputItems.Length; i++)
        {
            inventory.Remove(
                inputItems[i].GetItem(),
                inputItems[i].getQuantity());
        }

        inventory.Add(
            outputItem.GetItem(),
            outputItem.getQuantity());
    }
}