using UnityEngine;
using UnityEngine.UI;

namespace Features.AI.Detection
{
    public class DetectionSystem : MonoBehaviour
    {
        [Range(1, 360)]
        [SerializeField] private float detectionAngle = 60f;
        [SerializeField] private float detectionDistance = 10f;
        
        [Header("Layer Setup")]
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask obstacleLayer;
        
        [SerializeField] private Transform rayOrigin;

        [Header("Meter Settings")]
        [SerializeField] private float detectionBuildSpeed = 100f;
        [SerializeField] private float detectionDecaySpeed = 50f;

        [Header("Debug & UI Setup")]
        [SerializeField] private string debugDetectionLevel;
        [SerializeField] private GameObject uiContainer;
        [SerializeField] private Image uiFillBar;

        public float DetectionLevel => _currentDetectionLevel;
        
        private float _currentDetectionLevel;
        private float _sqrDetectionDistance;
        private float _cosHalfAngle;
        private LayerMask _combinedDetectionLayer;

        private void Awake()
        {
            _sqrDetectionDistance = detectionDistance * detectionDistance;
            _cosHalfAngle = Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad);
            _combinedDetectionLayer = playerLayer | obstacleLayer;
            
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
            debugDetectionLevel = $"Detection level: {_currentDetectionLevel}";
            
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
            Vector3 rayStart = rayOrigin != null ? rayOrigin.position : enemy.position;

            Vector3 playerCenterTarget = player.position;
            Vector3 playerHeadTarget = player.position;

            if (player.TryGetComponent<CharacterController>(out var playerController))
            {
                playerCenterTarget = player.TransformPoint(playerController.center);
                float internalHeadOffset = (playerController.height * 0.5f) - (playerController.radius * 0.3f);
                playerHeadTarget = playerCenterTarget + (Vector3.up * internalHeadOffset);
            }
            else
            {
                playerCenterTarget = player.position;
                playerHeadTarget = player.position + (Vector3.up * 1.5f);
            }

            Vector3 directionToCenter = playerCenterTarget - rayStart;
            float sqrDistance = directionToCenter.sqrMagnitude;

            if (sqrDistance > _sqrDetectionDistance)
                return false;

            currentDistance = Mathf.Sqrt(sqrDistance);
            directionToCenter.Normalize();
            
            float dotProduct = Vector3.Dot(enemy.forward, directionToCenter);
            if (dotProduct < _cosHalfAngle) return false;

            Vector3 directionToHead = (playerHeadTarget - rayStart).normalized;
            float distanceToHead = Vector3.Distance(rayStart, playerHeadTarget);

            Debug.DrawRay(rayStart, directionToCenter * currentDistance, Color.yellow);
            Debug.DrawRay(rayStart, directionToHead * distanceToHead, Color.cyan);

            bool isCenterVisible = false;
            bool isHeadVisible = false;

            if (Physics.Raycast(rayStart, directionToCenter, out RaycastHit centerHit, currentDistance, _combinedDetectionLayer))
            {
                if (((1 << centerHit.collider.gameObject.layer) & playerLayer) != 0)
                {
                    isCenterVisible = true;
                }
            }

            if (Physics.Raycast(rayStart, directionToHead, out RaycastHit headHit, distanceToHead, _combinedDetectionLayer))
            {
                if (((1 << headHit.collider.gameObject.layer) & playerLayer) != 0)
                {
                    isHeadVisible = true;
                }
            }

            return isCenterVisible || isHeadVisible;
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