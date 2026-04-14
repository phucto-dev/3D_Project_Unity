using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _acceleration = 15f;
    [SerializeField] private float _deceleration = 25f;

    [Header("Ref")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform mainCamera;

    [Header("GroundCheckSettings")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;

    private PlayerInput _inputSystem;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _walkToggleAction;

    private Vector3 _calculatedMoveDir;
    private Quaternion _targetRotation;
    private Vector2 _moveInput;
    private float _speed;
    private float _currentSpeed;
    private bool _jumpFlag;
    private bool _isMoving;
    private bool _isSprinting;
    private bool _isWalking;
    private bool _isGrounded;

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
        }
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
        animator.SetBool("IsMoving", _currentSpeed > 0.01f);
        float inputMagnitude = _moveInput.magnitude;
        Debug.Log(inputMagnitude);
        float horizontal = (_isSprinting ? 3 : (_isWalking ? 1 : 2)) * inputMagnitude;
        float vertical = (_isSprinting ? 3 : (_isWalking ? 1 : 2)) * _moveInput.y;
        animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        //animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
    }
    private void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        _jumpFlag = true;
    }
    private void HandleSprintOrRoll(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is TapInteraction)
        {
             animator.SetTrigger("Roll");
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
    }
}
