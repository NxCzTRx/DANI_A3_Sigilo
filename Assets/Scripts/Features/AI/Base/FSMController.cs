using UnityEngine;

namespace Features.AI.Base
{
    public class FsmController<T> : MonoBehaviour where T : FsmController<T>
    {
        protected State<T> currentState;

        protected virtual void Update()
        {
            if (currentState)
            {
                currentState.OnUpdate();
            }
        }

        public void ChangeState(State<T> newState)
        {
            if (currentState)
            {
                currentState.OnExit();
            }
            currentState = newState;
            currentState.OnEnter();
        }
    }
}
