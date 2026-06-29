using UnityEngine;

public class BossRoar : IBossState
{
    private string RoarAnimName = "Roar";
    private SkillCinematic _roarEffect;
    public void Enter(BossStateManager boss)
    {
        _roarEffect = boss.GetComponentInChildren<SkillCinematic>(true);
        boss.SetLocomotion(new GroundLocomotion());
        boss.Anim.CrossFade(RoarAnimName, 0.1f);
    }

    public void UpdateState(BossStateManager boss) { }
    public void OnAnimationEnded(BossStateManager boss) 
    {
        if (_roarEffect != null)
        {
            _roarEffect.enabled = false;
        }
        //boss.ChangeState(new BossGroundedIdleState());
        boss.ChangeState(new BossCombatState());
    }
    public void OnActionTriggered(BossStateManager boss)
    {
        if (_roarEffect != null)
        {
            _roarEffect.enabled = true;
        }
    }
    public void Exit(BossStateManager boss) { }
}
