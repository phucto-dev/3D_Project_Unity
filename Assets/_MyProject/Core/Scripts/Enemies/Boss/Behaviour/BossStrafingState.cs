using UnityEngine;

public class BossStrafingState : IBossState
{
    private float _strafeTimer;
    private float _strafeDuration;
    public void Enter(BossStateManager boss)
    {
        boss.SetLocomotion(new GroundLocomotion());
        boss.GetNavMeshAgent().updateRotation = false;
        boss.SetCurentSpeedType(BossSpeedType.Normal);
        _strafeDuration = Random.Range(2f, 4f);
        _strafeTimer = 0f;
    }
    public void UpdateState(BossStateManager boss)
    {
        _strafeTimer += Time.deltaTime;
        Vector3 dirToPlayer = (boss.Player.position - boss.transform.position).normalized;
        dirToPlayer.y = 0;
        Vector3 moveDir = dirToPlayer * -1;
        boss.MoveToDir(moveDir);
        if (dirToPlayer != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, targetRot, 5f * Time.deltaTime);
        }
        
        //boss.MoveToDir(moveDir);
        if (_strafeTimer >= _strafeDuration)
        {
            boss.ChangeState(new BossCombatState());
        }
    }
    public void Exit(BossStateManager boss)
    {
        boss.GetNavMeshAgent().updateRotation = true;
    }
    public void OnActionTriggered(BossStateManager boss)
    {

    }
    public void OnAnimationEnded(BossStateManager boss)
    {

    }
}
