using UnityEngine;

public class MeleeTracer : MonoBehaviour
{
    [Header("--- TRACER SETTINGS")]
    [SerializeField] private Transform[] _damagePoints;
    [SerializeField] private float _hitboxRadius;
    [SerializeField] private LayerMask _enemyLayer;

    private bool _isAttacking;
    private bool _hitOnce;
    private Vector3[] _previousPos;
    private HealthSystem enemyHealth;
    private DmgInfo _dmgInfo;
    private PlayerStatsManager _stats;

    private void Awake()
    {
        _stats = GetComponentInParent<PlayerStatsManager>();
    }

    private void Start()
    {
        _previousPos = new Vector3[_damagePoints.Length];
        if (_stats == null) return;
        _dmgInfo = new DmgInfo
        {
            Amount = _stats.AttackPower.GetValue(),
            Dealer = _stats.transform,
            IsCritical = false
        };
    }

    public void StartSwing()
    {
        _isAttacking = true;
        int i = 0;
        foreach (Transform damagePoint in _damagePoints)
        {
            _previousPos[i] = damagePoint.position;
            i++;
        }
    }

    public void StopSwing()
    {
        _isAttacking = false;
        _hitOnce = false;
        enemyHealth = null;
    }

    private void Update()
    {
        if (!_isAttacking) return;
        if (_hitOnce) return;
        for (int i = 0; i < _damagePoints.Length; i++)
        {
            Vector3 currentPos = _damagePoints[i].position;
            Vector3 prevPos = _previousPos[i];

            float distance = Vector3.Distance(currentPos, prevPos);
            Vector3 dir = (currentPos - prevPos).normalized;

            _previousPos[i] = currentPos;

            if (Physics.SphereCast(prevPos, _hitboxRadius, dir, out RaycastHit hit, distance, _enemyLayer))
            {
                HandleHit(hit.collider);
                _hitOnce = true;
                break;
            }

        }
    }

    private void HandleHit(Collider enemyCollider)
    {
        Debug.Log("Hit");
        if (enemyHealth == null)
        {
            enemyHealth = enemyCollider.GetComponent<HealthSystem>();
        }
        if (enemyHealth != null)
        {
            enemyHealth.TakeDmg(_dmgInfo);
        }
    }
}
