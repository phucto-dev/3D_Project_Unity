using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatsValue
{
    HP,
    Defense,
    Stamina,
    Mana,
    Attack,
    CR,
    CD,
    Haste
}
public class EntityStatsManager : MonoBehaviour
{
    [Header("--- STATS SOURCE ---")]
    [SerializeField] protected BaseStatsSO _baseStatsSO;

    [Header("--- ENTITY STATS ---")]
    public Stat MaxHealth;
    public Stat RunSpeed;
    public Stat WalkSpeed;
    public Stat SprintSpeed;
    public Stat AttackPower;
    public Stat AttackRange;
    public Stat DelayPerAttack;
    public Stat Haste;
    public Stat Poise;
    public Stat PoiseDamage;
    public Stat Defense;
    public Stat CritRate;
    public Stat CritDamage;

    [Header("--- FINAL STATS (USE THESE FOR LOGIC) ---")]
    public float FinalMaxHealth { get; protected set; }
    public float FinalAttackPower { get; protected set; }
    public float FinalDefense { get; protected set; }
    public float FinalCritRate { get; protected set; }
    public float FinalCritDamage { get; protected set; }
    public float FinalHaste { get; protected set; }
    public float FinalPoise { get; protected set; }


    public bool SuperArmor;

    private float _currentPoise;
    protected Dictionary<StatsValue, float> _dictStats = new Dictionary<StatsValue, float>();

    protected virtual void Awake()
    {
        if (_baseStatsSO == null) return;

        Defense = new Stat(_baseStatsSO.Defense.BaseValue);
        CritRate = new Stat(_baseStatsSO.CritRate.BaseValue);
        CritDamage = new Stat(_baseStatsSO.CritDamage.BaseValue);
        MaxHealth = new Stat(_baseStatsSO.MaxHealth.BaseValue, () => RecalculateFinalStats(null));
        RunSpeed = new Stat(_baseStatsSO.RunSpeed.BaseValue);
        WalkSpeed = new Stat(_baseStatsSO.WalkSpeed.BaseValue);
        SprintSpeed = new Stat(_baseStatsSO.SprintSpeed.BaseValue);
        AttackPower = new Stat(_baseStatsSO.AttackPower.BaseValue, () => RecalculateFinalStats(null));
        AttackRange = new Stat(_baseStatsSO.AttackRange.BaseValue);
        DelayPerAttack = new Stat(_baseStatsSO.DelayPerAttack.BaseValue);
        Haste = new Stat(_baseStatsSO.Haste.BaseValue);
        Poise = new Stat(_baseStatsSO.Poise.BaseValue);
        PoiseDamage = new Stat(_baseStatsSO.PoiseDamage.BaseValue);
        SuperArmor = !_baseStatsSO.IsAffectbyPoise;
        _currentPoise = Poise.GetValue();

        RecalculateFinalStats(new Dictionary<StatType, float>());
    }

    public bool IsRunOutPoise(float poiseDmg)
    {
        if (SuperArmor)
        {
            Debug.Log("SupperArmor Active: " + _currentPoise + " " + poiseDmg);
            return false;
        }
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
    public BaseStatsSO GetStatsData()
    {
        return _baseStatsSO;
    }
    public virtual void RecalculateFinalStats(Dictionary<StatType, float> eqBonuses)
    {
        if (eqBonuses == null) eqBonuses = new Dictionary<StatType, float>();

        // Lấy Base Values
        float baseHp = MaxHealth.GetValue();
        float baseAtk = AttackPower.GetValue();
        float baseDef = Defense.GetValue();

        // Tính Final HP, ATK, DEF = Base + Flat Bonus + (Base * Percent Bonus / 100)
        FinalMaxHealth = baseHp
                       + eqBonuses.GetValueOrDefault(StatType.BaseHP, 0f)
                       + (baseHp * eqBonuses.GetValueOrDefault(StatType.HPPercent, 0f) / 100f);

        FinalAttackPower = baseAtk
                         + eqBonuses.GetValueOrDefault(StatType.BaseATK, 0f)
                         + (baseAtk * eqBonuses.GetValueOrDefault(StatType.ATKPercent, 0f) / 100f);

        FinalDefense = baseDef
                     + eqBonuses.GetValueOrDefault(StatType.BaseDef, 0f)
                     + (baseDef * eqBonuses.GetValueOrDefault(StatType.DefPercent, 0f) / 100f);

        // Tính Final Crit (Cộng dồn trực tiếp)
        FinalCritRate = CritRate.GetValue() + eqBonuses.GetValueOrDefault(StatType.CritRate, 0f);
        FinalCritDamage = CritDamage.GetValue() + eqBonuses.GetValueOrDefault(StatType.CritDamage, 0f);

        // Tính các chỉ số Flat khác
        FinalHaste = Haste.GetValue() + eqBonuses.GetValueOrDefault(StatType.Haste, 0f);
        FinalPoise = Poise.GetValue() + eqBonuses.GetValueOrDefault(StatType.Poise, 0f);

        _dictStats[StatsValue.HP] = FinalMaxHealth;
        _dictStats[StatsValue.Attack] = FinalAttackPower;
        _dictStats[StatsValue.Defense] = FinalDefense;
        _dictStats[StatsValue.CR] = FinalCritRate;
        _dictStats[StatsValue.CD] = FinalCritDamage;
        _dictStats[StatsValue.Haste] = FinalHaste;
    }
}
