using System;
using System.Collections;
using UnityEngine;

public class SkillBuffController : MonoBehaviour, PlayerSkillEnd
{
    public float TransitionEffectTime;
    public GameObject SideLoopEffect;
    public event Action EndDuration;

    private SkillDataSO _data;
    private float _tickTimer;
    private float _lifeTimer;
    private HealthSystem _playerHealth;
    private PlayerStatsManager _playerStatsManager;
    private bool _hasRegenPassive;
    private CooldownTimer _timer;
    private AudioSource _audioSource;

    private void OnEnable()
    {
        EndDuration += StopAudio;
    }
    private void OnDisable()
    {
        EndDuration = null;
    }
    private void Update()
    {
        if (_data == null) return;

        if (!_data.IsToggle && _data.BuffDuration > 0)
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0)
            {
                EndDuration?.Invoke();
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
        if (_data.Audio == null) return;
        if (_data.Audio.Appear == null) return;
        if (_data.Audio.Appear.AttachType == SoundAttachType.Caster) return;
        if (_data.Audio.Appear.PlaybackType == SoundPlaybackType.Loop)
        {
            _audioSource = AudioManager.Instance.PlaySFXLoop(_data.Audio.Appear, transform.position);
        }
        else
        {
            AudioManager.Instance.PlaySFX(_data.Audio.Appear, transform.position);
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
    private void StopAudio()
    {
        if (_audioSource == null) return;
        AudioManager.Instance.StopSFXLoop(_audioSource);
    }
}
