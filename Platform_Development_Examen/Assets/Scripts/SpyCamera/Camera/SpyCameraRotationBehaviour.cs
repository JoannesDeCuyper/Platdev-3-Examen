using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyCameraRotationBehaviour : MonoBehaviour
{
    private float _verticalInput, _horizontalInput;
    private Camera _camera;
    private float _normalFOV, _zoomFOV;
    [SerializeField] private float _speed;
    [SerializeField] private float _minClampAngle, _maxClampAngle;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _normalFOV = 60.0f;
        _zoomFOV = 15.0f;
    }
    private void Update()
    {
        _verticalInput = Input.GetAxis("360_VerticalY");
        _horizontalInput = Input.GetAxis("360_HorizontalX");
    }

    private void FixedUpdate()
    {
        ApplyCameraRotation();
        ApplyCameraZoomInAndOut();
    }

    private void ApplyCameraRotation()
    {
        transform.Rotate(Vector3.left * _verticalInput);
        transform.Rotate(Vector3.up * _horizontalInput);
    }

    private void ApplyCameraZoomInAndOut()
    {
        //Zoom in
        if (Input.GetAxis("360_LeftTrigger") != 0)
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomFOV, Time.deltaTime);
        }

        //Zoom out
        if (Input.GetAxis("360_RightTrigger") != 0)
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _normalFOV, Time.deltaTime);
        }
    }
}
