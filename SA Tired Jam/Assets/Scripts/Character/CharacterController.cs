using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class CharacterController : MonoBehaviour
{
    public static CharacterController instance;
    [Header("Debug")]
    [SerializeField] bool paused = false;
    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] bool isMoving;
    [SerializeField] bool isSneaking;
    [SerializeField] bool isInteract;
    public float Health;

    [Header("References")]
    [SerializeField] Rigidbody foxRB;
    [SerializeField] GameObject foxGO;
    [SerializeField] Animator foxAnim;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] FoxControls foxControls;
    FoxInputHandler foxInputHandler;
    [Header("Settings")]
    [SerializeField] float speedMultiplier;
    [SerializeField] float walkSpeed;
    [SerializeField] float crouchSpeed;

    private void Awake()
    {
        instance = this;
        //Instantiate classes required by CharacterController
        if (playerInput == null)
        {
            playerInput = new PlayerInput();
        }
        if (foxInputHandler == null)
        {
            foxInputHandler = new FoxInputHandler();
        }
        if (foxControls == null)
        {
            foxControls = FoxInputHandler.foxControls;
        }
    }
    private void Start()
    {
        PauseScript.Instance.pauseEvent.AddListener(OnPause);
    }

    public void OnEnable()
    {
        FoxInputHandler.Enable();
        Debug.Log("Initialized");
        FoxInputHandler.OnMovePerformed.AddListener(InputMove);
        FoxInputHandler.OnCrouchPerformed.AddListener(OnCrouch);
        FoxInputHandler.OnInteractPerformed.AddListener(OnInteract);
    }
    public void OnDisable()
    {
        FoxInputHandler.OnMovePerformed.RemoveListener(InputMove);
        FoxInputHandler.OnCrouchPerformed.RemoveListener(OnCrouch);
        FoxInputHandler.OnInteractPerformed.RemoveListener(OnInteract);
    }
    void OnDestroy()
    {
        PauseScript.Instance.pauseEvent.RemoveListener(OnPause);
    }

    private void Update()
    {
        if (moveInput != Vector2.zero)
        {
            OnPlayerMove();
        }
    }

    //Movement
    public void OnPlayerMove()
    {
        Vector3 moveCombined = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = moveCombined.normalized;
        if (moveCombined != Vector3.zero)
        {
            foxRB.linearVelocity = new Vector3(moveDirection.x * speedMultiplier, 0, moveDirection.z * speedMultiplier);
            if (isSneaking)
            {
                speedMultiplier = crouchSpeed;
            }
            else
            {
                speedMultiplier = walkSpeed;
            }
        }
        else
        {
            speedMultiplier = 0;
            foxRB.linearVelocity = Vector3.zero;
        }
    }

    //Listeners
    void InputMove(Vector2 _moveInput)
    {
        this.moveInput = _moveInput;
        if (moveInput != Vector2.zero)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
    void OnCrouch(bool _sneaking)
    {
        this.isSneaking = _sneaking;
    }
    void OnInteract(bool _interact)
    {
        this.isInteract = _interact;
    }

    private void OnPause(bool _isPaused)
    {
        this.paused = _isPaused;
        if (paused)
        {
            isMoving = false;
            moveInput = Vector2.zero;
            OnDisable();
        }
        else
        {
            OnEnable();
        }
    }

}
