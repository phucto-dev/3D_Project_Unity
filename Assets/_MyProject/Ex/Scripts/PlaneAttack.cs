using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlaneAttack : MonoBehaviour
{
    [Header("Current Equipment")]
    public PlaneWeapon CurrentWeapon;

    [Header("Fire Points")]
    public Transform CenterFirePoint;
    public Transform LeftFirePoint;
    public Transform RightFirePoint;

    private PlayerInput _inputSystem;
    private InputAction _fireAction;

    private float _nextFireTime = 0f;
    private bool _isFiring;
    private bool _fireLeftNext = true;

    private int _currentAmmo;
    private bool _isReloading = false;
    private Coroutine _reloadCoroutine;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInput>();
        if (_inputSystem != null)
        {
            _fireAction = _inputSystem.actions["Attack"];
        }
    }

    private void OnEnable()
    {
        if (_fireAction != null)
        {
            _fireAction.started += ctx => _isFiring = true;
            _fireAction.canceled += ctx => _isFiring = false;
        }
    }

    private void OnDisable()
    {
        if (_fireAction != null)
        {
            _fireAction.started -= ctx => _isFiring = true;
            _fireAction.canceled -= ctx => _isFiring = false;
        }
    }
    private void Start()
    {
        if (CurrentWeapon != null)
        {
            _currentAmmo = CurrentWeapon.AmmoCapacity;
        }
    }

    private void Update()
    {
        if (CurrentWeapon == null || _isReloading) return;

        if (_currentAmmo <= 0)
        {
            _reloadCoroutine = StartCoroutine(ReloadRoutine());
            return;
        }

        if (_isFiring && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + CurrentWeapon.FireRate;
        }
    }

    public void EquipWeapon(PlaneWeapon newWeapon)
    {
        CurrentWeapon = newWeapon;

        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
        }
        _isReloading = false;
        _currentAmmo = CurrentWeapon.AmmoCapacity;
    }

    private void Shoot()
    {
        // GỌI SINGLETON TẠI ĐÂY
        IObjectPool<PlaneProjectile> currentPool = PlanePoolManager.Instance.GetProjectilePool(CurrentWeapon.ProjectilePrefab);

        if (CurrentWeapon.GunCount == 1)
        {
            SpawnProjectile(currentPool, CenterFirePoint);
            _currentAmmo--;
        }
        else if (CurrentWeapon.GunCount == 2)
        {
            //Transform firePoint = _fireLeftNext ? LeftFirePoint : RightFirePoint;
            SpawnProjectile(currentPool, LeftFirePoint);
            SpawnProjectile(currentPool, RightFirePoint);
            //_fireLeftNext = !_fireLeftNext;
            _currentAmmo--;
            _currentAmmo--;
        }
    }

    private void SpawnProjectile(IObjectPool<PlaneProjectile> pool, Transform spawnPoint)
    {
        PlaneProjectile bullet = pool.Get();

        // Đặt vị trí, góc và khởi tạo thông số
        bullet.transform.position = spawnPoint.position;
        bullet.transform.rotation = spawnPoint.rotation;
        bullet.Initialize(CurrentWeapon, pool, spawnPoint.forward);

        if (CurrentWeapon.MuzzleFlashVFX != null)
        {
            Instantiate(CurrentWeapon.MuzzleFlashVFX, spawnPoint.position, spawnPoint.rotation, spawnPoint); // Lưu ý: VFX cũng có thể pool nếu bạn muốn tối ưu sâu hơn
        }
    }

    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        Debug.Log($"Reloading: {CurrentWeapon.ReloadTime}s...");

        yield return new WaitForSeconds(CurrentWeapon.ReloadTime);
        _currentAmmo = CurrentWeapon.AmmoCapacity;
        _isReloading = false;

        Debug.Log($"Done! Current Ammo: {_currentAmmo}");
    }
}
