using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventorySO", menuName = "GameData/Player/Inventory")]
public class PlayerInventorySO : ScriptableObject
{
    public int ItemSlotQuantity = 35;
    [field: SerializeField] public ItemInstance[] PlayerInventory  { get; private set; }

    public event Action OnInventoryChanged;
    public event Action<int, ItemInstance> OnSlotUpdated;
    private void Awake()
    {
        PlayerInventory = new ItemInstance[ItemSlotQuantity];
    }

    // return number of leftover items. 0 means all items were fill into inventory and other number is the leftover.
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
                //Debug.Log("Inv" + PlayerInventory[i]);
                if (PlayerInventory[i] == null) continue;
                if (PlayerInventory[i].ItemDefinition == null)
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
