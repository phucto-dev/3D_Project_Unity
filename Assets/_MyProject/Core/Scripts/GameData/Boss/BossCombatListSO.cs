using System;
using System.Collections.Generic;
using UnityEngine;
public enum BossAttackType
{
    DashAndBite,
    CoreBeam,
    SummonStatues,
    GroundFireBreath,
    CarpetBombing
}
public enum BossPhase
{
    FirstPhase = 1,
    SecondPhase = 2
}
[Serializable]
public struct BossCombatInfo
{
    [Header("--- BASE SETUP ---")]
    public BossAttackType AttackType;
    public BossPhase PhaseUse;
    public float Weight;
    public float Cooldown;
    [Range(0, 100)] public float StaminaConsume;
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
