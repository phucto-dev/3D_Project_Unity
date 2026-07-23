using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : EntityStatsManager
{
    [Header("--- PLAYER STATS ---")]
    public Stat MaxStamina;
    public Stat MaxMana;
    public Stat JumpForce;
    public Stat RotationSpeed;
    public Stat RollSpeed;

    [Header("--- FINAL STATS ---")]
    public float FinalMaxStamina { get; private set; }
    public float FinalMaxMana { get; private set; }

    private float _currentStamina;
    private float _currentMana;

    private float _staminaCost;
    private float _staminaRecover;
    private float _manaRecoverPerSec;
    private float _timer;

    protected override void Awake()
    {
        if (_baseStatsSO == null) return;
        
        if (_baseStatsSO is PlayerStatsSO playerStats)
        {
            MaxStamina = new Stat(playerStats.MaxStamina.BaseValue);
            MaxMana = new Stat(playerStats.MaxMana.BaseValue);
            JumpForce = new Stat(playerStats.JumpForce.BaseValue);
            RotationSpeed = new Stat(playerStats.RotationSpeed.BaseValue);
            RollSpeed = new Stat(playerStats.RollSpeed.BaseValue);
            _currentStamina = MaxStamina.GetValue();
            _currentMana = MaxMana.GetValue();
            _staminaCost = new Stat(playerStats.StaminaCostPerRoll.BaseValue).GetValue();
            _staminaRecover = new Stat(playerStats.StaminaRecoverPerSec.BaseValue).GetValue();
            _manaRecoverPerSec = new Stat(playerStats.ManaRecoverPerSec.BaseValue).GetValue();
        }

        base.Awake();
        //RecalculateFinalStats(new Dictionary<StatType, float>());
    }
    private void OnEnable()
    {
        StartCoroutine(DelayedTriggerRoutine());
    }
    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer >= 1)
        {
            RecoverMana(_manaRecoverPerSec);
            _timer = 0;
        }
    }
    private IEnumerator DelayedTriggerRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (_baseStatsSO is PlayerStatsSO playerStats)
        {
            playerStats.TriggeredManaChanged(_currentMana, MaxMana.GetValue());
            playerStats.TriggeredStaminaChanged(_currentStamina, MaxStamina.GetValue());
        }
        RecalculateFinalStats(new Dictionary<StatType, float>());
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
    public void RecoverMana(float amount)
    {
        if (_currentMana == MaxMana.GetValue()) return;
        if (!(_baseStatsSO is PlayerStatsSO playerStats)) return;
        if (_currentMana > MaxMana.GetValue())
        {
            _currentMana = MaxMana.GetValue();
            playerStats.TriggeredManaChanged(_currentMana, MaxMana.GetValue());
            return;
        }
        _currentMana += amount;
        playerStats.TriggeredManaChanged(_currentMana, MaxMana.GetValue());
    }

    public override void RecalculateFinalStats(Dictionary<StatType, float> eqBonuses)
    {
        base.RecalculateFinalStats(eqBonuses);
        if (eqBonuses == null) eqBonuses = new Dictionary<StatType, float>();

        FinalMaxStamina = MaxStamina.GetValue() + eqBonuses.GetValueOrDefault(StatType.Stamina, 0f);
        FinalMaxMana = MaxMana.GetValue() + eqBonuses.GetValueOrDefault(StatType.Mana, 0f);

        // Điều chỉnh Current Values không được vượt quá Final Max mới (khi tháo đồ)
        
        _currentStamina = Mathf.Min(_currentStamina, FinalMaxStamina);
        _currentMana = Mathf.Min(_currentMana, FinalMaxMana);

        _dictStats[StatsValue.Stamina] = FinalMaxStamina;
        _dictStats[StatsValue.Mana] = FinalMaxMana;
        if (!(_baseStatsSO is PlayerStatsSO playerStats)) return;
        Debug.Log("Goi ne");
        playerStats.TriggeredStatsChanged(_dictStats);
    }
}
