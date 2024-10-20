using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.Serialization;

public class DogActions : MonoBehaviour
{
    [Header("Debug")]
    public float counter;
    [Header("Debug - Dog")]
    [SerializeField] string id;
    [SerializeField] Rigidbody dogRB;
    public ActionObject currentActionObject;
    public float remainingDistance;
    public Transform currentTarget;
    public bool isIdle;
    public bool isMoving;
    public bool isRunning;
    public bool isAttacking;
    public bool canHearNoise;
    public bool endLevel = false;
    public bool aiClear = false;

    [Header("Debug - Fox")]
    public bool foxTarget = false;
    public CharacterController fox;
    public Vector3 foxPosition;
    public Transform foxTransform;
    public Vector3 foxDirection;
    public float foxDistance;
    [Header("Debug - Coup")]
    public bool coupTarget = false;
    public ChickenCoup chickenCoup;
    public Vector3 coupPosition;
    public Transform coupTransform;
    public Vector3 coupDirection;
    public float coupDistance;
    [Header("Settings - Movement")]
    public List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] bool isActive = true;
    public int patrolIndex;
    public float patrolSpeed;
    public float chaseSpeed;
    public Transform endLevelPoint;
    [Header("References")]
    public Animator dogAnimator;
    public NavMeshAgent navAgent;

    private void Awake()
    {
        ValidateAndInitActionObject();
        dogRB = GetComponent<Rigidbody>();
        endLevel = false;
        aiClear = false;
    }
    private void OnEnable()
    {
        currentActionObject?.OnEnable();
    }
    private void OnDisable()
    {
        currentActionObject?.OnDisable();
    }
    private void Start()
    {
        RegisterAndConfigureActionObject();
        currentActionObject?.Start();
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        currentActionObject?.Update();
    }
    void FixedUpdate()
    {
        if (!isActive)
        {
            return;
        }

        currentActionObject?.FixedUpdate();
    }

    void OnDrawGizmos()
    {
        currentActionObject?.OnDrawGizmos();
    }
    private void ValidateAndInitActionObject()
    {
        if (currentActionObject == null)
        {
            //Debug.LogError ($"{gameObject.name} disabled");
            gameObject.SetActive(false);
            return;
        }

        currentActionObject = Instantiate(currentActionObject);
        //Debug.Log ($"{currentActionObject.name} {currentActionObject.GetInstanceID ()}");

        currentActionObject?.Awake();
    }
    void RegisterAndConfigureActionObject()
    {
        if (currentActionObject != null)
        {
            currentActionObject.dogActions = this;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Physics.IgnoreCollision(collision.collider, this.gameObject.GetComponent<Collider>());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            //Debug.Log("Dog can hear noise");
            if (other.gameObject.GetComponentInParent<CharacterController>() != null)
            {
                foxTarget = true;
                foxTransform = other.gameObject.GetComponentInParent<CharacterController>().transform;
            }
            if (other.gameObject.GetComponentInParent<ChickenCoup>() != null)
            {
                //Target is Coup
                coupTarget = true;
                chickenCoup = other.gameObject.GetComponentInParent<ChickenCoup>();
                coupTransform = other.gameObject.GetComponentInParent<ChickenCoup>().transform;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            if (other.gameObject.GetComponentInParent<CharacterController>() != null)
            {
                foxTarget = false;
                foxTransform = null;
            }
            if (other.gameObject.GetComponentInParent<ChickenCoup>() != null)
            {
                coupTarget = false;
                coupTransform = null;
            }
        }
    }

    //Create ID
    [ContextMenu("Generate guid for ID")]
    void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
}
