using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "GameData/Player/Stats/PlayerStats")]
public class PlayerStatsSO : BaseStatsSO
{
    [Header("--- SURVIVAL ---")]
    [SerializeField]
    private StatData _maxStamina = new StatData(100f);
    public StatData MaxStamina => _maxStamina;
    [SerializeField]
    private StatData _maxMana = new StatData(100f);
    public StatData MaxMana => _maxMana;
    [SerializeField]
    private StatData _mananaRecoverPerSec = new StatData(2f);
    public StatData ManaRecoverPerSec => _mananaRecoverPerSec;
    [SerializeField]
    private StatData _staminaCostPerRoll = new StatData(20f);
    public StatData StaminaCostPerRoll => _staminaCostPerRoll;
    [SerializeField]
    private StatData _staminaRecoverPerSec = new StatData(20f);
    public StatData StaminaRecoverPerSec => _staminaRecoverPerSec;
    [SerializeField]
    private StatData _rollSpeed = new StatData(5f);
    public StatData RollSpeed => _rollSpeed;
    [SerializeField]
    private StatData _jumpForce = new StatData(8f);
    public StatData JumpForce => _jumpForce;
    [SerializeField]
    private StatData _rotationSpeed = new StatData(15f);
    public StatData RotationSpeed => _rotationSpeed;

    public event Action<float, float> OnStaminaChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<Dictionary<StatsValue, float>> OnStatsChanged;

    public void TriggeredStaminaChanged(float current, float max)
    {
        OnStaminaChanged?.Invoke(current, max);
    }
    public void TriggeredManaChanged(float current, float max)
    {
        OnManaChanged?.Invoke(current, max);
    }
    public void TriggeredStatsChanged(Dictionary<StatsValue, float> dictStats)
    {
        OnStatsChanged?.Invoke(dictStats);
    }
}
