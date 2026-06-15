using System;
using UnityEngine;

public struct DmgInfo
{
    public float Amount;
    public Transform Dealer;
    public float PoiseDamage;
    public bool IsCritical;
}
public class HealthSystem : MonoBehaviour
{
    [Header("--- DO NOT EDIT ---")]
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }

    public event Action<float> OnHealthChanged;
    public event Action<float> OnRecovery;
    public event Action<DmgInfo> OnTakeDmg;
    public event Action OnDeath;

    private EntityStatsManager _stats;
    private int _invincibilitySourcesCount = 0;
    public bool IsInvincible => _invincibilitySourcesCount > 0;

    private void Awake()
    {
        _stats = GetComponentInParent<EntityStatsManager>();
    }

    private void Start()
    {
        if (_stats == null) return;

        MaxHealth = _stats.MaxHealth.GetValue();
        CurrentHealth = _stats.MaxHealth.GetValue();
    }
    public void TakeDmg(DmgInfo dmgInfo)
    {
        if (IsInvincible)
        {
            Debug.Log("Dodge: " + _invincibilitySourcesCount);
            return;
        }
        if (CurrentHealth <= 0) return;
        CurrentHealth -= dmgInfo.Amount;
        OnTakeDmg?.Invoke(dmgInfo);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDeath?.Invoke();
        }
    }
    public void ResetHP()
    {
        CurrentHealth = _stats.MaxHealth.GetValue();
    }
    public void RecoverHP(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        OnRecovery?.Invoke(amount);
    }
    public void AddInvincibility()
    {
        _invincibilitySourcesCount++;
    }

    public void RemoveInvincibility()
    {
        _invincibilitySourcesCount--;

        if (_invincibilitySourcesCount < 0) _invincibilitySourcesCount = 0;
    }
}
