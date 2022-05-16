using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stateHold : IState
{
    private readonly Enemy _enem;
    private readonly Animator _anim;
    private readonly int Speed = Animator.StringToHash("speed");

    private Vector3 _lastPos = Vector3.zero;
    public float HoldTime;

    public stateHold(Enemy enem, Animator anim)
    {
        _enem = enem;
        _anim = anim;
    }

    public void Tick()
    {
        HoldTime += Time.deltaTime;
    }

    public void OnEnter()
    {
        HoldTime = 0f;
        _anim.SetFloat(Speed, 0f);
    }

    public void OnExit()
    {
        _anim.SetFloat(Speed, 0f);
    }
}
