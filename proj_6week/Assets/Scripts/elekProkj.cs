using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elekProkj : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float prokjSpd;
    [SerializeField] private float maxLifeTime;
    [SerializeField] private float curLifeTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>()._isStunned = true;
        }
    }

    private void Update()
    {
        this.transform.Translate((Vector3.forward) * prokjSpd * Time.deltaTime);
        curLifeTime += maxLifeTime;
        if (curLifeTime >= maxLifeTime)
        {
            Destroy(this);
        }
    }
}
