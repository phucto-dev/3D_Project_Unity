using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossBlast : IBossAttackStrategy
{
    private BossCombatInfo _info;
    private string BlastBeamAnimName = "BlastBeam";
    public void SetCombatInfo(BossCombatInfo info)
    {
        _info = info;
    }
    public IEnumerator ExecuteRoutine(BossStateManager boss)
    {
        if (_info.IsUnityNull()) yield return null;

        boss.SetLocomotion(new GroundLocomotion());
        boss.Anim.CrossFade(BlastBeamAnimName, 0.1f);

        yield return new WaitForSeconds(1f);
    }
    public void AttackTrigger(BossStateManager boss)
    {
        GameObject beam = PoolManager.Instance.Get(_info.VFXID);
        if (beam == null) return;
        VFXBossSkill corebeam = beam.GetComponent<VFXBossSkill>();
        if (corebeam != null) corebeam.SetUp(_info);
        beam.transform.position = boss.MouthPoint.transform.position;
    }
}
