using System.Collections;
using UnityEngine;

public class BossSummonAttackStrategy : IBossAttackStrategy
{
    private BossAttackType _type;
    public BossSummonAttackStrategy()
    {
        _type = BossAttackType.SummonStatues;
    }
    public BossSummonAttackStrategy(BossAttackType type)
    {
        _type = type;
    }
    public void SetCombatInfo(BossCombatInfo info)
    {

    }
    public IEnumerator ExecuteRoutine(BossStateManager boss)
    {
        yield return new WaitForSeconds(0f);
        boss.ChangeState(new BossSummon(_type));
    }
    public void AttackTrigger(BossStateManager boss)
    {

    }
}
