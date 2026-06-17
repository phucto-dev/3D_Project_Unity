using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillVFXController : MonoBehaviour
{
    public event Action EndDuration;

    private SkillDataSO _data;
    private float _tickTimer;
    private float _lifeTimer;
    private DmgInfo _dmgInfo;
    private List<Collider> _targetsInRange = new List<Collider>();

    private void Update()
    {
        if (_data == null) return;

        if (!_data.IsToggle && _data.ActiveDuration > 0)
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0)
            {
                EndDuration?.Invoke();
                Destroy(gameObject);
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
    public void Initialize(SkillDataSO skillData, PlayerStatsManager playerStats)
    {
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
