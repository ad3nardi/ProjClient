using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class stateStunned : IState
{
    private readonly Enemy _enem;
    private readonly NavMeshAgent _nma;
    private readonly Animator _anim;
    private readonly int Speed = Animator.StringToHash("speed");

    private Vector3 _lastPos = Vector3.zero;
    public float TimeStuck;

    public stateStunned(Enemy enem, NavMeshAgent nma, Animator anim)
    {
        _enem = enem;
        _nma = nma;
        _anim = anim;
    }

    public void Tick()
    {
        if (Vector3.Distance(_enem.transform.position, _lastPos) <= 0f)
            TimeStuck += Time.deltaTime;

        _lastPos = _enem.transform.position;
    }

    public void OnEnter()
    {
        TimeStuck = 0f;
        _nma.enabled = true;
        _nma.SetDestination(_enem.transform.position);
        _anim.SetFloat(Speed, 0f);
    }

    public void OnExit()
    {
        _nma.enabled = false;
        _anim.SetFloat(Speed, 0f);
    }
}
