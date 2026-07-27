using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[Serializable]
public enum EngineTypeEnum
{
    RotorBodyEngine,
    RotorBackEngine,
}

public class Movement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float MaxSpeed;
    public float SpeedAcceleration;
    private float _currentSpeed;

    [Header("Lift Settings")]
    public float PushForce;
    public float MaxHeight;
    public float WarningHeight;
    private float _startHeight;
    private float _maxHeightAllow;
    private float _currentPushForce;

    [Header("Rotor Settings")]
    public float RotorMaxSpeed;
    public float RotorAcceleration;
    public float RotorDeceleration;
    private float _currentRotorSpeed;
    private bool _isRotorSpin;

    [Header("Pitch Settings (W/S)")]
    public float HeadUpDeg;
    public float HeadDownDeg;
    public float PitchSpeed = 5f;
    private float _targetPitch = 0f;
    private float _currentPitch = 0f;

    [Header("Yaw Settings (A/D)")]
    public float TurnSpeed = 50f;
    private float _currentYaw = 0f; // Lưu trữ góc quay trục Y tập trung

    [Header("Engine References")]
    public EngineType _rotorBody;
    public EngineType _rotorBack;

    [Header("Interact Settings (Key E)")]
    public float GrabRange = 3.0f;
    public LayerMask CargoLayer;
    public Transform CargoSocket;

    private FixedJoint _activeJoint;
    private Rigidbody _grabbedRigidbody;
    private PlayerInput _inputSystem;
    private InputAction _moveAction;
    private InputAction _pushAction;
    private InputAction _interactAction;
    private Rigidbody _rb;

    private Vector2 _moveInput;
    private bool _isGrounded;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
        if (_inputSystem == null) return;
        _moveAction = _inputSystem.actions["Move"];
        _pushAction = _inputSystem.actions["Sprint"];
        _interactAction = _inputSystem.actions["Interact"];
    }

    private void OnEnable()
    {
        _pushAction.performed += HandlePush;
        _interactAction.performed += HandleInteract;
    }

    private void OnDisable()
    {
        _pushAction.performed -= HandlePush;
        _interactAction.performed -= HandleInteract;
    }

    private void Start()
    {
        _currentRotorSpeed = 0;
        _startHeight = transform.position.y;
        _maxHeightAllow = _startHeight + MaxHeight;

        _currentYaw = transform.localEulerAngles.y;
    }

    private void Update()
    {
        if (_inputSystem == null) return;
        _moveInput = _moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckHeight();
        CheckRotorSpin();
        Push();
        Move();
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.5f);
    }

    private void Move()
    {
        float rotorSpeedPercent = _currentRotorSpeed / Mathf.Max(0.001f, RotorMaxSpeed);

        if (_isGrounded || rotorSpeedPercent <= 0.6f)
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, SpeedAcceleration * Time.fixedDeltaTime);
            _targetPitch = 0f;
        }
        else
        {
            if (Mathf.Abs(_moveInput.y) > 0.01f)
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, MaxSpeed, SpeedAcceleration * Time.fixedDeltaTime);
                _targetPitch = _moveInput.y > 0f ? HeadDownDeg : HeadUpDeg;
            }
            else
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, SpeedAcceleration * Time.fixedDeltaTime);
                _targetPitch = 0f;
            }
        }

        Vector3 moveDirection = transform.forward * _moveInput.y * _currentSpeed;
        Vector3 targetVelocity = new Vector3(moveDirection.x, _rb.linearVelocity.y, moveDirection.z);
        _rb.linearVelocity = targetVelocity;

        if (rotorSpeedPercent > 0.4f && Mathf.Abs(_moveInput.x) > 0.01f)
        {
            _currentYaw += _moveInput.x * TurnSpeed * Time.fixedDeltaTime;
        }

        _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, PitchSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
    }

    private void Push()
    {
        float rotorSpeedPercent = _currentRotorSpeed / Mathf.Max(0.001f, RotorMaxSpeed);
        bool isPushing = _pushAction.IsPressed();

        if (isPushing)
        {
            _currentPushForce = PushForce * rotorSpeedPercent;
        }
        else
        {
            _currentPushForce = Mathf.MoveTowards(_currentPushForce, 0f, PushForce * 0.5f * Time.fixedDeltaTime);
        }

        float currentHeight = transform.position.y;

        if (currentHeight > _maxHeightAllow - WarningHeight)
        {
            float heightT = (currentHeight - (_maxHeightAllow - WarningHeight)) / WarningHeight;
            heightT = Mathf.Clamp01(heightT);

            if (isPushing)
            {
                float hoverForce = _rb.mass * Mathf.Abs(Physics.gravity.y);
                _currentPushForce = Mathf.Lerp(_currentPushForce, hoverForce, heightT);

                if (currentHeight >= _maxHeightAllow && _rb.linearVelocity.y > 0)
                {
                    _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
                    _currentPushForce = hoverForce;
                }
            }
            else
            {
                float lowerBound = 0f;
                _currentPushForce = Mathf.Lerp(_currentPushForce, lowerBound, heightT);
            }
        }

        _rb.AddForce(Vector3.up * _currentPushForce, ForceMode.Force);
    }

    private void CheckHeight()
    {
        if (_currentRotorSpeed == 0)
        {
            _startHeight = transform.position.y;
            _maxHeightAllow = _startHeight + MaxHeight;
        }
    }

    private void CheckRotorSpin()
    {
        if (!_isRotorSpin)
        {
            _currentRotorSpeed = Mathf.MoveTowards(_currentRotorSpeed, 0, RotorDeceleration * Time.fixedDeltaTime);
        }
        else
        {
            _currentRotorSpeed = Mathf.MoveTowards(_currentRotorSpeed, RotorMaxSpeed, RotorAcceleration * Time.fixedDeltaTime);
        }

        _rotorBody.transform.Rotate(Vector3.up, _currentRotorSpeed * Time.fixedDeltaTime, Space.Self);
        _rotorBack.transform.Rotate(Vector3.right, _currentRotorSpeed * Time.fixedDeltaTime, Space.Self);
    }

    private void HandlePush(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is HoldInteraction)
        {
            _isRotorSpin = true;
        }

        if (ctx.interaction is TapInteraction)
        {
            _isRotorSpin = false;
        }
    }

    private void HandleInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("e11");
        if (ctx.interaction is HoldInteraction)
        {
            Debug.Log("eeee" + _activeJoint);
            if (_activeJoint != null)
            {
                ReleaseCargo();
                return;
            }

            TryGrabCargo();
        }
    }

    private void TryGrabCargo()
    {
        RaycastHit hit;

        // Bắn tia quét từ vị trí của Socket thay vì từ tâm máy bay để chính xác hơn
        if (Physics.Raycast(CargoSocket.position, Vector3.down, out hit, GrabRange, CargoLayer))
        {
            if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody targetRb))
            {
                _grabbedRigidbody = targetRb;

                // Điều này triệt tiêu 100% lực đẩy vô hình do 2 Collider đè vào nhau
                Collider helicopterCollider = GetComponent<Collider>();
                Collider cargoCollider = hit.collider;
                if (helicopterCollider != null && cargoCollider != null)
                {
                    Physics.IgnoreCollision(helicopterCollider, cargoCollider, true);
                }

                // 1. Đưa vật thể về vị trí Socket
                _grabbedRigidbody.transform.position = CargoSocket.position;
                _grabbedRigidbody.transform.rotation = CargoSocket.rotation;

                // Reset vận tốc tích tụ cũ của vật gắp về 0 trước khi nối
                _grabbedRigidbody.linearVelocity = Vector3.zero;
                _grabbedRigidbody.angularVelocity = Vector3.zero;

                // 2. Tạo mối nối FixedJoint
                _activeJoint = gameObject.AddComponent<FixedJoint>();
                _activeJoint.anchor = transform.InverseTransformPoint(CargoSocket.position);
                _activeJoint.connectedBody = _grabbedRigidbody;

                _activeJoint.breakForce = 20000f;
                _activeJoint.breakTorque = 20000f;

                _rb.centerOfMass = transform.InverseTransformPoint(CargoSocket.position);
                Debug.Log($"Đã gắp vật thể vào Socket: {hit.collider.name}");
            }
        }
    }

    private void ReleaseCargo()
    {
        if (_grabbedRigidbody != null)
        {
            // Mở lại va chạm giữa 2 vật
            Collider helicopterCollider = GetComponent<Collider>();
            Collider cargoCollider = _grabbedRigidbody.GetComponent<Collider>();
            if (helicopterCollider != null && cargoCollider != null)
            {
                Physics.IgnoreCollision(helicopterCollider, cargoCollider, false);
            }
        }
        if (_activeJoint != null)
        {
            // Hủy bỏ khớp nối vật lý lập tức
            Destroy(_activeJoint);
            _activeJoint = null;
        }
        _rb.ResetCenterOfMass();
        if (_grabbedRigidbody != null)
        {
            // Đánh thức vật thể dậy để nó chịu tác động trọng lực và rơi xuống đất ngay khi thả
            _grabbedRigidbody.WakeUp();
            _grabbedRigidbody = null;
        }

        Debug.Log("Đã thả vật thể tự do!");
    }
    private void OnJointBreak(float brokenForce)
    {
        _activeJoint = null;
        _grabbedRigidbody = null;
        Debug.LogWarning($"Khớp nối bị đứt do va đập mạnh với lực: {brokenForce}! Vật thể đã bị tuột.");
    }
}