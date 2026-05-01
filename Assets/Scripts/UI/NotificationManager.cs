using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VanzAI.UI
{
    /// <summary>
    /// 화면 하단에 알림 메시지를 출력하는 싱글톤 매니저.
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        private static NotificationManager _instance;
        public static NotificationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindAnyObjectByType<NotificationManager>();
                }
                return _instance;
            }
        }

        [Header("UI References")]
        public GameObject notificationPanel;
        public Text notificationText;

        [Header("Settings")]
        public float defaultDuration = 3f;

        private Coroutine _hideCoroutine;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            
            // 초기 상태: 숨김
            if (notificationPanel != null)
                notificationPanel.SetActive(false);
        }

        /// <summary>
        /// 메시지를 화면에 출력합니다.
        /// </summary>
        /// <param name="message">출력할 문자열</param>
        public void ShowNotification(string message)
        {
            ShowNotification(message, defaultDuration);
        }

        /// <summary>
        /// 메시지를 특정 시간 동안 출력합니다.
        /// </summary>
        /// <param name="message">출력할 문자열</param>
        /// <param name="duration">노출 시간(초)</param>
        public void ShowNotification(string message, float duration)
        {
            if (notificationPanel == null || notificationText == null)
            {
                Debug.LogWarning("[NotificationManager] UI references are missing!");
                return;
            }

            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);

            notificationText.text = message;
            notificationPanel.SetActive(true);

            _hideCoroutine = StartCoroutine(HideAfterDelay(duration));
        }

        private IEnumerator HideAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            notificationPanel.SetActive(false);
            _hideCoroutine = null;
        }
    }
}
