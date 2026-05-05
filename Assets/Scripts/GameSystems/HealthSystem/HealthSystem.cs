using System;
using UnityEngine;

public struct DmgInfo
{
    public float Amount;
    public Transform Dealer;
    public bool IsCritical;
}
public class HealthSystem : MonoBehaviour
{
    [Header("--- DO NOT EDIT ---")]
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }

    public event Action<float> OnHealthChanged;
    public event Action<DmgInfo> OnTakeDmg;
    public event Action OnDeath;

    private EntityStatsManager _stats;

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
        if (CurrentHealth <= 0) return;
        CurrentHealth -= dmgInfo.Amount;
        OnTakeDmg?.Invoke(dmgInfo);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDeath?.Invoke();
        }
    }
    
}
