using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public PlayerInfo Player;

    public PlayerInventorySO PlayerInventory;
    public PlayerEquipmentSO PlayerEquipment;

    public GameObject SlotContainerInventory;
    public GameObject SlotContainerEquipment;

    public int CurrentSelectedIndex { get; private set; }
    public ItemInstance CurrentSelectedItem { get; private set; }

    public event Action<ItemInstance> OnSlotDataChanged;

    private InventorySlot[] _listSlot;
    private EquipmentSlotUI[] _listEquipmentSlot;

    private void Awake()
    {
        if (SlotContainerInventory == null) return;
        if (SlotContainerEquipment == null) return;
        _listSlot = SlotContainerInventory.GetComponentsInChildren<InventorySlot>(true);
        _listEquipmentSlot = SlotContainerEquipment.GetComponentsInChildren<EquipmentSlotUI>(true);
    }
    private void OnEnable()
    {
        if (PlayerInventory == null) return;
        PlayerInventory.OnSlotUpdated += UpdateSlot;
        if (_listSlot == null || _listSlot.Count() == 0) return;
        foreach (InventorySlot _slot in _listSlot)
        {
            _slot.SelectedItem += GetCurrentSelectedItem;
        }
        RefreshUI(PlayerInventory.PlayerInventory);
    }
    private void OnDisable()
    {
        if (PlayerInventory == null) return;
        PlayerInventory.OnSlotUpdated -= UpdateSlot;
        if (_listSlot == null || _listSlot.Count() == 0) return;
        foreach (InventorySlot _slot in _listSlot)
        {
            _slot.SelectedItem -= GetCurrentSelectedItem;
        }
    }
    private void Start()
    {
        CurrentSelectedIndex = -1;
        CurrentSelectedItem = null;
        if (PlayerInventory == null) return;
    }
    public void UpdateSlot(int i, ItemInstance slot)
    {
        if (_listSlot == null || _listSlot.Length == 0) return;
        _listSlot[i].UpdateSlot(slot);
    }

    public void UpdateEquipmentSlot(int i, EquipmentInstance slot)
    {
        if (_listEquipmentSlot == null || _listEquipmentSlot.Length == 0) return;
        _listEquipmentSlot[i].UpdateSlot(slot);
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
        if (fromSlotItem == null)
        {
            Debug.LogWarning("from item == null: " + fromSlotItem);
            return;
        }
        if (toSlotItem == null)
        {            
            SwapItems(fromIndex, toIndex);
            return;
        }
        if (fromSlotItem.ItemDefinition == null || fromSlotItem.Amount == 0) return;
        if (toSlotItem.ItemDefinition == null)
        {
            toSlotItem = fromSlotItem;
            fromSlotItem = new ItemInstance();
        }
        else if (fromSlotItem.ItemDefinition == toSlotItem.ItemDefinition)
        {
            if (fromSlotItem is EquipmentInstance || fromSlotItem.ItemDefinition.MaxStack <= 1)
            {
                ItemInstance temp = toSlotItem;
                toSlotItem = fromSlotItem;
                fromSlotItem = temp;
            }
            else
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

    public void ProcessEquipmentChange(EquipmentSlot equipmentSlotType, bool fromInventoryStart, int inventoryIndex, int equipmentIndex)
    {
        if (fromInventoryStart)
        {
            ItemInstance invItem = PlayerInventory.BindingItem(PlayerInventory.PlayerInventory[inventoryIndex]);

            if (invItem == null || invItem.ItemDefinition == null || !(invItem is EquipmentInstance pendingEquip)) return;
            if (pendingEquip.GetEquipData().SlotType != equipmentSlotType) return;

            // Objective Event
            if (pendingEquip.GetEquipData().SlotType == EquipmentSlot.Weapon_RightHand)
            {
                GameEventManager.TriggerObjectiveAction(ObjectiveEventType.Equip, true, "EquipSword", 1);
            }

            EquipmentInstance currentlyEquipped = null;
            if (PlayerEquipment.EquippedItems.ContainsKey(equipmentSlotType))
            {
                currentlyEquipped = PlayerEquipment.EquippedItems[equipmentSlotType];
            }

            PlayerEquipment.EquipItem(pendingEquip);

            if (currentlyEquipped != null)
            {
                PlayerInventory.ChangeItem(inventoryIndex, currentlyEquipped);
            }
            else
            {
                PlayerInventory.ChangeItem(inventoryIndex, new ItemInstance());
            }

            UpdateSlot(inventoryIndex, PlayerInventory.PlayerInventory[inventoryIndex]);
            UpdateEquipmentSlot(equipmentIndex, pendingEquip);
        }
        else
        {
            if (!PlayerEquipment.EquippedItems.ContainsKey(equipmentSlotType)) return;

            EquipmentInstance equipmentItem = PlayerEquipment.EquippedItems[equipmentSlotType];
            ItemInstance targetInvSlot = PlayerInventory.PlayerInventory[inventoryIndex];
            if (targetInvSlot == null)
            {
                PlayerInventory.ChangeItem(inventoryIndex, equipmentItem);
                PlayerEquipment.UnequipItem(equipmentSlotType);
            }
            else if (targetInvSlot.ItemDefinition != null)
            {
                if (!(targetInvSlot is EquipmentInstance targetEquip) || targetEquip.GetEquipData().SlotType != equipmentSlotType)
                {
                    return;
                }

                // Swap
                PlayerInventory.ChangeItem(inventoryIndex, equipmentItem);
                PlayerEquipment.EquipItem(targetEquip);
            }
            else
            {
                // Unequip
                PlayerInventory.ChangeItem(inventoryIndex, equipmentItem);
                PlayerEquipment.UnequipItem(equipmentSlotType);
            }

            UpdateSlot(inventoryIndex, PlayerInventory.PlayerInventory[inventoryIndex]);
            UpdateEquipmentSlot(equipmentIndex, new EquipmentInstance());
        }
    }
    
    public void GetCurrentSelectedItem(int slotIndex)
    {
        Debug.Log("Setne");
        if (CurrentSelectedIndex != slotIndex)
        {
            Debug.Log("Setne1");
            CurrentSelectedItem = PlayerInventory.PlayerInventory[slotIndex];
            CurrentSelectedIndex = slotIndex;
            return;
        }
        CurrentSelectedIndex = -1;
        CurrentSelectedItem = null;
    }
    public void ClearSelectedItem()
    {
        CurrentSelectedIndex = -1;
        CurrentSelectedItem = null;
    }

    public void SwapItems(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= PlayerInventory.PlayerInventory.Length ||
            toIndex < 0 || toIndex >= PlayerInventory.PlayerInventory.Length)
            return;

        if (fromIndex == toIndex) return;

        ItemInstance temp = PlayerInventory.PlayerInventory[toIndex];
        PlayerInventory.ChangeItem(toIndex, PlayerInventory.PlayerInventory[fromIndex]);
        PlayerInventory.ChangeItem(fromIndex, temp);
        UpdateSlot(fromIndex, PlayerInventory.PlayerInventory[fromIndex]);
        UpdateSlot(toIndex, PlayerInventory.PlayerInventory[toIndex]);
    }
}
