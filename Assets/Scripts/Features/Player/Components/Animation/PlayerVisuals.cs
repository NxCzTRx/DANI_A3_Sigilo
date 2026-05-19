using System;
using Core.Communication;
using UnityEngine;

namespace Features.Player.Components.Animation
{
    public class PlayerVisuals : MediatorClientSystem<PlayerController>
    {
        private Animator _animator;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouched");
        private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
        private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
        private static readonly int LandTrigger = Animator.StringToHash("LandTrigger");

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();

            if (_animator == null)
                Debug.LogError("No animator found on player visuals");
        }

        private void OnEnable()
        {
            Mediator.OnMove += SetSpeed;
            Mediator.Crouched += SetCrouching;
            Mediator.OnSprint += SetSprinting;
            Mediator.Jumped += SetJumpTrigger;
            Mediator.Landed += SetLandTrigger;
        }

        private void OnDisable()
        {
            Mediator.OnMove -= SetSpeed;
            Mediator.Crouched -= SetCrouching;
            Mediator.OnSprint -= SetSprinting;
            Mediator.Jumped += SetJumpTrigger;
            Mediator.Landed += SetLandTrigger;
        }

        private void SetSpeed(Vector2 input)
        {
            var speed = input.sqrMagnitude > 0.01f ? input.magnitude : 0f;
            _animator.SetFloat(Speed, speed);
        }

        private void SetCrouching(bool isCrouching) =>
            _animator.SetBool(IsCrouching, isCrouching);

        private void SetSprinting(bool isSprinting) =>
            _animator.SetBool(IsSprinting, isSprinting);

        private void SetJumpTrigger() =>
            _animator.SetTrigger(JumpTrigger);

        private void SetLandTrigger() =>
            _animator.SetTrigger(LandTrigger);
    }
}