using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("--- REF ---")]
    public Transform attackPoint;
    public EnemyStats Stats;
    public LayerMask PlayerLayer;

    [Header("--- ANGLE ---")]
    private float _attackAngle = 90f;

    private DmgInfo _dmgInfo;

    private void Start()
    {
        if (Stats == null) return;
        _dmgInfo = new DmgInfo
        {
            Amount = Stats.AttackPower.BaseValue,
            Dealer = this.transform,
            IsCritical = false
        };
    }

    public void TriggerAttackHitBox()
    {
        if (attackPoint == null) return;
        if (Stats == null) return;
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, Stats.AttackRange.BaseValue, PlayerLayer);

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
                    Debug.Log("Hit Player");
                    HealthSystem playerHealth = hit.GetComponent<HealthSystem>();

                    if (playerHealth != null)
                    {
                        Debug.Log("Deal");
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        if (Stats == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, Stats.AttackRange.BaseValue);

        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -_attackAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, _attackAngle / 2f, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(attackPoint.position, leftBoundary * Stats.AttackRange.BaseValue);
        Gizmos.DrawRay(attackPoint.position, rightBoundary * Stats.AttackRange.BaseValue);
    }
}
