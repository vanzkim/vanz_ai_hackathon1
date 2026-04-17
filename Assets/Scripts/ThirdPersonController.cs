using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float speedChangeRate = 10.0f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Cinemachine")]
    [SerializeField] private GameObject cinemachineCameraTarget;
    [SerializeField] private float topClamp = 70.0f;
    [SerializeField] private float bottomClamp = -30.0f;

    // Player state
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // Cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // Components
    private PlayerInput _playerInput;
    private Animator _animator;
    private CharacterController _controller;
    private Camera _mainCamera;

    private Vector2 _move;
    private Vector2 _look;
    private bool _isCurrentDeviceMouse;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        ApplyGravity();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
        Debug.Log($"Input Move: {_move}");
    }

    private void OnLook(InputValue value)
    {
        _look = value.Get<Vector2>();
        Debug.Log($"Input Look: {_look}");
    }

    private void CameraRotation()
    {
        if (_look.sqrMagnitude >= 0.01f)
        {
            float deltaTimeMultiplier = _playerInput.currentControlScheme == "Keyboard&Mouse" ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _look.y * deltaTimeMultiplier;
        }

        // Clamp rotations
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        // Update target rotation
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        float targetSpeed = _move == Vector2.zero ? 0.0f : moveSpeed;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _move.magnitude;

        // Accelerate or decelerate
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // Normalise input direction
        Vector3 inputDirection = new Vector3(_move.x, 0.0f, _move.y).normalized;

        if (_move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);

            // Rotate to face input direction relative to camera
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // Move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // Update animator
        if (_animator != null)
        {
            _animator.SetFloat("Speed", _animationBlend);
        }
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded)
        {
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
