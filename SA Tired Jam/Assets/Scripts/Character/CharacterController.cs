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
    private Vector2 moveInput;
    private Vector3 moveDirection;
    [SerializeField] float speedMultiplier;
    private bool isMoving;
    private bool isSneaking;
    private bool isSprint;
    [SerializeField] bool isTrapped = false;
    [SerializeField] bool isInteract;
    [SerializeField] bool isInCoup = false;
    [SerializeField] List<GameObject> chickenCoup = new List<GameObject>();
    public float Health;

    [Header("References")]
    [SerializeField] Rigidbody foxRB;
    [SerializeField] GameObject foxGO;
    [SerializeField] Animator foxAnim;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] FoxControls foxControls;
    FoxInputHandler foxInputHandler;
    [SerializeField] Collider noiseCollider;
    [Header("Settings")]
    [SerializeField] float walkSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float trapSpeed;
    [SerializeField] float walkNoiseRadius;
    [SerializeField] float crouchNoiseRadius;
    [SerializeField] float sprintNoiseRadius;
    [SerializeField] float trapNoiseRadius;

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
        PauseScript.Instance?.pauseEvent.AddListener(OnPause);
    }

    public void OnEnable()
    {
        FoxInputHandler.Enable();
        //Debug.Log("Initialized");
        EnableMovement();
        FoxInputHandler.OnInteractPerformed.AddListener(OnInteract);
    }
    public void OnDisable()
    {
        DisableMovement();
        FoxInputHandler.OnInteractPerformed.RemoveListener(OnInteract);
    }
    void OnDestroy()
    {
        PauseScript.Instance?.pauseEvent.RemoveListener(OnPause);
    }

    void EnableMovement()
    {
        FoxInputHandler.OnMovePerformed.AddListener(InputMove);
        FoxInputHandler.OnCrouchPerformed.AddListener(OnCrouch);
        FoxInputHandler.OnSprintPerformed.AddListener(OnSprint);
    }
    void DisableMovement()
    {
        FoxInputHandler.OnMovePerformed.RemoveListener(InputMove);
        FoxInputHandler.OnCrouchPerformed.RemoveListener(OnCrouch);
        FoxInputHandler.OnSprintPerformed.RemoveListener(OnSprint);
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
        if (moveCombined != Vector3.zero && !isTrapped)
        {
            if (isSneaking)
            {
                speedMultiplier = crouchSpeed;
                noiseCollider.GetComponent<SphereCollider>().radius = crouchNoiseRadius;
            }
            else if (isSprint)
            {
                speedMultiplier = sprintSpeed;
                noiseCollider.GetComponent<SphereCollider>().radius = sprintNoiseRadius;
            }
            else
            {
                speedMultiplier = walkSpeed;
                noiseCollider.GetComponent<SphereCollider>().radius = walkNoiseRadius;
            }
            foxRB.linearVelocity = new Vector3(moveDirection.x * speedMultiplier, 0, moveDirection.z * speedMultiplier);
        }
        else if (isTrapped)
        {
            speedMultiplier = trapSpeed;
            noiseCollider.GetComponent<SphereCollider>().radius = trapNoiseRadius;
            foxRB.linearVelocity = new Vector3(moveDirection.x * speedMultiplier, 0, moveDirection.z * speedMultiplier);
        }
        else
        {
            speedMultiplier = 0;
            foxRB.linearVelocity = Vector3.zero;
            noiseCollider.GetComponent<SphereCollider>().radius = 1;
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
    void OnSprint(bool _isSprint)
    {
        this.isSprint = _isSprint;
    }
    void OnInteract(bool _interact)
    {
        this.isInteract = _interact;
        if (isInteract && chickenCoup.Count > 0 && chickenCoup[0] != null)
        {
            if (!isInCoup)
            {
                Debug.Log("Fox is entering Chicken Coup");
                isInCoup = true;
                chickenCoup[0].gameObject.GetComponent<ChickenCoup>().FoxEnterCoup();
                DisableMovement();
                foxGO.SetActive(false);
            }
            else
            {
                Debug.Log("Fox is exiting Chicken Coup");
                chickenCoup[0].gameObject.GetComponent<ChickenCoup>().FoxExitCoup();
                isInCoup = false;
                EnableMovement();
                foxGO.SetActive(true);
            }
        }
        else if (isInteract && chickenCoup.Count == 0)
        {
            Debug.Log("There is nothing to interact with");
        }
    }

    private void OnPause(bool _isPaused)
    {
        this.paused = _isPaused;
        if (paused)
        {
            isMoving = false;
            moveInput = Vector2.zero;
        }
    }

    //Colliders and Triggers
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            Debug.Log("Fox has entered into a trap");
            //Trap needs to affect the Player's speed for X time
            StartCoroutine(Trapped());
        }
        if (other.gameObject.layer == 9)
        {
            Debug.Log("Fox is close to a Chicken Coup");
            //Character needs to recognize that there's a Chicken Coup, and can interact with it
            chickenCoup.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            Debug.Log("Fox has left a Chicken Coup");
            //Character needs to recognize that there's a Chicken Coup, and can interact with it
            chickenCoup.Remove(other.gameObject);
        }
    }

    //Coroutines
    IEnumerator Trapped()
    {
        isTrapped = true;
        yield return new WaitForSeconds(5);
        isTrapped = false;
    }
}
