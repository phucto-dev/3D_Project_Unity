using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuantityDropRule
{
    public float Weight;
    public int MinAmount;
    public int MaxAmount;
}

[Serializable]
public class LootItem
{
    public ItemDefinitionSO Item;

    [Range(0f, 100f)] public float DropChance;

    [Header("--- QUANTITY RULE ---")]
    public List<QuantityDropRule> QuantityRules;

}

[Serializable]
public class ItemInstance
{
    public ItemDefinitionSO ItemDefinition;
    public int Amount;
    public ItemInstance()
    {

    }
    public ItemInstance(ItemDefinitionSO definition, int amount)
    {
        ItemDefinition = definition;
        Amount = amount;
    }
}
public abstract class ItemDefinitionSO : ScriptableObject
{
    [Header("--- ITEM INFORMATION ---")]
    public string ItemID;
    public string ItemName;
    [TextArea] public string Description;
    public Sprite ItemIcon;
    public GameObject DropPrefab;
    public int MaxStack = 99;
}

[CreateAssetMenu(fileName = "LootItemSO", menuName = "GameData/Items/Drop/LootItemSO")]
public class LootItemSO : ScriptableObject
{
    public List<LootItem> LootItems;

    public List<ItemInstance> GetRandomDrops()
    {
        List<ItemInstance> droppedItems = new List<ItemInstance>();

        foreach(LootItem lootItem in LootItems)
        {
            float rollHit = UnityEngine.Random.Range(0f, 100f);

            if (rollHit <= lootItem.DropChance)
            {
                int amountToDrop = CalWeightAmount(lootItem.QuantityRules);

                ItemInstance itemInstance = new ItemInstance(lootItem.Item, amountToDrop);

                droppedItems.Add(itemInstance);
            }
        }

        return droppedItems;
    }
    private int CalWeightAmount(List<QuantityDropRule> rules)
    {
        if (rules == null || rules.Count == 0) return 1;
        float totalWeight = 0f;
        foreach (QuantityDropRule rule in rules)
        {
            totalWeight += rule.Weight;
        }
        float randomRoll = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        foreach (QuantityDropRule rule in rules)
        {
            currentWeight += rule.Weight;
            if (randomRoll <= currentWeight)
            {
                return UnityEngine.Random.Range(rule.MinAmount, rule.MaxAmount + 1);
            }
        }
        return 1;
    }
}
