using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerEquipment", menuName = "GameData/Player/Equipment")]
public class PlayerEquipmentSO : ScriptableObject
{
    public Dictionary<EquipmentSlot, EquipmentInstance> EquippedItems = new Dictionary<EquipmentSlot, EquipmentInstance>();
    public event Action<EquipmentSlot, EquipmentInstance> OnEquipmentChanged;

    public void ClearAllEquippedItem()
    {
        foreach (var slot in new List<EquipmentSlot>(EquippedItems.Keys))
        {
            UnequipItem(slot);
        }
    }
    public void EquipItem(EquipmentInstance newItem)
    {
        EquipmentSlot slot = newItem.GetEquipData().SlotType;
        EquipmentInstance oldItem = null;

        if (EquippedItems.ContainsKey(slot))
        {
            oldItem = EquippedItems[slot];
        }

        EquippedItems[slot] = newItem;
        OnEquipmentChanged?.Invoke(slot, newItem);
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        if (EquippedItems.ContainsKey(slot))
        {
            EquippedItems.Remove(slot);
            OnEquipmentChanged?.Invoke(slot, null);
        }
    }
}
