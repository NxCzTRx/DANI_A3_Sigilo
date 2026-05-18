using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;
        
        private PlayerInput _playerInput;

        public event Action<Vector2> OnMove;
        public event Action<bool> OnSprint;
        public event Action<bool> OnCrouch;
        public event Action OnAttack;
        public event Action OnThrow;
        public event Action OnJump;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            _playerInput = GetComponent<PlayerInput>();

            if (_playerInput == null)
                Debug.LogError("No player input found");
        }

        private void OnEnable() => SubscribeToInput();

        private void OnDisable() => UnsubscribeFromInput();

        private void SubscribeToInput()
        {
            _playerInput.actions["Move"].performed += HandleMove;
            _playerInput.actions["Move"].canceled += HandleMove;
            _playerInput.actions["Sprint"].started += HandleSprint;
            _playerInput.actions["Sprint"].canceled += HandleSprint;
            _playerInput.actions["Crouch"].started += HandleCrouch;
            _playerInput.actions["Crouch"].canceled += HandleCrouch;
            _playerInput.actions["Attack"].started += HandleAttack;
            _playerInput.actions["Throw"].started += HandleThrow;
            _playerInput.actions["Jump"].started += HandleJump;
        }

        private void UnsubscribeFromInput()
        {
            _playerInput.actions["Move"].performed -= HandleMove;
            _playerInput.actions["Move"].canceled -= HandleMove;
            _playerInput.actions["Sprint"].started -= HandleSprint;
            _playerInput.actions["Sprint"].canceled += HandleSprint;
            _playerInput.actions["Crouch"].started -= HandleCrouch;
            _playerInput.actions["Crouch"].canceled += HandleCrouch;
            _playerInput.actions["Attack"].started -= HandleAttack;
            _playerInput.actions["Throw"].started -= HandleThrow;
            _playerInput.actions["Jump"].started += HandleJump;
        }

        private void HandleMove(InputAction.CallbackContext ctx) =>
            OnMove?.Invoke(ctx.ReadValue<Vector2>());
        
        private void HandleSprint(InputAction.CallbackContext ctx) =>
            OnSprint?.Invoke(ctx.ReadValueAsButton());
        
        private void HandleCrouch(InputAction.CallbackContext ctx) =>
            OnCrouch?.Invoke(ctx.ReadValueAsButton());

        private void HandleAttack(InputAction.CallbackContext ctx) =>
            OnAttack?.Invoke();
        
        private void HandleThrow(InputAction.CallbackContext ctx) => 
            OnThrow?.Invoke();

        private void HandleJump(InputAction.CallbackContext ctx) =>
            OnJump?.Invoke();
    }
}