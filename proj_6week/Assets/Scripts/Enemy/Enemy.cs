using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public delegate bool conditionDelegate();
public class Enemy : MonoBehaviour
{
    public event Action<int> OnEnemChange;
    private StateMachine _stateMachine;

    [Header("Plugins")]
    public List<Transform> PatrolNodes = new List<Transform>();
    public List<Transform> SearchNodes = new List<Transform>();
    private enemDetector _enemDetector;

    [Header("Settings")]
    public float _range;
    public float _holdTime;
    public float _stunTime;
    public float _maxSearchTime;
    public float _maxDetectTime;

    /* state */
    stateHold hold;
    statePatrol patrol;
    stateDetect detect;
    stateSearch search;
    stateAtkMove atkMove;
    stateAttack attack;
    stateStunned stunned;

    private void Awake()
    {
        _enemDetector = GetComponentInChildren<enemDetector>();
        /* cached shit */
        var navMeshAgent = GetComponent<NavMeshAgent>();
        var animator = GetComponent<Animator>();
        var fleeParticleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        var enemDetector = _enemDetector;

        _stateMachine = new StateMachine();

        /* create the states */
        hold    = new stateHold     (this, animator);
        patrol  = new statePatrol   (this, navMeshAgent, animator);
        search  = new stateSearch   (this, navMeshAgent, animator, enemDetector, SearchNodes);
        detect  = new stateDetect   (this, navMeshAgent, animator);
        atkMove = new stateAtkMove  (this, navMeshAgent, animator, enemDetector);
        attack  = new stateAttack   (this, navMeshAgent, animator);
        stunned = new stateStunned  (this, navMeshAgent, animator);

        /* create the transitions */
        _stateMachine.AddTransition(hold,       patrol, isHeldPoint);
        _stateMachine.AddTransition(patrol,     hold,   isReachedPatrolPoint);
        _stateMachine.AddTransition(atkMove,    attack, inAttackRange);

        /* create any Transisionts - break points */
        _stateMachine.AddAnyTransition(search,  isSearching);
        _stateMachine.AddAnyTransition(stunned, isStunned);

        /* create transitions out of break points */
        _stateMachine.AddTransition(stunned,    attack, stunOut);
        _stateMachine.AddTransition(search,     hold,   isReachedSearchPoint);
        _stateMachine.AddTransition(search,     atkMove,isDetecting);


        /* initial state */
        _stateMachine.SetState(patrol);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    /* CONDITIONS */
    private bool isHeldPoint()
    {
        return hold.HoldTime >= _holdTime;
    }
    private bool isReachedPatrolPoint()
    {
        bool reachedPoint = Vector3.Distance(transform.position, patrol._nma.destination) <= 1;
        return reachedPoint;
    }
    private bool isReachedSearchPoint()
    {
        bool reachedPoint = Vector3.Distance(transform.position, search._nma.destination) <= 1;
        return reachedPoint;
    }
    private bool inAttackRange()
    {
        bool reachedRange = Vector3.Distance(transform.position, atkMove._nma.destination) <= _range;
        return reachedRange;
    }
    
    private bool isStunned()
    {
        return false;
    }
    private bool stunOut()
    {
        return stunned.StunTime >= _stunTime;
    }

    /* Public Conditions - CALLED FROM OUTSIDE - */
    public bool isSearching()
    {
        bool serachTimeReached = _enemDetector.DetectionTime() >= _maxSearchTime;
        return serachTimeReached;
    }
    public bool isDetecting()
    {
        bool detectTimeReached = _enemDetector.DetectionTime() >= _maxDetectTime;
        return detectTimeReached;
    }
}
