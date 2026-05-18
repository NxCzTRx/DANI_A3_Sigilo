using UnityEngine;

namespace Core.Communication
{
    public interface IMediator {}
    
    public class MediatorClientSystem<T> : MonoBehaviour where T : IMediator
    {
        protected T Mediator;

        protected virtual void Awake()
        {
            Mediator = transform.root.GetComponent<T>();
            
            if (Mediator == null)
                Debug.LogError($"No mediator of type {typeof(T).Name} found in root");
        }
    }
}
