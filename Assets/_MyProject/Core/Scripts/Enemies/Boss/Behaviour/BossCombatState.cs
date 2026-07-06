using System.Collections.Generic;
using UnityEngine;

public class BossCombatState : IBossState
{
    private BossCombatInfo _bossCombatInfo;
    private IBossAttackStrategy _attackStrategy;
    public void Enter(BossStateManager boss)
    {
        _attackStrategy = BossAttackFactory.CreateStrategy(BossAttackType.CoreBeam);
        _bossCombatInfo = boss.BossCombatDataList.BossCombatStates[1];
        _attackStrategy.SetCombatInfo(_bossCombatInfo);
        boss.ExecuteAttack(_attackStrategy);
    }

    public void UpdateState(BossStateManager boss) { }
    public void OnAnimationEnded(BossStateManager boss)
    {
        //boss.ChangeState(new BossGroundedIdleState());
        boss.ChangeState(new BossSummonStatues());
    }
    public void OnActionTriggered(BossStateManager boss) 
    {
        if (_attackStrategy == null) return;
        _attackStrategy.AttackTrigger(boss);
    }
    public void Exit(BossStateManager boss) { }
}
