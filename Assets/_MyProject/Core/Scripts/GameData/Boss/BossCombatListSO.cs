using System;
using System.Collections.Generic;
using UnityEngine;
public enum BossAttackType
{
    DashAndBite,
    GroundFireBreath,
    CarpetBombing
}
[Serializable]
public struct BossCombatInfo
{
    public BossAttackType AttackType;
    public float Weight;
    public float Cooldown;
    public float MinDistance;
    public float MaxDistance;
}
[CreateAssetMenu(fileName = "NewBossCombat", menuName = "GameData/Boss/Data/CombatStates")]
public class BossCombatListSO : ScriptableObject
{
    public List<BossCombatInfo> BossCombatStates = new List<BossCombatInfo>();
}
