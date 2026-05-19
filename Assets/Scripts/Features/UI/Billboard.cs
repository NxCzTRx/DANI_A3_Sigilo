using UnityEngine;

namespace Features.UI
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private bool lockYRotation = true;
        
        private Transform _targetCamera;

        private void Start()
        {
            _targetCamera = Camera.main.transform;
        }
        
        private void LateUpdate()
        {
            if (_targetCamera == null) return;

            if (lockYRotation)
            {
                Vector3 targetPosition = transform.position + _targetCamera.forward;
                targetPosition.y = transform.position.y; 
                
                transform.LookAt(targetPosition);
            }
            else
            {
                transform.rotation = _targetCamera.rotation;
            }
        }
    }
}