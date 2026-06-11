using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEngine.ParticleSystem;

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

    [Header("--- WEAPON REF ---")]
    public GameObject WeaponSlot;

    [Header("--- BASE ANIMATOR REF ---")]
    public RuntimeAnimatorController AnimController;

    private PlayerStatsManager _stats;
    private PlayerAttack _playerAttack;
    private PlayerManager _playerManager;
    private Animator _animator;

    private void Awake()
    {
        _stats = GetComponentInParent<PlayerStatsManager>();
        _playerAttack = GetComponentInParent<PlayerAttack>();
        _playerManager = GetComponentInParent<PlayerManager>();
        if (_playerAttack != null)
        {
            _animator = _playerAttack.GetComponentInChildren<Animator>();
        }
    }

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
        GameObject weapon = null;
        EquipmentDataSO equipData = null;
        WeaponDataSO weaponData = null;
        if (newEquipment != null)
        {
            equipData = newEquipment.GetEquipData();
            if (slot == EquipmentSlot.Weapon_BothHand || slot == EquipmentSlot.Weapon_LeftHand || slot == EquipmentSlot.Weapon_RightHand)
            {
                if (equipData is WeaponDataSO weaponEquip )
                {
                    weaponData = weaponEquip;
                    weapon = weaponEquip.EquippedPrefab;
                }
            }
            else
            {
                mesh = equipData.EquipmentMesh;
                material = equipData.EquipmentMaterial;
            }
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
            case EquipmentSlot.Weapon_RightHand:
                if (WeaponSlot == null) break;
                for (int i = WeaponSlot.transform.childCount - 1; i >= 0; i--)
                {
                    Transform child = WeaponSlot.transform.GetChild(i);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }

                _animator.runtimeAnimatorController = AnimController;
                _playerAttack.SetEntryAttack(null, null);
                if (weapon != null)
                {
                    GameObject weaponInstance = Instantiate(weapon);
                    weaponInstance.transform.SetParent(WeaponSlot.transform, false);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;

                    MeleeTracer tracer = weaponInstance.GetComponentInChildren<MeleeTracer>();
                    if (tracer != null)
                    {
                        tracer.Initialize(_stats, _playerAttack, _playerManager, _animator);

                        if (weaponData == null) break;

                        _animator.runtimeAnimatorController = weaponData.OverrideController;
                        _playerAttack.SetEntryAttack(weaponData.EntryLightAttack, weaponData.EntryHeavyAttack);
                    }
                }
                break;
        }

        UpdatePlayerStats();
    }

    private void UpdatePlayerStats()
    {

    }
}
