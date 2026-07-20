using UnityEngine;

public class PlayerStatsManager : EntityStatsManager
{
    [Header("--- PLAYER STATS ---")]
    public Stat MaxStamina;
    public Stat MaxMana;

    private float _currentStamina;
    private float _currentMana;
    private float _staminaCost;
    private float _staminaRecover;
    protected override void Awake()
    {
        if (_baseStatsSO == null) return;
        base.Awake();
        if (_baseStatsSO is PlayerStatsSO playerStats)
        {
            MaxStamina = new Stat(playerStats.MaxStamina.BaseValue);
            MaxMana = new Stat(playerStats.MaxMana.BaseValue);
            _currentStamina = MaxStamina.GetValue();
            _currentMana = MaxMana.GetValue();
            _staminaCost = new Stat(playerStats.StaminaCostPerRoll.BaseValue).GetValue();
            _staminaRecover = new Stat(playerStats.StaminaRecoverPerSec.BaseValue).GetValue();
        }
    }
    public bool IsEnoughMana(float amount)
    {
        if (_currentMana < amount) return false;
        _currentMana -= amount;
        if (_baseStatsSO is PlayerStatsSO playerStats)
        {
            playerStats.TriggeredManaChanged(_currentMana, MaxMana.GetValue());
        }
        return true;
    }
    public bool IsRunOutStamnia()
    {
        if (_currentStamina < _staminaCost) return true;
        _currentStamina -= _staminaCost;
        if (_baseStatsSO is PlayerStatsSO playerStats)
        {
            playerStats.TriggeredStaminaChanged(_currentStamina, MaxStamina.GetValue());
        }
        return false;
    }
    public void RecoverStamina()
    {
        _currentStamina += _staminaRecover;
    }
}
