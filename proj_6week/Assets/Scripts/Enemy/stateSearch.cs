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

    private readonly int Speed = Animator.StringToHash("speed");

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
        _anim.SetFloat(Speed, 1f);
    }

    public void OnExit()
    {
        _nma.enabled = false;
        _anim.SetFloat(Speed, 0f);

        /* wrap around nodes list when we reach max */
        if (_curNodeIndex == _enem.PatrolNodes.Count)
            _curNodeIndex = 0;
    }
}
