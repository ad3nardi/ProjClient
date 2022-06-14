using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class stateDetect : IState
{
    private readonly Enemy _enem;
    private readonly NavMeshAgent _nma;
    private readonly Animator _anim;
    private readonly int Speed = Animator.StringToHash("detect");

    public bool EnemyInRange => detectPlayer != null;
    private PlayerCon detectPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerCon>())
            detectPlayer = other.GetComponent<PlayerCon>();
        
    }
    /*
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<PlayerCon>())
        {
            StartCoroutine(ClearPlayerAfterDelay());
        }
    }
    private IEnumerator ClearPlayerAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        detectPlayer = null;
    }
    */
    public Vector3 GetNearestPlayerPos()
    {
        return detectPlayer?.transform.position ?? Vector3.zero;
    }
    public stateDetect(Enemy enem, NavMeshAgent nma, Animator anim)
    {
        _enem = enem;
        _nma = nma;
        _anim = anim;
    }

    public void Tick()
    { 
    }

    public void OnEnter()
    {
        _nma.enabled = true;
        _nma.SetDestination(_enem.transform.position);
        _anim.SetFloat(Speed, 1f);
    }

    public void OnExit()
    {
        _nma.enabled = false;
        _anim.SetFloat(Speed, 0f);
    }
}
