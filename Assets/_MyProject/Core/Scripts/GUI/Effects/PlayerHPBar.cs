using UnityEngine;

public class PlayerHPBar : BarUI
{
    public BaseStatsSO BaseStats;
    private void OnEnable()
    {
        if (BaseStats != null)
            BaseStats.OnHealthChanged += SetTarget;
    }
    private void OnDisable()
    {
        if (BaseStats != null)
            BaseStats.OnHealthChanged -= SetTarget;
    }
}
