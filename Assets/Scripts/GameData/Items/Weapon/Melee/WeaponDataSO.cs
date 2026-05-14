using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "GameData/Items/Weapon/WeaponData")]
public class WeaponDataSO : ItemDefinitionSO
{
    [Header("--- WEAPON INFORMATION ---")]
    public GameObject EquippedPrefab;
    public AttackNodeSO EntryLightAttack;
    public AttackNodeSO EntryHeavyAttack;

    [Header("--- COMBAT ---")]
    public float BaseDamage = 20f;
    public float AttackRange = 1.5f;
    public float PoiseDamage = 5f;
    public float HitStopDuration = 0.08f;

    [Header("--- ANIMATION ---")]
    public AnimatorOverrideController OverrideController;

    [Header("--- VFX ---")]
    public GameObject HitVFXPrefab;
    public GameObject WeaponTrail;
}
