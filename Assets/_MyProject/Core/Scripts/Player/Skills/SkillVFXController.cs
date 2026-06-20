using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillVFXController : MonoBehaviour
{
    [SerializeField] private float _holdSkillDelayTime;

    public event Action EndDuration;
    public event Action<PlayerSkill, PlayerStatsManager> OnInit;

    private SkillDataSO _data;
    private float _tickTimer;
    private float _lifeTimer;
    private DmgInfo _dmgInfo;
    private List<Collider> _targetsInRange = new List<Collider>();

    private bool _flag = false;
    private void Update()
    {
        if (_data == null) return;
        if (!_data.IsToggle && _data.ActiveDuration > 0)
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0)
            {
                if (_data.SkillType != SkillType.Hold)
                {
                    EndDuration?.Invoke();
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    if (!_flag)
                    {
                        _flag = true;
                        EndDuration?.Invoke();
                    }
                    else
                    {
                        float offset = 0.2f;
                        if (_lifeTimer <= (_holdSkillDelayTime + offset) * -1)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
                return;
            }
        }
        _targetsInRange.RemoveAll(item => item == null || !item.gameObject.activeInHierarchy);
        if (_data.TickInterval > 0)
        {
            _tickTimer -= Time.deltaTime;
            if (_tickTimer <= 0)
            {
                foreach (Collider target in _targetsInRange)
                {
                    DealDamage(target);
                }
                _tickTimer = _data.TickInterval;
            }
        }
    }
    public void Initialize(SkillDataSO skillData, PlayerStatsManager playerStats, PlayerSkill playerSkill)
    {
        OnInit?.Invoke(playerSkill, playerStats);
        float baseDmg = playerStats.AttackPower.GetValue();
        float finalDmg = baseDmg * skillData.DmgScaleMultiplier;
        _data = skillData;
        //Debug.Log("_data: "+ skillData.TargetLayer);
        _tickTimer = 0f;
        _lifeTimer = skillData.ActiveDuration;
        _dmgInfo = new DmgInfo()
        {
            Amount = finalDmg,
            PoiseDamage = playerStats.PoiseDamage.GetValue(),
            Dealer = playerStats.transform,
            IsCritical = false
        };
        transform.localScale *= skillData.RangeScaleMultiplier;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _data.TargetLayer) != 0)
        {
            if (!_targetsInRange.Contains(other))
            {
                _targetsInRange.Add(other);

                if (_data.TickInterval <= 0)
                {
                    DealDamage(other);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_targetsInRange.Contains(other))
        {
            _targetsInRange.Remove(other);
        }
    }
    private void DealDamage(Collider target)
    {
        HealthSystem targetHealth = target.GetComponent<HealthSystem>();
        if (target == null) return;

        targetHealth.TakeDmg(_dmgInfo);
    }
}
