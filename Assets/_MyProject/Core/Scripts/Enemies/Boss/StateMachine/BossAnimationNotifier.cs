using Unity.VisualScripting;
using UnityEngine;

public class BossAnimationNotifier : StateMachineBehaviour
{
    private BossStateManager _boss;
    [Range(0, 1)] public float TriggerTime = 0.5f;
    private bool _hasTriggered;
    private bool _isEnded;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_boss == null)
        {
            _boss = animator.GetComponentInParent<BossStateManager>();
        }
        _hasTriggered = false;
        _isEnded = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(layerIndex))
        {
            return;
        }

        float time = stateInfo.normalizedTime % 1.0f;

        if (!_hasTriggered && time >= TriggerTime)
        {
            Debug.Log("Triggg: " + stateInfo.normalizedTime);
            _hasTriggered = true;
            _boss?.OnActionTriggered();
        }

        if (!_isEnded && time >= 0.99f)
        {
            _isEnded = true;
            _boss?.OnAnimationEnded();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_boss != null && !_isEnded)
        {
            _boss.OnAnimationEnded();
        }
    }
}
