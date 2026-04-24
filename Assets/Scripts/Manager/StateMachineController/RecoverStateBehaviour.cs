using UnityEngine;

public class RecoverStateBehaviour : StateMachineBehaviour
{
    private PlayerAttack _playerAttack;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerAttack = animator.GetComponentInParent<PlayerAttack>();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerAttack == null) return;
        if (animator.IsInTransition(layerIndex)) return;

        if (stateInfo.normalizedTime >= 0.95f)
        {
            _playerAttack.ResetCombatState();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
