using UnityEngine;

namespace Features.Player.Components.Movement
{
    public class PlayerMovementLogic
    {
        private float _verticalVelocity = 0f;
        private Vector3 _horizontalVelocity = Vector3.zero;
            
        public void Move(
            CharacterController controller,
            Vector2 direction,
            Transform cameraTransform, 
            float moveSpeed
        )
        {
            SetVerticalVelocity(controller);
            SetHorizontalVelocity(direction, cameraTransform, moveSpeed);
            
            var velocity = _horizontalVelocity + Vector3.up * _verticalVelocity;
            
            controller.Move(velocity);
        }
        
        private void SetHorizontalVelocity(Vector2 direction, Transform cameraTransform, float moveSpeed)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            
            Vector3 moveDir = (forward * direction.y + right * direction.x);
            
            _horizontalVelocity = moveDir * (moveSpeed * Time.deltaTime);
        }
        
        private void SetVerticalVelocity(CharacterController controller)
        {
            if (controller.isGrounded)
            {
                _verticalVelocity = -2f * Time.deltaTime; 
                return;
            }
            
            _verticalVelocity += Physics.gravity.y * Time.deltaTime * Time.deltaTime;
        }
    }
}