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
    private Animator anim;
    private CapsuleCollider pcCap;
    public CinemachineFreeLook freeCam;
    private float hor;
    private float ver;
    private bool _isCrouch;

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
    private Vector3 headOff;
    private Vector3 footOff;
    private bool vaultAir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        pcCap = GetComponent<CapsuleCollider>();
        camTran = cam.transform;
        freeCam.Priority = 11;
        moveSpd = walkSpd;
        vaultAir = false;
        headOff = new Vector3(0, 0.5f, 0);
        footOff = new Vector3(0, -0.5f, 0);
    }
    public void Move(float h, float v)
    {
        hor = h;
        ver = v;
    }
    public void Run(bool inp)
    {
        if (inp)
        {
            moveSpd = runSpd;
            //vault
            if (Physics.Raycast(transform.position + headOff, transform.forward, 0.5f))
            {
                //vault
            }
            //
        }
        else
            moveSpd = walkSpd;
    }
    public void Crouch(bool inp)
    {
        _isCrouch = !_isCrouch;
        if (_isCrouch)
        {
            pcCap.height = 1;
            pcCap.center = new Vector3(0, -0.5f, 0);
        }
        if (!_isCrouch)
        {
            pcCap.height = 2;
            pcCap.center = new Vector3(0, 0, 0);
        }
    }
    public void Jump(bool inp)
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
        {
            rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
        }
        else
            return;
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
