using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class statePatrol : IState
{
    private readonly Enemy _enem;
    private readonly NavMeshAgent _nma;
    private readonly Animator _anim;
    private readonly int Speed = Animator.StringToHash("speed");

    private Vector3 _lastPos = Vector3.zero;
    public float TimeStuck;
    public int _curNode;
    public bool _reachedPos;

    public statePatrol(Enemy enem, NavMeshAgent nma, Animator anim, int curNode)
    {
        _enem = enem;
        _nma = nma;
        _anim = anim;
        _curNode = curNode;
    }

    public void Tick()
    {
        if (Vector3.Distance(_enem.transform.position, _lastPos) <= 0f)
            TimeStuck += Time.deltaTime;

            _lastPos = _enem.transform.position;

        if (Vector3.Distance(_enem.transform.position, _nma.destination)<= 1)
        {
            _reachedPos = true;
        }
    }

    public void OnEnter()
    {
        TimeStuck = 0f;
        _reachedPos = false;
        _nma.enabled = true;
        if (_curNode == _enem.PatrolNodes.Count - 1)
            _curNode = 0;

        _nma.SetDestination(_enem.PatrolNodes[_curNode].position);
        _anim.SetFloat(Speed, 1f);
    }

    public void OnExit()
    {
        _nma.enabled = false;
        _anim.SetFloat (Speed, 0f);
        _enem._curNode++;
    }
}
