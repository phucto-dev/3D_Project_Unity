using System.Collections;
using System.Threading;
using UnityEngine;

public class BossBite : IBossAttackStrategy
{
    private string BiteAnimName = "BiteAttack";
    private float _biteOffset = 10f;
    float timeout = 10f;
    float timer = 0f;
    public void SetCombatInfo(BossCombatInfo info)
    {

    }
    public IEnumerator ExecuteRoutine(BossStateManager boss)
    {
        boss.SetLocomotion(new GroundLocomotion());
        boss.SetCurentSpeedType(BossSpeedType.Fast);

        while (Vector3.Distance(boss.transform.position, boss.Player.position) > _biteOffset && timer < timeout)
        {
            boss.MoveToTarget(boss.Player.position);
            timer += Time.deltaTime;
            yield return null;
        }

        boss.GetNavMeshAgent().ResetPath();
        Vector3 dir = (boss.Player.position - boss.transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            boss.transform.rotation = Quaternion.LookRotation(dir);
        }

        boss.Anim.CrossFade(BiteAnimName, 0.1f);
        yield return new WaitForSeconds(0.1f);
        float momentumDur = 1.2f;
        float momentumSpeed = boss.GetStats().RunSpeed.GetValue() + 2f;
        float timer2 = 0f;
        while (timer2 < momentumDur)
        {
            boss.GetNavMeshAgent().Move(boss.transform.forward * momentumSpeed * Time.deltaTime);
            timer2 += Time.deltaTime;
            yield return null;
        }
    }
    public void AttackTrigger(BossStateManager boss)
    {
        boss.ChangeState(new BossStrafingState());
    }
}
