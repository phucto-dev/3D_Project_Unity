using UnityEngine;

public class PlayerManaBar : BarUI
{
    public PlayerStatsSO BaseStats;
    private void OnEnable()
    {
        if (BaseStats != null)
        {
            BaseStats.OnManaChanged += SetTarget;
        }
    }
    private void OnDisable()
    {
        if (BaseStats != null)
            BaseStats.OnManaChanged -= SetTarget;
    }
}
