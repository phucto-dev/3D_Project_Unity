using UnityEngine;

public class BossEnterSecondPhase : IBossState
{
    private BossCombatInfo _bossCombatInfo;
    private IBossAttackStrategy _attackStrategy;
    private float _exitTime = 12;
    private float _exitTimer;
    public void Enter(BossStateManager boss)
    {
        foreach (var skill in boss.BossCombatDataList.BossCombatStates)
        {
            if (skill.AttackType == BossAttackType.SummonStatues)
            {
                _bossCombatInfo = skill;
            }
        }
        _attackStrategy = BossAttackFactory.CreateStrategy(_bossCombatInfo);
        _attackStrategy.SetCombatInfo(_bossCombatInfo);
        boss.GetStats().UsedStamina(_bossCombatInfo.StaminaConsume);
        boss.ExecuteAttack(_attackStrategy);
    }
    public void UpdateState(BossStateManager boss)
    {

    }
    public void Exit(BossStateManager boss)
    {
        _exitTimer += Time.deltaTime;
        if (_exitTimer >= _exitTime)
        {
            Debug.Log("Force Quit");
            boss.ChangeState(new BossDecisionState());
        }
    }
    public void OnActionTriggered(BossStateManager boss)
    {

    }
    public void OnAnimationEnded(BossStateManager boss)
    {

    }
}
