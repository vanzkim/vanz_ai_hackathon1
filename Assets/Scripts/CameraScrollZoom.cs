using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineThirdPersonFollow))]
public class CameraScrollZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSensitivity = 0.5f;
    public float minDistance = 1f;
    public float maxDistance = 10f;
    public float smoothTime = 0.1f;

    private CinemachineThirdPersonFollow _follow;
    private float _targetDistance;
    private float _currentVelocity;

    private void Awake()
    {
        _follow = GetComponent<CinemachineThirdPersonFollow>();
        _targetDistance = _follow.CameraDistance;
    }

    private void Update()
    {
        // Read mouse scroll delta
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Adjust target distance based on scroll
            _targetDistance -= scroll * zoomSensitivity * 0.01f;
            _targetDistance = Mathf.Clamp(_targetDistance, minDistance, maxDistance);
        }

        // Smoothly interpolate the camera distance
        if (Mathf.Abs(_follow.CameraDistance - _targetDistance) > 0.001f)
        {
            _follow.CameraDistance = Mathf.SmoothDamp(_follow.CameraDistance, _targetDistance, ref _currentVelocity, smoothTime);
        }
    }
}
