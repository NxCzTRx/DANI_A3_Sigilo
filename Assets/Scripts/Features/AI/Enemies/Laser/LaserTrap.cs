using Core.Managers;
using Features.Player;
using UnityEngine;

namespace Features.AI.Enemies.Laser
{
    public class LaserTrap : MonoBehaviour
    {
        [Header("Patrol Settings")]
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float speed = 2f;
        [SerializeField] private bool isMoving = true;
    
        private int _targetIndex = 0;

        private void Update()
        {
            if (isMoving && waypoints.Length >= 2)
            {
                MoveLaser();
            }
        }

        private void MoveLaser()
        {
            Transform target = waypoints[_targetIndex];
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                _targetIndex = (_targetIndex + 1) % waypoints.Length;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PlayerController _))
            {
                TriggerGameOver();
            }
        }

        private void TriggerGameOver()
        {
            GameManager.Instance.TriggerDetectionGameOver();
        }
    }
}