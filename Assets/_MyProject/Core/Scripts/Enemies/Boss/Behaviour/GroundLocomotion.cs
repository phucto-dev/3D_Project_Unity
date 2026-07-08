using UnityEngine;

public class GroundLocomotion : ILocomotionStrategy
{
    private string IdleBreathAnimName = "IdleBreathe";
    private string RunAnimName = "Run";
    private string WalkAnimName = "Walk";
    private string WalkBackAnimName = "Walk_Bw";

    private BossSpeedType _currentSpeedType;
    private float _currentSpeed;
    private string _currentAnimState;
    public void Enter(BossStateManager boss)
    {
        _currentSpeedType = BossSpeedType.Normal;
        boss.GetNavMeshAgent().speed = boss.GetStats().WalkSpeed.GetValue();
        boss.GetNavMeshAgent().enabled = true;
        boss.GetRigidbody().isKinematic = true;
    }
    public void MoveTo(BossStateManager boss, Vector3 targetPosition)
    {
        if (boss.GetNavMeshAgent().isOnNavMesh)
        {
            boss.GetNavMeshAgent().SetDestination(targetPosition);
            string targetAnim = (_currentSpeedType == BossSpeedType.Normal) ? WalkAnimName : RunAnimName;
            if (_currentAnimState != targetAnim)
            {
                boss.Anim.CrossFade(targetAnim, 0.1f);
                _currentAnimState = targetAnim;
            }
        }
    }
    public void MoveBack(BossStateManager boss, Vector3 dir)
    {
        if (boss.GetNavMeshAgent().isOnNavMesh)
        {
            boss.GetNavMeshAgent().Move(dir * Time.deltaTime * boss.GetStats().WalkSpeed.GetValue());
            string targetAnim = WalkBackAnimName;
            if (_currentAnimState != targetAnim)
            {
                boss.Anim.CrossFade(targetAnim, 0.1f);
                _currentAnimState = targetAnim;
            }
        }
    }
    public void SetSpeedType(BossStateManager boss, BossSpeedType speedType)
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

        if (_currentAnimState != IdleBreathAnimName)
        {
            boss.Anim.CrossFade(IdleBreathAnimName, 0.1f);
            _currentAnimState = IdleBreathAnimName;
        }
    }
    public void Exit(BossStateManager boss)
    {
        boss.GetNavMeshAgent().enabled = false;
    }
}
