using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class stateStunned : IState
{
    private readonly Enemy _enem;
    private readonly NavMeshAgent _nma;
    private readonly Animator _anim;
    private readonly int animSpd = Animator.StringToHash("speed");
    private readonly int animStun = Animator.StringToHash("stun");

    public float StunTime;

    public stateStunned(Enemy enem, NavMeshAgent nma, Animator anim)
    {
        _enem = enem;
        _nma = nma;
        _anim = anim;
    }
    public void Tick()
    {
        StunTime += Time.deltaTime;
    }
    public void OnEnter()
    {
        Debug.Log("stunned");

        StunTime = 0f;
        _nma.enabled = true;
        _nma.isStopped = true;
        _anim.SetFloat(animSpd, 0f);
        _anim.SetTrigger(animStun);
    }
    public void OnExit()
    {
        _enem._isStunned = false;
        _nma.isStopped = false;
        _nma.enabled = false;
        _anim.SetFloat(animSpd, 0f);
    }
}
