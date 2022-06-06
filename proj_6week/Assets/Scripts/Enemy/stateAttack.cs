using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class stateAttack : IState
{
    private readonly Enemy _enem;
    private readonly Animator _anim;
    private readonly float _fireRate;
    

    private readonly int _hashSpeed = Animator.StringToHash("speed");

    private float _fireTimer;

    public stateAttack(Enemy enem, Animator anim, float fireRate)
    {
        _enem = enem;
        _anim = anim;
        _fireRate = fireRate;
    }

    public void Tick()
    {
        Debug.Log("AttackState");     
        _fireTimer += Time.deltaTime;

        if (_fireTimer >= _fireRate)
        {
            _enem.Fire();
            _fireTimer = 0;
            Debug.Log("shootuing");
        }
        else
            return;
    }
    public void OnEnter()
    {
        _fireTimer = 0;
        _anim.enabled = true;
        _anim.SetFloat(_hashSpeed, 0);
    }

    public void OnExit()
    {

    }
}
