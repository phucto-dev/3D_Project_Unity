using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class MeleeTracer : MonoBehaviour
{
    [Header("--- WEAPON STATS ---")]
    public WeaponDataSO _weaponStats;

    [Header("--- TRACER SETTINGS ---")]
    [SerializeField] private Transform[] _damagePoints;
    [SerializeField] private float _hitboxRadius;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("--- SOUND SETTINGS ---")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _slashs;
    [SerializeField] private SoundConfigSO _slashHit;

    private bool _isAttacking;
    private bool _hitOnce;
    private Vector3[] _previousPos;
    private HealthSystem enemyHealth;
    private DmgInfo _dmgInfo;
    private PlayerStatsManager _stats;
    private PlayerAttack _playerAttack;
    private PlayerManager _playerManager;
    private Animator _animator;
    private TrailRenderer _trail;

    private void Awake()
    {
        //_stats = GetComponentInParent<PlayerStatsManager>();
        //_playerAttack = GetComponentInParent<PlayerAttack>();
        //_playerManager = GetComponentInParent<PlayerManager>();
        _trail = transform.parent.GetComponentInChildren<TrailRenderer>();
        //if (_playerAttack != null)
        //{
        //    _animator = _playerAttack.GetComponentInChildren<Animator>();
        //}
    }

    private void OnEnable()
    {
        //if (_playerManager != null)
        //{
        //    _playerManager.BeingHit += StopSwing;
        //}
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
        if (_trail != null) _trail.enabled = false;
        if (_stats == null) return;
        _dmgInfo = new DmgInfo
        {
            Amount = _stats.AttackPower.GetValue(),
            PoiseDamage = _stats.PoiseDamage.GetValue(),
            Dealer = _stats.transform,
            IsCritical = false
        };
    }

    public void Initialize(PlayerStatsManager ownerStats, PlayerAttack playerAttack, PlayerManager playerManager, Animator animator)
    {
        _stats = ownerStats;
        _playerAttack = playerAttack;
        _playerManager = playerManager;
        _animator = animator;
        if (_playerManager != null)
        {
            _playerManager.BeingHit += StopSwing;
        }
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
        if (_trail != null) _trail.enabled = true;
        PlaySlash();
    }

    public void StopSwing()
    {
        _isAttacking = false;
        _hitOnce = false;
        enemyHealth = null;
        if (_trail != null) _trail.enabled = false;
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
                HandleHit(hit);
                _hitOnce = true;
                break;
            }
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        if (hit.IsUnityNull()) return;
        Collider enemyCollider = hit.collider;

        if (enemyCollider.TryGetComponent<HealthSystem>(out HealthSystem targetHealth))
        {
            targetHealth.TakeDmg(_dmgInfo);

            Animator enemyAnimator = enemyCollider.transform.parent.GetComponentInChildren<Animator>();
            Debug.Log("enemy Anim: " + enemyAnimator.name);

            if (_playerAttack != null)
            {
                _playerAttack.RecoverManaOnHit();
                string vfxID = _playerAttack.GetVFXID();
                GameObject vfx = PoolManager.Instance.Get(vfxID);
                if (vfx != null)
                {
                    vfx.transform.position = hit.point;

                    if (hit.normal != Vector3.zero)
                    {
                        vfx.transform.rotation = Quaternion.LookRotation(hit.normal);
                    }
                    VFXPool vfxPool = vfx.GetComponentInChildren<VFXPool>();
                    if (vfxPool != null)
                    {
                        vfxPool.Setup(vfxID);
                        AudioManager.Instance.PlaySFX(_slashHit, hit.point);
                    }
                }
            }

            HitStopManager.Instance.TriggerHitStop(_weaponStats.HitStopDuration, _animator, enemyAnimator);
        }
    }

    private void CalDamageInfo()
    {
        if (_weaponStats == null) return;
        _dmgInfo.Amount = _stats.AttackPower.GetValue() + _weaponStats.MainStat.Value;
        _dmgInfo.PoiseDamage = _stats.PoiseDamage.GetValue() + _weaponStats.PoiseDamage;
    }

    public void PlaySlash()
    {
        if (_slashs.Length == 0)
            return;

        int index = Random.Range(0, _slashs.Length);

        _audioSource.PlayOneShot(
            _slashs[index],
            0.5f
        );
    }
}
