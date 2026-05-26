using UnityEngine;
using UnityEngine.Pool;

public class PlanePoolVFX : MonoBehaviour
{
    private IObjectPool<PlanePoolVFX> _managedPool;

    // Hàm này được gọi khi lấy VFX ra từ kho
    public void Initialize(IObjectPool<PlanePoolVFX> pool, float lifeTime = 2f)
    {
        _managedPool = pool;

        // Dùng Invoke để hẹn giờ thu hồi tự động theo thời lượng của hiệu ứng nổ
        Invoke(nameof(ReleaseToPool), lifeTime);
    }

    private void ReleaseToPool()
    {
        if (gameObject.activeSelf)
        {
            _managedPool.Release(this);
        }
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ReleaseToPool));
    }
}
