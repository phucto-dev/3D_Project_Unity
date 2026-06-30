using UnityEngine;

public class BossGroundedIdleState : IBossState
{
    private string IdleBreatheAnimName = "IdleBreathe";
    public void Enter(BossStateManager boss)
    {
        boss.SetLocomotion(new GroundLocomotion());
        boss.Anim.CrossFade(IdleBreatheAnimName, 0.1f);
        boss.GetNavMeshAgent().updateRotation = false;
    }

    public void UpdateState(BossStateManager boss)
    {
        boss.RotateFaceToPlayer();
    }
    public void OnAnimationEnded(BossStateManager boss) 
    {
        if (boss.IsRotating)
        {
            boss.IsRotating = false;
        }
    }
    public void OnActionTriggered(BossStateManager boss) { }
    public void Exit(BossStateManager boss)
    {
        boss.GetNavMeshAgent().updateRotation = true;
    }
}
