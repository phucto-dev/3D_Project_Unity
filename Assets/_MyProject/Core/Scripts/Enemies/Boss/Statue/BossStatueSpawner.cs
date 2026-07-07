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
    private void OnEnable()
    {
        _statue = null;
    }
    public void Activate()
    {
        if (PoolInfo == null) return;
        _statue = PoolManager.Instance.Get(PoolInfo.poolID);
        if (_statue == null) return;
        StartCoroutine(RiseStatueRoutine());
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
        if (script != null) script.Activate();
    }
}
