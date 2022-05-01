using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;
[Serializable]
public class InputMoveEv : UnityEvent<float, float> { }
[Serializable]
public class InpRunEv : UnityEvent<bool> { }
[Serializable]
public class InpJumpEv : UnityEvent<bool> { }
[Serializable]
public class InpNorthEv : UnityEvent<bool> { }
[Serializable]
public class InpSouthEv : UnityEvent<bool> { }
[Serializable]
public class InpEastEv : UnityEvent<bool> { }
[Serializable]
public class InpWestEv : UnityEvent<bool> { }
[Serializable]
public class InpFireEv : UnityEvent<bool> { }
[Serializable]
public class InpAimEv : UnityEvent<bool> { }
public class InputCon : MonoBehaviour
{
    PlayerInp con;
    public InputMoveEv evMove;
    public InpRunEv evRun;
    public InpJumpEv evJump;
    public InpNorthEv evNorth;
    public InpSouthEv evSouth;
    public InpEastEv evEast;
    public InpWestEv evWest;
    public InpFireEv evFire;
    public InpAimEv evAim;
    private void Awake()
    {
        con = new PlayerInp();
    }
    private void OnEnable()
    {
        con.InGameAP.Enable();
        con.InGameAP.Move.performed += OnMovePerform;
        con.InGameAP.Move.canceled += OnMovePerform;
        con.InGameAP.Run.performed += OnRunPerform;
        con.InGameAP.Run.canceled += OnRunPerform;
        con.InGameAP.Jump.performed += OnJumpPerform;
        con.InGameAP.North.performed += OnNorthPerform;
        con.InGameAP.South.performed += OnSouthPerform;
        con.InGameAP.East.performed += OnEastPerform;
        con.InGameAP.West.performed += OnWestPerform;
        con.InGameAP.Fire.performed += OnFirePerform;
        con.InGameAP.Aim.performed += OnAimPerform;
        con.InGameAP.Aim.canceled += OnAimPerform;
    }
    private void OnMovePerform(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        evMove.Invoke(moveInput.x, moveInput.y);
    }
    private void OnRunPerform(InputAction.CallbackContext context)
    {
        bool run = context.ReadValueAsButton();
        evRun.Invoke(run);
    }
    private void OnJumpPerform(InputAction.CallbackContext context)
    {
        bool jump = context.ReadValueAsButton();
        evJump.Invoke(jump);
    }
    private void OnNorthPerform(InputAction.CallbackContext context)
    {
        bool north = context.ReadValueAsButton();
        evNorth.Invoke(north);
    }
    private void OnSouthPerform(InputAction.CallbackContext context)
    {
        bool south = context.ReadValueAsButton();
        evSouth.Invoke(south);
    }
    private void OnEastPerform(InputAction.CallbackContext context)
    {
        bool east = context.ReadValueAsButton();
        evEast.Invoke(east);
    }
    private void OnWestPerform(InputAction.CallbackContext context)
    {
        bool west = context.ReadValueAsButton();
        evWest.Invoke(west);
    }
    private void OnFirePerform(InputAction.CallbackContext context)
    {
        bool fire = context.ReadValueAsButton();
        evFire.Invoke(fire);
    }
    private void OnAimPerform(InputAction.CallbackContext context)
    {
        bool aim = context.ReadValueAsButton();
        evAim.Invoke(aim);
    }
}

