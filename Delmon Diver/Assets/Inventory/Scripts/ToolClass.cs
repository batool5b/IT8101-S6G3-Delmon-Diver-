using UnityEngine;

[CreateAssetMenu(fileName ="new Item Class" , menuName ="Item/Tool")]
public class ToolClass : ItemClass
{
    [Header("Tool")]
    public ToolType toolType;

    public enum ToolType
    {
        weapon ,
        pickaxe,
        hammer,
        axe
    }

    public override void Use(simpleplayercontroller caller)
    {
        Debug.Log("Swing " + GetItem().itemName);
    }

    public override ToolClass GetTool()
    {
        return this;
    }
}
