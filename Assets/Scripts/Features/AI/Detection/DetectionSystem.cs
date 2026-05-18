using UnityEngine;

namespace Features.AI.Detection
{
    public class DetectionSystem : MonoBehaviour
    {
        [Range(1, 360)]
        [SerializeField] private float detectionAngle = 60f;
        [SerializeField] private float detectionDistance = 10f;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Transform rayOrigin;
        
        private float _sqrDetectionDistance;
        private float _cosHalfAngle;

        private void Awake()
        {
            _sqrDetectionDistance = detectionDistance * detectionDistance;
            _cosHalfAngle = Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad);
        }

        public bool IsPlayerDetected(Transform player, Transform enemy)
        {
            Vector3 directionToPlayer = player.position - enemy.position;
            
            float sqrDistance = directionToPlayer.sqrMagnitude;

            if (sqrDistance > _sqrDetectionDistance)
                return false;
            
            directionToPlayer.Normalize();
            float dotProduct = Vector3.Dot(enemy.forward, directionToPlayer);

            if (dotProduct < _cosHalfAngle) return false;

            var rayStart = rayOrigin != null ? rayOrigin.position : transform.position; 
            
            return !Physics.Raycast(rayStart, directionToPlayer, detectionDistance, obstacleLayer);
        }
        
private void OnDrawGizmos()
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;
            Vector3 originPosition = rayOrigin != null ? rayOrigin.position : transform.position;
            Quaternion originRotation = rayOrigin != null ? rayOrigin.rotation : transform.rotation;
            Gizmos.matrix = Matrix4x4.TRS(originPosition, originRotation, Vector3.one);

            Gizmos.color = Color.darkRed;
            
            float halfAngleRad = detectionAngle * 0.5f * Mathf.Deg2Rad;
            float radius = detectionDistance * Mathf.Sin(halfAngleRad);
            float coneDepth = detectionDistance * Mathf.Cos(halfAngleRad);
            Vector3 coneCenter = Vector3.forward * coneDepth;
            
            Vector3 topPoint = coneCenter + (Vector3.up * radius);
            Vector3 bottomPoint = coneCenter + (Vector3.down * radius);
            Vector3 leftPoint = coneCenter + (Vector3.left * radius);
            Vector3 rightPoint = coneCenter + (Vector3.right * radius);

            Gizmos.DrawLine(Vector3.zero, topPoint);
            Gizmos.DrawLine(Vector3.zero, bottomPoint);
            Gizmos.DrawLine(Vector3.zero, leftPoint);
            Gizmos.DrawLine(Vector3.zero, rightPoint);

#if UNITY_EDITOR
            UnityEditor.Handles.matrix = Gizmos.matrix;
            UnityEditor.Handles.color = Color.darkRed;
            
            UnityEditor.Handles.DrawWireDisc(coneCenter, Vector3.forward, radius);
            
            UnityEditor.Handles.color = new Color(0.5f, 0f, 0f, 0.1f);
            
            UnityEditor.Handles.DrawSolidArc(Vector3.zero, Vector3.up, leftPoint, detectionAngle, detectionDistance);
            UnityEditor.Handles.DrawSolidArc(Vector3.zero, Vector3.right, topPoint, detectionAngle, detectionDistance);
#endif
            
            Gizmos.matrix = originalMatrix;
        }
    }
}