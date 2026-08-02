using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public static class RarityMultiplierHelper
{
    public static float GetMultiplier(this EquipmentRarity rarity)
    {
        return rarity switch
        {
            EquipmentRarity.Common => 1.0f,
            EquipmentRarity.Rare => 1.25f,
            EquipmentRarity.Unique => 1.6f,
            EquipmentRarity.Legendary => 2.1f,
            EquipmentRarity.Mythic => 3.0f,
            _ => 1.0f
        };
    }
}
public enum EquipmentSlot
{
    Head, Chest, Arms, Legs, Feet, Belt,
    Weapon_RightHand, Weapon_LeftHand, Weapon_BothHand
}
public enum EquipmentRarity
{
    Common, Rare, Unique, Legendary, Mythic
}

[System.Serializable]
public enum StatType
{
    BaseHP,
    HPPercent,
    BaseDef,
    DefPercent,
    BaseATK,
    ATKPercent,
    Haste,
    CritRate,
    CritDamage,
    Poise,
    Stamina,
    Mana
}

[System.Serializable]
public struct ItemStat
{
    public StatType Type;
    public float Value;
}

public abstract class EquipmentDataSO : ItemDefinitionSO
{
    [Header("--- EQUIPMENT SETUP ---")]
    public EquipmentSlot SlotType;
    [Range(1, 5)]
    public int Tier = 1;

    [Header("--- UPGRADE SETUP ---")]
    private int MaxUpgrade;

    [Header("--- STATS ---")]
    public ItemStat MainStat;
    public ItemStat SubStat;

    [Header("--- MODULAR VISUALS ---")]
    [FormerlySerializedAs("ArmorMesh")]
    public Mesh EquipmentMesh;
    [FormerlySerializedAs("ArmorMaterial")]
    public Material EquipmentMaterial;

    private void OnEnable()
    {
        MaxStack = 1;
        MaxUpgrade = 4;
    }
}
public class EquipmentInstance: ItemInstance
{
    public int UpgradeLevel;
    public List<ItemStat> RandomAffixes;
    public EquipmentRarity Rarity;
    public List<ItemStat> BonusStats;

    public EquipmentInstance() : base()
    {

    }
    public EquipmentInstance(
    EquipmentDataSO definition,
    int amount = 1,
    GameObject dropPrefab = null,
    EquipmentRarity rarity = EquipmentRarity.Common,
    List<ItemStat> bonusStats = null,
    int upgradeLevel = 0,
    List<ItemStat> randomAffixes = null) : base(definition, amount, dropPrefab)
    {
        UpgradeLevel = upgradeLevel;
        RandomAffixes = randomAffixes ?? new List<ItemStat>();
        Rarity = rarity;
        BonusStats = bonusStats ?? null;
    }
    public EquipmentDataSO GetEquipData()
    {
        return ItemDefinition as EquipmentDataSO;
    }
    public ItemStat FinalMainStat
    {
        get
        {
            EquipmentDataSO data = GetEquipData();
            if (data == null) return new ItemStat();
            return CalculateFinalStat(data.MainStat);
        }
    }

    public ItemStat FinalSubStat
    {
        get
        {
            EquipmentDataSO data = GetEquipData();
            if (data == null) return new ItemStat();
            return CalculateFinalStat(data.SubStat);
        }
    }
    private ItemStat CalculateFinalStat(ItemStat baseStat)
    {
        float calculatedValue = baseStat.Value * Rarity.GetMultiplier();
        float upgradeMultiplier = 1f + (UpgradeLevel * 0.1f);
        calculatedValue *= upgradeMultiplier;

        if (BonusStats != null)
        {
            foreach (var bonus in BonusStats)
            {
                if (bonus.Type == baseStat.Type)
                {
                    calculatedValue += bonus.Value;
                }
            }
        }

        return new ItemStat
        {
            Type = baseStat.Type,
            Value = calculatedValue
        };
    }
}
