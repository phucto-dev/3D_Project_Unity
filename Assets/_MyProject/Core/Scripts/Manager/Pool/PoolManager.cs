using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    [Header("--- POOL SETTINGS ---")]
    [SerializeField] private PoolConfigSO _poolConfigSO;

    private Dictionary<string, ObjectPool<GameObject>> _pools = new Dictionary<string, ObjectPool<GameObject>>();
    private Dictionary<string, GameObject> _prefabsDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }
    
    private void InitializePools()
    {
        if (_poolConfigSO == null) return;

        foreach (PoolEntityConfig item in _poolConfigSO.poolItems)
        {
            if (_pools.ContainsKey(item.poolID)) continue;

            _prefabsDict[item.poolID] = item.prefab;

            ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
                createFunc: () => InstaintiateItem(item.poolID),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: item.defaultCapacity,
                maxSize: item.maxSize
            );

            _pools.Add(item.poolID, newPool);

            List<GameObject> prewarmList = new List<GameObject>();
            for (int i = 0; i < item.defaultCapacity; i++)
            {
                prewarmList.Add(newPool.Get());
            }
            foreach (var obj in prewarmList)
            {
                newPool.Release(obj);
            }
        }
    }

    private GameObject InstaintiateItem(string poolID)
    {
        GameObject obj = Instantiate(_prefabsDict[poolID], transform);

        PoolObject poolObj = obj.AddComponent<PoolObject>();
        poolObj.Setup(poolID);

        return obj;
    }

    public GameObject Get(string poolID)
    {
        if (_pools.TryGetValue(poolID, out var pool))
        {
            return pool.Get();
        }
        Debug.LogWarning($"Pool with ID {poolID} not found!");
        return null;
    }

    public void Release(string poolID, GameObject obj)
    {
        if (_pools.TryGetValue(poolID, out var pool))
        {
            pool.Release(obj);
        }
    }
}
