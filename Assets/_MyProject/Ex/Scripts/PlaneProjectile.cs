using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlaneProjectile : MonoBehaviour
{
    private float _damage;
    private float _speed;
    private float _lifeTime = 3f;
    private float _currentLifeTimer;
    private PlanePoolVFX _hitVFXPrefab;
    private IObjectPool<PlaneProjectile> _managedPool;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void Initialize(PlaneWeapon weaponData, IObjectPool<PlaneProjectile> pool, Vector3 direction)
    {
        _damage = weaponData.Damage;
        _speed = weaponData.ProjectileSpeed;
        _managedPool = pool;
        _hitVFXPrefab = weaponData.HitVFXPrefab;
        _currentLifeTimer = _lifeTime;
        _rb.linearVelocity = direction * _speed;
    }

    private void Update()
    {
        _currentLifeTimer -= Time.deltaTime;
        if (_currentLifeTimer <= 0)
        {
            ReleaseToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit Target: " + _damage);
        }
        if (_hitVFXPrefab != null)
        {
            IObjectPool<PlanePoolVFX> vfxPool = PlanePoolManager.Instance.GetVFXPool(_hitVFXPrefab);
            PlanePoolVFX vfxInstance = vfxPool.Get();

            vfxInstance.transform.position = transform.position;

            vfxInstance.Initialize(vfxPool, 2f);
        }
        ReleaseToPool();
    }

    private void ReleaseToPool()
    {
        if (gameObject.activeSelf)
        {
            _rb.linearVelocity = Vector3.zero;
            _managedPool.Release(this);
        }
    }
}
