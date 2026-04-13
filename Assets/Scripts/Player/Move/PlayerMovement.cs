using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _rotationSpeed;

    [Header("Ref")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform mainCamera;
    private PlayerInput _inputSystem;
    private InputAction _moveAction;
    private InputAction _jumpAction;

    private Vector3 _calculatedMoveDir;
    private Quaternion _targetRotation;
    private Vector2 _moveInput;
    private bool _jumpFlag;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInput>();
        if (_inputSystem)
        {
            _moveAction = _inputSystem.actions["Move"];
            _jumpAction = _inputSystem.actions["Jump"];
        }
    }

    private void OnEnable()
    {
        _jumpAction.performed += OnJump;
    }
    private void OnDisable()
    {
        _jumpAction.performed -= OnJump;
    }

    private void Update()
    {
        if (_inputSystem == null) return;
        _moveInput = _moveAction.ReadValue<Vector2>();

        RotateCharacter();
    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
    }

    private void Movement()
    {
        Vector3 targetVelocity = new Vector3(
            _calculatedMoveDir.x * _speed,
            rb.linearVelocity.y,
            _calculatedMoveDir.z * _speed
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
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
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
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        _jumpFlag = true;
    }

}
