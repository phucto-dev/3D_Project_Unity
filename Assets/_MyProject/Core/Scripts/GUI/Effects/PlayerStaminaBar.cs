using UnityEngine;

public class PlayerStaminaBar : BarUI
{
    public PlayerStatsSO BaseStats;
    private void OnEnable()
    {
        if (BaseStats != null)
            BaseStats.OnStaminaChanged += SetTarget;
    }
    private void OnDisable()
    {
        if (BaseStats != null)
            BaseStats.OnStaminaChanged -= SetTarget;
    }
}
