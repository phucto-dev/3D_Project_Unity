using UnityEngine;

public class AirLocomotion : ILocomotionStrategy
{
    public void Enter(BossStateManager boss)
    {
        boss.GetNavMeshAgent().enabled = false;
        boss.GetRigidbody().isKinematic = true;
    }
    public void MoveTo(BossStateManager boss, Vector3 targetPosition)
    {
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, Time.deltaTime * boss.FlySpeed);
        Vector3 dir = (targetPosition - boss.transform.position).normalized;
        if (dir != Vector3.zero)
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }
    public void Exit(BossStateManager boss)
    {

    }
}
