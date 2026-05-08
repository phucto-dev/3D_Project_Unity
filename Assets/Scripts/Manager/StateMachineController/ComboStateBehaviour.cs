using UnityEngine;

public class ComboStateBehaviour : StateMachineBehaviour
{
    [SerializeField, Range(0f, 1f)] private float _openComboWindowTime = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _transitionToNextAttack = 0.9f;
    [SerializeField, Range(0f, 1f)] private float _closeComboWindowTime = 0.95f;

    private PlayerAttack _playerAttack;
    private MeleeTracer _tracer;
    private float _swingTiming = 0f;
    private bool _hasTransitioned;
    private bool _isAttacking;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerAttack = animator.GetComponentInParent<PlayerAttack>();
        _tracer = animator.GetComponentInChildren<MeleeTracer>();
        _hasTransitioned = false;
        _isAttacking = false;

        if (_playerAttack != null)
        {
            _swingTiming = _playerAttack.GetCurrentAttackNode().SwingTiming;
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerAttack == null) return;
        if (animator.IsInTransition(layerIndex)) return;
        if (stateInfo.normalizedTime >= _swingTiming)
        {
            if (!_isAttacking)
            {
                if (_tracer != null)
                {
                    _tracer.StartSwing();
                }
                _isAttacking = true;
            }
        }
        if (stateInfo.normalizedTime >= _openComboWindowTime)
        {
            _playerAttack.OpenComboWindow();
        }
        if (!_hasTransitioned && stateInfo.normalizedTime >= _transitionToNextAttack)
        {
            _hasTransitioned = true;
            if (_isAttacking)
            {
                if (_tracer != null)
                {
                    _tracer.StopSwing();
                }
                _isAttacking = false;
            }
            _playerAttack.TryExecuteBufferedAttack();
        }
        if (stateInfo.normalizedTime >= _closeComboWindowTime)
        {
            _playerAttack.RecoveryAnimAndEndState();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (_playerAttack == null) return;
        //_playerAttack.ResetCombatState();
    }
}
