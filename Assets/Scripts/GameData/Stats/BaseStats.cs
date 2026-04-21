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
public class BaseStats : ScriptableObject
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

    [SerializeField, Tooltip("Attack per second)")]
    private StatData _attackSpeed = new StatData(1f, 0.1f, 5f);
    public StatData AttackSpeed => _attackSpeed;

    [SerializeField, Tooltip("Critical Rate (0.0 - 1.0)")]
    private StatData _critRate = new StatData(0.05f, 0f, 1f);
    public StatData CritRate => _critRate;

    [SerializeField, Tooltip("Critical Damage (1.5 = 150%)")]
    private StatData _critDamage = new StatData(1.5f, 1f, 5f);
    public StatData CritDamage => _critDamage;

    [Header("--- DEFENSE ---")]
    [SerializeField]
    private StatData _defense = new StatData(5f);
    public StatData Defense => _defense;

    [SerializeField, Tooltip("Stun Meter/Stagger")]
    private StatData _poise = new StatData(50f);
    public StatData Poise => _poise;

    [Header("--- MOBILITY ---")]
    [SerializeField]
    private StatData _movementSpeed = new StatData(5f, 1f, 20f);
    public StatData MovementSpeed => _movementSpeed;
}
