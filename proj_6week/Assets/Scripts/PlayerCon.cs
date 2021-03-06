using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class PlayerCon : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] public float hor;
    [SerializeField] public float ver;

    [Header("Plugins")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider pcCap;
    [SerializeField] private Animator anim;
    [SerializeField] public  CinemachineFreeLook freeCam;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform camTran;
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
    [SerializeField] public bool isGrounded;
    [SerializeField] public bool isRunning;
    [SerializeField] private bool vaultAir;
    [SerializeField] private bool _isCrouch;
    [SerializeField] private Vector3 headOff;
    [SerializeField] private Vector3 footOff;

    [Header("Combat Settings")]
    [SerializeField] private float maxHp;
    [SerializeField] private float curHP;
    [SerializeField] private float projkForce;
    [SerializeField] private float fireRate;
    [SerializeField] private float fireTimer;


    [Header("Attack Settings")]
    [SerializeField] public  int   dmg;
    [SerializeField] private int   comboIndex;
    [SerializeField] private int   maxCombo;
    [SerializeField] public float  animTime;
    [SerializeField] private float comboTimer;
    [SerializeField] private float comboTime;
    [SerializeField] private bool  canAttack;

    [Header("Animation Cache")]
    private readonly int _hashStateTime = Animator.StringToHash("curAnim");
    private readonly int _speed         = Animator.StringToHash("speed");
    private readonly int _hashAttack    = Animator.StringToHash("attack");
    private readonly int _hashAtkCount  = Animator.StringToHash("attackCount");
    private readonly int _hashJump      = Animator.StringToHash("jump");
    private readonly int _hashAirborne  = Animator.StringToHash("airborne");
    private readonly int _hashClimb     = Animator.StringToHash("climb");
    private readonly int _hashShock     = Animator.StringToHash("shock");

    private void Awake()
    {
        /*CACHE SHIT */
        rb = GetComponent<Rigidbody>();
        pcCap = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<Animator>();
        rb.isKinematic = false;
        anim.SetFloat(_speed, 0);

        /* Setup Camera */
        cam = Camera.main;
        camTran = cam.transform;
        freeCam.Priority = 11;

        /* Settings Cache */
        curHP = maxHp;
        moveSpd = walkSpd;
        vaultAir = false;
        isRunning = false;
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
            isRunning = true;
            anim.SetFloat(_speed, 1);
        }
        else
        {
            moveSpd = walkSpd;
            isRunning = false;
        }
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
        if (isGrounded)
        {
            isGrounded = false;
            rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
        }
        else
            return;
    }
    public void Fire(bool inp)
    {
        fireTimer = 0;
        if (fireTimer >= fireRate)
        {
            
            GameObject go = Instantiate(elecProj, transform.position, Quaternion.identity);
            go.GetComponent<Rigidbody>().AddForce(go.transform.forward * projkForce, ForceMode.Impulse);
            fireTimer = 0;
        }
        fireTimer += Time.deltaTime;
    }
    /* INPUT FOR ATTACK */
    public void Attack(bool inp)
    {
        //FREEZE MOVEMENT
        //rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.isKinematic = true;
        anim.SetTrigger(_hashAttack);

        //INCREASE COMBO INDEX
        comboIndex += 1;
        if (comboIndex > maxCombo)
            comboIndex = 0;
        // CALL ATTACK ANIMATION FOR CORRESPONDING INDEX
        anim.SetInteger(_hashAtkCount, comboIndex);
    }
    /* Reset Abliity to Move at end of attack animation*/
    private void animTimerEnded()
    {
        /* Test if constraints or kinematic is best
        rb.constraints &= ~RigidbodyConstraints.FreezePosition;
        moveSpd = walkSpd;
        */
        rb.isKinematic = false;
        comboIndex = 0;
    }
    /*toggle on and off in animation clip */
    public void HurtboxToggleEvent()
    {
        hurtboxGO.SetActive(!hurtboxGO.activeInHierarchy);
    }
    public void TakeDmg(float dmg)
    {
        curHP -= dmg;

        if(curHP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {

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
            anim.SetFloat(_speed, 0.5f);
        }
        else
            anim.SetFloat(_speed, 0);
        /*always move toward direction */
        rb.MovePosition(rb.position + moveDir * moveSpd * Time.deltaTime);

        isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.1f);
    }
    private void LateUpdate()
    {
        /* RESET ANIMATION TIME AFTER THE FRAME ENDS */
        animTime = Mathf.Repeat(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);
        anim.SetFloat(_hashStateTime, animTime);
        anim.ResetTrigger(_hashAttack);
    }
}
