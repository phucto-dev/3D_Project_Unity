using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    private Animator _animator;
    private EnemyStateManager _stateManager;
    private EnemyStatsManager _stats;

    [Header("Animator Hashes")]
    private readonly int _animVelocity = Animator.StringToHash("Velocity");
    private readonly int _animMove = Animator.StringToHash("IsMoving");
    private readonly int _animInCombat = Animator.StringToHash("IsInCombat");
    private readonly int _animHaste = Animator.StringToHash("Haste");

    private float _walkSpeed;
    private float _runSpeed;
    private bool _isWalking;
    private bool _isRunning;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _stateManager = GetComponent<EnemyStateManager>();
        _stats = GetComponent<EnemyStatsManager>();
    }
    private void Start()
    {
        if (_stats == null) return;
        _walkSpeed = _stats.WalkSpeed.GetValue();
        _runSpeed = _stats.RunSpeed.GetValue();
    }
    private void Update()
    {
        if (_animator == null) return;
        if (_stateManager == null) return;
        if (_stats == null) return;

        VelocityCheck();
    }

    private void VelocityCheck()
    {
        float speed = _stateManager.Agent.velocity.magnitude;
        float blendValue = 0f;
        if (speed <= _walkSpeed)
        {
            // 0 -> 0.5
            blendValue = Mathf.InverseLerp(0f, _walkSpeed, speed) * 0.5f;
        }
        else
        {
            // 0.5 -> 1
            blendValue = 0.5f + (Mathf.InverseLerp(_walkSpeed, _runSpeed, speed) * 0.5f);
        }
        _animator.SetFloat(_animVelocity, blendValue);
    }
    public AnimatorStateInfo GetStateInfo() => _animator.GetCurrentAnimatorStateInfo(0);
    public void EnableMovingAnim()
    {
        _animator.SetBool(_animMove, true);
    }
    public void DisableMovingAnim()
    {
        _animator.SetBool(_animMove, false);
    }
    public void EnableCombatAnim()
    {
        _animator.SetBool(_animInCombat, true);
    }
    public void DisableCombatAnim()
    {
        _animator.SetBool(_animInCombat, false);
    }

    public void DoARandomAttack(string name, float haste)
    {
        _animator.SetFloat(_animHaste, haste);
        _animator.CrossFade(name, 0.1f);
    }
    public void DoTargetAnim(string name)
    {
        _animator.CrossFade(name, 0.1f);
    }
}
