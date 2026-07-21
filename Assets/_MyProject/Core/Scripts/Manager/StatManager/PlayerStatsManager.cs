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
    private void OnEnable()
    {
        if (_baseStatsSO is PlayerStatsSO playerStats)
        {
            float timer = Time.time;
            while (Time.time - timer < 3f)
            {
                Debug.Log("Trong While: " + Time.time);
            }
            Debug.Log("Xong");
            playerStats.TriggeredManaChanged(_currentMana, MaxMana.GetValue());
            playerStats.TriggeredStaminaChanged(_currentStamina, MaxStamina.GetValue());
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
        return false;
    }
    public void ConsumeStamina()
    {
        if (_currentStamina < _staminaCost) return;
        _currentStamina -= _staminaCost;
        if (_baseStatsSO is PlayerStatsSO playerStats)
        {
            playerStats.TriggeredStaminaChanged(_currentStamina, MaxStamina.GetValue());
        }
    }
    public void RecoverStamina()
    {
        if (_currentStamina == MaxStamina.GetValue()) return;
        if (!(_baseStatsSO is PlayerStatsSO playerStats)) return;
        if (_currentStamina > MaxStamina.GetValue())
        {
            _currentStamina = MaxStamina.GetValue();
            playerStats.TriggeredStaminaChanged(_currentStamina, MaxStamina.GetValue());
            return;
        }
        _currentStamina += _staminaRecover;
        playerStats.TriggeredStaminaChanged(_currentStamina, MaxStamina.GetValue());
    }
    public bool CheckAllowRecoverStamina()
    {
        return !(_currentStamina >= MaxStamina.GetValue());
    }
}
