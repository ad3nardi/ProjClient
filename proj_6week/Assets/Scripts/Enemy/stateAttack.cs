using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class stateAttack : IState
{
    private readonly Enemy _enem;
    private readonly Animator _anim;
    private readonly float _fireRate;
    

    private readonly int Shoot = Animator.StringToHash("shoot");

    private float _fireTimer;

    public stateAttack(Enemy enem, Animator anim, float fireRate)
    {
        _enem = enem;
        _anim = anim;
        _fireRate = fireRate;
    }

    public void Tick()
    {
        if(_fireTimer >= _fireRate)
        {
            _enem.Fire();
            _fireTimer = 0;
        }
        _fireTimer += Time.deltaTime;
    }
    public void OnEnter()
    {
        _fireTimer = 0;
        _anim.enabled = true;
        _anim.SetTrigger(Shoot);
    }

    public void OnExit()
    {
        _anim.enabled = false;
    }
}
