using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryManager : MonoBehaviour
{
    public int ItemSlotQuantity = 35;
    public ItemInstance[] PlayerInventory { get; private set; }

    public event Action OnInventoryChanged;

    private void Start()
    {
        PlayerInventory = new ItemInstance[ItemSlotQuantity];
    }

    // return number of leftover items. 0 means all items were fill into inventory and other number is the leftover.
    public int AddItem(ItemInstance item)
    {
        int maxStack = item.ItemDefinition.MaxStack;
        foreach (ItemInstance itemSlot in PlayerInventory)
        {
            if (itemSlot != null && itemSlot.ItemDefinition == item.ItemDefinition )
            {
                if (itemSlot.Amount < maxStack)
                {
                    int spaceLeft = maxStack - itemSlot.Amount;

                    if (item.Amount <= spaceLeft)
                    {
                        itemSlot.Amount += item.Amount;
                        item.Amount = 0;
                        OnInventoryChanged?.Invoke();
                        return 0;
                    }
                    else
                    {
                        itemSlot.Amount = maxStack;
                        item.Amount -= spaceLeft;
                        OnInventoryChanged?.Invoke();
                    }
                }
            }
        }

        if (item.Amount > 0)
        {
            for (int i = 0; i < PlayerInventory.Length; i++)
            {
                if (PlayerInventory[i] == null)
                {
                    PlayerInventory[i] = item;
                    OnInventoryChanged?.Invoke();
                    return 0;
                }
            }
            return item.Amount;
        }

        return 0;
    }
}
