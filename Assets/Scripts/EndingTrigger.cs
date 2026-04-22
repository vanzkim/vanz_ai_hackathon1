using UnityEngine;

namespace VanzAI.Triggers
{
    /// <summary>
    /// 플레이어가 닿았을 때 엔딩 씬으로 전환하는 트리거.
    /// 열쇠를 가지고 있는지 확인합니다.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class EndingTrigger : MonoBehaviour
    {
        [Header("Audio Settings")]
        public AudioClip lockedSound;
        private AudioSource _audioSource;

        private bool _isTransitioning = false;
        private float _lastLockedSoundTime = 0f;
        private const float LockedSoundCooldown = 2.0f;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTransitioning) return;
            if (!IsPlayer(other)) return;

            // 키 보유 여부 확인
            if (PlayerInventory.Instance != null && !PlayerInventory.Instance.hasKey)
            {
                PlayLockedSound();
                return;
            }

            _isTransitioning = true;
            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.LoadScene(GameSceneManager.SceneType.Ending);
                Debug.Log("[EndingTrigger] Player entered with key! Loading EndingScene.");
            }
            else
            {
                Debug.LogError("[EndingTrigger] GameSceneManager instance not found!");
            }
        }

        private void PlayLockedSound()
        {
            if (Time.time - _lastLockedSoundTime < LockedSoundCooldown) return;

            if (_audioSource != null && lockedSound != null)
            {
                _audioSource.PlayOneShot(lockedSound);
                _lastLockedSoundTime = Time.time;
                Debug.Log("[EndingTrigger] Door is locked. Playing sound.");
            }
            else
            {
                Debug.LogWarning("[EndingTrigger] Locked sound or AudioSource is missing.");
            }
        }

        /// <summary>
        /// Player 태그이거나, 이름이 Player_Model / Player_Cutscene 인 경우 플레이어로 인정.
        /// </summary>
        private static bool IsPlayer(Collider other)
        {
            if (other.CompareTag(VanzConstants.PlayerTag)) return true;

            string rootName = other.transform.root.name;
            if (other.name == VanzConstants.PlayerCutsceneName ||
                rootName == VanzConstants.PlayerCutsceneName) return true;

            if (other.name == VanzConstants.PlayerModelName ||
                rootName == VanzConstants.PlayerModelName) return true;

            return false;
        }
    }
}

