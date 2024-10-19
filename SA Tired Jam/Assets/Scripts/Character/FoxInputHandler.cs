using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
public class FoxInputHandler
{
    public static FoxControls foxControls;
    //Events
    public static UnityEvent<Vector2> OnMovePerformed = new UnityEvent<Vector2>();
    public static UnityEvent<bool> OnCrouchPerformed = new UnityEvent<bool>();
    public static UnityEvent<bool> OnInteractPerformed = new UnityEvent<bool>();
    public static UnityEvent<bool> OnPausePerformed = new UnityEvent<bool>();
    //Values
    public static Vector2 moveInput;
    public static bool crouch = false;
    public static bool interact = false;
    public static bool pause = false;
    //
    static FoxInputHandler instance;
    public static FoxInputHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FoxInputHandler();
            }
            return instance;
        }
        private set { instance = value; }
    }
    public static void Enable()
    {
        if (foxControls == null)
        {
            foxControls = new FoxControls();
        }
        RegisterInputs();
        foxControls.Enable();
    }
    public static void Disable()
    {
        if (foxControls == null)
        {
            return;
        }
        foxControls.Disable();
    }
    public static void Dispose()
    {
        if (foxControls == null)
        {
            return;
        }
        foxControls.Dispose();
    }
    static void RegisterInputs()
    {
        //Move
        foxControls.Player.Move.performed += MovePerformed;
        foxControls.Player.Move.canceled += MovePerformed;
        //Crouch
        foxControls.Player.Crouch.performed += CrouchPerformed;
        foxControls.Player.Crouch.canceled += CrouchCanceled;
        //Interact
        foxControls.Player.Interact.performed += InteractPerformed;
        foxControls.Player.Interact.canceled += InteractCanceled;
    }
    private static void MovePerformed(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().normalized != Vector2.zero)
        {
            moveInput = context.ReadValue<Vector2>().normalized;
        }
        if (context.ReadValue<Vector2>().normalized == Vector2.zero)
        {
            moveInput = Vector2.zero;
        }
        OnMovePerformed?.Invoke(moveInput);
    }
    private static void CrouchPerformed(InputAction.CallbackContext context)
    {
        crouch = true;
        OnCrouchPerformed?.Invoke(crouch);
    }
    private static void CrouchCanceled(InputAction.CallbackContext context)
    {
        crouch = false;
        OnCrouchPerformed?.Invoke(crouch);
    }
    private static void InteractPerformed(InputAction.CallbackContext context)
    {
        interact = true;
        OnInteractPerformed?.Invoke(interact);
    }
    private static void InteractCanceled(InputAction.CallbackContext context)
    {
        interact = false;
        OnInteractPerformed?.Invoke(interact);
    }
}
