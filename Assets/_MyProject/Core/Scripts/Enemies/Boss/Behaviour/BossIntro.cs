using UnityEngine;

public class BossIntro : IBossState
{
    private string FlyStationAnimName = "FlyStationary";
    private string FlyStationToLandAnimName = "FlyStationaryToLanding";
    private Vector3 _landingTarget;
    private bool _isLandingSequenceStarted;
    private readonly float _landingThreshold = 0f;
    public void Enter(BossStateManager boss)
    {
        _isLandingSequenceStarted = false;
        boss.SetLocomotion(new AirLocomotion());
        boss.Anim.CrossFade(FlyStationAnimName, 0.1f);

        if (Physics.Raycast(boss.transform.position, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            _landingTarget = hit.point;
        }
        else
        {
            _landingTarget = boss.transform.position - new Vector3(0, 10f, 0);
        }
    }
    public void UpdateState(BossStateManager boss)
    {
        if (_isLandingSequenceStarted) return;
        boss.MoveToTarget(_landingTarget);
        boss.LookForward();
        float distanceToGround = Vector3.Distance(boss.transform.position, _landingTarget);
        if (distanceToGround <= _landingThreshold)
        {
            _isLandingSequenceStarted = true;
            boss.Anim.CrossFade(FlyStationToLandAnimName, 0.1f);
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
        if (_isLandingSequenceStarted)
        {
            boss.ChangeState(new BossRoar());
        }
    }
}
