using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.AI;

public class EnemyAIBehaviour : MonoBehaviour
{
    //Player
    [Header("Player")]
    [SerializeField] private Transform _player;

    private Vector3 direction;
    private float angle;

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

    private float _speed;
    private int _destinationPoint = 0;
    private float _timer;
    private float _time = 20.0f;

    //Behaviour Tree
    [Header("Behaviour Tree")]
    [SerializeField] private Transform _lightSwitchPos;
    [SerializeField] private bool isLightsOff = false;
    [SerializeField] private bool isPlayerSpot = false;

    private INode _rootNode;

    private void Start()
    {
        //Animation
        _animator = GetComponent<Animator>();

        //Navigation
        _timer = _time;
        _speed = (5.0f * 1000) / (60 * 60); //[m/s], 5km/h

        //Behaviour Tree
        INode lightsOffAI = new SequenceNode(
            new ConditionNode(IsLightsOff),
            new ActionNode(GoToLightSwitchPosition));

        INode seePlayerAI = new SequenceNode(
            new ConditionNode(IsPlayerSpot),
            new ActionNode(Shoot));

        _rootNode = new SelectorNode(lightsOffAI,seePlayerAI);

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
        {
            yield return _rootNode.Tick();
        }
    }

    IEnumerator<NodeResult> GoToLightSwitchPosition()
    {
        yield return NodeResult.Succes;
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
        //If the lights are on the enemy will be patrolling
        if (!isLightsOff)
        {
            if (!_enemy.pathPending && _enemy.remainingDistance < 0.5f)
                GoToNextPoint();

            _enemy.speed = _speed;
        }

        //If lights are off the enemy will walk towards the lightswitch
        if(isLightsOff)
            _enemy.destination = _lightSwitchPos.position;

        return isLightsOff;
    }

    bool IsPlayerSpot()
    {
        direction = _player.position - _enemy.transform.position;
        angle = Vector3.Angle(direction, transform.forward);

        if (direction.z < startPos.forward.z * FieldOfViewDistance)
        {
            //isPlayerSpot = true;
            //if (isPlayerSpot)
            //{
            //    _animator.SetBool("IsShooting", true);
            //}
        }
        else
        {
            isPlayerSpot = false;
            _animator.SetBool("IsShooting", false);
        }

        return isPlayerSpot;
    }

    //Navigation Methods
    private void GoToNextPoint()
    {
        if (_points.Length == 0)
            return;

        if (_enemy.destination.x == _points[_destinationPoint].position.x)
        {
            _timer -= Time.deltaTime;
            _animator.SetBool("IsIdle", true);
        }
        else
        {
            _animator.SetBool("IsIdle", false);
        }

        _enemy.destination = _points[_destinationPoint].position;

        if (_timer <= 0)
        {
            _timer = _time;
            _destinationPoint = (_destinationPoint + 1) % _points.Length;
        }
    }

    private void DrawEnemySight()
    {
        Debug.DrawRay(startPos.position, startPos.forward * FieldOfViewDistance, Color.red);
        Debug.DrawRay(startPos.position, startPos.forward * FieldOfViewDistance + startPos.right * FieldOfView, Color.yellow);
        Debug.DrawRay(startPos.position, startPos.forward * FieldOfViewDistance - startPos.right * FieldOfView, Color.blue);
    }
}

