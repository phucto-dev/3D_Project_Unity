using UnityEngine;

public class BossStrafingState : IBossState
{
    private float _strafeTimer;
    private float _strafeDuration;
    private float _yPos;
    public void Enter(BossStateManager boss)
    {
        boss.SetLocomotion(new GroundLocomotion());
        boss.GetNavMeshAgent().updateRotation = false;
        boss.SetCurentSpeedType(BossSpeedType.Normal);
        _strafeDuration = Random.Range(1f, 2f);
        _strafeTimer = 0f;
        _yPos = boss.GetGroundYPos();
    }
    public void UpdateState(BossStateManager boss)
    {
        _strafeTimer += Time.deltaTime;
        Vector3 dirToPlayer = (boss.Player.position - boss.transform.position);
        dirToPlayer.y = 0;
        dirToPlayer.Normalize();
        Vector3 moveDir = dirToPlayer * -1;
        boss.MoveToDir(moveDir);
        if (_yPos >= 0)
        {
            Vector3 finalPos = boss.transform.position;
            finalPos.y = _yPos;
            boss.transform.position = finalPos;
        }
        if (dirToPlayer != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, targetRot, 5f * Time.deltaTime);
        }
        
        if (_strafeTimer >= _strafeDuration)
        {
            boss.ChangeState(new BossDecisionState());
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
