using UnityEngine;
using VanzAI.UI;

namespace VanzAI.Triggers
{
    /// <summary>
    /// 플레이어가 열쇠 근처에 왔을 때 열쇠를 획득하고 소리를 재생하는 스크립트.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class KeyPickup : MonoBehaviour
    {
        [Header("Audio Settings")]
        public AudioClip pickupSound;
        private AudioSource _audioSource;

        private bool _isPickedUp = false;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isPickedUp) return;
            if (!IsPlayer(other)) return;

            CollectKey();
        }

        private void CollectKey()
        {
            _isPickedUp = true;
            
            // 인벤토리에 키 추가
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddKey();
            }

            // UI 알림 출력
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("Key obtained");
            }

            // 소리 재생
            if (_audioSource != null && pickupSound != null)
            {
                _audioSource.PlayOneShot(pickupSound);
                Debug.Log("[KeyPickup] Playing pickup sound.");
            }

            // 시각적 및 물리적 피드백: 메쉬 및 콜라이더 비활성화
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = false;

            MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var r in childRenderers) r.enabled = false;

            Debug.Log("[KeyPickup] Key collected and hidden.");
            
            // 일정 시간 후 오브젝트 파괴 (소리 재생 후)
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 1f);
        }

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
