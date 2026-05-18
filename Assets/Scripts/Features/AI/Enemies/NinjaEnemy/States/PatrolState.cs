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
            if (patrolPoints.Length <= 1)
                Debug.LogError($"Patrol state at {Controller.gameObject} has to have at least tow points");
            
            Controller.Agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        }

        public override void OnUpdate()
        {
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
            
        }
        
        private IEnumerator NextPatrolCoroutine()
        {
            yield return new WaitForSeconds(waitTimeAtPoint);
            _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
            Controller.Agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
            _isWaiting = false;
        }
    }
}
