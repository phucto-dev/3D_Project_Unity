using System.Collections;
using UnityEngine;

public class BossStatueSpawner : MonoBehaviour
{
    [Header("--- POOL INFO ---")]
    public PoolItemSO PoolInfo;

    [Header("--- SUMMON SETTINGS ---")]
    public float HeightOffset;
    public float RisingTime;

    private GameObject _statue;
    private BossCombatInfo _combatInfo;
    private BossStatsManager _stats;
    private void OnEnable()
    {
        _statue = null;
    }
    public void Activate(BossCombatInfo info, BossStatsManager stats)
    {
        _combatInfo = info;
        _stats = stats;
        if (PoolInfo == null) return;
        _statue = PoolManager.Instance.Get(PoolInfo.poolID);
        if (_statue == null) return;
        StartCoroutine(RiseStatueRoutine());
    }
    public void DeActivate()
    {
        if (_statue == null) return;
        BossStatue script = _statue.GetComponent<BossStatue>();
        if (script != null) script.DeActivate();
        PoolManager.Instance.Release(PoolInfo.poolID, _statue);
        _statue = null;
    }
    private IEnumerator RiseStatueRoutine()
    {
        Vector3 endPos = transform.position;
        Vector3 startPos = new Vector3(endPos.x, endPos.y - HeightOffset, endPos.z);

        _statue.transform.position = startPos;
        _statue.transform.rotation = transform.rotation;

        float timer = 0f;
        while (timer < RisingTime)
        {
            timer += Time.deltaTime;

            float t = timer / RisingTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            _statue.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        _statue.transform.position = endPos;
        BossStatue script = _statue.GetComponent<BossStatue>();
        if (script != null) script.Activate(_combatInfo, _stats);
    }
}
