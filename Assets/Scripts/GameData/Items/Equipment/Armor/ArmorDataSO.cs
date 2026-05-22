using UnityEngine;

[CreateAssetMenu(fileName = "ArmorData", menuName = "GameData/Items/Equipment/ArmorData")]
public class ArmorDataSO : EquipmentDataSO
{
    [Header("--- MODULAR VISUALS ---")]
    public Mesh ArmorMesh;
    public Material ArmorMaterial;
}
