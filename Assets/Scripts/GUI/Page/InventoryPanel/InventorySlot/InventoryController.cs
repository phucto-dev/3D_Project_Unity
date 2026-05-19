using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public PlayerInventorySO PlayerInventory;
    public GameObject SlotContainer;

    public event Action<ItemInstance> OnSlotDataChanged;

    private InventorySlot[] _listSlot;
    private void Awake()
    {
        if (SlotContainer == null) return;
        _listSlot = SlotContainer.GetComponentsInChildren<InventorySlot>(true);
    }
    private void OnEnable()
    {
        if (PlayerInventory == null) return;
        PlayerInventory.OnSlotUpdated += UpdateSlot;
    }
    private void OnDisable()
    {
        if (PlayerInventory == null) return;
        PlayerInventory.OnSlotUpdated -= UpdateSlot;
    }
    private void Start()
    {
        if (PlayerInventory == null) return;
        RefreshUI(PlayerInventory.PlayerInventory);
    }
    public void UpdateSlot(int i, ItemInstance slot)
    {
        if (_listSlot == null || _listSlot.Length == 0) return;
        _listSlot[i].UpdateSlot(slot);
    }
    public void RefreshUI(ItemInstance[] inventorySlots)
    {
        if (_listSlot == null || _listSlot.Length == 0) return;
        if (_listSlot.Length != inventorySlots.Length) return;

        for (int i = 0; i < _listSlot.Length; i++)
        {
            _listSlot[i].UpdateSlot(inventorySlots[i]);
        }
    }
    public void ProcessDragAndDrop(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return;

        ItemInstance fromSlotItem = PlayerInventory.PlayerInventory[fromIndex];
        ItemInstance toSlotItem = PlayerInventory.PlayerInventory[toIndex];

        if (fromSlotItem.ItemDefinition == null || fromSlotItem.Amount == 0) return;

        if (toSlotItem.ItemDefinition == null)
        {
            toSlotItem = fromSlotItem;
            fromSlotItem = new ItemInstance();
        }
        else if (fromSlotItem.ItemDefinition == toSlotItem.ItemDefinition)
        {
            int totalAmount = fromSlotItem.Amount + toSlotItem.Amount;
            
            if (totalAmount > toSlotItem.ItemDefinition.MaxStack)
            {
                if (toSlotItem.Amount == toSlotItem.ItemDefinition.MaxStack)
                {
                    toSlotItem.Amount = fromSlotItem.Amount;
                    fromSlotItem.Amount = fromSlotItem.ItemDefinition.MaxStack;
                }
                else
                {
                    toSlotItem.Amount = toSlotItem.ItemDefinition.MaxStack;
                    fromSlotItem.Amount = totalAmount - fromSlotItem.ItemDefinition.MaxStack;
                }
            }
            else
            {
                toSlotItem.Amount = totalAmount;
                fromSlotItem = new ItemInstance();
            }
        }
        else
        {
            ItemInstance temp = toSlotItem;
            toSlotItem = fromSlotItem;
            fromSlotItem = temp;
        }
        PlayerInventory.ChangeItem(fromIndex, fromSlotItem);
        PlayerInventory.ChangeItem(toIndex, toSlotItem);
        UpdateSlot(fromIndex, fromSlotItem);
        UpdateSlot(toIndex, toSlotItem);
    }
}
