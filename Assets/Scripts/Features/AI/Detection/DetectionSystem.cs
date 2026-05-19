using UnityEngine;
using UnityEngine.UI; // IMPORTANTE: Añadimos esto para poder usar el componente Image

namespace Features.AI.Detection
{
    public class DetectionSystem : MonoBehaviour
    {
        [Range(1, 360)]
        [SerializeField] private float detectionAngle = 60f;
        [SerializeField] private float detectionDistance = 10f;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Transform rayOrigin;

        [Header("Meter Settings")]
        [Tooltip("How many points per second will the meter rise when at maximum proximity?")]
        [SerializeField] private float detectionBuildSpeed = 100f;
        [Tooltip("By how many points per second will the meter decrease when the player hides?")]
        [SerializeField] private float detectionDecaySpeed = 50f;

        [Header("Debug & UI Setup")]
        [SerializeField] private string DEBUG_DetectionLevel;
        [SerializeField] private GameObject uiContainer;
        [SerializeField] private Image uiFillBar;

        public float DetectionLevel => _currentDetectionLevel;
        
        private float _currentDetectionLevel;
        private float _sqrDetectionDistance;
        private float _cosHalfAngle;

        private void Awake()
        {
            _sqrDetectionDistance = detectionDistance * detectionDistance;
            _cosHalfAngle = Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad);
            
            if (uiContainer != null)
            {
                uiContainer.SetActive(false);
            }
        }

        public bool IsPlayerDetected(Transform player, Transform enemy)
        {
            bool isVisuallyInSight = CheckVisualSight(player, enemy, out float distance);

            if (isVisuallyInSight)
            {
                float proximityFactor = 1f - (distance / detectionDistance);
                proximityFactor = Mathf.Clamp01(proximityFactor);
                
                _currentDetectionLevel += detectionBuildSpeed * proximityFactor * Time.deltaTime;
            }
            else
            {
                _currentDetectionLevel -= detectionDecaySpeed * Time.deltaTime;
            }
            
            _currentDetectionLevel = Mathf.Clamp(_currentDetectionLevel, 0f, 100f);
            DEBUG_DetectionLevel = $"Detection level: {_currentDetectionLevel}";
            
            UpdateDetectionUI();

            return _currentDetectionLevel >= 100f;
        }
        
        private void UpdateDetectionUI()
        {
            if (uiContainer == null || uiFillBar == null) return;

            if (_currentDetectionLevel > 0f)
            {
                if (!uiContainer.activeSelf)
                {
                    uiContainer.SetActive(true);
                }
                
                uiFillBar.fillAmount = _currentDetectionLevel / 100f;
            }
            else
            {
                if (uiContainer.activeSelf)
                {
                    uiFillBar.fillAmount = 0f;
                    uiContainer.SetActive(false);
                }
            }
        }

        private bool CheckVisualSight(Transform player, Transform enemy, out float currentDistance)
        {
            currentDistance = 0f;
            Vector3 directionToPlayer = player.position - enemy.position;
            float sqrDistance = directionToPlayer.sqrMagnitude;

            if (sqrDistance > _sqrDetectionDistance)
                return false;

            currentDistance = Mathf.Sqrt(sqrDistance);
            
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
            
            Gizmos.color = Color.Lerp(Color.darkRed, Color.red, _currentDetectionLevel / 100f);
            
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
            UnityEditor.Handles.color = Gizmos.color;
            
            UnityEditor.Handles.DrawWireDisc(coneCenter, Vector3.forward, radius);
            
            float alpha = Mathf.Lerp(0.05f, 0.3f, _currentDetectionLevel / 100f);
            UnityEditor.Handles.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, alpha);
            
            UnityEditor.Handles.DrawSolidArc(Vector3.zero, Vector3.up, leftPoint, detectionAngle, detectionDistance);
            UnityEditor.Handles.DrawSolidArc(Vector3.zero, Vector3.right, topPoint, detectionAngle, detectionDistance);
#endif
            
            Gizmos.matrix = originalMatrix;
        }
    }
}