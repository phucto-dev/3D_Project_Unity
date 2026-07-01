using System.Collections.Generic;
using UnityEngine;

public class BossCombatState : IBossState
{
    private BossCombatInfo _bossCombatInfo;
    public void Enter(BossStateManager boss)
    {
        IBossAttackStrategy attackStrategy = BossAttackFactory.CreateStrategy(BossAttackType.CoreBeam);
        _bossCombatInfo = boss.BossCombatDataList.BossCombatStates[1];
        attackStrategy.SetCombatInfo(_bossCombatInfo);
        boss.ExecuteAttack(attackStrategy);
    }

    public void UpdateState(BossStateManager boss) { }
    public void OnAnimationEnded(BossStateManager boss)
    {
        boss.ChangeState(new BossGroundedIdleState());
    }
    public void OnActionTriggered(BossStateManager boss) { }
    public void Exit(BossStateManager boss) { }
}
