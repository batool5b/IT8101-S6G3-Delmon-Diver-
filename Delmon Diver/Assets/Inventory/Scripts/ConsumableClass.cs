using UnityEngine;

[CreateAssetMenu(fileName ="new Item Class" , menuName ="Item/Consumable")]
public class ConsumableClass : ItemClass
{
    
    [Header("Consumable")]
    public float healthAdded;

    public override void Use(simpleplayercontroller caller)
    {
        Debug.Log("Eating a " + GetItem().itemName);
        Debug.Log("You've restored "+ healthAdded + " Health points");
        caller.inventory.UseSelected();
    }
    public override ConsumableClass GetConsumable()
    {
        return this;
    }

}
