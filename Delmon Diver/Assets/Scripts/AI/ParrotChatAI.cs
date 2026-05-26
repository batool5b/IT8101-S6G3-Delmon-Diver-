using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ParrotChatAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float chatRange = 3f;

    [Header("Chat UI")]
    public GameObject chatUI;

    private bool chatOpen = false;

    void Start()
    {
        if (chatUI != null)
        {
            chatUI.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        //close only if player moves away
        if (distance > chatRange && chatOpen)
        {
            CloseChat();
            return;
        }

        //do not use E to close/open chat while typing in the input field
        if (IsTypingInInputField())
        {
            return;
        }

        //open/close chat with E only when not typing
        if (distance <= chatRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleChat();
        }
    }

    void ToggleChat()
    {
        chatOpen = !chatOpen;

        if (chatUI != null)
        {
            chatUI.SetActive(chatOpen);
            chatUI.transform.SetAsLastSibling();
        }
    }

    void CloseChat()
    {
        chatOpen = false;

        if (chatUI != null)
        {
            chatUI.SetActive(false);
        }
    }

    bool IsTypingInInputField()
    {
        if (EventSystem.current == null) return false;

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null) return false;

        if (selectedObject.GetComponent<TMP_InputField>() != null)
        {
            return true;
        }

        if (selectedObject.GetComponent<InputField>() != null)
        {
            return true;
        }

        return false;
    }
}