using Features.AI.Base;
using UnityEngine;

namespace Features.AI.Enemies.NinjaEnemy.States
{
    public class AlertState : State<NinjaEnemyController>
    {
        private Vector3 _destination;
        private bool _isDestinationReached;
    
        [SerializeField] private float searchRadius = 5f;
        [SerializeField] private float waitTimeBetweenSearches = 1.5f;
        
        private int _searchMovesCount;
        private const int MaxSearchMoves = 3;
        
        private float _nextSearchTime;
        private bool _isWaiting;

        public override void OnEnter()
        {
            RefreshAlert();
        }
    
        public void RefreshAlert()
        {
            _searchMovesCount = 0;
            _isDestinationReached = false;
            _isWaiting = false; 
        
            _destination = Controller.PointOfInterest;
            Controller.Agent.SetDestination(_destination);
        }

        public override void OnUpdate()
        {
            if (Controller.Agent.pathPending) return;
            
            if (_isWaiting)
            {
                if (Time.time >= _nextSearchTime)
                {
                    _isWaiting = false;
                    ExecuteNextAction();
                }
                return;
            }
            
            if (!_isDestinationReached && Controller.Agent.remainingDistance <= Controller.Agent.stoppingDistance)
            {
                _isDestinationReached = true;

                Controller.TriggerOnSearch();
                StartWait();
            }
        }

        public override void OnExit()
        {
            _isDestinationReached = false;
            _isWaiting = false;
        }

        private void StartWait()
        {
            _isWaiting = true;
            _nextSearchTime = Time.time + waitTimeBetweenSearches;
        }

        private void ExecuteNextAction()
        {
            if (_searchMovesCount < MaxSearchMoves)
            {
                Search();
            }
            else
            {
                Controller.ChangeState(Controller.PatrolState);
            }
        }

        private void Search()
        {
            _searchMovesCount++;

            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * searchRadius;
            randomDirection += _destination; 
            randomDirection.y = _destination.y; 

            if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out UnityEngine.AI.NavMeshHit hit, searchRadius, 1))
            {
                _isDestinationReached = false; 
                Controller.Agent.SetDestination(hit.position);
            }
            else
            {
                _isDestinationReached = true;
            }
        }
    }
}