using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _lookAtPoint;
    [SerializeField] private float _camRotationSpeed;

    private float _horizontalInput, _verticalInput;
    private float _rotX, _rotY;
    private float _distanceZ;
    private float _minClampAngle, _maxClampAngle;
    private float _zoomOutFOV, _zoomInFOV, _zoomInAndOutSpeed;
    private Vector3 _distance;
    private Camera _camera;

    private void Start()
    {
        _distanceZ = 4.5f;
        _minClampAngle = 0.0f;
        _maxClampAngle = 60.0f;
        _zoomOutFOV = 80.0f;
        _zoomInFOV = 30.0f;
        _zoomInAndOutSpeed = 20.0f;
        _distance = new Vector3(0.0f, 0.0f, _distanceZ);
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
        ApplyFollowPlayer();
        ApplyRotateCamera();
        ApplyCameraZoomInAndOut();
    }

    private void ApplyFollowPlayer()
    {
        transform.position = _lookAtPoint.position - _distance;
    }

    private void ApplyRotateCamera()
    {
            _rotX = _horizontalInput;
            _rotY = _verticalInput;
            _rotY = Mathf.Clamp(_rotY, _minClampAngle, _maxClampAngle);

            Quaternion rotation = Quaternion.Euler(_rotY, _rotX, 0);

            Vector3 distance = new Vector3(0.0f, 0.0f, -_distanceZ);
            Vector3 position = rotation * distance + _lookAtPoint.position;

            transform.rotation = rotation;
            transform.position = position;


        //Rotating with bumpers
        if (Input.GetKey(KeyCode.Joystick1Button4))
            _horizontalInput += _camRotationSpeed * Time.fixedDeltaTime;

        if (Input.GetKey(KeyCode.Joystick1Button5))
            _horizontalInput -= _camRotationSpeed * Time.fixedDeltaTime;
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
































//[SerializeField] private Transform _playerPoint;
//[SerializeField] private float _speed;

//private Vector3 _InputAxis;
//private Camera _camera;
//private float _zoomOutFOV, _zoomInFOV;
//private float _verticalInput, _horizontalInput;

//private void Start()
//{
//    _camera = GetComponent<Camera>();
//    _zoomOutFOV = 80.0f;
//    _zoomInFOV = 30.0f;
//}
//private void Update()
//{
//    _horizontalInput = Input.GetAxis("360_HorizontalX") * _speed * Time.deltaTime;
//    _verticalInput = Input.GetAxis("360_VerticalY") * _speed * Time.deltaTime;
//}

//private void FixedUpdate()
//{
//    ApplyCameraController();
//    ApplyCameraRotationWithBumpers();
//    ApplyCameraZoomInAndOut();
//}

//private void ApplyCameraController()
//{
//    if (_horizontalInput > 1.4f || _horizontalInput < -1.4f)
//        transform.RotateAround(_playerPoint.position, Vector3.up, _horizontalInput);

//    if (_verticalInput > 1.4f || _verticalInput < -1.4f)
//        transform.RotateAround(_playerPoint.position, Vector3.left, _verticalInput);

//    transform.Rotate(_verticalInput, _horizontalInput, 0);

//    transform.LookAt(_playerPoint);
//}

//private void ApplyCameraRotationWithBumpers()
//{
//    if (Input.GetKey(KeyCode.Joystick1Button4))
//        transform.RotateAround(_playerPoint.position, Vector3.up, _speed * Time.fixedDeltaTime);

//    if (Input.GetKey(KeyCode.Joystick1Button5))
//        transform.RotateAround(_playerPoint.position, Vector3.down, _speed * Time.fixedDeltaTime);
//}

//private void ApplyCameraZoomInAndOut()
//{
//    //Zoom in
//    if (Input.GetAxis("360_LeftTrigger") != 0)
//        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomInFOV, Time.fixedDeltaTime * 20.0f);

//    //Zoom out
//    if (Input.GetAxis("360_RightTrigger") != 0)
//        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomOutFOV, Time.fixedDeltaTime * 20.0f);
//}