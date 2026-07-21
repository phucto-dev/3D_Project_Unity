using System;
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
    private StatData _staminaCostPerRoll = new StatData(20f);
    public StatData StaminaCostPerRoll => _staminaCostPerRoll;
    [SerializeField]
    private StatData _staminaRecoverPerSec = new StatData(20f);
    public StatData StaminaRecoverPerSec => _staminaRecoverPerSec;

    public event Action<float, float> OnStaminaChanged;
    public event Action<float, float> OnManaChanged;

    public void TriggeredStaminaChanged(float current, float max)
    {
        OnStaminaChanged?.Invoke(current, max);
    }
    public void TriggeredManaChanged(float current, float max)
    {
        OnManaChanged?.Invoke(current, max);
    }
}
