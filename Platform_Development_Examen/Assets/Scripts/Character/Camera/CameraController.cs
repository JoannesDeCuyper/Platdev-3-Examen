using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Transform _playerPoint;
    [SerializeField] private float _speed;

    private Camera _camera;
    private float _normalFOV, _zoomFOV;
    private float _verticalInput, _horizontalInput;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _normalFOV = 60.0f;
        _zoomFOV = 30.0f;
    }
    private void Update()
    {
        _verticalInput = Input.GetAxis("360_VerticalY");
        _horizontalInput = Input.GetAxis("360_HorizontalX");
    }

    private void FixedUpdate()
    {
        ApplyCameraController();
        ApplyCameraRotationWithBumpers();
        ApplyCameraZoomInAndOut();
    }

    private void ApplyCameraController()
    {
        transform.RotateAround(_playerPoint.position, Vector3.up, _horizontalInput * _speed * Time.fixedDeltaTime);
        transform.RotateAround(_playerPoint.position, Vector3.left, _verticalInput * _speed * Time.fixedDeltaTime);
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
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView,_zoomFOV, Time.deltaTime);

        //Zoom out
        if (Input.GetAxis("360_RightTrigger") != 0)
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView,_normalFOV, Time.deltaTime);
    }
}
