using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class PlayerCon : MonoBehaviour
{
    [Header("Plugins")]
    private Rigidbody rb;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform camTran;
    
    [SerializeField]
    private ParticleSystem shockFlash;
    public CinemachineFreeLook freeCam;
    private float hor;
    private float ver;
    private bool _isAim;

    [Header("Settings")]
    [SerializeField]
    private float rotateSpd;
    public float moveSpd;
    public float jumpForce;
    [SerializeField]
    private float walkSpd;
    [SerializeField]
    private float runSpd;
    [SerializeField]
    private float crouchSpd;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        camTran = cam.transform;
        freeCam.Priority = 11;
        _isAim = false;
        moveSpd = walkSpd;
    }
    public void Move(float h, float v)
    {
        hor = h;
        ver = v;
    }
    public void Run(bool inp)
    {
        if (inp)
            moveSpd = runSpd;
        else
            moveSpd = walkSpd;
    }
    public void Crouch(bool inp)
    {

    }
    public void Jump(bool inp)
    {
        rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
    }
    
    public void Fire(bool inp)
    {
        
    }
    private void FixedUpdate()
    {
        Vector3 moveDir = Vector3.right * hor + Vector3.forward * ver;
        Vector3 projectedCamForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        Quaternion rotateToCam = Quaternion.LookRotation(projectedCamForward, Vector3.up);
        moveDir = rotateToCam * moveDir;
        Quaternion rotateDir = Quaternion.LookRotation(moveDir, Vector3.up);
        if (hor != 0 || ver != 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateDir, rotateSpd * Time.deltaTime);
        }
        rb.MovePosition(rb.position + moveDir * moveSpd * Time.deltaTime);

    }
}
