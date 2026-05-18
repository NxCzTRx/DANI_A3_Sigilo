using System;
using Core.Communication;
using Features.Player.Structs;
using Features.Player.Components.Sound;
using UnityEngine;

namespace Features.Player.Components.Movement
{
    public class PlayerMovement : MediatorClientSystem<PlayerController>
    {
        [SerializeField] private PlayerSpeeds speeds;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float noiseCooldown = 0.2f;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;

        private readonly PlayerMovementLogic _playerMovementLogic = new();

        private CharacterController _characterController;
        private Camera _mainCamera;

        private Vector2 _direction;
        private bool _isSprinting;
        private bool _isCrouching;
        private bool _wasGrounded;
        
        private Vector3 _verticalVelocity;
        private float _nextNoiseTime = 0f;

        private float DesiredSpeed =>
            _isCrouching ? speeds.Crouch :
            _isSprinting ? speeds.Sprint :
            speeds.Walk;

        protected override void Awake()
        {
            base.Awake();
            _characterController = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
            _wasGrounded = true;
        }

        protected void OnEnable()
        {
            if (Mediator != null)
            {
                Mediator.OnMove += HandleMove;
                Mediator.OnSprint += HandleSprint;
                Mediator.OnCrouch += HandleCrouch;
                Mediator.OnJump += HandleJump;
            }
        }

        private void Update()
        {
            ApplyGravity();

            _playerMovementLogic.Move(
                _characterController,
                _direction,
                _mainCamera.transform,
                DesiredSpeed
            );
            
            _characterController.Move(_verticalVelocity * Time.deltaTime);
            
            RotateToMovement();
            CheckAndEmitNoise();
        }

        private void OnDisable()
        {
            if (Mediator != null)
            {
                Mediator.OnMove -= HandleMove;
                Mediator.OnSprint -= HandleSprint;
                Mediator.OnCrouch -= HandleCrouch;
                Mediator.OnJump -= HandleJump;
            }
        }

        private void ApplyGravity()
        {
            bool isCurrentGrounded = _characterController.isGrounded;

            if (isCurrentGrounded && _verticalVelocity.y < 0)
            {
                _verticalVelocity.y = -2f;

                if (!_wasGrounded)
                {
                    Mediator?.TriggerLanded();
                }
            }

            _verticalVelocity.y += gravity * Time.deltaTime;
            _wasGrounded = isCurrentGrounded;
        }

        private void HandleJump()
        {
            if (_characterController.isGrounded && !_isCrouching)
            {
                _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                Mediator?.TriggerJumped();
                _wasGrounded = false;
            }
        }
        
        private void RotateToMovement()
        {
            if (_direction.sqrMagnitude < 0.01f) return;

            var camForward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            var camRight = Vector3.ProjectOnPlane(_mainCamera.transform.right, Vector3.up).normalized;
            var direction = camForward * _direction.y + camRight * _direction.x;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(direction),
                rotationSpeed * Time.deltaTime
            );
        }
        
        private void CheckAndEmitNoise()
        {
            if (_direction.sqrMagnitude < 0.01f || !_characterController.isGrounded) return;
            if (_isCrouching) return;
            if (Time.time < _nextNoiseTime) return;
            
            NoiseType currentNoise = _isSprinting ? NoiseType.Sprint : NoiseType.NormalWalk;
            Mediator?.TriggerNoise(currentNoise);
            _nextNoiseTime = Time.time + noiseCooldown;
        }

        private void HandleMove(Vector2 input) => _direction = input;

        private void HandleSprint(bool isSprinting) => _isSprinting = isSprinting;

        private void HandleCrouch(bool isCrouching) => _isCrouching = isCrouching;
    }
}