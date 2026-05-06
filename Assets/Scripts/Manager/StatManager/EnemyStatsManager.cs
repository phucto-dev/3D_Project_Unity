using UnityEngine;

public class EnemyStatsManager : EntityStatsManager
{
    [Header("--- ENEMY STATS ---")]
    public Stat QuantityOfAttack;
    public Stat QuantityOfHurt;
    public Stat HurtDelay;

    protected override void Awake()
    {
        base.Awake();

        if (_baseStatsSO is EnemyStats _enemyStats)
        {
            QuantityOfAttack = new Stat(_enemyStats.NumberOfAttack);
            QuantityOfHurt = new Stat(_enemyStats.QuantityOfHurt);
            HurtDelay = new Stat(_enemyStats.HurtDelay.BaseValue);
        }
    }
}
