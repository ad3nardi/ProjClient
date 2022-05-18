using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemDetector : MonoBehaviour
{
    [Header("Plugins")]
    [SerializeField]
    private Enemy _enem;
    public Transform _pc;
    public bool playerInLOS;

    [Header("Settings")]
    private float maxSearchTime;
    private float maxDetectTime;
    public float detectTime;

    private void Awake()
    {
        /* cache shit */
        _enem = GetComponentInParent<Enemy>();
        playerInLOS = false;
        detectTime = 0;

        /* inherit timings from main enemy class */
        maxSearchTime = _enem._maxSearchTime;
        maxDetectTime = _enem._maxDetectTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            playerInLOS = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            playerInLOS = false;
    }
    private void FixedUpdate()
    {
        if (playerInLOS == true)
        {
            /* shoot ray to player location whilst in LoS */
            RaycastHit hit;
            if (Physics.Raycast(_enem.transform.position, _pc.position - _enem.transform.position, out hit))
            {
                Debug.DrawRay(_enem.transform.position, _pc.position - _enem.transform.position);
                /* check if ray hits player - checking cover */
                if (hit.collider.tag == "Player")
                {
                    /* add to detection time */
                    detectTime += Time.deltaTime;

                    /* check if reached detection time (LARGER VALUE) */
                    if (detectTime >= maxDetectTime)
                    {
                        _enem._canDetect = true;
                    }
                    /* otherwise check if reached search time (SMALLER VALUE) */
                    else if (detectTime >= maxSearchTime)
                    {
                        _enem._canSearch = true;
                    }
                }
            }
        }
        else return;
    }
    public void ResetDetectionTimer()
    {
        detectTime = 0;
    }
}
