using UnityEngine;
using System.Collections;
using VanzAI.Managers;

namespace VanzAI.Triggers
{
    public class ElevatorTrigger : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform targetLocation;
        
        [Header("Settings")]
        [SerializeField] private float travelDuration = 2.0f;
        [SerializeField] private bool oneShot = false;

        [Header("Audio")]
        [SerializeField] private AudioClip elevatorSound;
        [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;

        private bool _hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_hasTriggered && oneShot) return;
            
            // root를 확인하여 Player_Model인지 검사
            if (other.CompareTag("Player") || other.transform.root.name == "Player_Model")
            {
                _hasTriggered = true;
                StartCoroutine(ElevatorSequence(other.transform.root.gameObject));
            }
        }

        private IEnumerator ElevatorSequence(GameObject player)
        {
            // 0. Play Elevator Sound
            if (elevatorSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXGlobal(elevatorSound, soundVolume);
            }

            // 1. Fade Out
            if (GameSceneManager.Instance != null)
            {
                yield return GameSceneManager.Instance.FadeOut();
            }

            // 2. Teleport
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            if (targetLocation != null)
            {
                player.transform.position = targetLocation.position;
                player.transform.rotation = targetLocation.rotation;
            }
            else
            {
                Debug.LogWarning("[ElevatorTrigger] targetLocation is null!");
            }

            // 3. Wait for 'travel' time (darkness)
            yield return new WaitForSeconds(travelDuration);

            // Sync physics after teleport
            Physics.SyncTransforms();

            if (cc != null) cc.enabled = true;

            // 4. Fade In
            if (GameSceneManager.Instance != null)
            {
                yield return GameSceneManager.Instance.FadeIn();
            }
        }
    }
}
