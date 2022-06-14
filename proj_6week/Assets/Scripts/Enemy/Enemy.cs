using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public delegate bool conditionDelegate();
public class Enemy : MonoBehaviour
{
    public PlayerCon _pc;

    [Header("Plugins")]
    private StateMachine _stateMachine;
    private enemDetector _enemDetector;
    public GameObject _bulletPref;
    public ParticleSystem _muzzleFlash;
    public Transform _gunTrans;
    public Animator _anim;
    public List<Transform> PatrolNodes = new List<Transform>();
    public List<Transform> SearchNodes = new List<Transform>();

    [Header("Stat Settings")]
    [SerializeField] private float _range;
    [SerializeField] private float _holdTime;
    [SerializeField] private float _stunTime;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _bulletSpd;

    [Header("State Settings")]
    public float _maxSearchTime;
    public float _maxDetectTime;
    public bool _canSearch;
    public bool _canDetect;
    public bool _isStunned;    

    /* state */
    stateHold hold;
    statePatrol patrol;
    stateDetect detect;
    stateSearch search;
    stateAtkMove atkMove;
    stateAttack attack;
    stateStunned stunned;


    /*Animation Cache*/
    private readonly int _hashShoot = Animator.StringToHash("shoot");


    private void Awake()
    {
        _canSearch = false;
        _canDetect = false;
        _isStunned = false;
        _enemDetector = GetComponentInChildren<enemDetector>();
        _anim = GetComponentInChildren<Animator>();

        /* cached shit */
        var navMeshAgent = GetComponent<NavMeshAgent>();
        var enemDetector = _enemDetector;
        var fireRate = _fireRate;

        _stateMachine = new StateMachine();

        /* create the states */
        hold    = new stateHold     (this, _anim);
        patrol  = new statePatrol   (this, navMeshAgent, _anim);
        search  = new stateSearch   (this, navMeshAgent, _anim, enemDetector, SearchNodes);
        detect  = new stateDetect   (this, navMeshAgent, _anim);
        atkMove = new stateAtkMove  (this, navMeshAgent, _anim, enemDetector);
        attack  = new stateAttack   (this, _anim,     fireRate);
        stunned = new stateStunned  (this, navMeshAgent, _anim);

        /* create the transitions */
        _stateMachine.AddTransition(hold,       patrol, isHeldPoint);
        _stateMachine.AddTransition(patrol,     hold,   isReachedPatrolPoint);

        _stateMachine.AddTransition(hold,       search, isSearching);
        _stateMachine.AddTransition(patrol,     search, isSearching);

        _stateMachine.AddTransition(search,     atkMove,isDetecting);
        _stateMachine.AddTransition(search,     hold,   isReachedSearchPoint);

        _stateMachine.AddTransition(atkMove,    attack, inAttackRange);
        //_stateMachine.AddTransition(attack,    atkMove, notInAttackRange);

        /* create ANY Transisionts - BREAK POINTS */
        _stateMachine.AddAnyTransition(stunned, isStunned);

        /* create transitions OUT of BREAK points */
        _stateMachine.AddTransition(stunned,    attack, stunOut);

        /* initial state */
        _stateMachine.SetState(patrol);


    }

    private void Update()
    {
        _stateMachine.Tick();

    }
    /* ACTION FUNCTIONS */
    public void Fire()
    {
        transform.LookAt(_pc.transform.position);
        GameObject go = Instantiate(_bulletPref, _gunTrans.transform.position, _gunTrans.rotation);
        go.GetComponent<Rigidbody>().AddForce(_gunTrans.transform.forward * _bulletSpd, ForceMode.Impulse);
        _anim.SetTrigger(_hashShoot);
        _muzzleFlash.Play();
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
    private bool notInAttackRange()
    {
        bool reachedRange = Vector3.Distance(transform.position, atkMove._nma.destination) > _range;
        return reachedRange;
    }
    private bool stunOut()
    {
        bool stunTimeReached = stunned.StunTime >= _stunTime;
        return stunTimeReached;
    }

    /* Public Conditions - CALLED FROM OUTSIDE - */
    public bool isStunned()
    {
        return _isStunned;
    }
    public bool isSearching()
    {
        return _canSearch;
    }
    public bool isDetecting()
    {
        return _canDetect;
    }
    
}
