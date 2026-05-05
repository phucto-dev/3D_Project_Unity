using System;
using UnityEngine;

public class EntityStatsManager : MonoBehaviour
{
    [Header("--- STATS SOURCE ---")]
    [SerializeField] protected BaseStatsSO _baseStatsSO;

    [Header("--- ENTITY STATS ---")]
    public Stat MaxHealth;
    public Stat RunSpeed;
    public Stat WalkSpeed;
    public Stat AttackPower;
    public Stat AttackRange;
    public Stat DelayPerAttack;
    public Stat Haste;

    protected virtual void Awake()
    {
        if (_baseStatsSO == null) return;
        MaxHealth = new Stat(_baseStatsSO.MaxHealth.BaseValue);
        RunSpeed = new Stat(_baseStatsSO.RunSpeed.BaseValue);
        WalkSpeed = new Stat(_baseStatsSO.WalkSpeed.BaseValue);
        AttackPower = new Stat(_baseStatsSO.AttackPower.BaseValue);
        AttackRange = new Stat(_baseStatsSO.AttackRange.BaseValue);
        DelayPerAttack = new Stat(_baseStatsSO.DelayPerAttack.BaseValue);
        Haste = new Stat(_baseStatsSO.Haste.BaseValue);
    }

}
