using UnityEngine;

namespace AI.UtilitySystem
{
    public abstract class Action
    {
        public abstract float Evaluate(ActionObject actionObject);

        public abstract void Execute(ActionObject actionObject);
    }
}