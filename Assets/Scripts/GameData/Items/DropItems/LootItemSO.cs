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
    public ItemInfo Item;

    [Range(0f, 100f)] public float DropChance;

    [Header("--- QUANTITY RULE ---")]
    public List<QuantityDropRule> QuantityRules;

}
[Serializable]
public struct ItemInfo
{
    public string ItemName;
    public string ItemID;
    public SpriteRenderer ItemImg;
    public GameObject ItemPrefab;
    public GameObject ItemOrbDrop;
    public int Amount;
}

[CreateAssetMenu(fileName = "LootItemSO", menuName = "GameData/Items/Drop/LootItemSO")]
public class LootItemSO : ScriptableObject
{
    public List<LootItem> LootItems;

    public List<ItemInfo> GetRandomDrops()
    {
        List<ItemInfo> droppedItems = new List<ItemInfo>();

        foreach(LootItem lootItem in LootItems)
        {
            float rollHit = UnityEngine.Random.Range(0f, 100f);

            if (rollHit <= lootItem.DropChance)
            {
                int amountToDrop = CalWeightAmount(lootItem.QuantityRules);

                ItemInfo payload = new ItemInfo
                {
                    ItemName = lootItem.Item.ItemName,
                    ItemID = lootItem.Item.ItemName,
                    ItemImg = lootItem.Item.ItemImg,
                    ItemPrefab = lootItem.Item.ItemPrefab,
                    ItemOrbDrop = lootItem.Item.ItemOrbDrop,
                    Amount = amountToDrop,
                };

                droppedItems.Add(payload);
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
