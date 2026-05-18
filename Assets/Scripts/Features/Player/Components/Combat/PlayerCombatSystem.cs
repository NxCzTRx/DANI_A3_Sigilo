using System;
using System.Collections.Generic; // Necesario para HashSet
using Core.Communication;
using Features.AI.Definitions;
using UnityEngine;

namespace Features.Player.Components.Combat
{
    public class PlayerCombatSystem : MediatorClientSystem<PlayerController>
    {
        private readonly HashSet<IDamageableByPlayer> _damageablesInRange = new HashSet<IDamageableByPlayer>();

        [SerializeField] private float attackCooldown = 2f;
        
        private float _nextAttackTime = 0f;
        public bool CanAttack => Time.time >= _nextAttackTime;

        private void OnEnable()
        {
            if (Mediator != null) Mediator.OnAttack += Attack;
        }

        private void OnDisable()
        {
            if (Mediator != null) Mediator.OnAttack -= Attack;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageableByPlayer damageable))
            {
                _damageablesInRange.Add(damageable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IDamageableByPlayer damageable))
            {
                _damageablesInRange.Remove(damageable);
            }
        }

        private void Attack()
        {
            if (!CanAttack) return;
            
            IDamageableByPlayer closestDamageable = null;

            Vector3 playerPosition = transform.position;
            
            float closestSqrDistance = Mathf.Infinity;

            foreach (var damageable in _damageablesInRange)
            {
                if (damageable == null) continue;
                
                Vector3 directionToEnemy = damageable.Transform.position - playerPosition;
                float sqrDistance = directionToEnemy.sqrMagnitude;
                
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestDamageable = damageable;
                }
            }
            
            if (closestDamageable == null) return;
            
            closestDamageable.TakeDamage();
            
            if (closestDamageable.Equals(null))
            {
                _damageablesInRange.Remove(closestDamageable);
            }
            
            _nextAttackTime = Time.time + attackCooldown;
        }
    }
}