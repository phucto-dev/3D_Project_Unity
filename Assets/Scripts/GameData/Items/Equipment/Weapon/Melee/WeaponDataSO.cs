using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "GameData/Items/Equipment/WeaponData")]
public class WeaponDataSO : EquipmentDataSO
{
    [Header("--- WEAPON INFORMATION ---")]
    public GameObject EquippedPrefab;
    public AttackNodeSO EntryLightAttack;
    public AttackNodeSO EntryHeavyAttack;

    [Header("--- COMBAT ---")]
    public float AttackRange = 1.5f;
    public float MagicScale = 0;
    public float PoiseDamage = 5f;
    public float HitStopDuration = 0.08f;

    [Header("--- ANIMATION ---")]
    public AnimatorOverrideController OverrideController;

    [Header("--- VFX ---")]
    public GameObject HitVFXPrefab;
    public GameObject WeaponTrail;
}
