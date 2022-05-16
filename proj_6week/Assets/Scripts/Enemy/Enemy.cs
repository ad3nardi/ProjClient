using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public event Action<int> OnEnemChange;
    private StateMachine _stateMachine;

    public int _range;
    public int _holdTime;
    public bool _isStunned;
    public int _curNode = 0;

    public List<Transform> PatrolNodes = new List<Transform>();

    private void Awake()
    {
        var navMeshAgent = GetComponent<NavMeshAgent>();
        var animator = GetComponent<Animator>();
        var fleeParticleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        var curNode = _curNode;
        _isStunned = false;

        _stateMachine = new StateMachine();
        //STATES AND PARAMETERS NEEDED FOR EACH
        stateHold hold    = new stateHold     (this, animator);
        statePatrol patrol  = new statePatrol   (this, navMeshAgent, animator, curNode);
        IState detect  = new stateDetect   (this, navMeshAgent, animator);
        IState search  = new stateSearch   (this, navMeshAgent, animator);
        IState atkMove = new stateAtkMove  (this, navMeshAgent, animator);
        IState attack  = new stateAttack   (this, navMeshAgent, animator);
        IState stunned = new stateStunned  (this, navMeshAgent, animator);

        _stateMachine.AddTransition(hold,patrol, PatrolNodeHeld());
        _stateMachine.AddTransition(patrol,hold, ReachedPoint());

        _stateMachine.AddAnyTransition(stunned, () => _isStunned == true);
        _stateMachine.AddTransition(attack, stunned, () => _isStunned == false);

        _stateMachine.SetState(patrol);

        Func<bool> PatrolNodeHeld() => () => hold.HoldTime > _holdTime;
        Debug.Log(PatrolNodeHeld());

        Func<bool> ReachedPoint() => () => patrol._reachedPos == true;
        Debug.Log(ReachedPoint());

    }

    private void Update()
    {
        _stateMachine.Tick();
    }


}
