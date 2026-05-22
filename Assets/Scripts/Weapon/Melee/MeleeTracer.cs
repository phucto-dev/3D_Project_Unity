using UnityEngine;

public class MeleeTracer : MonoBehaviour
{
    [Header("--- WEAPON STATS ---")]
    public WeaponDataSO _weaponStats;

    [Header("--- TRACER SETTINGS ---")]
    [SerializeField] private Transform[] _damagePoints;
    [SerializeField] private float _hitboxRadius;
    [SerializeField] private LayerMask _enemyLayer;

    private bool _isAttacking;
    private bool _hitOnce;
    private Vector3[] _previousPos;
    private HealthSystem enemyHealth;
    private DmgInfo _dmgInfo;
    private PlayerStatsManager _stats;
    private PlayerAttack _playerAttack;
    private PlayerManager _playerManager;
    private Animator _animator;

    private void Awake()
    {
        _stats = GetComponentInParent<PlayerStatsManager>();
        _playerAttack = GetComponentInParent<PlayerAttack>();
        _playerManager = GetComponentInParent<PlayerManager>();
        if (_playerAttack != null)
        {
            _animator = _playerAttack.GetComponentInChildren<Animator>();
        }
    }

    private void OnEnable()
    {
        if (_playerManager != null)
        {
            _playerManager.BeingHit += StopSwing;
        }
    }
    private void OnDisable()
    {
        if (_playerManager != null)
        {
            _playerManager.BeingHit -= StopSwing;
        }
    }

    private void Start()
    {
        _previousPos = new Vector3[_damagePoints.Length];
        if (_stats == null) return;
        _dmgInfo = new DmgInfo
        {
            Amount = _stats.AttackPower.GetValue(),
            PoiseDamage = _stats.PoiseDamage.GetValue(),
            Dealer = _stats.transform,
            IsCritical = false
        };
    }

    public void StartSwing()
    {
        _isAttacking = true;
        CalDamageInfo();
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
            Animator enemyAnimator = enemyCollider.transform.parent.GetComponentInChildren<Animator>();
            Debug.Log("enemy Anim: " + enemyAnimator.name);
            HitStopManager.Instance.TriggerHitStop(_weaponStats.HitStopDuration, _animator, enemyAnimator);
        }
    }

    private void CalDamageInfo()
    {
        if (_weaponStats == null) return;
        _dmgInfo.Amount = _stats.AttackPower.GetValue() + _weaponStats.MainStat.Value;
        _dmgInfo.PoiseDamage = _stats.PoiseDamage.GetValue() + _weaponStats.PoiseDamage;
    }
}
