using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class CharacterControllerBehaviour : MonoBehaviour
{
    //Player
    [Header("Player")]
    [SerializeField] private float _maxXZVelocity;
    [SerializeField] private float _minXZVelocity;
    [SerializeField] private float _speed;

    private CharacterController _player;
    private Vector3 _inputMovement;
    private Vector3 _velocity = Vector3.zero;
    private float _verticalInput, _horizontalInput;
    private bool _isGravity = true;
    private BoxCollider _playerBoxCollider;
    private bool _AButton, _BButton, _XButton, _YButton, _L3Button;

    //Animation
    private Animator _animator;

    private int _verticalVelocityAnimator = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimator = Animator.StringToHash("HorizontalVelocity");
    private int _pushingAnimator = Animator.StringToHash("Pushing");
    private int _climbingLadderAnimator = Animator.StringToHash("ClimbingLadder");

    //Animation --> Walking
    public bool _isWalking;
    
    //Animation --> Crouching
    private bool _isCrouching;
    private int _crouchingAnimator = Animator.StringToHash("Crouching");

    [Header("Animations")]
    //Animation --> Cover
    [SerializeField] private GameObject[] _cover;
    private bool _isCover;

    //Animation --> Ladder
    [SerializeField] private GameObject _ladder;

    private bool _isClimbingLadder;

    //Animation --> Rope
    [SerializeField] private GameObject _rope;

    private bool _isClimbingRope;

    //Animation --> Hanging
    [SerializeField] private GameObject _ironBar;
    [SerializeField] private Transform _ironBarPos;

    private bool _isHanging;

    //Animation --> Pushing Box
    [SerializeField] private Rigidbody _crateRigidBody;
    [SerializeField] private float _mass;
    [SerializeField] private GameObject _crate;
    [SerializeField] private Transform _stopPos;

    private bool _isJumpOnCrate;
    private bool _isPushingCrate;

    //User Interface
    [SerializeField] private GameObject _interactMessage;

    private void Start()
    {
        _player = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerBoxCollider = GetComponent<BoxCollider>();

        _minXZVelocity = (5.0f * 1000) / (60 * 60); //[m/s], 5km/h
        _maxXZVelocity = (12.0f * 1000) / (60 * 60); //[m/s], 12km/h

        _mass = _crateRigidBody.mass;
    }

    private void Update()
    {
        //Character
        _verticalInput = Input.GetAxis("360_Vertical");
        _horizontalInput = Input.GetAxis("360_Horizontal");

        //Controller Buttons
        _AButton = Input.GetKey(KeyCode.Joystick1Button0);
        _BButton = Input.GetKey(KeyCode.Joystick1Button1);
        _XButton = Input.GetKey(KeyCode.Joystick1Button2);
        _YButton = Input.GetKey(KeyCode.Joystick1Button3);
        _L3Button = Input.GetKey(KeyCode.Joystick1Button8);

        _inputMovement = new Vector3(0.0f, 0.0f, _verticalInput);

        if (!_isCover && !_isClimbingLadder && !_isPushingCrate && !_isHanging)
            transform.Rotate(0.0f, _horizontalInput, 0.0f);

        if (_isCover)
            _inputMovement = new Vector3(_horizontalInput, 0.0f, 0.0f);

        if (_isClimbingLadder)
            _inputMovement = new Vector3(0.0f, _verticalInput * 0.3f, 0.0f);

        if (_isClimbingRope)
            _inputMovement = new Vector3(0.0f, _verticalInput * 0.3f, 0.0f);

        if (_isHanging)
            _inputMovement = new Vector3(_horizontalInput * 0.3f, 0.0f, 0.0f);

        //Input
        ApplyClampInput();
    }

    private void FixedUpdate()
    {
        ApplyGround();

        if (_isGravity)
            ApplyGravity();

        if (_isWalking)
        {
            //Character
            ApplyMovePlayer();

            //Animation
            ApplyMovementAnimation();
            ApplyCrouchingAnimation();
            ApplyCoverAnimation();
            ApplyLadderAnimation();
            ApplyRopeAnimation();
            ApplyHangingAnimation();
            ApplyPushingAnimation();
        }
    }

    //Input
    private void ApplyClampInput()
    {
        float max = 0.9f;
        float min = 0.1f;
        float input = 1.0f;

        if (_horizontalInput < max && _horizontalInput > min)
        {
            _horizontalInput = input;
        }
        if (_horizontalInput > -max && _horizontalInput < -min)
        {
            _horizontalInput = -input;
        }
        if (_verticalInput < max && _verticalInput > min)
        {
            _verticalInput = input;
        }
        if (_verticalInput > -max && _verticalInput < -min)
        {
            _verticalInput = -input;
        }
    }

    private void ApplyGround()
    {
        if (_player.isGrounded)
            _velocity -= Vector3.Project(_velocity, Physics.gravity);
    }

    private void ApplyGravity()
    {
        if (!_player.isGrounded)
        {
            _isGravity = true;
            _velocity.y += Physics.gravity.y * Time.fixedDeltaTime;

            Vector3 displacement = _velocity;
            _player.SimpleMove(displacement);
        }
    }

    private void ApplyMovePlayer()
    {
        _velocity = _inputMovement * _speed * Time.deltaTime;
        _velocity = transform.TransformDirection(_velocity);

        _player.Move(_velocity * _speed);
    }

    //Animation
    private void ApplyMovementAnimation()
    {
        _animator.SetFloat(_verticalVelocityAnimator, _verticalInput);
        _animator.SetFloat(_horizontalVelocityAnimator, _horizontalInput);

        float acceleration = 2.0f;
        float zoomedInFOV = 80.0f;
        bool _isRunning;

        if (!_isPushingCrate && !_isHanging && !_isClimbingRope && !_isClimbingLadder && !_isCover && !_isCrouching && _L3Button)
        {
            _isRunning = true;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomedInFOV, Time.deltaTime * _speed);
            _speed += acceleration * Time.fixedDeltaTime;
            _animator.SetFloat(_verticalVelocityAnimator, _verticalInput * 2.0f);

            if (_speed >= _maxXZVelocity && _isRunning)
                _speed = _maxXZVelocity;
        }
        else
        {
            _speed -= acceleration * Time.fixedDeltaTime;
            _speed = _minXZVelocity;
        }
    }

    private void ApplyCrouchingAnimation()
    {
        if (_AButton)
        {
            _isCrouching = !_isCrouching;
            _animator.SetBool("IsCrouching", _isCrouching);
            _animator.SetFloat(_crouchingAnimator, 1);
        }    
    }

    private void ApplyCoverAnimation()
    {
        foreach (GameObject cover in _cover)
        {
            if (_playerBoxCollider.bounds.Intersects(cover.GetComponent<BoxCollider>().bounds))
            {
                _isCover = true;
                transform.forward = cover.transform.forward;
                _animator.SetBool("IsCover", _isCover);
            }
            if (_isCover && _BButton)
            {
                _isCover = false;
                _animator.SetBool("IsCover", _isCover);
            }
        }
    }

    private void ApplyLadderAnimation()
    {
        if (_playerBoxCollider.bounds.Intersects(_ladder.GetComponent<BoxCollider>().bounds))
        {
            _isClimbingLadder = true;
            transform.forward = _ladder.transform.forward;
        }
        else
        {
            _isClimbingLadder = false;
        }
        if (_isClimbingLadder)
        {
            _animator.SetBool(_climbingLadderAnimator, true);
            _isGravity = false;
            _animator.SetBool("IsClimbingLadder", _isClimbingLadder);
        }
        if (_verticalInput != 0 && _isClimbingLadder)
        {
            _velocity = new Vector3(transform.position.x, transform.position.y + 0.01f * Time.fixedDeltaTime, transform.position.z);
        }
        if (!_isClimbingLadder)
        {
            _isClimbingLadder = false;
            _animator.SetBool("IsClimbingLadder", _isClimbingLadder);
        }
        if(!_isClimbingLadder && !_isClimbingRope)
        {
            _isGravity = true;
        }
    }

    private void ApplyRopeAnimation()
    {
        if (_playerBoxCollider.bounds.Intersects(_rope.GetComponent<BoxCollider>().bounds))
        {
            _isClimbingRope = true;
            transform.forward = _rope.transform.forward;
        }
        else
        {
            _isClimbingRope = false;
        }
        if (_isClimbingRope)
        {
            _isGravity = false;
            _animator.SetBool("IsClimbingRope", _isClimbingRope);
        }
        if (_verticalInput != 0 && _isClimbingRope)
        {
            _velocity = new Vector3(transform.position.x, transform.position.y + 0.01f * Time.fixedDeltaTime, transform.position.z);
        }
        if (!_isClimbingRope)
        {
            _isClimbingRope = false;
            _animator.SetBool("IsClimbingRope", _isClimbingRope);
        }
    }

    private void ApplyHangingAnimation()
    {
        if (transform.position.y > _ironBarPos.position.y && transform.position.x > _ironBarPos.position.x && _YButton)
        {
            _isHanging = true;
            transform.position = new Vector3(transform.position.x, _ironBar.transform.position.y - 2.3f, _ironBar.transform.position.z);
            transform.forward = _ironBar.transform.forward;
        }
        else
        {
            _isHanging = false;
        }
        if (_isHanging)
        {
            _isGravity = false;
            _animator.SetBool("IsHanging", _isHanging);
        }
        if (_verticalInput != 0 && _isHanging)
        {
            _velocity = new Vector3(transform.position.x + 0.01f * Time.fixedDeltaTime, transform.position.y, transform.position.z);
        }
        if (!_isHanging)
        {
            _isHanging = false;
            _animator.SetBool("IsHanging", _isHanging);
        }
    }

    private void ApplyPushingAnimation()
    {
        if (_playerBoxCollider.bounds.Intersects(_crate.GetComponent<BoxCollider>().bounds))
        {
            _interactMessage.SetActive(true);
            _animator.SetBool(_pushingAnimator, true);
        }
        else
        {
            _interactMessage.SetActive(false);
            _isPushingCrate = false;
        }
        if (_YButton && _playerBoxCollider.bounds.Intersects(_crate.GetComponent<BoxCollider>().bounds))
        {
            _isPushingCrate = true;
            transform.forward = _crate.transform.forward;
        }
        if (_isPushingCrate)
        {
            _animator.SetBool("IsPushing", _isPushingCrate);
        }
        if (_verticalInput != 0 && _isPushingCrate)
        {
            _crateRigidBody.isKinematic = false;
            _crateRigidBody.AddForce(Vector3.left * _mass);
        }
        if (!_isPushingCrate || _crate.transform.position.x >= _stopPos.position.x)
        {
            _crateRigidBody.isKinematic = true;
            _isPushingCrate = false;
            _animator.SetBool("IsPushing", _isPushingCrate);
        }
        if (_crate.transform.position.x >= _stopPos.position.x && _YButton)
        {
            _isJumpOnCrate = !_isJumpOnCrate;   
            _animator.SetBool("IsJumpOnCrate", _isJumpOnCrate);

            if (_isJumpOnCrate)
            {
                _playerBoxCollider.isTrigger = true;
                _crate.GetComponent<BoxCollider>().isTrigger = true;
            }
            if (!_isJumpOnCrate)
            {
                _crate.transform.position = new Vector3(_crate.transform.position.x - 0.05f, _crate.transform.position.y, _crate.transform.position.z);
                _playerBoxCollider.isTrigger = false;
                _crate.GetComponent<BoxCollider>().isTrigger = false;
            }
        }
    }
}