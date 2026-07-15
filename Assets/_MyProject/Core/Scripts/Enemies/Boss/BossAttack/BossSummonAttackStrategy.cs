using System.Collections;
using UnityEngine;

public class BossSummonAttackStrategy : IBossAttackStrategy
{
    private BossCombatInfo _bossCombatInfo;
    public BossSummonAttackStrategy()
    {

    }
    public BossSummonAttackStrategy(BossCombatInfo bossCombatInfo)
    {
        _bossCombatInfo = bossCombatInfo;
    }
    public void SetCombatInfo(BossCombatInfo info)
    {

    }
    public IEnumerator ExecuteRoutine(BossStateManager boss)
    {
        yield return new WaitForSeconds(0f);
        boss.ChangeState(new BossSummon(_bossCombatInfo));
    }
    public void AttackTrigger(BossStateManager boss)
    {

    }
}
