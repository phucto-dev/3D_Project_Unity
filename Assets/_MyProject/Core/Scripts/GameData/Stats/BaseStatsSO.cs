using System;
using UnityEngine;

[System.Serializable]
public struct StatData
{
    [SerializeField] private float baseValue;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    public float BaseValue => Mathf.Clamp(baseValue, minValue, maxValue);
    public float MinValue => minValue;
    public float MaxValue => maxValue;

    public StatData(float baseVal, float min = 0f, float max = 9999f)
    {
        baseValue = baseVal;
        minValue = min;
        maxValue = max;
    }
}

[CreateAssetMenu(fileName = "BaseStats", menuName = "GameData/Stats/BaseStats")]
public class BaseStatsSO : ScriptableObject
{
    [Header("--- SURVIVAL ---")]
    [SerializeField]
    private StatData _maxHealth = new StatData(0f);
    public StatData MaxHealth => _maxHealth;

    [SerializeField, Tooltip("HP regen per second")]
    private StatData _healthRegen = new StatData(0f);
    public StatData HealthRegen => _healthRegen;

    [Header("--- OFFENSE ---")]
    [SerializeField]
    private StatData _attackPower = new StatData(10f);
    public StatData AttackPower => _attackPower;

    [SerializeField, Tooltip("Frequency Of Attack)")]
    private StatData _haste = new StatData(1f, 0.1f, 99f);
    public StatData Haste => _haste;

    [SerializeField, Tooltip("Measure by Second)")]
    private StatData _delayPerAttack = new StatData(1f, 0.1f, 99f);
    public StatData DelayPerAttack => _delayPerAttack;

    [SerializeField, Tooltip("Critical Rate (0.0 - 1.0)")]
    private StatData _critRate = new StatData(0.05f, 0f, 1f);
    public StatData CritRate => _critRate;

    [SerializeField, Tooltip("Critical Damage (1.5 = 150%)")]
    private StatData _critDamage = new StatData(1.5f, 1f, 5f);
    public StatData CritDamage => _critDamage;

    [SerializeField]
    private StatData _attackRange = new StatData(2.5f, 1f, 99f);
    public StatData AttackRange => _attackRange;

    [SerializeField, Tooltip("Poise Damge to stun entity for a moment")]
    private StatData _poiseDamage = new StatData(10f);
    public StatData PoiseDamage => _poiseDamage;


    [Header("--- DEFENSE ---")]
    [SerializeField]
    private StatData _defense = new StatData(5f);
    public StatData Defense => _defense;

    [SerializeField]
    private bool _isAffectbyPoise = true; // default true
    public bool IsAffectbyPoise => _isAffectbyPoise;

    [SerializeField, Tooltip("Stun Meter/Stagger")]
    private StatData _poise = new StatData(50f);
    public StatData Poise => _poise;

    [Header("--- MOBILITY ---")]
    [SerializeField]
    private StatData _walkSpeed = new StatData(2f, 1f, 20f);
    public StatData WalkSpeed => _walkSpeed;
    [SerializeField]
    private StatData _runSpeed = new StatData(6f, 1f, 20f);
    public StatData RunSpeed => _runSpeed;

    public event Action<float, float> OnHealthChanged;

    public void TriggeredHealthChanged(float current, float max)
    {
        OnHealthChanged?.Invoke(current, max);
    }
}
