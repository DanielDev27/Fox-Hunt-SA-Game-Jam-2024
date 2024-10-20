
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Action = AI.UtilitySystem.Action;

[CreateAssetMenu(fileName = "AIUtilityActionObject", menuName = "AI/UtilitySystem/ActionObject", order = 0)]

public class DogAI : ActionObject
{
    [Header("Settings")]
    public float updateFrequency;
    [SerializeField] DogActions dogActionsRef;
    [Header("References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private List<Action> actions = new List<Action>();
    [SerializeField] Action bestAction;
    public override void Awake() { }

    public override void OnEnable() { }

    public override void OnDisable() { }
    public override void Start()
    {
        dogActions.counter = 0;
        //Fox Identifier @ Start
        dogActions.fox = CharacterController.instance;
        dogActions.chickenCoup = FindAnyObjectByType<ChickenCoup>();
        dogActions.foxTransform = null;
        dogActions.coupTransform = null;
        dogActions.canHearNoise = false;
        dogActions.foxDistance = Vector3.Distance(dogActions.fox.gameObject.transform.position, dogActions.transform.position);
        dogActions.coupDistance = Vector3.Distance(dogActions.chickenCoup.gameObject.transform.position, dogActions.transform.position);
        //Actions List
        actions = new List<Action>{
            new IdleAction(),
            new PatrolAction(),
            new ChaseAction(),
            //new AttackAction(),
        };
        //Patrol Movement
        dogActions.currentTarget = dogActions.patrolPoints[dogActions.patrolIndex];
        dogActions.navAgent.destination = dogActions.patrolPoints[dogActions.patrolIndex].position;
        dogActions.remainingDistance = dogActions.navAgent.remainingDistance;
        //UtilitySystem
        dogActions.StartCoroutine(UtilitySystemCoroutine());
    }

    public override void Update()
    {
        if (dogActions.foxTarget)
        {
            dogActions.foxPosition = dogActions.foxTransform.position;
            dogActions.foxDistance = Vector3.Distance(dogActions.foxPosition,
            dogActions.transform.position);
            dogActions.foxDirection = dogActions.foxPosition - dogActions.transform.position;
            dogActions.navAgent.SetDestination(dogActions.foxPosition);
            dogActions.remainingDistance = dogActions.navAgent.remainingDistance;
        }
        if (dogActions.coupTarget)
        {
            dogActions.coupPosition = dogActions.coupTransform.position;
            dogActions.coupDistance = Vector3.Distance(dogActions.chickenCoup.gameObject.transform.position,
            dogActions.transform.position);
            dogActions.coupDirection = dogActions.coupPosition - dogActions.transform.position;
            dogActions.navAgent.SetDestination(dogActions.coupPosition);
            dogActions.remainingDistance = dogActions.navAgent.remainingDistance;
        }
        else
        {
            dogActions.remainingDistance = dogActions.navAgent.remainingDistance;

        }
    }
    public override void FixedUpdate() { }
    public override void OnDrawGizmos() { }
    IEnumerator UtilitySystemCoroutine()
    {
        while (true)
        {
            EvaluateUtility();
            yield return new WaitForSeconds(updateFrequency);
        }
    }
    void EvaluateUtility()
    {
        bestAction = null;
        float _bestUtility = float.NegativeInfinity;
        foreach (var _action in actions)
        {
            float _utility = _action.Evaluate(this);
            //Debug.Log ($"Evaluating: {_action} | {_utility}");
            if (_utility > _bestUtility)
            {
                _bestUtility = _utility;
                bestAction = _action;
            }
        }

        //Debug.Log ($"<color=green>{soldierActions.name} Executing: {_bestAction}</color>");
        bestAction?.Execute(this);
    }

    void StopAI()
    {
        dogActions.endLevel = true;
    }

}

public class IdleAction : Action
{
    public override float Evaluate(ActionObject actionObject)
    {
        if (actionObject.dogActions.navAgent.remainingDistance < 0.01f && !actionObject.dogActions.endLevel &&
        !actionObject.dogActions.aiClear && actionObject.dogActions.counter < 5 && !actionObject.dogActions.foxTarget
        && !actionObject.dogActions.coupTarget)
        {
            return 1 / actionObject.dogActions.remainingDistance;
        }
        else
        {
            return 0;
        }
    }

    public override void Execute(ActionObject actionObject)
    {
        //Debug bools
        actionObject.dogActions.isIdle = true;
        actionObject.dogActions.isMoving = false;
        actionObject.dogActions.isRunning = false;
        actionObject.dogActions.isAttacking = false;

        //Actions
        ((DogAI)actionObject).Update();
        if (actionObject.dogActions.remainingDistance < 0.01f && !actionObject.dogActions.aiClear)
        {
            while (((DogAI)actionObject).dogActions.counter < 5)
            {
                ((DogAI)actionObject).dogActions.counter += ((DogAI)actionObject).updateFrequency;
                //Debug.Log(((DogAI)actionObject).dogActions.counter);
                return;
            }
            actionObject.dogActions.patrolIndex++;
            if (actionObject.dogActions.patrolIndex >= actionObject.dogActions.patrolPoints.Count)
            {
                actionObject.dogActions.patrolIndex = 0;
            }
            ((DogAI)actionObject).dogActions.counter = 0;
            ((DogAI)actionObject).dogActions.currentTarget =
            actionObject.dogActions.patrolPoints[actionObject.dogActions.patrolIndex];
            actionObject.dogActions.navAgent.destination =
            actionObject.dogActions.patrolPoints[actionObject.dogActions.patrolIndex].position;
        }

    }
}
public class PatrolAction : Action
{
    public override float Evaluate(ActionObject actionObject)
    {
        if (!actionObject.dogActions.endLevel && !actionObject.dogActions.aiClear
        && !actionObject.dogActions.foxTarget && !actionObject.dogActions.coupTarget)
        {
            switch (actionObject.dogActions.navAgent.remainingDistance)
            {
                case >= 0.1f:
                    return 1;
                case < 0.1f:
                    return 0;
            }
        }
        return 0;
    }

    public override void Execute(ActionObject actionObject)
    {
        //Debug bools
        actionObject.dogActions.isIdle = false;
        actionObject.dogActions.isMoving = true;
        actionObject.dogActions.isRunning = false;
        actionObject.dogActions.isAttacking = false;

        //Actions
        actionObject.dogActions.navAgent.isStopped = false;
        actionObject.dogActions.navAgent.SetDestination
        (actionObject.dogActions.patrolPoints[actionObject.dogActions.patrolIndex].position);
        actionObject.dogActions.navAgent.speed = actionObject.dogActions.patrolSpeed;
        ((DogAI)actionObject).dogActions.currentTarget =
            actionObject.dogActions.patrolPoints[actionObject.dogActions.patrolIndex];
        ((DogAI)actionObject).Update();

        while (actionObject.dogActions.navAgent.pathPending || actionObject.dogActions.navAgent.remainingDistance >
        actionObject.dogActions.navAgent.stoppingDistance)
            return;
    }
}
public class ChaseAction : Action
{
    public override float Evaluate(ActionObject actionObject)
    {
        if (((DogAI)actionObject).dogActions.foxTarget || ((DogAI)actionObject).dogActions.coupTarget)
        {
            return 10;
        }
        else
        {
            return 0;
        }
    }

    public override void Execute(ActionObject actionObject)
    {
        //Debug bools
        actionObject.dogActions.isIdle = false;
        actionObject.dogActions.isMoving = false;
        actionObject.dogActions.isRunning = true;
        actionObject.dogActions.isAttacking = false;

        //Actions
        if (((DogAI)actionObject).dogActions.foxTarget && !((DogAI)actionObject).dogActions.coupTarget)
        {
            ((DogAI)actionObject).dogActions.currentTarget = actionObject.dogActions.foxTransform;
            actionObject.dogActions.navAgent.SetDestination(actionObject.dogActions.foxPosition);
        }
        if (((DogAI)actionObject).dogActions.foxTarget && ((DogAI)actionObject).dogActions.coupTarget)
        {
            ((DogAI)actionObject).dogActions.currentTarget = actionObject.dogActions.foxTransform;
            actionObject.dogActions.navAgent.SetDestination(actionObject.dogActions.foxPosition);
        }
        if (!((DogAI)actionObject).dogActions.foxTarget && ((DogAI)actionObject).dogActions.coupTarget)
        {
            ((DogAI)actionObject).dogActions.currentTarget = actionObject.dogActions.coupTransform;
            actionObject.dogActions.navAgent.SetDestination(actionObject.dogActions.coupPosition);
        }
        ((DogAI)actionObject).Update();
        actionObject.dogActions.navAgent.speed = actionObject.dogActions.chaseSpeed;


    }
}
/*public class AttackAction : Action
{
    public override float Evaluate(ActionObject actionObject)
    {
        throw new System.NotImplementedException();
    }

    public override void Execute(ActionObject actionObject)
    {
    //Debug bools
        actionObject.dogActions.isIdle = false;
        actionObject.dogActions.isMoving = true;
        actionObject.dogActions.isRunning = false;
        actionObject.dogActions.isAttacking = false;

        ((DogAI)actionObject).dogActions.remainingDistance = ((DogAI)actionObject).dogActions.navAgent.remainingDistance;
        ((DogAI)actionObject).dogActions.foxDistance = Vector3.Distance(((DogAI)actionObject).dogActions.fox.gameObject.transform.position,
        ((DogAI)actionObject).dogActions.transform.position);
    }
}*/
