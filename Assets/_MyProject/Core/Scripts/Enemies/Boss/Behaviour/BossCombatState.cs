using UnityEngine;

public class BossCombatState : IBossState
{
    public void Enter(BossStateManager boss)
    {
        IBossAttackStrategy attackStrategy = BossAttackFactory.CreateStrategy(BossAttackType.DashAndBite);
        boss.ExecuteAttack(attackStrategy);
    }

    public void UpdateState(BossStateManager boss) { }
    public void OnAnimationEnded(BossStateManager boss) { }
    public void OnActionTriggered(BossStateManager boss) { }
    public void Exit(BossStateManager boss) { }
}
