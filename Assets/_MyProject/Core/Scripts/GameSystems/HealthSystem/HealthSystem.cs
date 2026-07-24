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

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnRecovery;
    public event Action<DmgInfo> OnTakeDmg;
    public event Action OnDeath;

    private EntityStatsManager _stats;
    private int _invincibilitySourcesCount = 0;
    public bool IsInvincible => _invincibilitySourcesCount > 0;

    private void Awake()
    {
        _stats = GetComponentInParent<EntityStatsManager>();
        Debug.Log("Stats???? : " + _stats);
    }

    private void Start()
    {
        if (_stats == null) return;

        MaxHealth = _stats.FinalMaxHealth;
        CurrentHealth = _stats.FinalMaxHealth;
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
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        if (_stats.GetStatsData() != null)
            _stats.GetStatsData().TriggeredHealthChanged(CurrentHealth, MaxHealth);
    }
    public void ResetHP()
    {
        CurrentHealth = MaxHealth;
        Debug.Log("Health Reset out");
        if (_stats.GetStatsData() != null)
        {
            _stats.GetStatsData().TriggeredHealthChanged(MaxHealth, MaxHealth);
            Debug.Log("Health Reset");
        }
    }
    public void UpdateMaxHP()
    {
        if (_stats == null) return;
        Debug.Log("Stats???? 2: " + _stats);
        Debug.Log("Stats???? 3: " + _stats.FinalMaxHealth);
        MaxHealth = _stats.FinalMaxHealth;
        if (CurrentHealth > _stats.FinalMaxHealth) CurrentHealth = _stats.FinalMaxHealth;
        if (_stats.GetStatsData() != null)
        {
            _stats.GetStatsData().TriggeredHealthChanged(CurrentHealth, MaxHealth);
            Debug.Log("Health Updated");
        }
    }
    public void RecoverHP(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        OnRecovery?.Invoke(amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        if (_stats.GetStatsData() != null)
            _stats.GetStatsData().TriggeredHealthChanged(CurrentHealth, MaxHealth);
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
