using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2.0f;
    public float rotationSpeed = 10.0f;
    public float gravity = -9.81f;

    [Header("Camera Look")]
    public Transform cameraTarget;
    public float lookSensitivity = 0.1f;
    public float topClamp = 70.0f;
    public float bottomClamp = -30.0f;

    private CharacterController _controller;
    private Animator _animator;
    private PlayerInput _playerInput;
    private Camera _mainCamera;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _verticalVelocity;
    private float _cinemachineTargetPitch;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        ReadInput();
        HandleGravity();
        HandleRotation();
        HandleMovement();
    }

    private void ReadInput()
    {
        _moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();
        _lookInput = _playerInput.actions["Look"].ReadValue<Vector2>();
    }

    private void HandleGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }
        _verticalVelocity += gravity * Time.deltaTime;
    }

    private void HandleMovement()
    {
        Vector3 targetDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;

        if (_moveInput != Vector2.zero)
        {
            // Move relative to camera
            targetDirection = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0) * targetDirection;
        }

        Vector3 move = targetDirection * walkSpeed + new Vector3(0, _verticalVelocity, 0);
        _controller.Move(move * Time.deltaTime);

        // Update Animator
        float speed = _moveInput.magnitude * walkSpeed;
        _animator.SetFloat("Speed", speed);
    }

    private void HandleRotation()
    {
        // Player Rotation (Horizontal)
        if (_lookInput.x != 0)
        {
            transform.Rotate(Vector3.up * _lookInput.x * lookSensitivity);
        }

        // Camera Target Rotation (Vertical)
        if (cameraTarget != null)
        {
            _cinemachineTargetPitch -= _lookInput.y * lookSensitivity;
            _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, bottomClamp, topClamp);
            cameraTarget.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
        }
    }
}
