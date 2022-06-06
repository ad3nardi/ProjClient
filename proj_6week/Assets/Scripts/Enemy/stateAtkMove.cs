using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class stateAtkMove : IState
{
    private readonly Enemy _enem;
    public NavMeshAgent _nma;
    private readonly Animator _anim;
    private enemDetector _ED;
    private readonly List<Transform> _SN;

    private readonly int _hashSpeed = Animator.StringToHash("speed");

    private Transform closestNode;
    public float TimeInSearch;
    private int _curNodeIndex;

    public stateAtkMove(Enemy enem, NavMeshAgent nma, Animator anim, enemDetector ed)
    {
        _enem = enem;
        _nma = nma;
        _anim = anim;
        _ED = ed;
    }

    public void Tick()
    {

    }

    public void OnEnter()
    {
        Debug.Log("atkMoving");
        _nma.enabled = true;
        _nma.SetDestination(_ED._pc.position);
        _anim.SetFloat(_hashSpeed, 1f);
    }

    public void OnExit()
    {
        _nma.enabled = false;
        _anim.SetFloat(_hashSpeed, 0f);
    }
}
