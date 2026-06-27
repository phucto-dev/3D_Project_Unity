using UnityEngine;

public class BossGroundedIdleState : IBossState
{
    private string IdleBreatheAnimName = "IdleBreathe";
    public void Enter(BossStateManager boss)
    {
        boss.SetLocomotion(new GroundLocomotion());
        boss.Anim.CrossFade(IdleBreatheAnimName, 0.1f);
    }

    public void UpdateState(BossStateManager boss) { }
    public void OnAnimationEnded(BossStateManager boss) { }
    public void OnActionTriggered(BossStateManager boss) { }
    public void Exit(BossStateManager boss) { }
}
