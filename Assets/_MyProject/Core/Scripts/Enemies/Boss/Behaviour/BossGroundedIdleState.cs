using UnityEngine;

public class BossGroundedIdleState : IBossState
{
    private string IdleBreatheAnimName = "IdleBreathe";
    private float _staminaRecoverPerSec = 40f;
    private float _timer = 0f;
    private float _restTime = 2f;
    public void Enter(BossStateManager boss)
    {
        boss.SetLocomotion(new GroundLocomotion());

        int layerIndex = 0;
        AnimatorStateInfo currentState = boss.Anim.GetCurrentAnimatorStateInfo(layerIndex);
        AnimatorStateInfo nextState = boss.Anim.GetNextAnimatorStateInfo(layerIndex);
        bool isInTransition = boss.Anim.IsInTransition(layerIndex);

        bool isAlreadyIdle = currentState.IsName(IdleBreatheAnimName) ||
                             (isInTransition && nextState.IsName(IdleBreatheAnimName));

        if (!isAlreadyIdle)
        {
            boss.Anim.CrossFade(IdleBreatheAnimName, 0.1f);
        }

        boss.GetNavMeshAgent().updateRotation = false;
        _timer = 0f;
    }

    public void UpdateState(BossStateManager boss)
    {
        _timer += Time.deltaTime;
        boss.GetStats().RecoverStamina(_staminaRecoverPerSec * Time.deltaTime);

        if (_timer >= _restTime)
        {
            boss.ChangeState(new BossDecisionState());
        }
    }
    public void OnAnimationEnded(BossStateManager boss) 
    {

    }
    public void OnActionTriggered(BossStateManager boss) { }
    public void Exit(BossStateManager boss)
    {
        boss.GetNavMeshAgent().updateRotation = true;
    }
}
