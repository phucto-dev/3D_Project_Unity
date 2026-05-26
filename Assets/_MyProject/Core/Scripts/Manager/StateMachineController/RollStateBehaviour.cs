using Unity.VisualScripting;
using UnityEngine;

public class RollStateBehaviour : StateMachineBehaviour
{
    [SerializeField, Range(0, 1)] private float _iframeStartTime;
    [SerializeField, Range(0, 1)] private float _iframeEndTime;

    private HealthSystem _healthSystem;
    private bool _hasStarted;
    private bool _hasStopped;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _healthSystem = animator.transform.parent.GetComponentInChildren<HealthSystem>();
        _hasStarted = false;
        _hasStopped = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_healthSystem == null)
        {
            Warning.Error("RollStateManager: Can't find HealthSystem");
            return;
        }

        if (stateInfo.normalizedTime >= _iframeStartTime && !_hasStarted)
        {
            _healthSystem.AddInvincibility();
            _hasStarted = true;
        }
        if (stateInfo.normalizedTime >= _iframeEndTime && !_hasStopped)
        {
            _healthSystem.RemoveInvincibility();
            _hasStopped = true;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_hasStarted && !_hasStopped && _healthSystem != null)
        {
            _healthSystem.RemoveInvincibility();
            Debug.Log("remove");
        }
    }
}
