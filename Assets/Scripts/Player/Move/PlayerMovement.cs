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

    [Header("Ref")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform mainCamera;

    [Header("GroundCheckSettings")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Animator Hashes")]
    private readonly int _animMove = Animator.StringToHash("IsMoving");
    private readonly int _animHorizontal = Animator.StringToHash("Horizontal");
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
    private bool _isLanding;
    private Vector3 _rollDir;
    private float _fallVelocityY;
    private float _landVelocityBaseValue = -5f;
    private float _startStandTime;
    private bool _isCameraLockOn;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInput>();
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
        _jumpAction.performed += HandleJumpInput;
        _sprintAction.performed += HandleSprintOrRoll;
        _sprintAction.canceled += HandleSprintStop;
    }
    private void OnDisable()
    {
        _jumpAction.performed -= HandleJumpInput;
        _sprintAction.performed -= HandleSprintOrRoll;
        _sprintAction.canceled -= HandleSprintStop;
    }

    private void Start()
    {
        if (animator != null) animator.SetBool("IsLockOnCamera", false);
        _isLanding = true;
        _moveable = true;
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
        if (_isRolling)
        {
            Rolling();
            return;
        }
        if (!_moveable) return;
        Movement();
        Jump();
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

        if (_isSprinting) _speed = _sprintSpeed;
        else if (_isWalking) _speed = _walkSpeed;
        else _speed = _runSpeed;
    }
    private void Movement()
    {
        float currentAccelerate = _isMoving ? _acceleration : _deceleration;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, _speed, currentAccelerate * Time.deltaTime);
        Vector3 targetVelocity = new Vector3(
            _calculatedMoveDir.x * _currentSpeed,
            rb.linearVelocity.y,
            _calculatedMoveDir.z * _currentSpeed
        );
        rb.linearVelocity = targetVelocity;

        if (_calculatedMoveDir != Vector3.zero)
        {
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, _targetRotation, _rotationSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(smoothRotation);
        }
    }

    private void Jump()
    {
        if (_jumpFlag)
        {
            if (_isGrounded)
            {
                animator.SetTrigger(_animJump);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
            _jumpFlag = false;
        }
    }
    private void RotateCharacter()
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

        _targetRotation = Quaternion.LookRotation(_calculatedMoveDir);
        //Debug.Log("_calculatedMoveDir " + _calculatedMoveDir + "_targetRotation " + _targetRotation);
    }
    private void AnimationProcess()
    {
        // Moving
        animator.SetBool(_animMove, _currentSpeed > 0.01f);
        float inputMagnitude = _moveInput.magnitude;
        //Debug.Log(inputMagnitude);
        float horizontal = (_isSprinting ? 3 : (_isWalking ? 1 : 2)) * inputMagnitude;
        float vertical = (_isSprinting ? 3 : (_isWalking ? 1 : 2)) * _moveInput.y;
        animator.SetFloat(_animHorizontal, horizontal, 0.1f, Time.deltaTime);
        //animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);

        // Landing
        animator.SetBool(_animGround, _isGrounded);

        // Falling
        animator.SetFloat(_animVerticalVelocity, _fallVelocityY);
    }
    private void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        _jumpFlag = true;
    }
    private void HandleSprintOrRoll(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is TapInteraction)
        {
            if (_isRolling || !_isGrounded) return;
            animator.SetTrigger(_animRoll);
            StartCoroutine(RollRoutine());
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
        _isGrounded = Physics.CheckSphere(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
        if (!_isGrounded) _fallVelocityY = rb.linearVelocity.y;
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
            }
            else
            {
                if (_fallVelocityY == 0) return;
                _fallVelocityY = 0;
            }
        }
    }

    private void StatusCamInfo()
    {

    }
}
