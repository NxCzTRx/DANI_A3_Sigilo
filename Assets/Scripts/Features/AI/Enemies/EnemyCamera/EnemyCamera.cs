using System.Collections.Generic;
using Core.Managers;
using Features.AI.Detection;
using UnityEngine;

namespace Features.AI.Enemies.EnemyCamera
{
    [RequireComponent(typeof(DetectionSystem))]
    public class EnemyCamera : MonoBehaviour
    {
        [Header("Patrol")]
        [SerializeField] private List<Vector3> rotationAngles; 
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float waitTimeAtAngle = 2f;
        [SerializeField] private LayerMask targetLayer;

        private DetectionSystem _detectionSystem;
        private int _currentAngleIndex;
        private float _nextRotationTime;
        private bool _isWaiting;
        private Quaternion _targetRotation;
        private readonly Collider[] _targetBuffer = new Collider[1];

        private void Awake()
        {
            _detectionSystem = GetComponent<DetectionSystem>();

            if (rotationAngles != null && rotationAngles.Count > 0)
            {
                _targetRotation = Quaternion.Euler(rotationAngles[0]);
                transform.rotation = _targetRotation;
            }
            else
            {
                _targetRotation = transform.rotation;
            }
        }

        private void Update()
        {
            HandlePatrolRotation();
            CheckDetection();
        }

        private void HandlePatrolRotation()
        {
            if (rotationAngles == null || rotationAngles.Count <= 1) return;

            if (_isWaiting)
            {
                if (Time.time >= _nextRotationTime)
                {
                    _isWaiting = false;
                    _currentAngleIndex = (_currentAngleIndex + 1) % rotationAngles.Count;
                    _targetRotation = Quaternion.Euler(rotationAngles[_currentAngleIndex]);
                }
                return;
            }
            
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.5f)
            {
                _isWaiting = true;
                _nextRotationTime = Time.time + waitTimeAtAngle;
            }
        }

        private void CheckDetection()
        {
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 25f, _targetBuffer, targetLayer);
            if (numColliders == 0) return;

            Transform player = _targetBuffer[0].transform;
            
            if (_detectionSystem.IsPlayerDetected(player, transform))
            {
                OnPlayerDetected(player);
            }
        }

        private void OnPlayerDetected(Transform player)
        {
            GameManager.Instance.TriggerDetectionGameOver();
        }
    }
}