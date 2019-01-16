using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyCameraRotationBehaviour : MonoBehaviour
{
    [SerializeField] private float _camRotationSpeed;

    private float _horizontalInput, _verticalInput;
    private float _rotX, _rotY;
    private float _minClampAngle, _maxClampAngle;
    private float _zoomOutFOV, _zoomInFOV, _zoomInAndOutSpeed;
    private Vector3 _distance;
    private Camera _camera;

    private void Start()
    {
        _minClampAngle = 0.0f;
        _maxClampAngle = 60.0f;
        _zoomOutFOV = 60.0f;
        _zoomInFOV = 15.0f;
        _zoomInAndOutSpeed = 20.0f;
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        //Input
        _horizontalInput += Input.GetAxis("360_HorizontalX") * _camRotationSpeed * Time.deltaTime;
        _verticalInput += Input.GetAxis("360_VerticalY") * _camRotationSpeed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ApplyRotateCamera();
        ApplyCameraZoomInAndOut();
    }

    private void ApplyRotateCamera()
    {
        _rotX = _horizontalInput;
        _rotY = _verticalInput;
        _rotY = Mathf.Clamp(_rotY, _minClampAngle, _maxClampAngle);

        Quaternion rotation = Quaternion.Euler(_rotY, _rotX, 0.0f);

        transform.rotation = rotation;
    }

    private void ApplyCameraZoomInAndOut()
    {
        //Zoom in
        if (Input.GetAxis("360_LeftTrigger") != 0.0f)
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomInFOV, Time.fixedDeltaTime * _zoomInAndOutSpeed);

        //Zoom out
        if (Input.GetAxis("360_RightTrigger") != 0.0f)
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomOutFOV, Time.fixedDeltaTime * _zoomInAndOutSpeed);
    }
}





















//private float _verticalInput, _horizontalInput;
//private Camera _camera;
//private float _normalFOV, _zoomFOV;
//[SerializeField] private float _speed;
//[SerializeField] private float _minClampAngle, _maxClampAngle;

//private void Start()
//{
//    _camera = GetComponent<Camera>();
//    _normalFOV = 60.0f;
//    _zoomFOV = 15.0f;
//}
//private void Update()
//{
//    _verticalInput = Input.GetAxis("360_VerticalY");
//    _horizontalInput = Input.GetAxis("360_HorizontalX");
//}

//private void FixedUpdate()
//{
//    ApplyCameraRotation();
//    ApplyCameraZoomInAndOut();
//}

//private void ApplyCameraRotation()
//{
//    transform.Rotate(Vector3.left * _verticalInput);
//    transform.Rotate(Vector3.up * _horizontalInput);
//}

//private void ApplyCameraZoomInAndOut()
//{
//    //Zoom in
//    if (Input.GetAxis("360_LeftTrigger") != 0)
//    {
//        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomFOV, Time.deltaTime);
//    }

//    //Zoom out
//    if (Input.GetAxis("360_RightTrigger") != 0)
//    {
//        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _normalFOV, Time.deltaTime);
//    }
//}