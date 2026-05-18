using System;
using Core.Communication;
using Features.AI.Base;
using Features.AI.Definitions;
using Features.AI.Detection;
using Features.AI.Enemies.NinjaEnemy.States;
using Features.AI.Health;
using UnityEngine;
using UnityEngine.AI;

namespace Features.AI.Enemies.NinjaEnemy
{
    public class NinjaEnemyController : FsmController<NinjaEnemyController>, IAlertable, IMediator
    {
        [SerializeField] private GameObject player;
        public GameObject Player => player;

        // --- Components ---
        public NavMeshAgent Agent { get; private set; }
        public DetectionSystem DetectionSystem { get; private set; }
        public EnemyHealth EnemyHealth { get; private set; }

        // --- States ---
        public PatrolState PatrolState { get; private set; }
        public AlertState AlertState { get; private set; }

        // --- Data ---
        public Vector3 PointOfInterest { get; private set; }

        // --- Events ---
        public event Action OnSearch;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();

            PatrolState = GetComponent<PatrolState>();
            AlertState = GetComponent<AlertState>();

            DetectionSystem = GetComponent<DetectionSystem>();
            EnemyHealth = GetComponent<EnemyHealth>();

            PatrolState.InitController(this);
            AlertState.InitController(this);
        }

        private void OnEnable()
        {
            EnemyHealth.OnDeath += HandleDeath;
        }

        private void Start()
        {
            ChangeState(PatrolState);
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (DetectionSystem.IsPlayerDetected(Player.transform, gameObject.transform))
            {
                //player found
            }
        }

        private void OnDisable()
        {
            EnemyHealth.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            //Spawn VFX
            Destroy(gameObject);
        }

        public void OnHearNoise(Vector3 position)
        {
            PointOfInterest = position;

            if (currentState == AlertState)
            {
                AlertState.RefreshAlert();
                return;
            }

            ChangeState(AlertState);
        }

        public void TriggerOnSearch() =>
            OnSearch?.Invoke();
    }
}