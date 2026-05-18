using Features.AI.Base;
using UnityEngine;

public abstract class State<T> : MonoBehaviour where T : FsmController<T>
{
    protected T Controller;
    
    public virtual void InitController(T controller)
    {
        Controller = controller;
    }
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
