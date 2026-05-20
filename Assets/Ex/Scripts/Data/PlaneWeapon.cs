using UnityEngine;

[CreateAssetMenu(fileName = "PlaneWeapon", menuName = "Exercise/PlaneWeapon")]
public class PlaneWeapon : ScriptableObject
{
    public string WeaponName;
    public float Damage = 10f;
    public float FireRate = 0.1f;
    public float ProjectileSpeed = 50f;

    [Header("Quantity of guns")]
    [Range(1, 2)]
    public int GunCount = 1;

    [Header("Ammo & Reload")]
    public int AmmoCapacity = 30;
    public float ReloadTime = 2f;

    public PlaneProjectile ProjectilePrefab;
    public GameObject MuzzleFlashVFX;
    public PlanePoolVFX HitVFXPrefab;
}
