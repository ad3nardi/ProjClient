using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class PlayerCon : MonoBehaviour
{
    /*Input Settings */
    private float hor;
    private float ver;
    private bool _isCrouch;

    [Header("Plugins")]
    private Rigidbody rb;
    private CapsuleCollider pcCap;
    public CinemachineFreeLook freeCam;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform camTran;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject elecProj;
    [SerializeField] private GameObject hurtboxGO;

    [Header("Movement Settings")]
    [SerializeField] private float rotateSpd;
    [SerializeField] private float walkSpd;
    [SerializeField] private float runSpd;
    [SerializeField] private float crouchSpd;
    [SerializeField] private float jumpForce;
    [SerializeField] private float moveSpd;

    [Header("Parkour Settings")]
    [SerializeField] private Vector3 headOff;
    [SerializeField] private Vector3 footOff;
    [SerializeField] private bool vaultAir;
    
    [Header("Stat Settings")]
    [SerializeField] private float maxHp;
    [SerializeField] private float curHP;
    [SerializeField] private float projkForce;
    [SerializeField] private float fireRate;
    [SerializeField] private float _fireTimer;


    [Header("Attack Settings")]
    [SerializeField] private bool  canAttack;
    [SerializeField] public int    dmg;
    [SerializeField] private int   comboIndex;
    [SerializeField] private int   maxCombo;
    [SerializeField] private float comboTimer;
    [SerializeField] private float comboTime;
    [SerializeField] private float animTime;

    [Header("Animation Cache")]
    private readonly int _hashStateTime = Animator.StringToHash("stateStage");
    private readonly int _hashAttack    = Animator.StringToHash("attack");

    private void Awake()
    {
        /*CACHE SHIT */
        rb = GetComponent<Rigidbody>();
        pcCap = GetComponent<CapsuleCollider>();

        /* Setup Camera */
        cam = Camera.main;
        camTran = cam.transform;
        freeCam.Priority = 11;

        /* Settings Cache */
        curHP = maxHp;
        moveSpd = walkSpd;
        vaultAir = false;
        headOff = new Vector3(0, 0.5f, 0);
        footOff = new Vector3(0, -0.5f, 0);

        /* Attack Cache */
        comboIndex = 0;
        comboTimer = 0f;
    }
    /* Take in move inputs */
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
        _fireTimer = 0;
        if (_fireTimer >= fireRate)
        {
            
            GameObject go = Instantiate(elecProj, transform.position, Quaternion.identity);
            go.GetComponent<Rigidbody>().AddForce(go.transform.forward * projkForce, ForceMode.Impulse);
            _fireTimer = 0;
        }
        _fireTimer += Time.deltaTime;
    }
    /* INPUT FOR ATTACK */
    private void Attack(bool inp)
    {
        //FREEZE MOVEMENT
        rb.constraints = RigidbodyConstraints.FreezePosition;
        //INCREASE COMBO INDEX
        comboIndex += 1;
        if (comboIndex > maxCombo)
            comboIndex = 0;
        // CALL ATTACK ANIMATION FOR CORRESPONDING INDEX
        anim.SetInteger(_hashAttack, comboIndex);
    }
    public void TakeDmg(float dmg)
    {
        curHP -= dmg;
    }

    private void Update()
    {
        /* RESET THE ANIMATION TIMER IF IT ENDS ON THE FRAME */
        if (0.9f <= animTime && animTime <= 1f)
            animTimerEnded();
    }
    private void FixedUpdate()
    {
        /* MOVEMENT */
        Vector3 moveDir = Vector3.right * hor + Vector3.forward * ver;
        Vector3 projectedCamForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        Quaternion rotateToCam = Quaternion.LookRotation(projectedCamForward, Vector3.up);
        moveDir = rotateToCam * moveDir;
        Quaternion rotateDir = Quaternion.LookRotation(moveDir, Vector3.up);
        /* if no input = dont rotate */
        if (hor != 0 || ver != 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateDir, rotateSpd * Time.deltaTime);
        }
        /*always move toward direction */
        rb.MovePosition(rb.position + moveDir * moveSpd * Time.deltaTime);

        /* RESET ANIMATION TIME AFTER THE FRAME ENDS */
        animTime = Mathf.Repeat(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);
        anim.SetFloat(_hashStateTime, animTime);
        anim.ResetTrigger(_hashAttack);
    }
    /* Reset Abliity to Move at end of attack animation*/
    private void animTimerEnded()
    {
        rb.constraints &= ~RigidbodyConstraints.FreezePosition;
        moveSpd = walkSpd;
        comboIndex = 0;
        anim.SetInteger(_hashAttack, 0);
    }
    /*toggle on and off in animation clip */
    public void HurtboxToggleEvent()
    {
        hurtboxGO.SetActive(!hurtboxGO.activeInHierarchy);
    }
}
