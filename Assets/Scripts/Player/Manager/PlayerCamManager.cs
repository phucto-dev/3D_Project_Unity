using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerCamManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera _lockOnCamera;
    [SerializeField] private Transform _camLockOnPivot;
    [SerializeField] private float _lockOnRadius = 15f;
    [SerializeField] private float _lockOnRadiusOffset = 2f;
    [SerializeField] private float _camRotationSpeed = 15f;

    [Header("Settings")]
    [SerializeField] private LayerMask _enemyLayer;
    public bool IsLockedOn { get; private set; }

    private const string _enemyTag = TagConstant.Enemy_Tag;
    private Vector3 _detecionRange;
    public event Action<Transform> SentLockOnTarget;
    private Transform _currentTarget;
    private PlayerInput _inputSystem;
    private InputAction _lockOnCameraAction;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInput>();
        if (_inputSystem)
        {
            _lockOnCameraAction = _inputSystem.actions["TargetLock"];
        }
    }

    private void OnEnable()
    {
        _lockOnCameraAction.performed += OnTargetLockInput;
    }
    private void OnDisable()
    {
        _lockOnCameraAction.performed -= OnTargetLockInput;
    }

    private void Update()
    {
        if (IsLockedOn && _currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, _currentTarget.position);
            if (distance > _lockOnRadius + _lockOnRadiusOffset || !_currentTarget.gameObject.activeInHierarchy)
            {
                SentLockOnTarget?.Invoke(ClearTarget());
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsLockedOn || _currentTarget == null) return;
        if (_camLockOnPivot != null)
        {
            Vector3 dirToEnemy = _currentTarget.position - _camLockOnPivot.position;
            dirToEnemy.y += 1.0f;
            Quaternion targetCamRotation = Quaternion.LookRotation(dirToEnemy);

            _camLockOnPivot.rotation = Quaternion.Slerp(_camLockOnPivot.rotation, targetCamRotation, _camRotationSpeed * Time.deltaTime);
        }
    }

    public void OnTargetLockInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ToggleLockOn();
        }
    }

    private void ToggleLockOn()
    {
        if (IsLockedOn)
        {
            SentLockOnTarget?.Invoke(ClearTarget());
        }
        else
        {
            SentLockOnTarget?.Invoke(FindLockOnTarget());
        }
    }

    private Transform FindLockOnTarget()
    {
        _detecionRange = transform.position + transform.forward * _lockOnRadiusOffset;
        Collider[] targets = Physics.OverlapSphere(_detecionRange, _lockOnRadius, _enemyLayer);

        if (targets.Length == 0) return null;

        float nearestTargetDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (Collider target in targets)
        {
            if (target.CompareTag(_enemyTag))
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance < nearestTargetDistance)
                {
                    nearestTargetDistance = distance;
                    nearestTarget = target.transform;
                } 
            }
        }
        if (nearestTarget != null)
        {
            _currentTarget = nearestTarget;
            IsLockedOn = true;
            _lockOnCamera.Priority = 20;

            return _currentTarget;
        }

        return null;
    }

    private Transform ClearTarget()
    {
        IsLockedOn = false;
        _currentTarget = null;
        _lockOnCamera.Priority = 0;
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        _detecionRange = transform.position + transform.forward * _lockOnRadiusOffset;
        Gizmos.DrawWireSphere(_detecionRange, _lockOnRadius);
    }

    public Transform GetCurrentTarget() => _currentTarget;

}
