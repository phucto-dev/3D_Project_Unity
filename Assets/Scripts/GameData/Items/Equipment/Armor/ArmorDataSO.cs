using UnityEngine;

public enum ArmorSlotType
{
    Head,
    Chest,
    Arms,
    Belt,
    Legs,
    Feet,
}
[CreateAssetMenu(fileName = "ArmorData", menuName = "GameData/Items/Equipment/ArmorData")]
public class ArmorDataSO : ItemDefinitionSO
{
    [Header("--- EQUIP SETUP ---")]
    public ArmorSlotType SlotType;

    [Header("--- MODULAR VISUALS ---")]
    public Mesh ArmorMesh;
    public Material ArmorMaterial;

    [Header("--- ARMOR STATS ---")]
    public int Defense;
    public int HealthBonus;
    public int MoveSpeedPenalty;

    private void OnEnable()
    {
        MaxStack = 1;
    }
}
