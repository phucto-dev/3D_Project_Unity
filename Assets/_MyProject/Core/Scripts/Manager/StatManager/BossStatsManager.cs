using UnityEngine;

public class BossStatsManager : EntityStatsManager
{
    private float _currentStamina;
    private float _maxStamina;
    private BossStatsSO _bossStats;
    protected override void Awake()
    {
        base.Awake();
        if (!(_baseStatsSO is BossStatsSO bossStats)) return;
        _bossStats = bossStats;
        _maxStamina = _bossStats.TotalBossStamina.BaseValue;
        _currentStamina = _bossStats.TotalBossStamina.BaseValue;
    }
    public float GetCurrentStamina() => _currentStamina;
    public float GetMaxStamina() => _maxStamina;
    public void UsedStamina(float amount)
    {
        _currentStamina -= amount;
        if (_currentStamina < 0) _currentStamina = 0;
    }
    public void RecoverStamina(float amount)
    {
        _currentStamina += amount;
        if (_currentStamina > _maxStamina) _currentStamina = _maxStamina;
    }

}
