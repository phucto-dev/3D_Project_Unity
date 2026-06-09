using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("--- REF ---")]
    public Transform attackPoint;
    public LayerMask PlayerLayer;

    [Header("--- ANGLE ---")]
    private float _attackAngle = 90f;

    [Header("--- SETTINGS FOR RANGE ---")]
    public PoolItemSO ProjectilePoolInfo;
    public PlayerInfo Player;
    public float ProjectileSpeed = 20f;

    private DmgInfo _dmgInfo;
    private EnemyStatsManager _stats;

    private void Awake()
    {
        _stats = GetComponentInParent<EnemyStatsManager>();
    }

    private void Start()
    {
        if (_stats == null) return;
        _dmgInfo = new DmgInfo
        {
            Amount = _stats.AttackPower.GetValue(),
            Dealer = this.transform,
            PoiseDamage = _stats.PoiseDamage.GetValue(),
            IsCritical = false
        };
    }

    // for melee
    public void TriggerAttackHitBox()
    {
        if (attackPoint == null) return;
        if (_stats == null) return;
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, _stats.AttackRange.GetValue(), PlayerLayer);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;
            else
            {
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                dirToTarget.y = 0;
                Vector3 forward = transform.forward;
                forward.y = 0;

                if (Vector3.Angle(forward, dirToTarget) <= _attackAngle/2)
                {
                    HealthSystem playerHealth = hit.GetComponent<HealthSystem>();

                    if (playerHealth != null)
                    {
                        Debug.Log("Deal");
                        SetCurrentDmgInfo();
                        playerHealth.TakeDmg(_dmgInfo);
                    }

                    break;
                }
                else
                {
                    Debug.Log("Player is out of range");
                }
            }
        }
    }

    // range
    public void TriggerFireAttack()
    {
        if (attackPoint == null) return;
        if (_stats == null) return;
        if (Player == null) return;

        GameObject projectile = PoolManager.Instance.Get(ProjectilePoolInfo.poolID);
        if (projectile == null) return;
        projectile.transform.position = attackPoint.position;

        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        if (projectileController != null)
        {
            ProjectileData data = new ProjectileData();
            data.PoolID = ProjectilePoolInfo.poolID;
            data.Speed = ProjectileSpeed;
            data.Damage = _stats.AttackPower.GetValue();
            data.PoiseDamage = _stats.PoiseDamage.GetValue();
            data.IsAttackProjectile = true;
            data.TargetLayer = PlayerLayer;
            data.MoveType = ProjectileMoveType.Straight;
            data.TargetDirection = Player.PlayerTransform.position;
            projectileController.Fire(data);
        }
    }

    private void SetCurrentDmgInfo()
    {
        _dmgInfo.Amount = _stats.AttackPower.GetValue();
        _dmgInfo.PoiseDamage = _stats.PoiseDamage.GetValue();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        if (_stats == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, _stats.AttackRange.GetValue());

        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -_attackAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, _attackAngle / 2f, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(attackPoint.position, leftBoundary * _stats.AttackRange.GetValue());
        Gizmos.DrawRay(attackPoint.position, rightBoundary * _stats.AttackRange.GetValue());
    }
}
