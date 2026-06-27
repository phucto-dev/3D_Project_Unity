using UnityEngine;

public class GroundLocomotion : ILocomotionStrategy
{
    public void Enter(BossStateManager boss)
    {
        boss.GetNavMeshAgent().enabled = true;
        boss.GetRigidbody().isKinematic = true;
    }
    public void MoveTo(BossStateManager boss, Vector3 targetPosition)
    {
        if (boss.GetNavMeshAgent().isOnNavMesh) boss.GetNavMeshAgent().SetDestination(targetPosition);
    }
    public void Exit(BossStateManager boss)
    {
        boss.GetNavMeshAgent().enabled = true;
    }
}
