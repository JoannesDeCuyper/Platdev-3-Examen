using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyCameraControllerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] _spyCam;
    [SerializeField] private GameObject _playerCam;
    [SerializeField] private CharacterControllerBehaviour _characterControllerScript;
    private int _number = 0;

    private bool _isSpyMode = false;

    private void Update()
    { 
        ApplyGoIntoSpyMode();
        ApplySwitchCamera();

        if(_number > 3)
            _number = 0;
    }

    private void ApplySwitchCamera()
    {
        if (_isSpyMode)
        {
            _characterControllerScript._isWalking = false;

            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                _number = _number + 1;
                _playerCam.SetActive(false);
                _spyCam[_number].SetActive(true);
            }
        }
    }

    private void ApplyGoIntoSpyMode()
    {
        if(Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            _isSpyMode = !_isSpyMode;
            _playerCam.SetActive(false);
            _spyCam[_number].SetActive(true);
        }
        else if(!_isSpyMode)
        {
            _characterControllerScript._isWalking = true;
            _playerCam.SetActive(true);

            for (int i = 0; i < _spyCam.Length; i++)
            {
                _spyCam[i].SetActive(false);
            }
        }
    }
}
