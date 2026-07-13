using UnityEngine;

public class BossHurtState : IBossState
{
    private string HurtAnimName = "GetHit1";
    private float _hurtAnimLength;
    private float _timer;
    public void Enter(BossStateManager boss)
    {
        boss.Anim.CrossFade(HurtAnimName, 0.1f);
        _timer = 0f;
        _hurtAnimLength = 0f;
    }
    public void UpdateState(BossStateManager boss)
    {
        _timer += Time.deltaTime;

        if (_hurtAnimLength == 0f && _timer >= 0.1f)
        {
            AnimatorStateInfo currentInfo = boss.Anim.GetCurrentAnimatorStateInfo(0);
            if (currentInfo.IsName(HurtAnimName))
            {
                _hurtAnimLength = currentInfo.length;
            }
        }

        if (_hurtAnimLength > 0f && _timer >= _hurtAnimLength)
        {
            boss.ChangeState(new BossDecisionState());
        }
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
