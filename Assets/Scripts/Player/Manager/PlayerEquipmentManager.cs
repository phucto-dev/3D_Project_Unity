using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    public PlayerEquipmentSO PlayerEquipment;
    public PlayerStatsManager PlayerStats;

    [Header("--- ARMOR REF ---")]
    public SkinnedMeshRenderer HeadSkinnedMesh;
    public SkinnedMeshRenderer ChestSkinnedMesh;
    public SkinnedMeshRenderer ArmsSkinnedMesh;
    public SkinnedMeshRenderer BeltSkinnedMesh;
    public SkinnedMeshRenderer LegsSkinnedMesh;
    public SkinnedMeshRenderer FeetSkinnedMesh;

    private void OnEnable()
    {
        if (PlayerEquipment != null) PlayerEquipment.OnEquipmentChanged += HandleEquipmentChanged;
    }
    private void OnDisable()
    {
        if (PlayerEquipment != null) PlayerEquipment.OnEquipmentChanged -= HandleEquipmentChanged;
    }

    private void HandleEquipmentChanged(EquipmentSlot slot, EquipmentInstance newEquipment)
    {
        Mesh mesh = null;
        Material material = null;
        if (newEquipment != null)
        {
            EquipmentDataSO equipData = newEquipment.GetEquipData();
            mesh = equipData.EquipmentMesh;
            material = equipData.EquipmentMaterial;
        }

        switch (slot)
        {
            case EquipmentSlot.Head:
                HeadSkinnedMesh.sharedMesh = mesh;
                HeadSkinnedMesh.sharedMaterial = material;
                break;
            case EquipmentSlot.Chest:
                ChestSkinnedMesh.sharedMesh = mesh;
                ChestSkinnedMesh.sharedMaterial = material;
                break;
            case EquipmentSlot.Arms:
                ArmsSkinnedMesh.sharedMesh = mesh;
                ArmsSkinnedMesh.sharedMaterial = material;
                break;
            case EquipmentSlot.Belt:
                BeltSkinnedMesh.sharedMesh = mesh;
                BeltSkinnedMesh.sharedMaterial = material;
                break;
            case EquipmentSlot.Legs:
                LegsSkinnedMesh.sharedMesh = mesh;
                LegsSkinnedMesh.sharedMaterial = material;
                break;
            case EquipmentSlot.Feet:
                FeetSkinnedMesh.sharedMesh = mesh;
                FeetSkinnedMesh.sharedMaterial = material;
                break;
        }

        UpdatePlayerStats();
    }

    private void UpdatePlayerStats()
    {

    }
}
