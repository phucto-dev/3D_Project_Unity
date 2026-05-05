using UnityEngine;

public class EnemyStatsManager : EntityStatsManager
{
    [Header("--- ENEMY STATS ---")]
    public Stat QuantityOfAttack;

    protected override void Awake()
    {
        base.Awake();

        if (_baseStatsSO is EnemyStats _enemyStats)
        {
            QuantityOfAttack = new Stat(_enemyStats.NumberOfAttack);
        }
    }
}
