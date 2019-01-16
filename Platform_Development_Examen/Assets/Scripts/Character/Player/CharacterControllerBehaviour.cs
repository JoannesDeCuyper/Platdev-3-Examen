using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class CharacterControllerBehaviour : MonoBehaviour
{
    //Player
    [Header("Player")]
    [SerializeField] private Transform _yBotPlayer;
    [SerializeField] private float _dragOnGround;
    public bool IsWalking;

    private CharacterController _player;
    private BoxCollider _playerBoxCollider;
    private Vector3 _inputMovement;
    private Vector3 _velocity = Vector3.zero;
    private float _verticalInput, _horizontalInput;
    private float _maxXZVelocity, _minXZVelocity;
    private float _speed;
    private bool _isGravity = true;
    private bool _AButton, _BButton, _XButton, _YButton, _L3Button;

    //Hacking computer
    [Header("Computer")]
    [SerializeField] private GameObject _computer;
    [SerializeField] private AnimationClip _hackingClip;
    public bool IsHacking;

    private float _hackingTimer;
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
    [Header("Cover")]
    //Animation --> Cover
    [SerializeField] private GameObject[] _crouchingCover;
    [SerializeField] private GameObject[] _standingCover;
    public bool IsCrouchingCover;
    public bool IsStandingCover;

    [Header("Ladder")]
    //Animation --> Ladder
    [SerializeField] private GameObject _ladder;

    private bool _isClimbingLadder;

    [Header("Rope")]
    //Animation --> Rope
    [SerializeField] private GameObject _rope;

    private bool _isClimbingRope;

    [Header("IronBar")]
    //Animation --> Hanging
    [SerializeField] private GameObject _ironBar;
    [SerializeField] private Transform _ironBarPos;

    private bool _isJumpingOnRope;
    private bool _isHanging = false;

    [Header("Crate")]
    //Animation --> Pushing Crate
    [SerializeField] private GameObject _crate;
    [SerializeField] private Transform _stopPos;

    private Rigidbody _crateRigidBody;
    private float _mass;
    private bool _isJumpOnCrate;
    private bool _isPushingCrate;

    //Animation --> Wall
    private bool _isClimbingWall = false;

    [Header("User Iterface")]
    //User Interface
    [SerializeField] private GameObject _interactBallMessage;
    [SerializeField] private GameObject _interactCrateMessage;
    [SerializeField] private GameObject _interactHackMessage;
    [SerializeField] private GameObject _interactClimbWallMessage;
    [SerializeField] private GameObject _interactStandUpMessage;
    [SerializeField] private GameObject _interactGetOutOfCoverMessage;
    [SerializeField] private GameObject _interactThrowBallMessage;
    [SerializeField] private GameObject _interactTurnOffLightsMessage;
    [SerializeField] private GameObject _interactIronBarMessage;
    [SerializeField] private GameObject _cameraMessage;
    [SerializeField] private GameObject _winScreen;

    [Header("IK")]
                        //Hands
    [Header("Hands")]
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;
                        //Crate
    [Header("Crate")]
    [SerializeField] private Transform _leftHandCrateTarget;
    [SerializeField] private Transform _rightHandCrateTarget;
    [SerializeField] private Transform _cratePos;
                        //Rope
    [Header("Rope")]
    [SerializeField] private Transform[] _leftHandRopeTarget;
    [SerializeField] private Transform[] _rightHandRopeTarget;
    [SerializeField] private Transform _leftFootRopeTarget;
    [SerializeField] private Transform _rightFootRopeTarget;

    private int _leftHandRopeNumber;
    private int _rightHandRopeNumber;
                        //Wall
    [Header("Wall")]
    [SerializeField] private Transform _wallTarget;
    [SerializeField] private Transform _wallPos;
    [SerializeField] private Vector3 _currentWallTargetPosition;
    [SerializeField] private Vector3 _currentWallPosPosition;

    private bool _isOnWall;
                        //Ladder
    [Header("Ladder")]
    [SerializeField] private Transform[] _leftHandLadderTarget;
    [SerializeField] private Transform[] _rightHandLadderTarget;

    private int _leftHandLadderNumber;
    private int _rightHandLadderNumber;
                        //Ball
    [Header("Ball")]
    [SerializeField] private GameObject _ball;
    [SerializeField] private Transform _rightHandBallTarget;
    [SerializeField] private Transform _ballGroundTarget;
    [SerializeField] private AnimationClip _pickingUpClip;
    [SerializeField] private AnimationClip _throwingBallClip;
    public bool IsBallAtPosition;

    private int _throwBallNumber = 0;
    private float _pickingUpTimer;
    private float _throwingBallTimer;
    private bool _isPickingUpBall;
    private bool _isThrowIdle;
    private bool _isThrowing;

    //Lights
    [Header("Lights")]
    [SerializeField] private GameObject _lightTrigger;
    [SerializeField] private Transform _lightSwitchTarget;
    public bool IsLightsOn = true;

    private float _pushButtonTimer;
    private bool _isPushingButton;

    //Golden Egg
    [Header("GoldenEgg")]
    [SerializeField] private GameObject _goldenEgg;

    private bool _isDancing;
    private GameObject[] _lights;

    //IK Behaviour scripts
    private PushingBoxBehaviour _pushingBoxIKBehaviour;
    private ClimbingRopeBehaviour _climbingRopeIKBehaviour;
    private ClimbingLadderBehaviour _climbingLadderIKBehaviour;
    private PickUpBallBehaviour _pickUpBallIKBehaviour;
    private TurnOffLightsBehaviour _turnOffLightsIKBehaviour;

    private void Start()
    {
        //Player
        _player = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerBoxCollider = GetComponent<BoxCollider>();

        //IK
        _pushingBoxIKBehaviour = _animator.GetBehaviour<PushingBoxBehaviour>();
        _climbingRopeIKBehaviour = _animator.GetBehaviour<ClimbingRopeBehaviour>();
        _climbingLadderIKBehaviour = _animator.GetBehaviour<ClimbingLadderBehaviour>();
        _pickUpBallIKBehaviour = _animator.GetBehaviour<PickUpBallBehaviour>();
        _turnOffLightsIKBehaviour = _animator.GetBehaviour<TurnOffLightsBehaviour>();

        //Minimum and maximun speed
        _minXZVelocity = (5.0f * 1000) / (60 * 60); //[m/s], 5km/h
        _maxXZVelocity = (12.0f * 1000) / (60 * 60); //[m/s], 12km/h

        //Crate
        _crateRigidBody = _crate.GetComponent<Rigidbody>();
        _mass = _crateRigidBody.mass;

        //Hacking computer
        _hackingTimer = _hackingClip.length;

        //Lights
        _lights = GameObject.FindGameObjectsWithTag("Lights");
        _pushButtonTimer = _hackingClip.length;

        //Ball
        _pickingUpTimer = _pickingUpClip.length / 2;
        _throwingBallTimer = _throwingBallClip.length;

        //Wall
        _currentWallTargetPosition = _wallTarget.position;
        _currentWallPosPosition = _wallPos.position;
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

        if (!_isDancing && !_isOnWall && !_isHacking && !IsCrouchingCover && !IsStandingCover
            && !_isClimbingLadder && !_isPushingCrate && !_isClimbingRope && !_isHanging )
            transform.Rotate(0.0f, _horizontalInput, 0.0f);

        if (IsCrouchingCover || IsStandingCover || _isHanging)
            _inputMovement = new Vector3(_horizontalInput * 0.4f, 0.0f, 0.0f);

        if (_isClimbingLadder)
            _inputMovement = new Vector3(0.0f, _verticalInput * 0.41f, 0.0f);

        if (_isClimbingRope)
            _inputMovement = new Vector3(0.0f, _verticalInput * 0.1f, 0.0f);

        if (_isOnWall && _horizontalInput != 0)
        {
            _wallTarget.position += new Vector3(_horizontalInput * 0.5f * Time.deltaTime, 0.0f, 0.0f);
            _wallPos.position += new Vector3(_horizontalInput * 0.5f * Time.deltaTime, 0.0f, 0.0f);
        }

        if (_leftHandRopeNumber >= 10)
            _leftHandRopeNumber = 10;

        //Input
        ApplyClampInput();
    }

    private void FixedUpdate()
    {
        ApplyGroundPlayer();
        ApplyDragOnGround();

        if (_isGravity)
            ApplyGravityPlayer();

        if (IsWalking)
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

        //Golden Egg
        ApplyEndMission();
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
            _animator.SetBool("IsFalling", true);
            _player.Move(_velocity);
        }
        if (_player.isGrounded)
        {
            _isGravity = false;
            _animator.SetBool("IsFalling", false);
        }
    }

    private void ApplyDragOnGround()
    {
        _velocity *= (1 - Time.fixedDeltaTime * _dragOnGround);
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

        if (!_isPushingCrate && !_isHanging && !_isClimbingRope && !_isClimbingLadder 
            && !IsStandingCover && !IsCrouchingCover && !_isCrouching && _L3Button)
        {
            _isRunning = true;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomedInFOV, Time.deltaTime * _speed);
            _speed += acceleration * Time.fixedDeltaTime;
            _animator.SetFloat(_verticalVelocityAnimator, _verticalInput * acceleration);

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
                transform.forward = crouchingCover.transform.forward;

                IsCrouchingCover = true;
                _animator.SetBool("IsCover", IsCrouchingCover);
                _interactGetOutOfCoverMessage.SetActive(true);
            }

            if (IsCrouchingCover && _BButton)
            {
                IsCrouchingCover = false;
                _animator.SetBool("IsCover", IsCrouchingCover);
                _interactGetOutOfCoverMessage.SetActive(false);
            }
        }
        //Big cover objects
        foreach (GameObject standingCover in _standingCover)
        {
            if (_playerBoxCollider.bounds.Intersects(standingCover.GetComponent<BoxCollider>().bounds))
            {
                transform.forward = standingCover.transform.forward;

                IsStandingCover = true;
                _animator.SetBool("IsStandingCover", IsStandingCover);
                _interactGetOutOfCoverMessage.SetActive(true);
            }

            if (IsStandingCover && _BButton)
            {
                IsStandingCover = false;
                _animator.SetBool("IsStandingCover", IsStandingCover);
                _interactGetOutOfCoverMessage.SetActive(false);
            }
        }
    }

    private void ApplyLadderAnimation()
    {
        if (_playerBoxCollider.bounds.Intersects(_ladder.GetComponent<BoxCollider>().bounds))
        {
            int number = 1;

            _climbingLadderIKBehaviour.LeftHandLadderTarget = _leftHandLadderTarget[_leftHandLadderNumber];
            _climbingLadderIKBehaviour.RightHandLadderTarget = _rightHandLadderTarget[_rightHandLadderNumber];

            if(_leftHand.position.y >= _leftHandLadderTarget[_leftHandLadderNumber].position.y)
                _leftHandLadderNumber += number;
            
            if (_rightHand.position.y >= _rightHandLadderTarget[_rightHandLadderNumber].position.y)
                _rightHandLadderNumber += number;

            if (_leftHand.position.y >= _leftHandLadderTarget[9].position.y)
                transform.position = _ironBarPos.position;

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

        if (_verticalInput != 0.0f && _isClimbingLadder)
        {
            float speed = 0.01f;

            _velocity = new Vector3(transform.position.x, transform.position.y + speed * Time.fixedDeltaTime, transform.position.z);
        }

        if (!_isClimbingLadder)
        {
            _isClimbingLadder = false;
            _animator.SetBool("IsClimbingLadder", _isClimbingLadder);
        }

        if (transform.position.y > _ironBarPos.position.y && transform.position.x > _ironBarPos.position.x)
        {
            _leftHandLadderNumber = 0;
            _rightHandLadderNumber = 0;
        }

        //Gravity
        if (!_isClimbingLadder && !_isClimbingRope)
            _isGravity = true;
    }

    private void ApplyRopeAnimation()
    {
        if (_playerBoxCollider.bounds.Intersects(_rope.GetComponent<BoxCollider>().bounds))
        {
            int number = 1;

            //Hand
            _climbingRopeIKBehaviour.LeftHandRopeTarget = _leftHandRopeTarget[_leftHandRopeNumber];
            _climbingRopeIKBehaviour.RightHandRopeTarget = _rightHandRopeTarget[_rightHandRopeNumber];
            //Foot
            _climbingRopeIKBehaviour.LeftFootRopeTarget = _leftFootRopeTarget;
            _climbingRopeIKBehaviour.RightFootRopeTarget = _rightFootRopeTarget;

            if (_leftHand.position.y >= _leftHandRopeTarget[_leftHandRopeNumber].position.y)
                _leftHandRopeNumber += number;
            
            if (_rightHand.position.y >= _rightHandRopeTarget[_rightHandRopeNumber].position.y)
                _rightHandRopeNumber += number;

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
            Vector3 playerRotation = new Vector3(6.0f, 0.0f, 0.0f);

            _yBotPlayer.transform.rotation = Quaternion.Euler(playerRotation);
            _rightHandRopeNumber = 10;
            transform.position = _wallTarget.position;
            _isOnWall = true;
            _animator.SetBool("IsOnWall", _isOnWall);
            _interactClimbWallMessage.SetActive(true);
        }

        if(_isOnWall && _YButton)
        {
            _isClimbingWall = true;
            _animator.SetBool("IsClimbingWall",_isClimbingWall);
            _interactStandUpMessage.SetActive(true);
        }

        if(_interactStandUpMessage.activeSelf == true)
            _interactClimbWallMessage.SetActive(false);
        
        if (_isClimbingWall && _BButton)
        {
            _yBotPlayer.transform.rotation = Quaternion.Euler(Vector3.zero);
            _interactStandUpMessage.SetActive(false);
            _rightHandRopeNumber = 0;
            _leftHandRopeNumber = 0;
            _isOnWall = false;
            _isClimbingWall = false;
            _animator.SetBool("IsOnWall", _isOnWall);
            _animator.SetBool("IsClimbingWall", _isClimbingWall);
            transform.position = _wallPos.position;
            _wallTarget.position = _currentWallTargetPosition;
            _wallPos.position = _currentWallPosPosition;
        }
    }

    private void ApplyHangingAnimation()
    {
        if (transform.position.y > _ironBarPos.position.y && transform.position.x > _ironBarPos.position.x)
            _interactIronBarMessage.SetActive(true);
        else
            _interactIronBarMessage.SetActive(false);

        if (transform.position.y > _ironBarPos.position.y && transform.position.x > _ironBarPos.position.x && _YButton)
        {
            _isJumpingOnRope = true;
            _animator.SetBool("IsJumpingOnRope", _isJumpingOnRope);
            transform.forward = _ironBar.transform.forward;
        }
        else
        {
            _isJumpingOnRope = false;
            _animator.SetBool("IsJumpingOnRope", _isJumpingOnRope);
        }

        if (_animator.GetBool("IsJumpingOnRope") && _YButton)
        {
            float yPos = 2.35f;

            _isHanging = true;
            transform.position = new Vector3(transform.position.x, _ironBar.transform.position.y - yPos, _ironBar.transform.position.z);
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
        {
            float speed = 0.01f;

            _velocity = new Vector3(transform.position.x + speed * Time.fixedDeltaTime, transform.position.y, transform.position.z);
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
            _pushingBoxIKBehaviour.LeftHandBoxTarget = _leftHandCrateTarget;
            _pushingBoxIKBehaviour.RightHandBoxTarget = _rightHandCrateTarget;

            _interactCrateMessage.SetActive(true);
            _animator.SetBool(_pushingAnimator, true);
        }
        else
        {
            _interactCrateMessage.SetActive(false);
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
        int number = 1;

        if (_playerBoxCollider.bounds.Intersects(_ball.GetComponent<SphereCollider>().bounds))
        {
            _pickUpBallIKBehaviour.PickingUpClip = _pickingUpClip;
            _pickUpBallIKBehaviour.PickingUpTimer = _pickingUpTimer;
            _pickUpBallIKBehaviour.IsPickingUpBall = _isPickingUpBall;

            _interactBallMessage.SetActive(true);
        }
        else
            _interactBallMessage.SetActive(false);

        if (_YButton && _playerBoxCollider.bounds.Intersects(_ball.GetComponent<SphereCollider>().bounds))
        {
            _isPickingUpBall = true;
            _animator.SetBool("IsPickingUp", _isPickingUpBall);
            _pickUpBallIKBehaviour.RightHandBallTarget = _rightHandBallTarget;
        }

        if (_isPickingUpBall)
            _pickingUpTimer -= Time.deltaTime;

        if (_pickingUpTimer <= 3.4f)
        {
            _interactBallMessage.SetActive(false);
            _ball.transform.parent = _rightHand;
        }

        if (_pickingUpTimer <= 0)
            _interactThrowBallMessage.SetActive(true);

        if (_BButton && _isPickingUpBall)
        {
            _animator.SetBool("IsPickingUp", !_isPickingUpBall);
            _throwBallNumber = number;
            _isThrowing = true;
            _animator.SetBool("IsThrowing", _isThrowing);
        }

        if (_throwBallNumber == number)
        {
            _interactThrowBallMessage.SetActive(false);
            _ball.transform.SetParent(_ballGroundTarget);
            _ball.transform.position = _ballGroundTarget.transform.position;
            _throwingBallTimer -= Time.deltaTime;
            IsBallAtPosition = true;
        }

        if (_throwingBallTimer <= 0)
        {
            _throwingBallTimer = 0;
            _animator.SetBool("IsThrowing", !_isThrowing);
            _animator.SetBool("IsPickingUp", !_isPickingUpBall);
        }
    }

    private void ApplyJumpOnCrateAnimation()
    {
        if (_crate.transform.position.x >= _stopPos.position.x)
        {
            Vector3 crateCenter = Vector3.zero;
            Vector3 crateSize = new Vector3(1.25f, 1.25f, 1.25f);

            _crate.GetComponent<BoxCollider>().center = crateCenter;
            _crate.GetComponent<BoxCollider>().size = crateSize;

            if (_YButton)
            {
                _isJumpOnCrate = true;
                _animator.SetBool("IsJumpOnCrate", _isJumpOnCrate);
            }
            if (_isJumpOnCrate && _BButton)
            {
                float speed = 0.05f;

                _isJumpOnCrate = false;
                _animator.SetBool("IsJumpOnCrate", _isJumpOnCrate);
                transform.position = _cratePos.position;
                _crate.transform.position = new Vector3(_crate.transform.position.x - speed, _crate.transform.position.y, _crate.transform.position.z);
            }
        }
    }

    //Hacking computer
    private void ApplyHackingComputer()
    {
        //Display user interface feedback
        if(_playerBoxCollider.bounds.Intersects(_computer.GetComponent<MeshCollider>().bounds))
            _interactHackMessage.SetActive(true);
        
        if (_playerBoxCollider.bounds.Intersects(_computer.GetComponent<MeshCollider>().bounds) && _YButton)
        {
            IsHacking = true;
            _isHacking = true;
            _animator.SetBool("IsHacking", _isHacking);
            _cameraMessage.SetActive(true);
            _interactHackMessage.SetActive(false);
            _computer.GetComponent<MeshCollider>().enabled = false;
            _computer.GetComponent<BoxCollider>().enabled = true;
        }
        if (_animator.GetBool("IsHacking"))
        {
            _speed = 0;
            IsWalking = false;
            _hackingTimer -= Time.fixedDeltaTime;
        }
        if (_hackingTimer <= 0)
        {
            _isHacking = false;
            _speed = _minXZVelocity;
            _hackingTimer = _hackingClip.length;
            _animator.SetBool("IsHacking", _isHacking);
        }
    }

    //Lights
    private void ApplyTurnOffLights()
    {
        if (IsLightsOn)
        {
            for (int i = 0; i < _lights.Length; i++)
                _lights[i].SetActive(true);

            if (_playerBoxCollider.bounds.Intersects(_lightTrigger.GetComponent<BoxCollider>().bounds))
                _interactTurnOffLightsMessage.SetActive(true);
            else
                _interactTurnOffLightsMessage.SetActive(false);

            if (_playerBoxCollider.bounds.Intersects(_lightTrigger.GetComponent<BoxCollider>().bounds) && _YButton)
            {
                _turnOffLightsIKBehaviour.LightSwitchTarget = _lightSwitchTarget;
                IsLightsOn = false;
                _isPushingButton = true;
                _animator.SetBool("IsPushingButton", _isPushingButton);
            }
        }
        if (_animator.GetBool("IsPushingButton"))
            _pushButtonTimer -= Time.deltaTime;

        if (_pushButtonTimer <= 0)
        {
            _isPushingButton = false;
            _animator.SetBool("IsPushingButton", _isPushingButton);
        }

        if (!IsLightsOn)
        {
            _interactTurnOffLightsMessage.SetActive(false);

            for (int i = 0; i < _lights.Length; i++)
                _lights[i].SetActive(false);
        }
    }

    //Golden Egg
    private void ApplyEndMission()
    {
        if(_playerBoxCollider.bounds.Intersects(_goldenEgg.GetComponent<BoxCollider>().bounds))
        {
            IsWalking = false;
            _isDancing = true;
            _animator.SetBool("IsDancing", _isDancing);
            _winScreen.SetActive(true);
        }
    }
}





//private Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
//{
//    Vector3 distance = target - origin;
//    Vector3 distanceXZ = distance;
//    distanceXZ.y = 0;

//    float distY = distance.y;
//    float distXZ = distanceXZ.magnitude;

//    float velocityY = distY / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;
//    float velocityXZ = distXZ / time;

//    Vector3 restult = distanceXZ.normalized;
//    restult *= velocityXZ;
//    restult.y = velocityY;

//    return restult;
//}