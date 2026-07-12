using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossBlast : IBossAttackStrategy
{
    private BossCombatInfo _info;
    private string BlastBeamAnimName = "BlastBeam";
    private bool _hasFired = false;
    public void SetCombatInfo(BossCombatInfo info)
    {
        _info = info;
    }
    public IEnumerator ExecuteRoutine(BossStateManager boss)
    {
        if (_info.IsUnityNull()) yield return null;

        boss.SetLocomotion(new GroundLocomotion());

        boss.Anim.CrossFade(BlastBeamAnimName, 0.1f);

        yield return new WaitForSeconds(0.1f);
        int layerIndex = 0;
        float timeout = 3f;
        float timer = 0f;

        AnimatorStateInfo ac = boss.Anim.GetCurrentAnimatorStateInfo(layerIndex);
        float animLength = ac.length;

        while (!_hasFired && timer < timeout)
        {
            if (boss.Player != null)
            {
                Vector3 offset = boss.Player.position - boss.transform.position;
                offset.y = 0;

                if (offset != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(offset.normalized);
                    boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, targetRot, 10f * Time.deltaTime);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(animLength - 0.1f);
        boss.ChangeState(new BossDecisionState());
    }
    public void AttackTrigger(BossStateManager boss)
    {
        _hasFired = true;
        GameObject beam = PoolManager.Instance.Get(_info.VFXID);
        if (beam == null) return;
        VFXBossSkill corebeam = beam.GetComponent<VFXBossSkill>();
        if (corebeam != null) corebeam.SetUp(_info);
        beam.transform.position = boss.MouthPoint.transform.position;
        beam.transform.rotation = boss.MouthPoint.transform.rotation;
    }
}
