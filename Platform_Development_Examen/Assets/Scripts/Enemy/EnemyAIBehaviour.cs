using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAIBehaviour : MonoBehaviour
{
    //Player
    [Header("Player")]
    [SerializeField] private Transform _player;
    [SerializeField] private CharacterControllerBehaviour _playerBehaviour;

    private Vector3 direction;

    //Animation
    private Animator _animator;

    //Sight
    [Header("Sight")]
    [SerializeField] private Transform startPos;
    [SerializeField] private float FieldOfView = 2.0f;
    [SerializeField] private float FieldOfViewDistance = 15.0f;

    //Navigation
    [Header("Navigation")]
    [SerializeField] private NavMeshAgent _enemy;
    [SerializeField] private Transform[] _points;
    [SerializeField] private float _standingTimer;
    [SerializeField] private float _standTime = 5.0f;

    private float _speed;
    private int _destinationPoint = 0;

    //Lights
    [SerializeField] private bool _isOnLightSwitchPos = false;
    [SerializeField] private float _turnOnLightsTimer;
    [SerializeField] private float _turnOnLightsTime = 2.0f;
    [SerializeField] private float _enemyNumber;

    //Behaviour Tree
    [Header("Behaviour Tree")]
    [SerializeField] private Transform _lightSwitchPos;
    [SerializeField] private bool _isPlayerSpot = true;

    private INode _rootNode;

    public float timer = 1.5f;

    private void Start()
    {
        //Animation
        _animator = GetComponent<Animator>();

        //Navigation
        _standingTimer = _standTime;
        _speed = (5.0f * 1000) / (60 * 60); //[m/s], 5km/h

        //Lights
        _turnOnLightsTimer = _turnOnLightsTime;

        //Behaviour Tree
        INode AILight = new SelectorNode(
            new ConditionNode(IsLightsOff),
            new ActionNode(WalkToLightSwitch));
        INode AIShoot = new SelectorNode(
            new ConditionNode(IsPlayerSpot),
            new ActionNode(Shoot));

        _rootNode = new SequenceNode(AILight,AIShoot);

        StartCoroutine(RunTree());
    }

    private void Update()
    {
        //Sight
        DrawEnemySight();
    }

    //Behaviour Tree
    IEnumerator RunTree()
    {
        while (Application.isPlaying)
            yield return _rootNode.Tick();
    }

    IEnumerator<NodeResult> Shoot()
    {
        yield return NodeResult.Succes;
    }
    IEnumerator<NodeResult> WalkToBall()
    {
        yield return NodeResult.Succes;
    }
    IEnumerator<NodeResult> WalkToLightSwitch()
    {
        yield return NodeResult.Succes;
    }

    bool IsLightsOff()
    {
        if (_playerBehaviour.IsLightsOn)
        {
            FieldOfViewDistance = 15.0f;
            if (!_enemy.pathPending && _enemy.remainingDistance < 0.5f)
                GoToNextPoint();

            _enemy.speed = _speed;
        }
        
        //If lights are off the enemy will walk towards the lightswitch
        if (!_playerBehaviour.IsLightsOn)
        {
            FieldOfViewDistance = 5.0f;
            _animator.SetBool("IsIdle", false);
            _enemy.destination = _lightSwitchPos.position;
        }
        //Turn on light
        if (_enemy.pathEndPosition.x == _lightSwitchPos.position.x && _enemy.remainingDistance < 0.5f)
        {
            _turnOnLightsTimer -= Time.deltaTime;

            if (_turnOnLightsTimer <= 0)
            {
                _turnOnLightsTimer = _turnOnLightsTime;
                _isOnLightSwitchPos = true;
                _playerBehaviour.IsLightsOn = true;
            }
        }

        return _playerBehaviour.IsLightsOn;
    }

    bool IsPlayerSpot()
    {
        float distance = Vector3.Distance(_enemy.transform.position, _player.position);

        if (distance < FieldOfViewDistance)
            _isPlayerSpot = true;

        if (_isPlayerSpot == true)
        {
            timer -= Time.deltaTime;
            Time.timeScale = 0.3f;
            _player.GetComponent<Animator>().SetBool("IsDead", true);
            _playerBehaviour._isWalking = false;
            transform.LookAt(_player);
            _animator.SetBool("IsShooting", true);
            _speed = 0;
        }
        if(timer <= 0.5f)
            timer = 0;
       
        if(timer == 0)
            Time.timeScale = 1;
        
        return _isPlayerSpot;
    }

    //Navigation Methods
    private void GoToNextPoint()
    {
        if (_enemy.destination.x == _points[_destinationPoint].position.x)
        {
            _enemy.transform.position = _points[_destinationPoint].position;
            _standingTimer -= Time.deltaTime;
            _animator.SetBool("IsIdle", true);
        }
        else
            _animator.SetBool("IsIdle", false);

        _enemy.destination = _points[_destinationPoint].position;

        if (_standingTimer <= 0)
        {
            _standingTimer = _standTime;
            _destinationPoint = (_destinationPoint + 1) % _points.Length;
        }
    }

    private void DrawEnemySight()
    {
        Debug.DrawRay(startPos.position, startPos.forward * FieldOfViewDistance, Color.red);
        Debug.DrawRay(startPos.position, startPos.forward * FieldOfViewDistance + startPos.right * FieldOfView, Color.yellow);
        Debug.DrawRay(startPos.position, startPos.forward * FieldOfViewDistance - startPos.right * FieldOfView, Color.blue);

        Debug.DrawLine(_enemy.transform.position, _player.position,Color.cyan);
    }
}

