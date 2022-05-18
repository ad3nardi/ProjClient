using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletProkj : MonoBehaviour
{
    public float dmg;
    public float bulletSpd;

    [SerializeField] private float maxLifeTime; 
    private float lifeTime = 0; 
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerCon>().TakeDmg(dmg);
        }
    }
    private void Update()
    {
        lifeTime += maxLifeTime;
        if(lifeTime >= maxLifeTime)
        {
            Destroy(this);
        }
    }
}
