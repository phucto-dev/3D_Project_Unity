using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerAttack : MonoBehaviour
{
    [Header("--- INPUT ---")]
    [SerializeField] private AttackNodeSO _baseEntryLightAttack;
    [SerializeField] private AttackNodeSO _baseEntryHeavyAttack;
    [SerializeField] private AttackNodeSO _entryLightAttack;
    [SerializeField] private AttackNodeSO _entryHeavyAttack;
    [SerializeField] private Animator _animator;

    private PlayerInput _inputSystem;
    private InputAction _attackAction;

    private AttackNodeSO _currentAttackNode;
    private bool _isComboWindowOpen = false;
    private int _attackLayerIndex;
    private AttackNodeSO _bufferedAttackNode;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInput>();
        _playerMovement = GetComponent<PlayerMovement>();
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        _attackLayerIndex = _animator.GetLayerIndex("Attack");
        if (_inputSystem)
        {
            _attackAction = _inputSystem.actions["Attack"];
        }
    }

    private void OnEnable()
    {
        if (_attackAction == null) return;
        _attackAction.performed += PerformAttack;
    }
    private void OnDisable()
    {
        if (_attackAction == null) return;
        _attackAction.performed -= PerformAttack;
    }

    private void Start()
    {

    }

    public void PerformAttack(InputAction.CallbackContext ctx)
    {
        if (_attackLayerIndex == -1) return;
        if (_playerMovement != null)
        {
            if (_playerMovement.GetRollInfo()) return;
            if (!_playerMovement.GetIsGround()) return;
        }
        if (ctx.interaction is HoldInteraction)
        {
            OnHeavyAttack();
        }
        if (ctx.interaction is TapInteraction)
        {
            OnLightAttack();
        }
        _animator.SetLayerWeight(_attackLayerIndex, 1f);
    }

    public void OnLightAttack()
    {
        if (_currentAttackNode == null)
        {
            ExecuteAttack(_entryLightAttack == null ? _baseEntryLightAttack : _entryLightAttack);
            return;
        }
        else
        {
            if (_isComboWindowOpen && _currentAttackNode.NextLightAttack != null)
            {
                _bufferedAttackNode = _currentAttackNode.NextLightAttack;
            }
        }
    }

    public void OnHeavyAttack()
    {
        if (_currentAttackNode == null)
        {
            ExecuteAttack(_entryHeavyAttack == null ? _baseEntryHeavyAttack : _entryHeavyAttack);
            return;
        }
        else
        {
            if (_isComboWindowOpen && _currentAttackNode.NextHeavyAttack != null)
            {
                _bufferedAttackNode = _currentAttackNode.NextHeavyAttack;
            }
        }
    }

    private void ExecuteAttack(AttackNodeSO targetNode)
    {
        _currentAttackNode = targetNode;
        _isComboWindowOpen = false;
        _bufferedAttackNode = null;
        _animator.CrossFadeInFixedTime(targetNode.AnimStateName, targetNode.TransitionDuaration);
    }

    public void TryExecuteBufferedAttack()
    {
        if (_bufferedAttackNode != null)
        {
            ExecuteAttack(_bufferedAttackNode);
        }
    }
    public void OpenComboWindow()
    {
        _isComboWindowOpen = true;
    }

    public void ResetCombatState()
    {
        _currentAttackNode = null;
        _isComboWindowOpen = false;
        _animator.SetLayerWeight(_attackLayerIndex, 0f);
    }

    public void ClearEquip()
    {
        _entryLightAttack = null;
        _entryHeavyAttack = null;
    }

    public AttackNodeSO GetCurrentAttackNode() => _currentAttackNode;
}
