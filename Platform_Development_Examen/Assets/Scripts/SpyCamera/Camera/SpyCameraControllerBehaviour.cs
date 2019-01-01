using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyCameraControllerBehaviour : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private GameObject[] _spyCam;
    [SerializeField] private GameObject _playerCam;
    [SerializeField] private CharacterControllerBehaviour _characterControllerScript;

    [Header("User Interface")]
    [SerializeField] private GameObject _switchCameraMessage;
    [SerializeField] private GameObject _zoomCameraMessage;
    [SerializeField] private GameObject _interactGetOutOfCoverMessage;

    public int _number = 0;
    private bool _AButton, _XButton;
    private bool _isSpyMode = false;

    private void Update()
    {
        _XButton = Input.GetKeyDown(KeyCode.Joystick1Button2);
        _AButton = Input.GetKeyDown(KeyCode.Joystick1Button0);

        ApplyGoIntoSpyMode();
        ApplySwitchCamera();

        if (_number > 3)
            _number = 0;
    }

    private void ApplySwitchCamera()
    {
        if (_isSpyMode)
        {
            _characterControllerScript._isWalking = false;

            if (_AButton)
            {
                _number = _number + 1;
                _playerCam.SetActive(false);
                _spyCam[_number].SetActive(true);
            }
        }
    }

    private void ApplyGoIntoSpyMode()
    {
        if(_XButton && _characterControllerScript.IsHacking)
        {
            _interactGetOutOfCoverMessage.SetActive(false);
            _switchCameraMessage.SetActive(true);
            _zoomCameraMessage.SetActive(true);
            _isSpyMode = !_isSpyMode;
            _playerCam.SetActive(false);
            _spyCam[_number].SetActive(true);
        }
        else if(!_isSpyMode)
        {
            _switchCameraMessage.SetActive(false);
            _zoomCameraMessage.SetActive(false);
            _characterControllerScript._isWalking = true;
            _playerCam.SetActive(true);

            for (int i = 0; i < _spyCam.Length; i++)
            {
                _spyCam[i].SetActive(false);
            }
        }
    }
}
