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
    public Stat Poise;
    public Stat PoiseDamage;
    public bool SuperArmor;

    private float _currentPoise;

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
        Poise = new Stat(_baseStatsSO.Poise.BaseValue);
        PoiseDamage = new Stat(_baseStatsSO.PoiseDamage.BaseValue);
        SuperArmor = _baseStatsSO.IsAffectbyPoise;
        _currentPoise = Poise.GetValue();
    }

    public bool IsRunOutPoise(float poiseDmg)
    {
        if (SuperArmor) return false;
        _currentPoise -= poiseDmg;
        Debug.Log("_currentPoise: " + _currentPoise + " " + poiseDmg);
        return _currentPoise <= 0;
    }
    public void RecoverPoise()
    {
        _currentPoise = Poise.GetValue();
    }
    public void SetSuperArmor(bool value)
    {
        SuperArmor = value;
    }
}
