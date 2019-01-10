using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerPoint;
    [SerializeField] private float _speed;

    private Vector3 _InputAxis;
    private Camera _camera;
    private float _zoomOutFOV, _zoomInFOV;
    private float _verticalInput, _horizontalInput;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _zoomOutFOV = 80.0f;
        _zoomInFOV = 30.0f;
    }
    private void Update()
    {
        _horizontalInput = Input.GetAxis("360_HorizontalX") * _speed * Time.deltaTime;
        _verticalInput = Input.GetAxis("360_VerticalY") * _speed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ApplyCameraController();
        ApplyCameraRotationWithBumpers();
        ApplyCameraZoomInAndOut();
    }

    private void ApplyCameraController()
    {
        if (_horizontalInput > 1.4f || _horizontalInput < -1.4f)
            transform.RotateAround(_playerPoint.position, Vector3.up, _horizontalInput);

        if (_verticalInput > 1.4f || _verticalInput < -1.4f)
            transform.RotateAround(_playerPoint.position, Vector3.left, _verticalInput);

        transform.Rotate(_verticalInput, _horizontalInput, 0);

        transform.LookAt(_playerPoint);
    }

    private void ApplyCameraRotationWithBumpers()
    {
        if (Input.GetKey(KeyCode.Joystick1Button4))
            transform.RotateAround(_playerPoint.position, Vector3.up, _speed * Time.fixedDeltaTime);

        if (Input.GetKey(KeyCode.Joystick1Button5))
            transform.RotateAround(_playerPoint.position, Vector3.down, _speed * Time.fixedDeltaTime);
    }

    private void ApplyCameraZoomInAndOut()
    {
        //Zoom in
        if (Input.GetAxis("360_LeftTrigger") != 0)
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomInFOV, Time.fixedDeltaTime * 20.0f);

        //Zoom out
        if (Input.GetAxis("360_RightTrigger") != 0)
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomOutFOV, Time.fixedDeltaTime * 20.0f);
    }
}

    //[SerializeField] private Transform _playerPoint;
    //[SerializeField] private float _speed;
    //private float _horizontalInput, _verticalInput;
    //private Vector3 _offset;
    //private Camera _camera;
    //private float _zoomOutFOV, _zoomInFOV;
    //private bool _isRotatingWithBumper = false;

//private void Start()
//{
//    _offset = _playerPoint.transform.position - transform.position;

//    _camera = GetComponent<Camera>();
//    _zoomOutFOV = 80.0f;
//    _zoomInFOV = 30.0f;
//}
//private void Update()
//{
//    _horizontalInput = Input.GetAxis("360_HorizontalX") * _speed * Time.deltaTime;
//    _verticalInput = Input.GetAxis("360_VerticalY") * _speed * Time.deltaTime;
//    _playerPoint.Rotate(_verticalInput, _horizontalInput, 0.0f);
//}
//private void FixedUpdate()
//{
//    ApplyCameraRotation();
//    ApplyCameraRotationWithBumpers();
//    ApplyCameraZoomInAndOut();
//}
//private void ApplyCameraRotation()
//{
//    if (!_isRotatingWithBumper)
//    {
//        float desiredAngleY = _playerPoint.transform.eulerAngles.y;
//        float desiredAngleX = _playerPoint.transform.eulerAngles.x;
//        float desiredAngleZ = _playerPoint.transform.eulerAngles.z;

//        desiredAngleZ = 0;

//        Quaternion rotation = Quaternion.Euler(desiredAngleX, desiredAngleY, desiredAngleZ);
//        transform.position = _playerPoint.transform.position - (rotation * _offset);
//        transform.LookAt(_playerPoint.transform);
//    }
//}
//private void ApplyCameraRotationWithBumpers()
//{
//    if (Input.GetKey(KeyCode.Joystick1Button4))
//    {
//        _isRotatingWithBumper = true;
//        transform.RotateAround(_playerPoint.position, Vector3.up, _speed * Time.fixedDeltaTime);
//    }
//    else if (Input.GetKey(KeyCode.Joystick1Button5))
//    {
//        _isRotatingWithBumper = true;
//        transform.RotateAround(_playerPoint.position, Vector3.down, _speed * Time.fixedDeltaTime);
//    }
//    else
//        _isRotatingWithBumper = false;
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
