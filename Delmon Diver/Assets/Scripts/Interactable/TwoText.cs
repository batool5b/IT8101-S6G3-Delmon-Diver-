using UnityEngine;

public class TwoText : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt = "Press E to Open";
    public string InteractionPrompt => _prompt;

    [TextArea(3, 10)] // This makes a big editable box in the Inspector
    [SerializeField] private string _popupMessage = "You found something interesting!";

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Opening Chest");
        
        interactor.OpenBigPopup(_popupMessage);
        
        return true;
    }
}