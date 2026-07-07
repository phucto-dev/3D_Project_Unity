using UnityEngine;

public class BossSummonStatues : IBossState
{
    private string RoarAnimName = "AirRoar";
    private SkillCinematic _roarEffect;

    private bool _isRoaring = false;
    public void Enter(BossStateManager boss)
    {
        _roarEffect = boss.GetComponentInChildren<SkillCinematic>(true);
        boss.SetLocomotion(new AirLocomotion());
        boss.StartCoroutine(boss.TakeOffCoroutine(() => {
            boss.Anim.CrossFade(RoarAnimName, 0.1f);

            _isRoaring = true;
        }));
    }
    public void UpdateState(BossStateManager boss)
    {

    }
    public void Exit(BossStateManager boss)
    {

    }
    public void OnActionTriggered(BossStateManager boss)
    {
        if (_roarEffect != null)
        {
            _roarEffect.enabled = true;
            boss.SummonStatues();
        }
    }
    public void OnAnimationEnded(BossStateManager boss)
    {
        if (!_isRoaring) return;
        if (_roarEffect != null)
        {
            _roarEffect.enabled = false;
        }
        boss.StartCoroutine(boss.LandCoroutine(() =>
        {
            boss.SetLocomotion(new GroundLocomotion());
            boss.ChangeState(new BossCombatState());
        }));
    }
}
