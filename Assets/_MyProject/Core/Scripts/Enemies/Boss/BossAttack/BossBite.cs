using System.Collections;
using UnityEngine;

public class BossBite : IBossAttackStrategy
{
    public IEnumerator ExecuteRoutine(BossStateManager boss)
    {
        yield return new WaitForSeconds(0f);
    }
}
