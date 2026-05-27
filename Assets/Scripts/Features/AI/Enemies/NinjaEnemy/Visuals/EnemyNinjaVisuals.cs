using System;
using Core.Communication;
using UnityEngine;

namespace Features.AI.Enemies.NinjaEnemy.Visuals
{
    public class EnemyNinjaVisuals : MonoBehaviour
    {
        private Animator _animator;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int SearchTrigger = Animator.StringToHash("SearchTrigger");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetSpeed(float sqrMagnitude)
        {
            _animator.SetFloat(Speed, sqrMagnitude);
        }
        
        public void UpdateSearchTrigger()
        {
            _animator.SetTrigger(SearchTrigger);
        }
    }
}
