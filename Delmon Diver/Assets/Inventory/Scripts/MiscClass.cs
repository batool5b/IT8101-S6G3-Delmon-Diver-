using UnityEngine;

[CreateAssetMenu(fileName ="new Item Class" , menuName ="Item/Misc")]
public class MiscClass : ItemClass
{
    public override void Use(simpleplayercontroller caller)
    {
        Debug.Log("Using " + GetItem().itemName);
    }
    public override MiscClass GetMisc()
    {
        return this;
    }
}
