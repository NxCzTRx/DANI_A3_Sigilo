using System;
using System.Collections.Generic;
using Core.Communication;
using Features.AI.Definitions;
using UnityEngine;

namespace Features.Player.Components.Combat
{
    public class PlayerCombatSystem : MediatorClientSystem<PlayerController>
    {
        private readonly HashSet<IDamageableByPlayer> _damageablesInRange = new HashSet<IDamageableByPlayer>();

        [Header("Attack Settings")]
        [SerializeField] private float attackCooldown = 2f;
        
        [Header("UI / Visual Indicator")]
        [SerializeField] private GameObject killIndicatorUI;
        
        private float _nextAttackTime = 0f;
        private float _nextCleanTime = 0f;
        private const float CleanInterval = 0.2f;

        public bool CanAttack => Time.time >= _nextAttackTime;

        protected override void Awake()
        {
            base.Awake();
            UpdateKillIndicatorState();
        }

        private void OnEnable()
        {
            if (Mediator != null) Mediator.OnAttack += Attack;
        }

        private void OnDisable()
        {
            if (Mediator != null) Mediator.OnAttack -= Attack;
        }

        private void Update()
        {
            if (Time.time >= _nextCleanTime)
            {
                _nextCleanTime = Time.time + CleanInterval;
                UpdateKillIndicatorState();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageableByPlayer damageable))
            {
                _damageablesInRange.Add(damageable);
                UpdateKillIndicatorState();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IDamageableByPlayer damageable))
            {
                _damageablesInRange.Remove(damageable);
                UpdateKillIndicatorState();
            }
        }

        private void Attack()
        {
            if (!CanAttack) return;
            
            CleanDeadTargets();
            
            IDamageableByPlayer closestDamageable = null;
            Vector3 playerPosition = transform.position;
            float closestSqrDistance = Mathf.Infinity;

            foreach (var damageable in _damageablesInRange)
            {
                if (damageable == null || damageable.Transform == null) continue;
                
                Vector3 directionToEnemy = damageable.Transform.position - playerPosition;
                float sqrDistance = directionToEnemy.sqrMagnitude;
                
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestDamageable = damageable;
                }
            }
            
            if (closestDamageable == null) return;
            
            _damageablesInRange.Remove(closestDamageable);
            
            closestDamageable.TakeDamage();
            
            _nextAttackTime = Time.time + attackCooldown;
            
            UpdateKillIndicatorState();
        }

        private void UpdateKillIndicatorState()
        {
            if (killIndicatorUI == null) return;

            CleanDeadTargets();

            bool hasTargets = _damageablesInRange.Count > 0;
            
            if (killIndicatorUI.activeSelf != hasTargets)
            {
                killIndicatorUI.SetActive(hasTargets);
            }
        }

        private void CleanDeadTargets()
        {
            _damageablesInRange.RemoveWhere(target => target == null || target.Equals(null) || target.Transform == null);
        }
    }
}