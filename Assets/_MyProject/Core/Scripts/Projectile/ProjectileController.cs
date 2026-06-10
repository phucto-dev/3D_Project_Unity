using UnityEngine;

public enum ProjectileMoveType
{
    Straight,
    Homing
}

public struct ProjectileData
{
    public string PoolID;
    public float Speed;
    public float Damage;
    public float PoiseDamage;
    public bool IsAttackProjectile;
    public LayerMask TargetLayer;
    public ProjectileMoveType MoveType;
    public Vector3 TargetDirection;
    public Transform HomingTarget;
}
public class ProjectileController : MonoBehaviour
{
    public PoolItemSO HitVFX;

    private ProjectileData _data;
    private bool _isFired = false;
    private Vector3 _dirToTarget;

    public void Fire(ProjectileData data)
    {
        _data = data;
        _isFired = true;
        Vector3 playerDir = new Vector3(_data.TargetDirection.x, _data.TargetDirection.y + 1f, _data.TargetDirection.z);
        _dirToTarget = (playerDir - transform.position).normalized;
    }

    private void Update()
    {
        if (!_isFired) return;

        switch (_data.MoveType)
        {
            case ProjectileMoveType.Straight:
                transform.position += _dirToTarget * (_data.Speed * Time.deltaTime);
                break;

            case ProjectileMoveType.Homing:
                if (_data.HomingTarget != null)
                {
                    Vector3 dir = (_data.HomingTarget.position - transform.position).normalized;
                    transform.position += dir * (_data.Speed * Time.deltaTime);
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.isTrigger) return;
        if (!_isFired) return;

        _isFired = false;

        GameObject hitObj = PoolManager.Instance.Get(HitVFX.poolID);

        if (_data.IsAttackProjectile)
        {
            if (((1 << other.gameObject.layer) & _data.TargetLayer) != 0)
            {
                if (other.TryGetComponent(out HealthSystem playerHealth))
                {
                    DmgInfo _dmgInfo = new DmgInfo
                    {
                        Amount = _data.Damage,
                        Dealer = this.transform,
                        PoiseDamage = _data.PoiseDamage,
                        IsCritical = false
                    };
                    playerHealth.TakeDmg(_dmgInfo);
                }
            }
        }

        if (hitObj != null)
        {
            VFXPool vfxScript = hitObj.GetComponent<VFXPool>();
            if (vfxScript != null)
            {
                vfxScript.Setup(HitVFX.poolID);
            }
            hitObj.transform.position = this.transform.position;
        }

        PoolManager.Instance.Release(_data.PoolID, this.gameObject);
    }
}
