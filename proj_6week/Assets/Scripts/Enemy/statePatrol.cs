using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class statePatrol : IState
{
    private readonly Enemy _enem;
    public NavMeshAgent _nma;
    private readonly Animator _anim;
    private readonly int _hashSpeed = Animator.StringToHash("speed");

    private Vector3 _lastPos = Vector3.zero;
    public float TimeStuck;
    public int _curNodeIndex;

    public statePatrol(Enemy enem, NavMeshAgent nma, Animator anim)
    {
        _enem = enem;
        _nma = nma;
        _anim = anim;
    }

    public void Tick()
    {
        if (Vector3.Distance(_enem.transform.position, _lastPos) <= 0.1f)
            TimeStuck += Time.deltaTime;

        _lastPos = _enem.transform.position;
    }

    public void OnEnter()
    {
        Debug.Log("patroling");
        TimeStuck = 0f;
        _nma.enabled = true;


        _nma.SetDestination(_enem.PatrolNodes[_curNodeIndex].position);
        _anim.SetFloat(_hashSpeed, 1f);
    }

    public void OnExit()
    {
        _nma.enabled = false;
        _anim.SetFloat (_hashSpeed, 0f);

        _curNodeIndex++;
        /* wrap around nodes list when we reach max */
        if (_curNodeIndex == _enem.PatrolNodes.Count)
            _curNodeIndex = 0;
    }
}
