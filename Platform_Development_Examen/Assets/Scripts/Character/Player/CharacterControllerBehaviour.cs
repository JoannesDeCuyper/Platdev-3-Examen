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

    public bool _isWalking;

    private CharacterController _player;
    private Vector3 _inputMovement;
    private Vector3 _velocity = Vector3.zero;
    private BoxCollider _playerBoxCollider;
    private float _verticalInput, _horizontalInput;
    public bool _isGravity = true;
    private bool _AButton, _BButton, _XButton, _YButton, _L3Button;

    [Header("Computer")]
    //Hacking computer
    [SerializeField] private GameObject _computer;
    [SerializeField] private AnimationClip _clip;
    [SerializeField] private float _timer;

    private bool _ishacking;
    public bool IsHacking;

    //Animation
    private Animator _animator;
    private int _verticalVelocityAnimator = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimator = Animator.StringToHash("HorizontalVelocity");
    private int _pushingAnimator = Animator.StringToHash("IsPushing");
    private int _climbingLadderAnimator = Animator.StringToHash("IsClimbingLadder");
    private int _climbingRopeAnimator = Animator.StringToHash("IsClimbingRope");
    private int _hangingAnimator = Animator.StringToHash("IsHanging");

    [Header("Animations")]
    //Animation --> Crouching
    private bool _isCrouching;

    //Animation --> Cover
    [SerializeField] private GameObject[] _crouchingCover;
    [SerializeField] private GameObject[] _StandingCover;

    private bool _isCrouchingCover;
    private bool _isStandingCover;

    //Animation --> Ladder
    [SerializeField] private GameObject _ladder;

    private bool _isClimbingLadder;

    //Animation --> Rope
    [SerializeField] private GameObject _rope;

    private bool _isClimbingRope;

    //Animation --> Hanging
    [SerializeField] private GameObject _ironBar;
    [SerializeField] private Transform _ironBarPos;

    private bool _isHanging = false;

    //Animation --> Pushing Box
    [SerializeField] private Rigidbody _crateRigidBody;
    [SerializeField] private GameObject _crate;
    [SerializeField] private Transform _stopPos;

    private float _mass;
    private bool _isJumpOnCrate;
    private bool _isPushingCrate;

    [Header("User Iterface")]
    //User Interface
    [SerializeField] private GameObject _interactBoxMessage;
    [SerializeField] private GameObject _interactHackMessage;
    [SerializeField] private GameObject _cameraMessage;

    [Header("IK")]
    //IK
    [SerializeField] private Transform _leftHandBoxTarget;
    [SerializeField] private Transform _rightHandBoxTarget;
    [SerializeField] private bool _isIK;

    //Lights
    [Header("Lights")]
    [SerializeField] private GameObject _lightTrigger;

    private GameObject[] _lights;
    public bool IsLightsOn = true;

    public bool _isOnGround;

    private void Start()
    {
        //Player
        _player = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerBoxCollider = GetComponent<BoxCollider>();

        _minXZVelocity = (5.0f * 1000) / (60 * 60); //[m/s], 5km/h
        _maxXZVelocity = (12.0f * 1000) / (60 * 60); //[m/s], 12km/h

        _mass = _crateRigidBody.mass;

        //IK
        _leftHandBoxTarget = _animator.GetBehaviour<PushingBoxBehaviour>().LeftHandBoxTarget;
        _rightHandBoxTarget = _animator.GetBehaviour<PushingBoxBehaviour>().RightHandBoxTarget;
        _isIK = _animator.GetBehaviour<PushingBoxBehaviour>().IsIK;

        //Lights
        _lights = GameObject.FindGameObjectsWithTag("Lights");

        //Hacking computer
        _timer = _clip.length;
    }

    private void Update()
    {
        //Character
        _verticalInput = Input.GetAxis("360_Vertical");
        _horizontalInput = Input.GetAxis("360_Horizontal");

        //Controller Buttons
        _AButton = Input.GetKeyUp(KeyCode.Joystick1Button0);
        _BButton = Input.GetKey(KeyCode.Joystick1Button1);
        _XButton = Input.GetKey(KeyCode.Joystick1Button2);
        _YButton = Input.GetKey(KeyCode.Joystick1Button3);
        _L3Button = Input.GetKey(KeyCode.Joystick1Button8);

        _inputMovement = new Vector3(0.0f, 0.0f, _verticalInput);

        if (!_ishacking && !_isCrouchingCover && !_isStandingCover && !_isClimbingLadder && !_isPushingCrate && !_isClimbingRope && !_isHanging)
            transform.Rotate(0.0f, _horizontalInput, 0.0f);

        if (_isCrouchingCover || _isStandingCover || _isHanging)
            _inputMovement = new Vector3(_horizontalInput, 0.0f, 0.0f);

        if (_isClimbingLadder || _isClimbingRope)
            _inputMovement = new Vector3(0.0f, _verticalInput * 0.3f, 0.0f);

        //if (_isHanging)
        //    _inputMovement = new Vector3(_horizontalInput * 0.3f, 0.0f, 0.0f);

        //Input
        ApplyClampInput();
    }

    private void FixedUpdate()
    {
        ApplyGroundPlayer();

        if (_isGravity)
            ApplyGravityPlayer();

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
            ApplyJumpOnCrateAnimation();
        }

        //Hacking computer
        HackingComputer();

        //Lights
        TurnOffLights();
    }

    //Input
    private void ApplyClampInput()
    {
        float max = 0.9f;
        float min = 0.1f;
        float input = 1.0f;

        if (_horizontalInput < max && _horizontalInput > min)
            _horizontalInput = input;
        
        if (_horizontalInput > -max && _horizontalInput < -min)   
            _horizontalInput = -input;
        
        if (_verticalInput < max && _verticalInput > min)    
            _verticalInput = input;
        
        if (_verticalInput > -max && _verticalInput < -min) 
            _verticalInput = -input;
        
    }

    //Player
    private void ApplyGroundPlayer()
    {
        if (_player.isGrounded)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity);
        }
    }

    private void ApplyGravityPlayer()
    {
        if (!_player.isGrounded)
        {
            _isGravity = true;
            _velocity.y += Physics.gravity.y * Time.fixedDeltaTime;

            _player.Move(_velocity);
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
        float normalFOV = 60.0f;

        bool _isRunning;

        if (!_isPushingCrate && !_isHanging && !_isClimbingRope && !_isClimbingLadder && !_isCrouchingCover && !_isCrouching && _L3Button)
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
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, normalFOV, Time.deltaTime * _speed);
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
        }    
    }

    private void ApplyCoverAnimation()
    {
        foreach (GameObject crouchingCover in _crouchingCover)
        {
            if (_playerBoxCollider.bounds.Intersects(crouchingCover.GetComponent<BoxCollider>().bounds))
            {
                _isCrouchingCover = true;
                transform.forward = crouchingCover.transform.forward;
                _animator.SetBool("IsCover", _isCrouchingCover);
            }
            if (_isCrouchingCover && _BButton)
            {
                _isCrouchingCover = false;
                _animator.SetBool("IsCover", _isCrouchingCover);
            }
        }
        foreach (GameObject standingCover in _StandingCover)
        {
            if (_playerBoxCollider.bounds.Intersects(standingCover.GetComponent<BoxCollider>().bounds))
            {
                _isStandingCover = true;
                transform.forward = standingCover.transform.forward;
                _animator.SetBool("IsStandingCover", _isStandingCover);
            }
            if (_isStandingCover && _BButton)
            {
                _isStandingCover = false;
                _animator.SetBool("IsStandingCover", _isStandingCover);
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
            _isClimbingLadder = false;
        
        if (_isClimbingLadder)
        {
            _animator.SetBool(_climbingLadderAnimator, true);
            _isGravity = false;
            _animator.SetBool("IsClimbingLadder", _isClimbingLadder);
        }
        if (_verticalInput != 0 && _isClimbingLadder)
            _velocity = new Vector3(transform.position.x, transform.position.y + 0.01f * Time.fixedDeltaTime, transform.position.z);
       
        if (!_isClimbingLadder)
        {
            _isClimbingLadder = false;
            _animator.SetBool("IsClimbingLadder", _isClimbingLadder);
        }
        if(!_isClimbingLadder && !_isClimbingRope)
            _isGravity = true;
    }

    private void ApplyRopeAnimation()
    {
        if (_playerBoxCollider.bounds.Intersects(_rope.GetComponent<BoxCollider>().bounds))
        {
            _isClimbingRope = true;
            transform.forward = _rope.transform.forward;
        }
        else
            _isClimbingRope = false;

        if (_isClimbingRope)
        {
            _animator.SetBool(_climbingRopeAnimator, true);
            _isGravity = false;
            _animator.SetBool("IsClimbingRope", _isClimbingRope);
        }
        if (_verticalInput != 0 && _isClimbingRope)
            _velocity = new Vector3(transform.position.x, transform.position.y + 0.01f * Time.fixedDeltaTime, transform.position.z);

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
            transform.position = new Vector3(transform.position.x, _ironBar.transform.position.y - 2.3f, _ironBar.transform.position.z + 0.5f);
            transform.forward = _ironBar.transform.forward;
        }
        else
            _isHanging = false;

        if (_isHanging)
        {
            _animator.SetBool(_hangingAnimator, true);
            _isGravity = false;
            _animator.SetBool("IsHanging", _isHanging);
        }
        if (_verticalInput != 0 && _isHanging)
            _velocity = new Vector3(transform.position.x + 0.01f * Time.fixedDeltaTime, transform.position.y, transform.position.z);
       
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
            PushingBoxBehaviour b = _animator.GetBehaviour<PushingBoxBehaviour>();
            b.IsIK = true;
            b.LeftHandBoxTarget = _leftHandBoxTarget;
            _isIK = true;
            _interactBoxMessage.SetActive(true);
            _animator.SetBool(_pushingAnimator, true);
        }
        else
        {
            _interactBoxMessage.SetActive(false);
            _isPushingCrate = false;
        }
        if (_YButton && _playerBoxCollider.bounds.Intersects(_crate.GetComponent<BoxCollider>().bounds))
        {
            _isPushingCrate = true;
            transform.forward = _crate.transform.forward;
        }
        if (_isPushingCrate)
            _animator.SetBool("IsPushing", _isPushingCrate);
        
        if (_verticalInput != 0 && _isPushingCrate)
        {
            _crateRigidBody.isKinematic = false;
            _crateRigidBody.AddForce(Vector3.left * _mass);
        }
        if (!_isPushingCrate || _crate.transform.position.x >= _stopPos.position.x)
        {
            _isIK = false;
            _crateRigidBody.isKinematic = true;
            _isPushingCrate = false;
            _animator.SetBool("IsPushing", _isPushingCrate);
        }
    }

    private void ApplyJumpOnCrateAnimation()
    {
        if (_crate.transform.position.x >= _stopPos.position.x && _YButton)
        {
            _isJumpOnCrate = !_isJumpOnCrate;
            _animator.SetBool("IsJumpOnCrate", _isJumpOnCrate);

            if (_isJumpOnCrate)
            {
                _animator.SetBool("IsJumpOnCrate", _isJumpOnCrate);
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

    //Hacking computer
    private void HackingComputer()
    {
        if(_playerBoxCollider.bounds.Intersects(_computer.GetComponent<MeshCollider>().bounds))
            _interactHackMessage.SetActive(true);
        
        if (_YButton && _playerBoxCollider.bounds.Intersects(_computer.GetComponent<MeshCollider>().bounds))
        {
            _ishacking = true;
            _animator.SetBool("IsPushingButton", _ishacking);
            _cameraMessage.SetActive(true);
            _interactHackMessage.SetActive(false);
            _computer.GetComponent<MeshCollider>().enabled = false;
            _computer.GetComponent<BoxCollider>().enabled = true;
            IsHacking = true;
        }
        if (_animator.GetBool("IsPushingButton"))
        {
            _speed = 0;
            _isWalking = false;
            _timer -= Time.fixedDeltaTime;
        }
        if (_timer <= 0)
        {
            _ishacking = false;
            _speed = _minXZVelocity;
            _timer = 1;
            _animator.SetBool("IsPushingButton", _ishacking);
        }
    }

    //Lights
    private void TurnOffLights()
    {
        if(_playerBoxCollider.bounds.Intersects(_lightTrigger.GetComponent<BoxCollider>().bounds))
            IsLightsOn = false;
        
        if(IsLightsOn)
        {
            for (int i = 0; i < _lights.Length; i++)
                _lights[i].SetActive(true);    
        }
        if (!IsLightsOn)
        {
            for (int i = 0; i < _lights.Length; i++)
                _lights[i].SetActive(false);
        }
    }
}