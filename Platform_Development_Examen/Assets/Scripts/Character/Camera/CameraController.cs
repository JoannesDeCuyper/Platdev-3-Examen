using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Transform _playerPoint;
    [SerializeField] private float _speed;

    private Vector3 _InputAxis;
    private Camera _camera;
    private float _zoomOutFOV, _zoomInFOV;
    public float _verticalInput, _horizontalInput;

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
        transform.RotateAround(_playerPoint.position, Vector3.up, _horizontalInput);
        transform.RotateAround(_playerPoint.position, Vector3.left, _verticalInput);
        transform.LookAt(_playerPoint.position);
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
        if (Input.GetAxis("360_LeftTrigger") != 0 )
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView,_zoomInFOV, Time.fixedDeltaTime * 20.0f);

        //Zoom out
        if (Input.GetAxis("360_RightTrigger") != 0)
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView,_zoomOutFOV, Time.fixedDeltaTime * 20.0f);
    }
}
