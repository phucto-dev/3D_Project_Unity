using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class BossStatue : MonoBehaviour
{
    [Header("--- REF ---")]
    public GameObject SpawnPoint;
    public PoolItemSO SkillVFX;

    [Header("--- SETUP ---")]
    public float SpawnInterval = 2f;
    public int MaxRetries = 5;

    private BoxCollider _spawnArea;
    private Coroutine _current;
    private BossCombatInfo _combatInfo;
    private BossStatsManager _stats;

    private void Awake()
    {
        _spawnArea = GetComponent<BoxCollider>();
    }
    
    private void OnDisable()
    {
        if (_current != null)
        {
            _current = null;
            StopAllCoroutines();
        }
    }
    public void Activate(BossCombatInfo info, BossStatsManager stats)
    {
        _combatInfo = info;
        _stats = stats;
        _current = StartCoroutine(SpawnMeteorRoutine());
    }
    public void DeActivate()
    {
        if (_current != null) StopCoroutine(_current);
        _current = null;
    }
    private IEnumerator SpawnMeteorRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);
            SpawnMeteor();
        }
    }
    private void SpawnMeteor()
    {
        if (_spawnArea == null) return;
        if (SkillVFX == null) return;
        Vector3 validGroundPoint = GetValidSpawnPoint();

        Vector3 finalSpawnPosition = new Vector3(
            validGroundPoint.x,
            _spawnArea.bounds.max.y,
            validGroundPoint.z
        );

        GameObject vfx = PoolManager.Instance.Get(SkillVFX.poolID);
        if (vfx != null)
        {
            vfx.transform.position = finalSpawnPosition;
            PooledObjectMove skillAtack = vfx.GetComponent<PooledObjectMove>();
            if (skillAtack != null) skillAtack.SetUpHitObject(_combatInfo, _stats);
        }
    }

    private Vector3 GetValidSpawnPoint()
    {
        if (_spawnArea == null) return transform.position;
        Bounds bounds = _spawnArea.bounds;

        for (int i = 0; i < MaxRetries; i++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);
            float minY = bounds.min.y;
            Vector3 randomPos = new Vector3(randomX, minY, randomZ);

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return transform.position;
    }
}
