using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hurtbox : MonoBehaviour
{
    /* turn on and off with animation event */
    private int _dmg;
    private void Awake()
    {
        /* cache damage value from main player class */
        _dmg = GetComponentInParent<PlayerCon>().dmg;

    }
    public void OnTriggerEnter(Collider other)

    {
        if (other.tag == "enemy")
        {
            other.GetComponent<enemHealth>().TakeDamage(_dmg);
        }
    }
}
