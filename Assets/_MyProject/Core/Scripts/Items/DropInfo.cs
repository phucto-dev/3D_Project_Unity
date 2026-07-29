using System;
using UnityEngine;

public class DropInfo : MonoBehaviour, IInteractable
{
    [field: SerializeField] public int InteractionPriority { get; private set; }
    public InteractType GetInteractType() => InteractType.Loot;
    public string GetInteractText() => "Loot";
    public ItemInstance ItemData { get; private set; }
    public void Interact()
    {

    }
    public void Initialize(ItemInstance itemInstance)
    {
        ItemData = BindingItem(itemInstance);
    }
    private ItemInstance BindingItem(ItemInstance item)
    {
        ItemInstance result;
        if (item is EquipmentInstance equipItem)
        {
            Debug.Log("La item");
            result = new EquipmentInstance(equipItem.GetEquipData(), equipItem.Amount, equipItem.DropPrefab, equipItem.Rarity, equipItem.BonusStats, equipItem.UpgradeLevel, equipItem.RandomAffixes);
        }
        else
        {
            result = new ItemInstance(item.ItemDefinition, item.Amount, item.DropPrefab);
        }
        return result;
    }

    public void SetAmount(int amount)
    {
        if (ItemData == null) return;
        ItemData.Amount = amount;
    }

    public void RemovePrefab()
    {
        gameObject.SetActive(false);
    }
}
