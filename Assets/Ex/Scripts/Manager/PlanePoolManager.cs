using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Pool;

public class PlanePoolManager : MonoBehaviour
{
    public static PlanePoolManager Instance { get; private set; }

    // Dùng Instance ID của Prefab làm Key để tra cứu cực nhanh (O(1))
    private Dictionary<int, IObjectPool<PlaneProjectile>> _projectilePools = new Dictionary<int, IObjectPool<PlaneProjectile>>();
    private Dictionary<int, IObjectPool<PlanePoolVFX>> _vfxPools = new Dictionary<int, IObjectPool<PlanePoolVFX>>();
    private void Awake()
    {
        // Singleton cốt lõi: Đảm bảo chỉ có 1 PlanePoolManager tồn tại trên toàn bộ Game
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Giữ tồn tại khi chuyển Scene
    }

    /// <summary>
    /// Lấy ra (hoặc tạo mới) một Pool quản lý loại đạn yêu cầu
    /// </summary>
    public IObjectPool<PlaneProjectile> GetProjectilePool(PlaneProjectile prefab, int defaultCapacity = 150, int maxSize = 250)
    {
        int key = prefab.gameObject.GetInstanceID();

        // Trả về Pool nếu đã tồn tại
        if (_projectilePools.TryGetValue(key, out var existingPool))
        {
            return existingPool;
        }

        // Tạo Pool mới nếu chưa có
        IObjectPool<PlaneProjectile> newPool = new ObjectPool<PlaneProjectile>(
            createFunc: () =>
            {
                PlaneProjectile instance = Instantiate(prefab);
                // Giải quyết rủi ro PRO: Gắn viên đạn vào PlanePoolManager để không bị hủy khi qua màn
                instance.transform.SetParent(this.transform);
                return instance;
            },
            actionOnGet: obj =>
            {
                // Tách ra khỏi PlanePoolManager khi bay để không ảnh hưởng local transform
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
            },
            actionOnRelease: obj =>
            {
                obj.gameObject.SetActive(false);
                // Thu hồi lại vào trong lòng PlanePoolManager khi không dùng
                obj.transform.SetParent(this.transform);
            },
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _projectilePools.Add(key, newPool);
        return newPool;
    }

    public IObjectPool<PlanePoolVFX> GetVFXPool(PlanePoolVFX prefab)
    {
        int key = prefab.gameObject.GetInstanceID();

        if (_vfxPools.TryGetValue(key, out var existingPool))
        {
            return existingPool;
        }

        IObjectPool<PlanePoolVFX> newPool = new ObjectPool<PlanePoolVFX>(
            createFunc: () =>
            {
                PlanePoolVFX instance = Instantiate(prefab);
                instance.transform.SetParent(this.transform); // Đưa vào làm con của PoolManager để giữ qua màn
                return instance;
            },
            actionOnGet: obj =>
            {
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
            },
            actionOnRelease: obj =>
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(this.transform);
            },
            actionOnDestroy: Destroy
        );

        _vfxPools.Add(key, newPool);
        return newPool;
    }
}
