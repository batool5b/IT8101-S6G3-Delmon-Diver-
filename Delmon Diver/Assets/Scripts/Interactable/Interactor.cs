using Fungus;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{

[SerializeField] private Transform _interactionPoint;

[SerializeField] private float _interactionPointRaduis = 0.5f;

[SerializeField] private LayerMask _interactableMask;

[SerializeField] private InteractionUI _interactionUI;

[SerializeField] public ThirdPersonCameraController cam;


private IInteractable _interactable;
private readonly Collider[] _colliders = new Collider[3];

[Header("Big Popup UI")]
[SerializeField] private GameObject _bigPopupPanel;
[SerializeField] private TMPro.TextMeshProUGUI _bigPopupText;

public void OpenBigPopup(string message)
{
    _interactionUI.Close(); 
    _bigPopupText.text = message;
    cam.EnableCursor();
    _bigPopupPanel.SetActive(true);

    Time.timeScale = 0f;
    
}

public void CloseBigPopup()
{
    _bigPopupPanel.SetActive(false);
    cam.DisableCursor();
    Time.timeScale = 1f; // Resume the game
}

[SerializeField] public int _numFound;

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position,_interactionPointRaduis ,_colliders ,_interactableMask);
    
        if (_numFound > 0)
        {
            _interactable = _colliders[0].GetComponent<IInteractable>();

            if (_interactable != null )
            {
                if (!_interactionUI.isDisplayed)
                {
                    _interactionUI.SetUp(_interactable.InteractionPrompt);
                }

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    _interactable.Interact(this);
                }
            }
        }
        else
        {
            if(_interactable != null)
            {
                _interactable = null;
            }
            if (_interactionUI.isDisplayed)
            {
                _interactionUI.Close();
            }
        }
    
    }

}
