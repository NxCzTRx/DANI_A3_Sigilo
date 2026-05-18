using System;
using Core.Communication;
using UnityEngine;

namespace Features.Player.Components.Items
{
    public class PlayerThrowSystem : MediatorClientSystem<PlayerController>
    {
        [SerializeField] private GameObject throwablePrefab;
        [SerializeField] private Transform throwPoint;      
        [SerializeField] private float throwForce = 15f;
        [SerializeField] private float throwCooldown = 1.5f;

        private float _nextThrowTime;

        private void OnEnable()
        {
            if (Mediator != null)
            {
                Mediator.OnThrow += ThrowItem;
            }
        }

        private void OnDisable()
        {
            if (Mediator != null)
            {
                Mediator.OnThrow -= ThrowItem;
            }
        }

        private void ThrowItem()
        {
            if (Time.time < _nextThrowTime) return;
            if (throwablePrefab == null || throwPoint == null) return;
            
            _nextThrowTime = Time.time + throwCooldown;

            GameObject clonedItem = Instantiate(throwablePrefab, throwPoint.position, throwPoint.rotation);
            
            if (clonedItem.TryGetComponent(out Rigidbody rb))
            {
                Vector3 throwDirection = throwPoint.forward + (Vector3.up * 0.15f);
                rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
            }
        }
    }
}