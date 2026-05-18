using System;
using Features.AI.Definitions;
using UnityEngine;

namespace Features.AI.Health
{
    public class EnemyHealth : MonoBehaviour, IDamageableByPlayer
    {
        public Transform Transform => gameObject.transform;
        
        public event Action OnDeath;
        
        public void TakeDamage()
        {
            OnDeath?.Invoke();
        }
    }
}
