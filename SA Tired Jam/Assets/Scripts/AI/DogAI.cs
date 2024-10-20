
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
        dogActions.foxTransform = null;
        dogActions.canHearFox = false;
        //Actions List
        actions = new List<Action>{
            new IdleAction(),
            new PatrolAction(),
            /*new ChaseAction(),
            new AttackAction(),*/
        };
        //Patrol Movement
        dogActions.navAgent.destination = dogActions.patrolPoints[dogActions.patrolIndex].position;
        dogActions.remainingDistance = dogActions.navAgent.remainingDistance;
        //UtilitySystem
        dogActions.StartCoroutine(UtilitySystemCoroutine());
    }

    public override void Update()
    {
        dogActions.remainingDistance = dogActions.navAgent.remainingDistance;
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
        !actionObject.dogActions.aiClear && actionObject.dogActions.counter < 5)
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
            actionObject.dogActions.navAgent.destination =
            actionObject.dogActions.patrolPoints[actionObject.dogActions.patrolIndex].position;
        }
    }
}
public class PatrolAction : Action
{
    public override float Evaluate(ActionObject actionObject)
    {
        if (!actionObject.dogActions.endLevel && !actionObject.dogActions.aiClear)
        {
            float _foxDistance = ((DogAI)actionObject).dogActions.foxDistance;
            switch (actionObject.dogActions.navAgent.remainingDistance)
            {
                case >= 0.1f when ((DogAI)actionObject).dogActions.foxTransform != null:
                    {
                        switch (_foxDistance)
                        {
                            case > 20:
                                return 1;
                            case < 20:
                                return 0;
                        }
                        break;
                    }
                case >= 0.1f:
                    return actionObject.dogActions.navAgent.remainingDistance;
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
        while (actionObject.dogActions.navAgent.pathPending || actionObject.dogActions.navAgent.remainingDistance >
        actionObject.dogActions.navAgent.stoppingDistance)
            return;
    }
}
/*public class ChaseAction : Action
{
    public override float Evaluate(ActionObject actionObject)
    {
        throw new System.NotImplementedException();
    }

    public override void Execute(ActionObject actionObject)
    {
        throw new System.NotImplementedException();
    }
}
public class AttackAction : Action
{
    public override float Evaluate(ActionObject actionObject)
    {
        throw new System.NotImplementedException();
    }

    public override void Execute(ActionObject actionObject)
    {
        throw new System.NotImplementedException();
    }
}*/
