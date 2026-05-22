using System.Collections;
using Features.AI.Base;
using UnityEngine;

namespace Features.AI.Enemies.NinjaEnemy.States
{
    public class PatrolState : State<NinjaEnemyController>
    {
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float waitTimeAtPoint = 2f;
        
        private int _currentPatrolIndex = 0;
        private bool _isWaiting = false;
        
        public override void OnEnter()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                Debug.LogWarning($"Patrol state at {Controller.gameObject} has no points assigned.");
                return;
            }
            
            Controller.Agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        }

        public override void OnUpdate()
        {
            if (patrolPoints.Length <= 1) return;

            if (Controller.Agent.remainingDistance <= Controller.Agent.stoppingDistance &&
                !Controller.Agent.pathPending
                && !_isWaiting)
            {
                _isWaiting = true;
                StartCoroutine(NextPatrolCoroutine());
            }
        }

        public override void OnExit()
        {
            if (_isWaiting)
            {
                StopAllCoroutines();
                _isWaiting = false;
            }
        }
        
        private IEnumerator NextPatrolCoroutine()
        {
            yield return new WaitForSeconds(waitTimeAtPoint);
            
            if (patrolPoints.Length > 1)
            {
                _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
                Controller.Agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
            }
            
            _isWaiting = false;
        }
    }
}