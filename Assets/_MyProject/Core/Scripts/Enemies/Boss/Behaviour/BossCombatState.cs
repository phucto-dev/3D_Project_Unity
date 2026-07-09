using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossCombatState : IBossState
{
    private BossCombatInfo _bossCombatInfo;
    private IBossAttackStrategy _attackStrategy;
    private List<BossCombatInfo> _validSkills = new List<BossCombatInfo>();
    public void Enter(BossStateManager boss)
    {
        BossPhase currentPhase = boss.GetCurrentPhase();
        float currentStamina = boss.GetStats().GetCurrentStamina();

        foreach (var skill in boss.BossCombatDataList.BossCombatStates)
        {
            if (skill.PhaseUse <= currentPhase && skill.StaminaConsume <= currentStamina)
            {
                _validSkills.Add(skill);
            }
        }
        if (_validSkills.Count == 0)
        {
            boss.ChangeState(new BossDecisionState());
            return;
        }

        _bossCombatInfo = GetSkillByWeight(_validSkills);
        _attackStrategy = BossAttackFactory.CreateStrategy(_bossCombatInfo.AttackType);
        _attackStrategy.SetCombatInfo(_bossCombatInfo);
        boss.GetStats().UsedStamina(_bossCombatInfo.StaminaConsume);
        boss.ExecuteAttack(_attackStrategy);
    }

    public void UpdateState(BossStateManager boss) { }
    public void OnAnimationEnded(BossStateManager boss)
    {
        boss.ChangeState(new BossDecisionState());
    }
    public void OnActionTriggered(BossStateManager boss) 
    {
        if (_attackStrategy == null) return;
        _attackStrategy.AttackTrigger(boss);
    }
    public void Exit(BossStateManager boss) { }

    private BossCombatInfo GetSkillByWeight(List<BossCombatInfo> validSkills)
    {
        float totalWeight = 0f;
        int count = validSkills.Count;

        for (int i = 0; i < count; i++)
        {
            totalWeight += validSkills[i].Weight;
        }

        if (totalWeight <= 0f)
        {
            return validSkills[0];
        }

        float randomPoint = UnityEngine.Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < count; i++)
        {
            cumulativeWeight += validSkills[i].Weight;

            if (randomPoint <= cumulativeWeight)
            {
                return validSkills[i];
            }
        }
        return validSkills[count - 1];
    }
}
