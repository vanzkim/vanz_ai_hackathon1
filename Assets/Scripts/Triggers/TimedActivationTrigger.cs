using UnityEngine;
using System.Collections;
using VanzAI.Managers;
using VanzAI; // Added for VanzConstants

namespace VanzAI.Triggers
{
    /// <summary>
    /// 플레이어가 트리거에 닿으면 대상 오브젝트를 일정 시간 동안 활성화하고 효과음을 재생합니다.
    /// </summary>
    public class TimedActivationTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject targetObject;
        [SerializeField] private float duration = 2.5f;
        [SerializeField] private bool oneShot = true;

        [Header("Audio")]
        [SerializeField] private AudioClip impactSfx;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] [Range(0f, 1f)] private float volume = 1.0f;

        private bool _hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_hasTriggered && oneShot) return;

            // VanzConstants.PlayerTag ("Player")를 사용하여 플레이어 확인
            if (other.CompareTag(VanzConstants.PlayerTag) || other.name.Contains("Player"))
            {
                if (oneShot)
                {
                    _hasTriggered = true;
                    // 트리거가 다시 발동되지 않도록 콜라이더를 즉시 비활성화합니다.
                    var col = GetComponent<Collider>();
                    if (col != null) col.enabled = false;
                }
                StartCoroutine(TriggerRoutine());
            }
        }

        private IEnumerator TriggerRoutine()
        {
            // 오브젝트 활성화
if (targetObject != null)
            {
                targetObject.SetActive(true);
            }

            // 효과음 재생
            if (impactSfx != null)
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(impactSfx, volume);
                }
                else if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(impactSfx, transform.position, volume);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(impactSfx, transform.position, volume);
                }
            }

            // 대기
            yield return new WaitForSeconds(duration);

            // 오브젝트 다시 비활성화
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }
        }
    }
}
