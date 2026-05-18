using System;
using System.Collections.Generic;
using Core.Communication;
using Features.AI.Definitions;
using UnityEngine;

namespace Features.Player.Components.Sound
{
    public class PlayerNoiseSystem : MediatorClientSystem<PlayerController>
    {
        [Serializable]
        public struct NoiseSettings
        {
            public NoiseType noiseType;
            public float radius;
        }
        
        [SerializeField] private List<NoiseSettings> noiseConfigurations;
        [SerializeField] private LayerMask enemyLayer;

        private readonly Dictionary<NoiseType, NoiseSettings> _noiseDictionary =
            new Dictionary<NoiseType, NoiseSettings>();
        
        private readonly Collider[] _overlapBuffer = new Collider[20];
        private readonly HashSet<IAlertable> _alertedEnemiesFilter = new HashSet<IAlertable>();

        protected override void Awake()
        {
            base.Awake();
            
            foreach (var config in noiseConfigurations)
            {
                if (!_noiseDictionary.ContainsKey(config.noiseType))
                {
                    _noiseDictionary.Add(config.noiseType, config);
                }
            }
        }

        private void OnEnable()
        {
            Mediator.OnNoise += MakeNoise;
        }

        private void OnDisable()
        {
            Mediator.OnNoise -= MakeNoise;
        }

        public void MakeNoise(NoiseType type)
        {
            if (!_noiseDictionary.TryGetValue(type, out NoiseSettings settings))
                return;
            
            var numColliders = Physics.OverlapSphereNonAlloc(
                transform.position,
                settings.radius,
                _overlapBuffer,
                enemyLayer);

            if (numColliders == 0) return;
            
            _alertedEnemiesFilter.Clear();

            for (int i = 0; i < numColliders; i++)
            {
                Collider col = _overlapBuffer[i];
                if (col == null) continue;
                
                IAlertable alertable = col.GetComponentInParent<IAlertable>();

                if (alertable == null) continue;
                
                if (!_alertedEnemiesFilter.Add(alertable)) continue;

                alertable.OnHearNoise(transform.position);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (noiseConfigurations == null) return;

            Gizmos.color = Color.blue;
            foreach (var config in noiseConfigurations)
            {
                Gizmos.DrawWireSphere(transform.position, config.radius);
            }
        }
    }
}