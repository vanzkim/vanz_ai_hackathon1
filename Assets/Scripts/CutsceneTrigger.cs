using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace VanzAI.Triggers
{
    /// <summary>
    /// 플레이어가 트리거 콜라이더에 들어왔을 때 1회 재생되는 트리거.
    /// 모드에 따라 Timeline 재생, 씬 동작(조명/오브젝트 활성화), 또는 둘 다 수행한다.
    /// 콜라이더(IsTrigger=true)는 수동으로 세팅되어 있다고 가정한다.
    /// </summary>
    [AddComponentMenu("Vanz/Cutscene Trigger")]
    [DisallowMultipleComponent]
    public class CutsceneTrigger : MonoBehaviour
    {
        public enum TriggerMode
        {
            Timeline,
            SceneAction,
            Both
        }

        [Header("Mode")]
        [SerializeField] private TriggerMode mode = TriggerMode.Timeline;

        [Tooltip("체크 시 1회만 발동하고 이후 무시됨. 해제 시 플레이어가 나갔다 다시 들어올 때마다 재발동.")]
        [SerializeField] private bool oneShot = true;

        [Header("Player Detection")]
        [Tooltip("빈 값이 아니면 루트 혹은 충돌 오브젝트의 이름이 이 값과 일치해야 함. 기본: Player_Model")]
        [SerializeField] private string playerName = "Player_Model";

        [Tooltip("빈 값이 아니면 루트 혹은 충돌 오브젝트의 태그가 이 값과 일치해야 함. 둘 다 지정되면 이름/태그 모두 만족해야 함.")]
        [SerializeField] private string playerTag = "";

        [Header("Timeline (Mode: Timeline / Both)")]
        [SerializeField] private PlayableDirector director;

        [Tooltip("컷씬 재생 동안 .enabled = false 처리할 컴포넌트 목록. (플레이어 컨트롤러/입력 스크립트 등)")]
        [SerializeField] private MonoBehaviour[] disableDuringCutscene;

        [Header("Scene Action (Mode: SceneAction / Both)")]
        [Tooltip("트리거 발동 시 SetActive(true) 처리할 GameObject 목록")]
        [SerializeField] private GameObject[] activate;

        [Tooltip("트리거 발동 시 SetActive(false) 처리할 GameObject 목록")]
        [SerializeField] private GameObject[] deactivate;

        [Tooltip("트리거 발동 시 Light.enabled = true 처리할 대상")]
        [SerializeField] private Light[] lightsOn;

        [Tooltip("트리거 발동 시 Light.enabled = false 처리할 대상")]
        [SerializeField] private Light[] lightsOff;

        [Header("Events (추후 확장용 - 워프/체인 컷씬 등 이곳에 연결)")]
        [SerializeField] private UnityEvent onTriggered;
        [SerializeField] private UnityEvent onFinished;

        private bool hasPlayed;
        private bool isRunning;

        private void OnTriggerEnter(Collider other)
        {
            if (isRunning) return;
            if (oneShot && hasPlayed) return;
            if (!IsPlayer(other)) return;

            hasPlayed = true;
            isRunning = true;

            onTriggered?.Invoke();

            bool wantSceneAction = mode == TriggerMode.SceneAction || mode == TriggerMode.Both;
            bool wantTimeline = mode == TriggerMode.Timeline || mode == TriggerMode.Both;

            if (wantSceneAction)
            {
                ApplySceneAction();
            }

            if (wantTimeline)
            {
                PlayTimeline();
            }
            else
            {
                Finish();
            }
        }

        private bool IsPlayer(Collider other)
        {
            // 자식 콜라이더까지 고려해서 루트 기준과 충돌 오브젝트 자기 자신 둘 다 확인
            var root = other.transform.root;

            bool nameMatch = string.IsNullOrEmpty(playerName)
                             || root.name == playerName
                             || other.name == playerName;

            bool tagMatch = string.IsNullOrEmpty(playerTag)
                            || other.CompareTag(playerTag)
                            || root.CompareTag(playerTag);

            return nameMatch && tagMatch;
        }

        private void ApplySceneAction()
        {
            if (activate != null)
            {
                foreach (var go in activate)
                {
                    if (go != null) go.SetActive(true);
                }
            }

            if (deactivate != null)
            {
                foreach (var go in deactivate)
                {
                    if (go != null) go.SetActive(false);
                }
            }

            if (lightsOn != null)
            {
                foreach (var l in lightsOn)
                {
                    if (l != null) l.enabled = true;
                }
            }

            if (lightsOff != null)
            {
                foreach (var l in lightsOff)
                {
                    if (l != null) l.enabled = false;
                }
            }
        }

        private void PlayTimeline()
        {
            if (director == null)
            {
                Debug.LogWarning($"[CutsceneTrigger] '{name}': Timeline 모드지만 PlayableDirector가 할당되지 않아 스킵함.", this);
                Finish();
                return;
            }

            SetPlayerComponentsEnabled(false);
            director.stopped += OnDirectorStopped;
            director.Play();
        }

        private void OnDirectorStopped(PlayableDirector pd)
        {
            if (pd != director) return;

            director.stopped -= OnDirectorStopped;
            SetPlayerComponentsEnabled(true);
            Finish();
        }

        private void SetPlayerComponentsEnabled(bool enabled)
        {
            if (disableDuringCutscene == null) return;
            foreach (var c in disableDuringCutscene)
            {
                if (c != null) c.enabled = enabled;
            }
        }

        private void Finish()
        {
            isRunning = false;
            onFinished?.Invoke();
        }

        private void OnDisable()
        {
            // 씬 전환/오브젝트 비활성화 등으로 콜백 누수 방지
            if (director != null)
            {
                director.stopped -= OnDirectorStopped;
            }
        }
    }
}
