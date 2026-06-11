using System.Collections;
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
    [SerializeField] private float _blendTimeBetweenLayers = 0.2f;

    [Header("Animator Hashes")]
    private readonly int _animRecovery = Animator.StringToHash("Recovery");

    private PlayerInput _inputSystem;
    private InputAction _attackAction;
    private PlayerManager _playerManager;

    private Animator _animator;
    private AttackNodeSO _currentAttackNode;
    private bool _isComboWindowOpen = false;
    private int _attackLayerIndex;
    private AttackNodeSO _bufferedAttackNode;

    private PlayerMovement _playerMovement;
    private Coroutine _fadeLayerCoroutine;

    private bool _isStun = false;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInput>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerManager = GetComponent<PlayerManager>();
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
        if (_playerManager != null)
        {
            _playerManager.BeingHit += EnableBeStun;
            _playerManager.DoneBeingHit += DisableBeStun;
        }
        _attackAction.performed += PerformAttack;
    }
    private void OnDisable()
    {
        if (_attackAction == null) return;
        if (_playerManager != null)
        {
            _playerManager.BeingHit -= EnableBeStun;
            _playerManager.DoneBeingHit -= DisableBeStun;
        }
        _attackAction.performed -= PerformAttack;
    }

    private void Start()
    {

    }

    public void PerformAttack(InputAction.CallbackContext ctx)
    {
        if (_isStun) return;
        if (_attackLayerIndex == -1) return;
        if (_playerMovement != null)
        {
            if (_playerMovement.GetRollInfo()) return;
            if (!_playerMovement.GetIsGround()) return;
        }
        if (_fadeLayerCoroutine != null)
        {
            StopCoroutine(_fadeLayerCoroutine);
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
        _animator.SetBool(_animRecovery, false);
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
    public void RecoveryAnimAndEndState()
    {
        if (_currentAttackNode == null) return;
        if (_currentAttackNode.HasRecoveryAnim)
        {
            _animator.SetBool(_animRecovery, true);
        }
        else
        {
            ResetCombatState();
        }
    }
    public void ResetCombatState()
    {
        _currentAttackNode = null;
        _isComboWindowOpen = false;

        if (_fadeLayerCoroutine != null)
        {
            StopCoroutine(_fadeLayerCoroutine);
        }

        _fadeLayerCoroutine = StartCoroutine(FadeOutAttackLayer(_blendTimeBetweenLayers));
    }

    private IEnumerator FadeOutAttackLayer(float fadeDuration)
    {
        float startWeight = _animator.GetLayerWeight(_attackLayerIndex);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / fadeDuration;

            float currentWeight = Mathf.Lerp(startWeight, 0f, percent);
            _animator.SetLayerWeight(_attackLayerIndex, currentWeight);

            yield return null;
        }

        _animator.SetLayerWeight(_attackLayerIndex, 0f);
        _fadeLayerCoroutine = null;
    }

    public void ClearEquip()
    {
        _entryLightAttack = null;
        _entryHeavyAttack = null;
    }
    public AttackNodeSO GetCurrentAttackNode() => _currentAttackNode;

    public void EnableBeStun()
    {
        _isStun = true;
        ResetCombatState();
    }
    public void DisableBeStun()
    {
        _isStun = false;
    }
    public void SetEntryAttack(AttackNodeSO entryLight, AttackNodeSO entryHeavy)
    {
        _entryLightAttack = entryLight;
        _entryHeavyAttack = entryHeavy;
    }
}
