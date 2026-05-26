using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Diagnostics;


public class ItemClass : ScriptableObject
{
    [Header("Item")]
    public string itemName;
    public Sprite itemIcon;

    public bool isStackable = true;


    public virtual void Use(simpleplayercontroller caller)
    {
        Debug.Log("Using");
    }
    public virtual ItemClass GetItem() {return this;}
    public virtual ToolClass GetTool() {return null;}
    public virtual ConsumableClass GetConsumable() {return null;}
    public virtual MiscClass GetMisc() {return null;}

}
