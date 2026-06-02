using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
}
