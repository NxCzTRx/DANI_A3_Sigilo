using System;
using Core.Communication;
using Core.Input;
using Features.Player.Components.Sound;
using Unity.Cinemachine;
using UnityEngine;

namespace Features.Player
{
    public class PlayerController : MonoBehaviour, IMediator
    {
        private InputManager _inputManager;

        private bool _isInputInit = false;

        public event Action<Vector2> OnMove;
        public event Action<bool> OnCrouch;
        public event Action<bool> Crouched;
        public event Action<bool> OnSprint;
        public event Action OnAttack;
        public event Action OnThrow;
        public event Action<NoiseType> OnNoise;
        public event Action OnJump;
        public event Action Jumped;
        public event Action Landed;

        private void OnEnable()
        {
            if (_inputManager != null)
                SubscribeToInput();
        }

        private void Start()
        {
            _inputManager = InputManager.Instance;

            if (_inputManager == null)
            {
                Debug.LogError("No input manager found");
                return;
            }

            if (!_isInputInit)
                SubscribeToInput();
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null)
                UnsubscribeFromInput();
        }

        private void SubscribeToInput()
        {
            _inputManager.OnMove += HandleMove;
            _inputManager.OnSprint += HandleSprint;
            _inputManager.OnCrouch += HandleCrouch;
            _inputManager.OnAttack += HandleAttack;
            _inputManager.OnThrow += HandleThrow;
            _inputManager.OnJump += HandleJump;

            _isInputInit = true;
        }

        private void UnsubscribeFromInput()
        {
            _inputManager.OnMove -= HandleMove;
            _inputManager.OnSprint -= HandleSprint;
            _inputManager.OnCrouch -= HandleCrouch;
            _inputManager.OnAttack -= HandleAttack;
            _inputManager.OnThrow -= HandleThrow;
            _inputManager.OnJump -= HandleJump;

            _isInputInit = false;
        }

        public void TriggerNoise(NoiseType noiseType) =>
            OnNoise?.Invoke(noiseType);

        public void TriggerJumped() =>
            Jumped?.Invoke();

        public void TriggerLanded() =>
            Landed?.Invoke();
        
        public void TriggerCrouched(bool state) =>
            Crouched?.Invoke(state);

        private void HandleMove(Vector2 input)
        {
            var direction = input.normalized;
            OnMove?.Invoke(direction);
        }

        private void HandleSprint(bool state) =>
            OnSprint?.Invoke(state);

        private void HandleCrouch(bool state) =>
            OnCrouch?.Invoke(state);

        private void HandleAttack() =>
            OnAttack?.Invoke();

        private void HandleThrow() =>
            OnThrow?.Invoke();

        private void HandleJump() =>
            OnJump?.Invoke();
    }
}