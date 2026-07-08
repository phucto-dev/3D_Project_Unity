using System.Collections;
using UnityEngine;

public class BossSummonAttackStrategy : IBossAttackStrategy
{
    public void SetCombatInfo(BossCombatInfo info)
    {

    }
    public IEnumerator ExecuteRoutine(BossStateManager boss)
    {
        yield return new WaitForSeconds(0f);
        boss.ChangeState(new BossSummonStatues());
    }
    public void AttackTrigger(BossStateManager boss)
    {

    }
}
