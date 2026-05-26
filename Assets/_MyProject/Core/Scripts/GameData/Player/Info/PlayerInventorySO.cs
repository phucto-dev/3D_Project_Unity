using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventorySO", menuName = "GameData/Player/Inventory")]
public class PlayerInventorySO : ScriptableObject
{
    public int ItemSlotQuantity = 35;

    [field: SerializeField]
    public ItemInstance[] PlayerInventory  { get; private set; }

    public List<ItemDefinitionSO> startingItems;

    public event Action OnInventoryChanged;
    public event Action<int, ItemInstance> OnSlotUpdated;
    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        PlayerInventory = new ItemInstance[ItemSlotQuantity];
        GenerateStartingItems();
    }

    private void GenerateStartingItems()
    {
        foreach (var def in startingItems)
        {
            if (def == null) continue;
            ItemInstance newInstance;

            if (def is EquipmentDataSO equipDef)
            {
                newInstance = new EquipmentInstance(equipDef, 1, null, EquipmentRarity.Common);
            }
            else
            {
                newInstance = new ItemInstance(def, 1, null);
            }

            AddItem(newInstance);
        }
    }

    public int AddItem(ItemInstance item)
    {
        int maxStack = item.ItemDefinition.MaxStack;
        int j = 0;
        foreach (ItemInstance itemSlot in PlayerInventory)
        {
            if (itemSlot != null && itemSlot.ItemDefinition == item.ItemDefinition)
            {
                if (itemSlot.Amount < maxStack)
                {
                    int spaceLeft = maxStack - itemSlot.Amount;

                    if (item.Amount <= spaceLeft)
                    {
                        itemSlot.Amount += item.Amount;
                        item.Amount = 0;
                        OnSlotUpdated?.Invoke(j, itemSlot);
                        return 0;
                    }
                    else
                    {
                        itemSlot.Amount = maxStack;
                        item.Amount -= spaceLeft;
                        OnSlotUpdated?.Invoke(j, itemSlot);
                    }
                }
            }
            j++;
        }

        if (item.Amount > 0)
        {
            for (int i = 0; i < PlayerInventory.Length; i++)
            {
                if (PlayerInventory[i] == null || PlayerInventory[i].ItemDefinition == null)
                {
                    if (item.Amount <= maxStack)
                    {
                        PlayerInventory[i] = item;
                        OnSlotUpdated?.Invoke(i, PlayerInventory[i]);
                        return 0;
                    }
                    else
                    {
                        PlayerInventory[i].ItemDefinition = item.ItemDefinition;
                        PlayerInventory[i].Amount = maxStack;
                        OnSlotUpdated?.Invoke(i, PlayerInventory[i]);
                        item.Amount = item.Amount - maxStack;
                        int leftover = AddItem(item);
                        return leftover;
                    }
                }
            }
            //Debug.Log("int" + item.Amount);
            return item.Amount;
        }

        return 0;
    }

    public void ChangeItem(int index, ItemInstance item)
    {
        if (index >= PlayerInventory.Length) return;
        PlayerInventory[index] = item;
    }
}
