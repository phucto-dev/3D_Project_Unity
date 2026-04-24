using UnityEngine;

public class ComboStateBehaviour : StateMachineBehaviour
{
    [SerializeField, Range(0f, 1f)] private float _openComboWindowTime = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _transitionToNextAttack = 0.9f;
    [SerializeField, Range(0f, 1f)] private float _closeComboWindowTime = 0.95f;

    private PlayerAttack _playerAttack;
    private bool _hasTransitioned;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerAttack = animator.GetComponentInParent<PlayerAttack>();
        _hasTransitioned = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerAttack == null) return;
        if (animator.IsInTransition(layerIndex)) return;
        if (stateInfo.normalizedTime >= _openComboWindowTime)
        {
            _playerAttack.OpenComboWindow();
        }
        if (!_hasTransitioned && stateInfo.normalizedTime >= _transitionToNextAttack)
        {
            _hasTransitioned = true;
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
