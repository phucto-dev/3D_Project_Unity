using UnityEngine;

public class BossDieState : IBossState
{
    private string DieAnimName = "Death";
    public void Enter(BossStateManager boss)
    {
        boss.SetLocomotion(new GroundLocomotion());
        boss.CloseHurtBox();
        boss.Anim.CrossFade(DieAnimName, 0.1f);
    }
    public void UpdateState(BossStateManager boss)
    {

    }
    public void Exit(BossStateManager boss)
    {

    }
    public void OnActionTriggered(BossStateManager boss)
    {

    }
    public void OnAnimationEnded(BossStateManager boss)
    {

    }
}
