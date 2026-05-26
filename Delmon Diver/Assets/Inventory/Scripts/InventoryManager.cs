using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Fungus;

public class InventoryManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject itemCursor;
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] private GameObject hotBarSlotsHolder;
    [SerializeField] private GameObject inventory;

    [Header("Crafting")]
    [SerializeField] public GameObject CraftingPanel;
    [SerializeField] public GameObject CraftingSlots;

    [SerializeField] private List<Crafting> craftings = new List<Crafting>();

    [Header("Testing")]
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    [Header("Initial State")]
    [SerializeField] private SlotClass[] startingItems;

    [Header("Post-Run Items")]
    public SlotClass[] items;

    [Header("Hotbar")]
    [SerializeField] public GameObject hotBarSelector;
    [SerializeField] public int selectedSlotIndex = 0;

    public ItemClass selectedItem;

    public ThirdPersonCameraController cam;
    private InputManagement inputManager;
    private GameObject[] slots;
    public GameObject[] hotbarSlots;

    private SlotClass movingSlot;
    private SlotClass tempSlot;
    private SlotClass originalSlot;

    private bool isMovingItem = false;

    public bool IsOpen => inventory != null && inventory.activeSelf;

    private void Start()
    {
        inputManager = Object.FindAnyObjectByType<InputManagement>();
        if (itemCursor != null) itemCursor.SetActive(false);
        if (CraftingPanel != null) CraftingPanel.SetActive(false);
        if (inventory != null) inventory.SetActive(false);

        if (slotsHolder != null)
        {
            slots = new GameObject[slotsHolder.transform.childCount];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = slotsHolder.transform.GetChild(i).gameObject;
            }

            items = new SlotClass[slots.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new SlotClass();
            }
        }

        if (hotBarSlotsHolder != null)
        {
            hotbarSlots = new GameObject[hotBarSlotsHolder.transform.childCount];
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                hotbarSlots[i] = hotBarSlotsHolder.transform.GetChild(i).gameObject;
            }
        }

        if (startingItems != null)
        {
            for (int i = 0; i < startingItems.Length; i++)
            {
                if (startingItems[i] != null && startingItems[i].GetItem() != null && i < items.Length)
                {
                    items[i].AddItem(startingItems[i].GetItem(), startingItems[i].getQuantity());
                }
            }
        }

        RefreshUI();
        RefreshCraftingUI();

        if (itemToAdd != null) Add(itemToAdd, 1);
        if (itemToRemove != null) Remove(itemToRemove);
    }

    private void Update()
    {
        if (inputManager == null) return;

        if (inventory != null && inventory.activeSelf)
        {
            if (itemCursor != null) itemCursor.SetActive(isMovingItem);
            
            if (isMovingItem && itemCursor != null)
            {
                itemCursor.transform.position = inputManager.uiPointInput;

                Image cursorImage = itemCursor.GetComponent<Image>();
                if (movingSlot != null && movingSlot.GetItem() != null)
                {
                    if (cursorImage != null)
                    {
                        cursorImage.enabled = true;
                        cursorImage.sprite = movingSlot.GetItem().itemIcon;
                    }
                }
                else
                {
                    if (cursorImage != null) cursorImage.enabled = false;
                }
            }

            if (inputManager.uiClickInput)
            {
                if (TryCraftAtMouse()) return;

                if (isMovingItem) EndItemMove();
                else BeginItemSlot();
            }
        }
        else
        {
            if (itemCursor != null) itemCursor.SetActive(false);
        }

        if (hotbarSlots != null && hotbarSlots.Length > 0)
        {
            float scroll = inputManager.uiScrollWheelInput.y;
            if (scroll > 0f)
            {
                selectedSlotIndex++;
                if (selectedSlotIndex >= hotbarSlots.Length) selectedSlotIndex = 0;
            }
            else if (scroll < 0f)
            {
                selectedSlotIndex--;
                if (selectedSlotIndex < 0) selectedSlotIndex = hotbarSlots.Length - 1;
            }

            if (hotBarSelector != null && hotbarSlots[selectedSlotIndex] != null)
            {
                hotBarSelector.transform.position = hotbarSlots[selectedSlotIndex].transform.position;
            }

            if (items != null)
            {
                int hotbarStartIndex = items.Length - hotbarSlots.Length;
                if (hotbarStartIndex >= 0 && (hotbarStartIndex + selectedSlotIndex) < items.Length)
                {
                    selectedItem = items[hotbarStartIndex + selectedSlotIndex].GetItem();
                }
            }
        }
    }

    #region INVENTORY UTILS
    public bool Add(ItemClass item, int quantity)
    {
        SlotClass slot = Contains(item);
        if (slot != null && item.isStackable)
        {
            slot.AddQuantity(quantity);
        }
        else
        {
            bool foundEmpty = false;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null)
                {
                    items[i].AddItem(item, quantity);
                    foundEmpty = true;
                    break;
                }
            }
            if (!foundEmpty) return false;
        }
        RefreshUI();
        return true;
    }

    public bool Remove(ItemClass item)
    {
        SlotClass temp = Contains(item);
        if (temp != null)
        {
            if (temp.getQuantity() > 1) temp.SubQuantity(1);
            else temp.Clear();
        }
        else return false;

        RefreshUI();
        return true;
    }

    public bool Remove(ItemClass item, int quantity)
    {
        int total = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item) total += items[i].getQuantity();
        }
        if (total < quantity) return false;

        int remaining = quantity;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item)
            {
                int removeAmount = Mathf.Min(remaining, items[i].getQuantity());
                items[i].SubQuantity(removeAmount);
                remaining -= removeAmount;
                if (remaining <= 0) break;
            }
        }
        RefreshUI();
        return true;
    }

    public void UseSelected()
    {
        int hotbarStartIndex = items.Length - hotbarSlots.Length;
        items[hotbarStartIndex + selectedSlotIndex].SubQuantity(1);
        if (items[hotbarStartIndex + selectedSlotIndex].getQuantity() <= 0)
        {
            items[hotbarStartIndex + selectedSlotIndex].Clear();
        }
        RefreshUI();
    }

    public SlotClass Contains(ItemClass item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item) return items[i];
        }
        return null;
    }

    public bool Contains(ItemClass item, int quantity)
    {
        int total = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item) total += items[i].getQuantity();
        }
        return total >= quantity;
    }

    private void Craft(Crafting recipe)
    {
        if (recipe.CanCraft(this))
        {
            recipe.Craft(this);
            CraftingPanel.SetActive(false);
            inventory.SetActive(true);
            RefreshUI();
            RefreshCraftingUI();
        }
        else Debug.Log("Can't craft");
    }
    #endregion

    #region CRAFTING CLICK
    private bool TryCraftAtMouse()
    {
        if (CraftingPanel == null || !CraftingPanel.activeSelf) return false;
        for (int i = 0; i < CraftingSlots.transform.childCount; i++)
        {
            if (i >= craftings.Count || craftings[i] == null) continue;
            Transform craftingSlot = CraftingSlots.transform.GetChild(i);
            RectTransform outputRect = craftingSlot.GetChild(2).GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(outputRect, inputManager.uiPointInput))
            {
                Craft(craftings[i]);
                return true;
            }
        }
        return false;
    }
    #endregion

    #region UI
    public void CraftingButton()
    {
        inventory.SetActive(false);
        CraftingPanel.SetActive(true);
    }

    private void RefreshCraftingUI()
    {
        if (CraftingSlots == null) return;
        for (int i = 0; i < CraftingSlots.transform.childCount; i++)
        {
            Transform craftingSlot = CraftingSlots.transform.GetChild(i);
            if (i >= craftings.Count || craftings[i] == null)
            {
                craftingSlot.gameObject.SetActive(false);
                continue;
            }
            craftingSlot.gameObject.SetActive(true);
            Crafting recipe = craftings[i];
            Image input1Image = craftingSlot.GetChild(0).GetChild(0).GetComponent<Image>();
            TMP_Text input1Text = craftingSlot.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
            if (recipe.inputItems.Length > 0 && recipe.inputItems[0] != null && recipe.inputItems[0].GetItem() != null)
            {
                ItemClass item = recipe.inputItems[0].GetItem();
                input1Image.enabled = true;
                input1Image.sprite = item.itemIcon;
                input1Text.text = item.isStackable ? recipe.inputItems[0].getQuantity().ToString() : "";
            }
            else { input1Image.enabled = false; input1Text.text = ""; }

            Image input2Image = craftingSlot.GetChild(1).GetChild(0).GetComponent<Image>();
            TMP_Text input2Text = craftingSlot.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
            if (recipe.inputItems.Length > 1 && recipe.inputItems[1] != null && recipe.inputItems[1].GetItem() != null)
            {
                ItemClass item = recipe.inputItems[1].GetItem();
                input2Image.enabled = true;
                input2Image.sprite = item.itemIcon;
                input2Text.text = item.isStackable ? recipe.inputItems[1].getQuantity().ToString() : "";
            }
            else { input2Image.enabled = false; input2Text.text = ""; }

            Image outputImage = craftingSlot.GetChild(2).GetChild(0).GetComponent<Image>();
            TMP_Text outputText = craftingSlot.GetChild(2).GetChild(1).GetComponent<TMP_Text>();
            ItemClass outputItem = recipe.outputItem.GetItem();
            if (outputItem != null)
            {
                outputImage.enabled = true;
                outputImage.sprite = outputItem.itemIcon;
                outputText.text = outputItem.isStackable ? recipe.outputItem.getQuantity().ToString() : "";
            }
            else { outputImage.enabled = false; outputText.text = ""; }
        }
    }

    public void RefreshUI()
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            ItemClass currentItem = items[i].GetItem();
            Image iconImage = slots[i].transform.GetChild(0).GetComponent<Image>();
            TMP_Text quantityText = slots[i].transform.GetChild(1).GetComponent<TMP_Text>();
            if (currentItem != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = currentItem.itemIcon;
                quantityText.text = currentItem.isStackable ? items[i].getQuantity().ToString() : "";
            }
            else { iconImage.enabled = false; iconImage.sprite = null; quantityText.text = ""; }
        }
        RefreshHotbar();
    }

    public void RefreshHotbar()
    {
        if (hotbarSlots == null) return;
        int hotbarStartIndex = items.Length - hotbarSlots.Length;
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            SlotClass slotData = items[hotbarStartIndex + i];
            ItemClass currentItem = slotData.GetItem();
            Image iconImage = hotbarSlots[i].transform.GetChild(0).GetComponent<Image>();
            TMP_Text quantityText = hotbarSlots[i].transform.GetChild(1).GetComponent<TMP_Text>();
            if (currentItem != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = currentItem.itemIcon;
                quantityText.text = currentItem.isStackable ? slotData.getQuantity().ToString() : "";
            }
            else { iconImage.enabled = false; iconImage.sprite = null; quantityText.text = ""; }
        }
    }
    #endregion

    #region MOVING ITEMS
    private bool BeginItemSlot()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null) return false;
        movingSlot = new SlotClass(originalSlot.GetItem(), originalSlot.getQuantity());
        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }

    private bool BeginItemSlotHalf()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null) return false;
        int splitAmount = Mathf.CeilToInt(originalSlot.getQuantity() / 2f);
        movingSlot = new SlotClass(originalSlot.GetItem(), splitAmount);
        originalSlot.SubQuantity(splitAmount);
        if (originalSlot.getQuantity() <= 0) originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }

    private bool EndItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
        {
            bool added = Add(movingSlot.GetItem(), movingSlot.getQuantity());
            if (added) { movingSlot.Clear(); isMovingItem = false; }
            RefreshUI();
            return added;
        }

        if (originalSlot.GetItem() != null)
        {
            if (originalSlot.GetItem() == movingSlot.GetItem())
            {
                if (originalSlot.GetItem().isStackable)
                {
                    originalSlot.AddQuantity(movingSlot.getQuantity());
                    movingSlot.Clear(); isMovingItem = false;
                    RefreshUI(); return true;
                }
                else return false;
            }
            else
            {
                tempSlot = new SlotClass(originalSlot.GetItem(), originalSlot.getQuantity());
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.getQuantity());
                movingSlot = tempSlot;
                isMovingItem = true;
                RefreshUI(); return true;
            }
        }
        else
        {
            originalSlot.AddItem(movingSlot.GetItem(), movingSlot.getQuantity());
            movingSlot.Clear(); isMovingItem = false;
        }
        RefreshUI();
        return true;
    }

    private bool EndItemMove_Left()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null) return false;
        if (originalSlot.GetItem() != null && originalSlot.GetItem() != movingSlot.GetItem())
        {
            tempSlot = new SlotClass(originalSlot.GetItem(), originalSlot.getQuantity());
            originalSlot.AddItem(movingSlot.GetItem(), movingSlot.getQuantity());
            movingSlot = tempSlot;
            isMovingItem = true;
            RefreshUI(); return true;
        }
        if (!movingSlot.GetItem().isStackable)
        {
            tempSlot = new SlotClass(originalSlot.GetItem(), originalSlot.getQuantity());
            originalSlot.AddItem(movingSlot.GetItem(), movingSlot.getQuantity());
movingSlot = tempSlot;
            isMovingItem = true;
            RefreshUI(); return true;
        }
        if (originalSlot.GetItem() != null && originalSlot.GetItem() == movingSlot.GetItem())
        {
            originalSlot.AddQuantity(1);
        }
        else originalSlot.AddItem(movingSlot.GetItem(), 1);
        movingSlot.SubQuantity(1);
        if (movingSlot.getQuantity() <= 0) { movingSlot.Clear(); isMovingItem = false; }
        RefreshUI();
        return true;
    }

    private SlotClass GetClosestSlot()
    {
        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                RectTransform slotRect = slots[i].GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, inputManager.uiPointInput)) return items[i];
            }
        }
        if (hotbarSlots != null)
        {
            int hotbarStartIndex = items.Length - hotbarSlots.Length;
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                RectTransform slotRect = hotbarSlots[i].GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, inputManager.uiPointInput)) return items[hotbarStartIndex + i];
            }
        }
        return null;
    }
    #endregion
}