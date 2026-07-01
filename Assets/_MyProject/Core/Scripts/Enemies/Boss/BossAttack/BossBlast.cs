using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossBlast : IBossAttackStrategy
{
    private BossCombatInfo _info;
    private string SpreadFireAnimName = "SpreadFire";
    public void SetCombatInfo(BossCombatInfo info)
    {
        _info = info;
    }
    public IEnumerator ExecuteRoutine(BossStateManager boss)
    {
        if (_info.IsUnityNull()) yield return null;

        boss.SetLocomotion(new GroundLocomotion());

        boss.Anim.CrossFade(SpreadFireAnimName, 0.1f);
        GameObject beam = PoolManager.Instance.Get(_info.VFXID);
        if (beam == null) yield return null;
        CoreBeam corebeam = beam.GetComponent<CoreBeam>();
        if (corebeam != null) corebeam.SetUp(_info);
        beam.transform.position = boss.MouthPoint.transform.position;

        yield return new WaitForSeconds(1f);
    }
}
