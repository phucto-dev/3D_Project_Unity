using System.Collections.Generic;
using UnityEngine;

public class BossCombatState : IBossState
{
    private BossCombatInfo _bossCombatInfo;
    private IBossAttackStrategy _attackStrategy;
    public void Enter(BossStateManager boss)
    {
        _attackStrategy = BossAttackFactory.CreateStrategy(BossAttackType.DashAndBite);
        _bossCombatInfo = boss.BossCombatDataList.BossCombatStates[0];
        _attackStrategy.SetCombatInfo(_bossCombatInfo);
        boss.ExecuteAttack(_attackStrategy);
    }

    public void UpdateState(BossStateManager boss) { }
    public void OnAnimationEnded(BossStateManager boss)
    {
        boss.ChangeState(new BossGroundedIdleState());
        //boss.ChangeState(new BossStrafingState());
    }
    public void OnActionTriggered(BossStateManager boss) 
    {
        if (_attackStrategy == null) return;
        _attackStrategy.AttackTrigger(boss);
    }
    public void Exit(BossStateManager boss) { }
}
