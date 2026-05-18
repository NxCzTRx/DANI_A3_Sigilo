using System;
using Core.Communication;
using UnityEngine;

namespace Features.AI.Enemies.NinjaEnemy.Visuals
{
    public class EnemyNinjaVisuals : MediatorClientSystem<NinjaEnemyController>
    {
        private Animator _animator;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int SearchTrigger = Animator.StringToHash("SearchTrigger");

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            Mediator.OnSearch += UpdateSearchTrigger;
        }

        private void OnDisable()
        {
            Mediator.OnSearch += UpdateSearchTrigger;
        }

        private void FixedUpdate()
        {
            _animator.SetFloat(Speed, Mediator.Agent.velocity.sqrMagnitude);
        }
        
        private void UpdateSearchTrigger()
        {
            _animator.SetTrigger(SearchTrigger);
        }
    }
}
