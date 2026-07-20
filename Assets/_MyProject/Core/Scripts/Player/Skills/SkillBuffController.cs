using System.Collections;
using UnityEngine;

public class SkillBuffController : MonoBehaviour
{
    public float TransitionEffectTime;
    public GameObject SideLoopEffect;

    private SkillDataSO _data;
    private float _tickTimer;
    private float _lifeTimer;
    private HealthSystem _playerHealth;
    private PlayerStatsManager _playerStatsManager;
    private bool _hasRegenPassive;
    private CooldownTimer _timer;
    private void Update()
    {
        if (_data == null) return;

        if (!_data.IsToggle && _data.BuffDuration > 0)
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (_data.TickInterval > 0)
        {
            _tickTimer -= Time.deltaTime;
            if (_tickTimer <= 0)
            {
                if (_playerHealth != null)
                    _playerHealth.RecoverHP(_data.HealRegenPassiveAmount);
                _tickTimer = _data.TickInterval;
            }
        }
    }

    public void Initialize(SkillDataSO skillData, PlayerStatsManager playerStats, HealthSystem playerHealthSystem)
    {
        _data = skillData;
        _tickTimer = 0f;
        _lifeTimer = skillData.BuffDuration;
        _playerHealth = playerHealthSystem;
        _playerStatsManager = playerStats;

        if (_data != null)
        {
            OnGainBuff();
            if (SideLoopEffect != null)
            {
                StartCoroutine(TransitionEffect());
            }
        }
    }
    private void OnGainBuff()
    {
        if (_playerHealth == null || _playerStatsManager == null) return;
        if (_data.GainInvincible)
        {
            _playerHealth.AddInvincibility();
        }
        if (_data.GainSuperArmor)
        {
            _playerStatsManager.SetSuperArmor(true);
        }
        if (_data.GainHealthRegenPassive)
        {
            _hasRegenPassive = true;
        }
        if (_data.RecoveryHealth)
        {
            _playerHealth.RecoverHP(_data.HealRegenAmount);
        }
    }

    private IEnumerator TransitionEffect()
    {
        yield return new WaitForSeconds(TransitionEffectTime);
        Instantiate(SideLoopEffect, this.transform);
    }
}
