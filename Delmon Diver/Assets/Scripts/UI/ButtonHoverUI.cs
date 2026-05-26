using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Text buttonText;

    public int normalSize = 36;
    public int hoverSize = 46;

    void Start()
    {
        // If you forgot to assign it in the inspector, try to find it automatically
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<Text>();
        }

        // Double check it's actually found before using it
        if (buttonText != null)
        {
            buttonText.fontSize = normalSize;
        }
        else
        {
            Debug.LogError($"ButtonHoverUI on {gameObject.name} is missing a Text component link!", this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.fontSize = hoverSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.fontSize = normalSize;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.fontSize = normalSize;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void OnDisable()
    {
        if (buttonText != null) buttonText.fontSize = normalSize;
    }
}