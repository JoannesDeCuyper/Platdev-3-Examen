using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class CharacterControllerBehaviour : MonoBehaviour
{
    //Player
    [Header("Player")]
    [SerializeField] NavMeshAgent _agent;
    public bool _isWalking;

    private float _maxXZVelocity;
    private float _minXZVelocity;
    private float _speed;
    private CharacterController _player;
    private Vector3 _inputMovement;
    private Vector3 _velocity = Vector3.zero;
    private BoxCollider _playerBoxCollider;
    private float _verticalInput, _horizontalInput;
    private bool _isGravity = true;
    private bool _AButton, _BButton, _XButton, _YButton, _L3Button;

    //Hacking computer
    [Header("Computer")]
    [SerializeField] private GameObject _computer;
    [SerializeField] private AnimationClip _hackingClip;
    [SerializeField] private float _hackingTimer;
    public bool IsHacking;

    private bool _isHacking;

    //Animation
    private Animator _animator;
    private int _verticalVelocityAnimator = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimator = Animator.StringToHash("HorizontalVelocity");
    private int _pushingAnimator = Animator.StringToHash("IsPushing");
    private int _climbingLadderAnimator = Animator.StringToHash("IsClimbingLadder");
    private int _climbingRopeAnimator = Animator.StringToHash("IsClimbingRope");
    private int _hangingAnimator = Animator.StringToHash("IsHanging");

    //Animation --> Crouching
    private bool _isCrouching;

    [Header("Animations")]
    //Animation --> Cover
    [SerializeField] private GameObject[] _crouchingCover;
    [SerializeField] private GameObject[] _standingCover;

    public bool _isCrouchingCover;
    public bool _isStandingCover;

    //Animation --> Ladder
    [SerializeField] private GameObject _ladder;

    private bool _isClimbingLadder;

    //Animation --> Rope
    [SerializeField] private GameObject _rope;

    private bool _isClimbingRope;

    //Animation --> Wall
    [SerializeField] private float _wallTimer = 1;
    [SerializeField] private bool _isClimbingWall = false;

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
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;
            //Crate
    [SerializeField] private Transform _leftHandCrateTarget;
    [SerializeField] private Transform _rightHandCrateTarget;
            //Rope
    [SerializeField] private Transform[] _leftHandRopeTarget;
    [SerializeField] private Transform[] _rightHandRopeTarget;
    [SerializeField] private Transform _leftFootRopeTarget;
    [SerializeField] private Transform _rightFootRopeTarget;
    [SerializeField] private int _leftHandRopeNumber;
    [SerializeField] private int _rightHandRopeNumber;
            //Wall
    [SerializeField] private Transform _wallTarget;
    [SerializeField] private Transform _wallPos;

    private bool _isOnWall;
            //Ladder
    [SerializeField] private Transform[] _leftHandLadderTarget;
    [SerializeField] private Transform[] _rightHandLadderTarget;
    [SerializeField] private int _leftHandLadderNumber;
    [SerializeField] private int _rightHandLadderNumber;
            //Ball
    [SerializeField] private GameObject _ball;
    [SerializeField] private Transform _rightHandBallTarget;

    private bool _pickingUpBall;

    private PushingBoxBehaviour _pushingIKBehaviour;
    private ClimbingRopeBehaviour _climbingRopeIKBehaviour;
    private ClimbingLadderBehaviour _climbingLadderIKBehaviour;
    private PickUpBallBehaviour _pickingUpBallBehaviour;

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
        _pushingIKBehaviour = _animator.GetBehaviour<PushingBoxBehaviour>();
        _climbingRopeIKBehaviour = _animator.GetBehaviour<ClimbingRopeBehaviour>();
        _climbingLadderIKBehaviour = _animator.GetBehaviour<ClimbingLadderBehaviour>();
        _pickingUpBallBehaviour = _animator.GetBehaviour<PickUpBallBehaviour>();

        //Lights
        _lights = GameObject.FindGameObjectsWithTag("Lights");

        //Hacking computer
        _hackingTimer = _hackingClip.length;
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

        if (!_isOnWall && !_isHacking && !_isCrouchingCover && !_isStandingCover && !_isClimbingLadder && !_isPushingCrate && !_isClimbingRope && !_isHanging)
            transform.Rotate(0.0f, _horizontalInput, 0.0f);

        if (_isCrouchingCover || _isStandingCover || _isHanging)
            _inputMovement = new Vector3(_horizontalInput * 0.4f, 0.0f, 0.0f);

        if (_isClimbingLadder)
            _inputMovement = new Vector3(0.0f, _verticalInput * 0.41f, 0.0f);

        if (_isClimbingRope)
            _inputMovement = new Vector3(0.0f, _verticalInput, 0.0f);

        if (_isOnWall && _horizontalInput != 0)
        {
            _wallTarget.position += new Vector3(_horizontalInput * 0.5f * Time.deltaTime, 0, 0);
            _wallPos.position += new Vector3(_horizontalInput * 0.5f * Time.deltaTime, 0, 0);
        }

        //if (transform.position == _wallTarget.position)
        //{
        //    _leftHandRopeNumber = 0;
        //    _rightHandRopeNumber = 0;
        //}

        if (_leftHandRopeNumber >= 10)
            _leftHandRopeNumber = 10;

        if (_XButton)
            _wallTimer -= Time.deltaTime;

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
            ApplyWallAnimation();
            ApplyHangingAnimation();
            ApplyPushingAnimation();
            ApplyBallAnimation();
            ApplyJumpOnCrateAnimation();
        }

        //Hacking computer
        ApplyHackingComputer();

        //Lights
        ApplyTurnOffLights();
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

            //Set speed to running speed
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
        //Small cover objects
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
        //Big cover objects
        foreach (GameObject standingCover in _standingCover)
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
            _climbingLadderIKBehaviour.LeftHandLadderTarget = _leftHandLadderTarget[_leftHandLadderNumber];
            _climbingLadderIKBehaviour.RightHandLadderTarget = _rightHandLadderTarget[_rightHandLadderNumber];

            if(_leftHand.position.y >= _leftHandLadderTarget[_leftHandLadderNumber].position.y)
            {
                _leftHandLadderNumber += 1;
            }
            if (_rightHand.position.y >= _rightHandLadderTarget[_rightHandLadderNumber].position.y)
            {
                _rightHandLadderNumber += 1;
            }
            if(_leftHand.position.y >= _leftHandLadderTarget[9].position.y)
            {
                transform.position = _ironBarPos.position;
            }

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
        //Gravity
        if (!_isClimbingLadder && !_isClimbingRope)
            _isGravity = true;
    }

    private void ApplyRopeAnimation()
    {
        if (_playerBoxCollider.bounds.Intersects(_rope.GetComponent<BoxCollider>().bounds))
        {
            //Hand
            _climbingRopeIKBehaviour.LeftHandRopeTarget = _leftHandRopeTarget[_leftHandRopeNumber];
            _climbingRopeIKBehaviour.RightHandRopeTarget = _rightHandRopeTarget[_rightHandRopeNumber];
            //Foot
            _climbingRopeIKBehaviour.LeftFootRopeTarget = _leftFootRopeTarget;
            _climbingRopeIKBehaviour.RightFootRopeTarget = _rightFootRopeTarget;

            if (_leftHand.position.y >= _leftHandRopeTarget[_leftHandRopeNumber].position.y)
            {
                _leftHandRopeNumber += 1;
            }
            if (_rightHand.position.y >= _rightHandRopeTarget[_rightHandRopeNumber].position.y)
            {
                _rightHandRopeNumber += 1;
            }

            _isClimbingRope = true;
        }
        else
            _isClimbingRope = false;

        if (_isClimbingRope)
        {
            transform.forward = _rope.transform.forward;
            transform.position = new Vector3(_rope.transform.position.x, transform.position.y, transform.position.z);
            _animator.SetBool(_climbingRopeAnimator, true);
            _isGravity = false;
            _animator.SetBool("IsClimbingRope", _isClimbingRope);
        }

        //if (_verticalInput != 0 && _isClimbingRope) 
        //    _velocity = new Vector3(transform.position.x, transform.position.y + 0.01f * Time.fixedDeltaTime, transform.position.z);

        if (!_isClimbingRope)
        {
            _isClimbingRope = false;
            _animator.SetBool("IsClimbingRope", _isClimbingRope);
        }
    }

    private void ApplyWallAnimation()
    {
        if (_rightHandRopeNumber >= 10)
        {
            _rightHandRopeNumber = 10;
            transform.position = _wallTarget.position;
            _isOnWall = true;
            _animator.SetBool("IsOnWall", _isOnWall);
        }
        if(_isOnWall && _XButton)
        {
            _isClimbingWall = true;
            _animator.SetBool("IsClimbingWall",_isClimbingWall);
        }
        if (_isClimbingWall && _YButton)
        {
            _rightHandRopeNumber = 0;
            _leftHandRopeNumber = 0;
            _isOnWall = false;
            _isClimbingWall = false;
            _animator.SetBool("IsOnWall", _isOnWall);
            _animator.SetBool("IsClimbingWall", false);
            transform.position = _wallPos.position;
        }
    }

    private void ApplyHangingAnimation()
    {
        if (transform.position.y > _ironBarPos.position.y && transform.position.x > _ironBarPos.position.x && _YButton)
        {
            _isHanging = true;
            transform.position = new Vector3(transform.position.x, _ironBar.transform.position.y - 2.35f, _ironBar.transform.position.z);
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
            _pushingIKBehaviour.IsIK = true;
            _pushingIKBehaviour.LeftHandBoxTarget = _leftHandCrateTarget;
            _pushingIKBehaviour.RightHandBoxTarget = _rightHandCrateTarget;

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
            _crateRigidBody.isKinematic = true;
            _isPushingCrate = false;
            _animator.SetBool("IsPushing", _isPushingCrate);
        }
    }

    private void ApplyBallAnimation()
    {
        if (_playerBoxCollider.bounds.Intersects(_ball.GetComponent<SphereCollider>().bounds))
        {
            _pickingUpBall = true;
            _animator.SetBool("IsPickingUp", _pickingUpBall);
            _pickingUpBallBehaviour.RightHandBallTarget = _rightHandBallTarget;
        }
        else
        { 
            _pickingUpBall = false;
            _animator.SetBool("IsPickingUp", _pickingUpBall);
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
    private void ApplyHackingComputer()
    {
        //Display user interface feedback
        if(_playerBoxCollider.bounds.Intersects(_computer.GetComponent<MeshCollider>().bounds))
            _interactHackMessage.SetActive(true);
        
        if (_YButton && _playerBoxCollider.bounds.Intersects(_computer.GetComponent<MeshCollider>().bounds))
        {
            _isHacking = true;
            _animator.SetBool("IsPushingButton", _isHacking);
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
            _hackingTimer -= Time.fixedDeltaTime;
        }
        if (_hackingTimer <= 0)
        {
            _isHacking = false;
            _speed = _minXZVelocity;
            _hackingTimer = 1;
            _animator.SetBool("IsPushingButton", _isHacking);
        }
    }

    //Lights
    private void ApplyTurnOffLights()
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