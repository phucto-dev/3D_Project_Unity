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
public class RarityDropRule
{
    public float Weight;
    public EquipmentRarity Rarity;
}

[Serializable]
public class LootItem
{
    public ItemDefinitionSO Item;

    [Range(0f, 100f)] public float DropChance;

    [Header("--- QUANTITY RULE ---")]
    public List<QuantityDropRule> QuantityRules;
    public List<RarityDropRule> RarityRules;
}

[Serializable]
public class ItemInstance
{
    public ItemDefinitionSO ItemDefinition;
    public int Amount;
    public GameObject DropPrefab;
    public ItemInstance()
    {

    }
    public ItemInstance(ItemDefinitionSO definition, int amount, GameObject dropPrefab)
    {
        ItemDefinition = definition;
        Amount = amount;
        DropPrefab = dropPrefab;
    }
}
public abstract class ItemDefinitionSO : ScriptableObject
{
    [Header("--- ITEM INFORMATION ---")]
    public string ItemID;
    public string ItemName;
    [TextArea] public string Description;
    public Sprite ItemIcon;
    public int MaxStack = 99;
}

[CreateAssetMenu(fileName = "LootItemSO", menuName = "GameData/Items/Drop/LootItemSO")]
public class LootItemSO : ScriptableObject
{
    public List<LootItem> LootItems;
    public OrbDropSO OrbDrop;

    public List<ItemInstance> GetRandomDrops()
    {
        List<ItemInstance> droppedItems = new List<ItemInstance>();

        foreach(LootItem lootItem in LootItems)
        {
            float rollHit = UnityEngine.Random.Range(0f, 100f);

            if (rollHit <= lootItem.DropChance)
            {
                int amountToDrop = CalWeightAmount(lootItem.QuantityRules);
                EquipmentRarity rarity = RandomRarityDrop(lootItem.RarityRules, lootItem);
                GameObject orbVisual = null;
                foreach (RarityOrbMapping orb in OrbDrop.OrbDrop)
                {
                    if (orb.Rarity == rarity)
                    {
                        orbVisual = orb.OrbPrefab;
                        break;
                    }
                }
                ItemInstance itemInstance = new ItemInstance(lootItem.Item, amountToDrop, orbVisual);

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

    private EquipmentRarity RandomRarityDrop(List<RarityDropRule> rules, LootItem lootItem)
    {
        if (!(lootItem.Item is EquipmentDataSO)) return EquipmentRarity.Common;
        if (rules == null || rules.Count == 0) return EquipmentRarity.Common;
        float totalWeight = 0f;
        foreach (RarityDropRule rule in rules)
        {
            totalWeight += rule.Weight;
        }
        float randomRoll = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        foreach (RarityDropRule rule in rules)
        {
            currentWeight += rule.Weight;
            if (randomRoll <= currentWeight)
            {
                return rule.Rarity;
            }
        }
        return EquipmentRarity.Common;
    }
}
