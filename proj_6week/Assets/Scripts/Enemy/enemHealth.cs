using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private int maxHp;
    private int curHp;

    private void Awake()
    {
        curHp = maxHp;
    }
    public void TakeDamage(int dmg)
    {
        curHp -= dmg;
    }
}
