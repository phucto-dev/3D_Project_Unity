using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _rollSpeed = 5;
    [SerializeField] private float _rollDuration = 0.96f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _acceleration = 15f;
    [SerializeField] private float _deceleration = 25f;
    [SerializeField] private float _standTime = 0.80f;
    [SerializeField] private float _maxSlopeAngle = 45f;

    [Header("Ref")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Collider _playerCollider;
    [SerializeField] private PhysicsMaterial _slipperyMat;
    [SerializeField] private PhysicsMaterial _brakeMat;

    [Header("GroundCheckSettings")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Animator Hashes")]
    private readonly int _animMove = Animator.StringToHash("IsMoving");
    private readonly int _animHorizontal = Animator.StringToHash("Horizontal");
    private readonly int _animVertical = Animator.StringToHash("Vertical");
    private readonly int _animGround = Animator.StringToHash("IsGround");
    private readonly int _animVerticalVelocity = Animator.StringToHash("VerticalVelocity");
    private readonly int _animJump = Animator.StringToHash("Jump");
    private readonly int _animRoll = Animator.StringToHash("Roll");

    private PlayerInput _inputSystem;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _walkToggleAction;
    private InputAction _lockOnCameraAction;

    private PlayerCamManager playerCamManager;
    private PlayerAttack _playerAttack;
    private PlayerManager _playerManager;
    private PlayerSkill _playerSkill;

    private Vector3 _calculatedMoveDir;
    private Quaternion _targetRotation;
    private Vector2 _moveInput;
    private float _speed;
    private float _currentSpeed;
    private bool _jumpFlag;
    private bool _isMoving;
    private bool _moveable;
    private bool _isSprinting;
    private bool _isWalking;
    private bool _isGrounded;
    private bool _isRolling;
    private Coroutine _rollCoroutine;
    private bool _isLanding;
    private Vector3 _rollDir;
    private float _fallVelocityY;
    private float _landVelocityBaseValue = -5f;
    private float _startStandTime;
    private bool _isCameraLockOn = false;
    private Transform _targetLockOn;
    private bool _isAttacking;
    private Vector2 _lastMoveDir;
    private bool _isStun = false;
    private bool _isUsingSkill = false;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInput>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerManager = GetComponent<PlayerManager>();
        _playerCollider = GetComponent<Collider>();
        _playerSkill = GetComponent<PlayerSkill>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (_inputSystem)
        {
            _moveAction = _inputSystem.actions["Move"];
            _jumpAction = _inputSystem.actions["Jump"];
            _sprintAction = _inputSystem.actions["Sprint"];
            _walkToggleAction = _inputSystem.actions["WalkToggle"];
            _lockOnCameraAction = _inputSystem.actions["TargetLock"];
        }
        playerCamManager = GetComponentInChildren<PlayerCamManager>();
    }

    private void OnEnable()
    {
        if (playerCamManager != null) playerCamManager.SentLockOnTarget += HandleLockOnCam;
        if (_playerManager != null)
        {
            _playerManager.BeingHit += EnableBeStun;
            _playerManager.DoneBeingHit += DisableBeStun;
        }
        if (_playerSkill != null)
        {
            _playerSkill.OnUsingSkill += SetAllowMovement;
        }
        _jumpAction.performed += HandleJumpInput;
        _sprintAction.performed += HandleSprintOrRoll;
        _sprintAction.canceled += HandleSprintStop;
    }
    private void OnDisable()
    {
        if (playerCamManager != null) playerCamManager.SentLockOnTarget -= HandleLockOnCam;
        if (_playerManager != null)
        {
            _playerManager.BeingHit -= EnableBeStun;
            _playerManager.DoneBeingHit -= DisableBeStun;
        }
        if (_playerSkill != null)
        {
            _playerSkill.OnUsingSkill -= SetAllowMovement;
        }
        _jumpAction.performed -= HandleJumpInput;
        _sprintAction.performed -= HandleSprintOrRoll;
        _sprintAction.canceled -= HandleSprintStop;
    }

    private void Start()
    {
        if (animator != null) animator.SetBool("IsLockOnCamera", false);
        _isLanding = true;
        _moveable = true;
        _isAttacking = false;
        _rollCoroutine = null;
    }

    private void Update()
    {
        if (_inputSystem == null) return;
        _moveInput = _moveAction.ReadValue<Vector2>();
        _isMoving = _moveInput != Vector2.zero;

        RotateCharacter();
        AnimationProcess();
    }

    private void FixedUpdate()
    {
        speedManager();
        CheckGrounded();
        StandAfterHardLand();
        CheckAttacking();
        if (_isStun) return;
        if (_isRolling)
        {
            Rolling();
            return;
        }
        if (!_moveable) return;
        if (_isUsingSkill) return;
        if (_isAttacking) return;
        Movement();
        //Jump();
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheckPoint != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
        }
    }
    private void speedManager()
    {
        if (!_isMoving)
        {
            _speed = 0f;
            return;
        }
        if (_isCameraLockOn)
        {
            if (_isSprinting) _speed = _runSpeed;
            else if (_isWalking) _speed = _walkSpeed;
            else _speed = _walkSpeed;
        }
        else
        {
            if (_isSprinting) _speed = _sprintSpeed;
            else if (_isWalking) _speed = _walkSpeed;
            else _speed = _runSpeed;
        }
    }
    private void Movement()
    {
        float currentAccelerate = _isMoving ? _acceleration : _deceleration;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, _speed, currentAccelerate * Time.deltaTime);
        bool isFullyStopped = (_calculatedMoveDir == Vector3.zero && _currentSpeed <= 0.01f);
        Vector3 targetVelocity;
        // 5/31/2026
        float groundAngle = GetGroundAngle(out Vector3 groundNormal);
        if (_isGrounded && isFullyStopped && !_jumpFlag)
        {
            _playerCollider.material = _brakeMat;
            return;
        }
        else
        {
            _playerCollider.material = _slipperyMat;
        }
        if (_isGrounded && !_jumpFlag)
        {
            Vector3 slopeMoveDir = Vector3.ProjectOnPlane(_calculatedMoveDir, groundNormal).normalized;
            targetVelocity = slopeMoveDir * _currentSpeed;

            if (_calculatedMoveDir == Vector3.zero)
            {
                targetVelocity = Vector3.zero;
            }

            if (_calculatedMoveDir != Vector3.zero)
            {
                Vector3 checkOrigin = transform.position + Vector3.up * 0.5f;
                float playerRadius = 0.3f;
                float checkDistance = 0.4f;

                if (Physics.SphereCast(checkOrigin, playerRadius, _calculatedMoveDir, out RaycastHit wallHit, checkDistance, _groundLayer))
                {
                    float wallAngle = Vector3.Angle(Vector3.up, wallHit.normal);

                    if (wallAngle > _maxSlopeAngle)
                    {
                        float pushIntoWallForce = Vector3.Dot(targetVelocity, wallHit.normal);

                        if (pushIntoWallForce < 0)
                        {
                            targetVelocity -= pushIntoWallForce * wallHit.normal;
                        }

                        targetVelocity.y = Mathf.Min(targetVelocity.y, 0f);
                    }
                }
            }
            rb.linearVelocity = targetVelocity;
        }
        //else
        //{
        //    float currentY = rb.linearVelocity.y;
        //    if (currentY > 0 && !_jumpFlag) currentY = 0;

        //    targetVelocity = new Vector3(_calculatedMoveDir.x * _currentSpeed, currentY, _calculatedMoveDir.z * _currentSpeed);
        //}
        //targetVelocity = new Vector3(
        //    _calculatedMoveDir.x * _currentSpeed,
        //    rb.linearVelocity.y,
        //    _calculatedMoveDir.z * _currentSpeed
        //);
        

        if (_calculatedMoveDir != Vector3.zero)
        {
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, _targetRotation, _rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRotation);
        }
    }

    private void Jump()
    {
        if (!_jumpFlag ) return;
        animator.SetTrigger(_animJump);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
    private void RotateCharacter()
    {
        if (_isCameraLockOn && _targetLockOn != null)
        {
            Vector3 directionToTarget = _targetLockOn.position - transform.position;

            directionToTarget.y = 0f;

            if (directionToTarget != Vector3.zero)
            {
                _targetRotation = Quaternion.LookRotation(directionToTarget);
            }

            CalculateMoveDirection();
            return;
        }

        CalculateMoveDirection();
        if (_calculatedMoveDir != Vector3.zero)
        {
            _targetRotation = Quaternion.LookRotation(_calculatedMoveDir);
        }
        //Debug.Log("_calculatedMoveDir " + _calculatedMoveDir + "_targetRotation " + _targetRotation);
    }
    private void CalculateMoveDirection()
    {
        if (_moveInput == Vector2.zero)
        {
            _calculatedMoveDir = Vector3.zero;
            return;
        }

        Vector3 camForward = mainCamera.forward;
        Vector3 camRight = mainCamera.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        _calculatedMoveDir = (camForward * _moveInput.y + camRight * _moveInput.x).normalized;
    }

    private void AnimationProcess()
    {
        if (_isRolling) return;
        // Moving
        animator.SetBool(_animMove, _currentSpeed > 0.01f);

        if (_moveInput.magnitude > 0.01f)
        {
            _lastMoveDir = _moveInput.normalized;
        }
        float targetAnimValue = _isSprinting ? 3f : (_isWalking ? 1f : 2f);
        float speedRatio = (_speed > 0.01f) ? (_currentSpeed / _speed) : 0f;
        float currentAnimMagnitude = speedRatio * targetAnimValue;

        if (_isCameraLockOn)
        {
            float horizontal = _lastMoveDir.x * currentAnimMagnitude;
            float vertical = _lastMoveDir.y * currentAnimMagnitude;

            animator.SetFloat(_animHorizontal, horizontal, 0.2f, Time.deltaTime);
            animator.SetFloat(_animVertical, vertical, 0.2f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat(_animHorizontal, currentAnimMagnitude, 0.2f, Time.deltaTime);
        }

        // Landing
        animator.SetBool(_animGround, _isGrounded);

        // Falling
        animator.SetFloat(_animVerticalVelocity, _fallVelocityY);
    }
    private void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        if (_isGrounded && !_isRolling && !_jumpFlag)
        {
            _jumpFlag = true;
            Jump();
        }
    }
    private void HandleSprintOrRoll(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is TapInteraction)
        {
            if (_isStun) return;
            if (_isRolling || !_isGrounded) return;
            if (_playerAttack != null)
            {
                _playerAttack.ResetCombatState();
                _playerSkill.ChangeState(SkillState.End);
                RotateCharacter();
                if (_calculatedMoveDir != Vector3.zero)
                {
                    rb.MoveRotation(_targetRotation);
                }
            }
            animator.SetTrigger(_animRoll);

            if (_rollCoroutine != null) StopCoroutine(_rollCoroutine);
            _rollCoroutine = StartCoroutine(RollRoutine());
        }
        else if (ctx.interaction is HoldInteraction)
        {
             _isSprinting = true;
        }
    }
    private void HandleSprintStop(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is HoldInteraction)
        {
             _isSprinting = false;
        }
    }
    private void HandleWalkToggleInput(InputAction.CallbackContext ctx)
    {
        
    }

    private void CheckGrounded()
    {
        //_isGrounded = Physics.CheckSphere(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
        //if (!_isGrounded) _fallVelocityY = rb.linearVelocity.y;

        // 6/1/2026
        bool isPhysicallyGrounded = Physics.CheckSphere(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
        if (_jumpFlag)
        {
            _isGrounded = false;

            if (rb.linearVelocity.y < 0f)
            {
                _jumpFlag = false;
            }
        }
        else
        {
            _isGrounded = isPhysicallyGrounded;
        }

        if (!_isGrounded)
        {
            _fallVelocityY = rb.linearVelocity.y;
        }
    }

    private void Rolling()
    {
        rb.linearVelocity = new Vector3(_rollDir.x * _rollSpeed, rb.linearVelocity.y, _rollDir.z * _rollSpeed);
    }
    private IEnumerator RollRoutine()
    {
        _isRolling = true;

        if (_calculatedMoveDir != Vector3.zero)
        {
            _rollDir = _calculatedMoveDir;
        }
        else
        {
            _rollDir = transform.forward;
        }

        yield return new WaitForSeconds(_rollDuration);

        _isRolling = false;
    }
    private void StandAfterHardLand()
    {
        if (!_moveable)
        {
            if (Time.time - _startStandTime >= _standTime)
            {
                _moveable = true;
                _fallVelocityY = 0;
            }
            return;
        }
        if (_isGrounded)
        {
            if (_fallVelocityY < _landVelocityBaseValue)
            {
                _startStandTime = Time.time;
                _moveable = false;
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                if (_fallVelocityY == 0) return;
                _fallVelocityY = 0;
            }
        }
    }

    private void HandleLockOnCam(Transform target)
    {
        if (target == null)
        {
            _isCameraLockOn = false;
            _targetLockOn = null;
            animator.SetBool("IsLockOnCamera", false);
            return;
        }

        _isCameraLockOn = true;
        _targetLockOn = target;
        animator.SetBool("IsLockOnCamera", true);
    }

    private float GetGroundAngle(out Vector3 groundNormal)
    {
        groundNormal = Vector3.up;

        if (!_isGrounded) return 0f;

        Vector3 rayStart = _groundCheckPoint.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, _groundCheckRadius + 0.3f, _groundLayer))
        {
            groundNormal = hit.normal;
            return Vector3.Angle(Vector3.up, groundNormal);
        }

        return 0f;
    }

    private void CheckAttacking()
    {
        if (_playerAttack == null) return;
        //if (!_isMoving) return;
        //if (!_jumpFlag) return;
        //if (!_isRolling) return;
        if (_playerAttack.GetCurrentAttackNode() != null)
        {
            rb.linearVelocity = Vector3.zero;
            _isAttacking = true;
        }
        else
        {
            _isAttacking = false;
        }
    }

    public void ForceStopRolling()
    {
        if (_isRolling)
        {
            if (_rollCoroutine != null) StopCoroutine(_rollCoroutine);
            _isRolling = false;
        }
    }

    public bool GetRollInfo() => _isRolling;
    public bool GetIsGround() => _isGrounded;

    public void EnableBeStun()
    {
        _isStun = true;
        rb.linearVelocity = Vector3.zero;
    }
    public void DisableBeStun()
    {
        _isStun = false;
    }
    public void SetMainCamera(Transform cam)
    {
        mainCamera = cam;
    }
    public void SetAllowMovement(bool value)
    {
        rb.linearVelocity = Vector3.zero;
        _isUsingSkill = value;
        _moveable = !value;
    }
}
