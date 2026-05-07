using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "GameData/Items/Weapon/WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    [Header("--- INFORMATION ---")]
    public string WeaponName = "New Weapon";
    [TextArea(2, 4)] public string Description;
    public GameObject EquippedPrefab;
    public GameObject DropPrefab;
    public AttackNodeSO EntryLightAttack;
    public AttackNodeSO EntryHeavyAttack;

    [Header("--- COMBAT ---")]
    public float BaseDamage = 20f;
    public float AttackRange = 1.5f;
    public float PoiseDamage = 5f;

    [Header("--- ANIMATION ---")]
    public AnimatorOverrideController OverrideController;

    [Header("--- VFX ---")]
    public GameObject HitVFXPrefab;
    public GameObject WeaponTrail;
}
