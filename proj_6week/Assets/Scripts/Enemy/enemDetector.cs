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
    private float detectTime;

    private void Awake()
    {
        _enem = GetComponentInParent<Enemy>();
    //   maxDetectTime = _enem._maxDetectTime;
    //   maxSearchTime = _enem._maxSearchTime;
        playerInLOS = false;
        detectTime = 0;
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
            RaycastHit hit;
            if (Physics.Raycast(_enem.transform.position, _pc.position, out hit))
            {
                if (hit.transform.tag == "Player")
                {
                    DetectionTime();
                }
            }
        }
        else return;
    }
    public float DetectionTime()
    {
        Debug.Log(detectTime);
        detectTime += Time.deltaTime;
        return detectTime;
    }
    public void ResetDetectionTimer()
    {
        detectTime = 0;
    }
}
