using System.Collections.Generic;
using Features.AI.Definitions;
using UnityEngine;

namespace Features.Player.Components.Items
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ObjectNoiseEmitter : MonoBehaviour
    {
        [SerializeField] private float alertRadius = 8f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float minImpactVelocity = 1.5f;

        private readonly Collider[] _overlapBuffer = new Collider[15];
        private readonly HashSet<IAlertable> _alertedEnemiesFilter = new();
        
        private bool _hasImpacted;

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasImpacted) return;
            if (collision.relativeVelocity.magnitude < minImpactVelocity) return;

            _hasImpacted = true;

            ExecuteNoiseFilter();
            Destroy(gameObject, 2f);
        }

        private void ExecuteNoiseFilter()
        {
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, alertRadius, _overlapBuffer, enemyLayer);
            if (numColliders == 0) return;

            _alertedEnemiesFilter.Clear();

            for (int i = 0; i < numColliders; i++)
            {
                Collider col = _overlapBuffer[i];
                if (col == null) continue;

                IAlertable alertable = col.GetComponentInParent<IAlertable>();
                if (alertable == null || _alertedEnemiesFilter.Contains(alertable)) continue;

                _alertedEnemiesFilter.Add(alertable);
                alertable.OnHearNoise(transform.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, alertRadius);
        }
    }
}