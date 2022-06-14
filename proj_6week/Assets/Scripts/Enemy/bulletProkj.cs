using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletProkj : MonoBehaviour
{
    [SerializeField] public float dmg;
    [SerializeField] public float bulletSpd;

    [SerializeField] private float maxLifeTime;
    [SerializeField] private float lifeTime = 0; 
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerCon>().TakeDmg(dmg);
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        lifeTime += Time.deltaTime;
        if(lifeTime >= maxLifeTime)
        {
            Destroy(gameObject);
        }
    }
}
