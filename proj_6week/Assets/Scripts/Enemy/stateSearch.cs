using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class stateSearch : IState
{
    private readonly Enemy _enem;
    public NavMeshAgent _nma;
    private readonly Animator _anim;
    private enemDetector _ED;
    private readonly List<Transform> _SN;

    private readonly int _hashSpeed = Animator.StringToHash("speed");
    private readonly int _hashSearch = Animator.StringToHash("searching");

    private Transform closestNode;
    public float TimeInSearch;
    private int _curNodeIndex;

    public stateSearch(Enemy enem, NavMeshAgent nma, Animator anim, enemDetector ed, List<Transform> sn)
    {
        _enem = enem;
        _nma = nma;
        _anim = anim;
        _SN = sn;
        _ED = ed;
    }

    public void Tick()
    {
        TimeInSearch += Time.deltaTime;
    }

    public void OnEnter()
    {
        Debug.Log("searching");

        /*check for cloesest search node */
        float minDist = Mathf.Infinity;
        Vector3 currentPos = _enem.transform.position;
        foreach (var item in _SN)
        {
            float dist = Vector3.Distance(_enem.transform.position, _SN[_curNodeIndex].position);
            if (minDist <= dist)
                {
                    minDist = dist;
                    closestNode = _SN[_curNodeIndex];
                }
        }

        TimeInSearch = 0f;
        _nma.enabled = true;
        //_nma.SetDestination(closestNode.position);
        _anim.SetFloat(_hashSpeed, 1f);
        _anim.SetBool(_hashSearch, true);

    }

    public void OnExit()
    {
        _nma.enabled = false;
        _anim.SetFloat(_hashSpeed, 0f);
        _anim.SetBool(_hashSearch, false);

        /* wrap around nodes list when we reach max */
        if (_curNodeIndex == _enem.PatrolNodes.Count)
            _curNodeIndex = 0;
    }
}
