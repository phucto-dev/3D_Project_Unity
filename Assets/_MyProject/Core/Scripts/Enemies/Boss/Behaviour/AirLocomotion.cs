using UnityEngine;

public class AirLocomotion : ILocomotionStrategy
{
    private string FlyStationaryAnimName = "FlyStationary";
    private string FlyAnimName = "Fly";
    private string GlideAnimName = "Glide";

    private BossSpeedType _currentSpeedType;
    private float _currentSpeed;
    private string _currentAnimState;
    public void Enter(BossStateManager boss)
    {
        _currentSpeedType = BossSpeedType.Normal;
        boss.GetNavMeshAgent().speed = boss.GetStats().WalkSpeed.GetValue();
        boss.GetNavMeshAgent().enabled = false;
        boss.GetRigidbody().isKinematic = true;
    }
    public void MoveTo(BossStateManager boss, Vector3 targetPosition)
    {
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, Time.deltaTime * boss.GetStats().WalkSpeed.GetValue());
        Vector3 dir = (targetPosition - boss.transform.position).normalized;
        if (dir != Vector3.zero)
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        string targetAnim = (_currentSpeedType == BossSpeedType.Normal) ? FlyAnimName : GlideAnimName;
        if (_currentAnimState != targetAnim)
        {
            boss.Anim.CrossFade(targetAnim, 0.1f);
            _currentAnimState = targetAnim;
        }
    }
    public void MoveBack(BossStateManager boss, Vector3 dir)
    {

    }
    public void SetSpeedType(BossStateManager  boss, BossSpeedType speedType)
    {
        _currentSpeedType = speedType;
        if (speedType == BossSpeedType.Normal) boss.GetNavMeshAgent().speed = boss.GetStats().WalkSpeed.GetValue();
        else boss.GetNavMeshAgent().speed = boss.GetStats().RunSpeed.GetValue();
    }
    public void Stop(BossStateManager boss)
    {
        if (boss.GetNavMeshAgent().isOnNavMesh)
        {
            boss.GetNavMeshAgent().ResetPath();
        }

        if (_currentAnimState != FlyStationaryAnimName)
        {
            boss.Anim.CrossFade(FlyStationaryAnimName, 0.1f);
            _currentAnimState = FlyStationaryAnimName;
        }
    }
    public void Exit(BossStateManager boss)
    {

    }
}
