using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;} 
        
        [Header("UI Reference")]
        [Tooltip("The Game Over UI panel or screen GameObject that should appear upon detection.")]
        [SerializeField] private GameObject gameOverCanvas;

        [Header("Gameplay Settings")]
        [Tooltip("How many seconds to wait before reloading the scene.")]
        [SerializeField] private float delayBeforeRestart = 3f;

        private bool _isSequenceTriggered = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                if (gameOverCanvas != null)
                {
                    gameOverCanvas.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Call this method when the player is completely detected to trigger the game over sequence.
        /// </summary>
        public void TriggerDetectionGameOver()
        {
            if (_isSequenceTriggered) return;
            _isSequenceTriggered = true;

            StartCoroutine(GameOverSequenceRoutine());
        }

        private IEnumerator GameOverSequenceRoutine()
        {
            Time.timeScale = 0f;
            
            if (gameOverCanvas != null)
            {
                gameOverCanvas.SetActive(true);
            }
            
            yield return new WaitForSecondsRealtime(delayBeforeRestart);
            
            Time.timeScale = 1f;
            
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}
