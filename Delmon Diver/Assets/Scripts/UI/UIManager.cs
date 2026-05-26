using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject craftingPanel;
    public GameObject dangerPanel;
    public GameObject levelCompletePanel;
    public GameObject chatPanel;
    public GameObject dialogPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inputManager = Object.FindAnyObjectByType<InputManagement>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Hide all UI at start so they don't block character movement
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (craftingPanel != null) craftingPanel.SetActive(false);
        if (chatPanel != null) chatPanel.SetActive(false);
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
    }

    public bool IsAnyUIVisible()
    {
        return (inventoryPanel != null && inventoryPanel.activeSelf) ||
               (craftingPanel != null && craftingPanel.activeSelf) ||
               (chatPanel != null && chatPanel.activeSelf) ||
               (dialogPanel != null && dialogPanel.activeSelf) ||
               (levelCompletePanel != null && levelCompletePanel.activeSelf);
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;
        
        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);
        
        if (newState)
        {
            if (craftingPanel != null) craftingPanel.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (!IsAnyUIVisible())
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void SetDangerVisible(bool visible)
    {
        if (dangerPanel != null) dangerPanel.SetActive(visible);
    }

    public void ToggleChat()
    {
        if (chatPanel == null) return;
        
        bool newState = !chatPanel.activeSelf;
        chatPanel.SetActive(newState);
        
        if (newState)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (!IsAnyUIVisible())
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private InputManagement inputManager;

    private void Update()
    {
        if (inputManager == null)
            inputManager = Object.FindAnyObjectByType<InputManagement>();

        if (inputManager != null)
        {
            if (inputManager.toggleInventoryInput) ToggleInventory();
            if (inputManager.toggleChatInput) ToggleChat();
        }
    }

    public void ShowLevelComplete()
{
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
