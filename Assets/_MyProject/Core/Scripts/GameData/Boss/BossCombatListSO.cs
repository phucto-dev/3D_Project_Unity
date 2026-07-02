using System;
using System.Collections.Generic;
using UnityEngine;
public enum BossAttackType
{
    DashAndBite,
    CoreBeam,
    GroundFireBreath,
    CarpetBombing
}
[Serializable]
public struct BossCombatInfo
{
    [Header("--- BASE SETUP ---")]
    public BossAttackType AttackType;
    public float Weight;
    public float Cooldown;
    public float MinDistance;
    public float MaxDistance;
    public string VFXID;

    [Header("--- AIR SETUP ---")]
    public float FlyHeight;

    [Header("--- DMG SCALE ---")]
    public float DmgHitMultiple;
    [Tooltip("-1 is Instance. Otherwise, tick hit")]
    public float TimeHitPerNumberSecond;
}
[CreateAssetMenu(fileName = "NewBossCombat", menuName = "GameData/Boss/Data/CombatStates")]
public class BossCombatListSO : ScriptableObject
{
    public List<BossCombatInfo> BossCombatStates = new List<BossCombatInfo>();
}
