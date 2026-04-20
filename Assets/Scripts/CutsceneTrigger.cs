using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using VanzAI.Managers;

namespace VanzAI.Triggers
{
    /// <summary>
    /// 플레이어가 트리거 콜라이더에 들어왔을 때 1회 재생되는 트리거.
    /// CutsceneManager와 연동하여 조작용/컷씬용 모델 스왑을 처리합니다.
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

        [Tooltip("체크 시 1회만 발동하고 이후 무시됨.")]
        [SerializeField] private bool oneShot = true;

        [Header("Player Swapping")]
        [Tooltip("컷씬 동안 조작용 플레이어 대신 사용할 전용 모델 (있을 경우만). 이 값이 세팅되면 disableDuringCutscene는 무시됨.")]
        [SerializeField] private GameObject cutscenePlayer;

        [Header("Player Detection")]
        [Tooltip("빈 값이 아니면 루트 혹은 충돌 오브젝트의 이름이 이 값과 일치해야 함. 기본: Player_Model")]
        [SerializeField] private string playerName = VanzConstants.PlayerModelName;

        [Tooltip("빈 값이 아니면 루트 혹은 충돌 오브젝트의 태그가 이 값과 일치해야 함.")]
        [SerializeField] private string playerTag = VanzConstants.PlayerTag;

        [Header("Timeline (Mode: Timeline / Both)")]
        [SerializeField] private PlayableDirector director;

        [Tooltip("컷씬 재생 동안 비활성화할 컴포넌트들 (모델 교체를 안 할 때 유용)")]
        [SerializeField] private MonoBehaviour[] disableDuringCutscene;

        [Header("Scene Action (Mode: SceneAction / Both)")]
        [SerializeField] private GameObject[] activate;
        [SerializeField] private GameObject[] deactivate;
        [SerializeField] private Light[] lightsOn;
        [SerializeField] private Light[] lightsOff;

        [Header("Events")]
        [SerializeField] private UnityEvent onTriggered;
        [SerializeField] private UnityEvent onFinished;

        private bool hasPlayed;
        private bool isRunning;
        private bool subscribed; // director.stopped 구독 상태 추적

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

            if (wantSceneAction) ApplySceneAction();

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
            var root = other.transform.root;
            bool nameMatch = string.IsNullOrEmpty(playerName) || root.name == playerName || other.name == playerName;
            bool tagMatch = string.IsNullOrEmpty(playerTag) || other.CompareTag(playerTag) || root.CompareTag(playerTag);
            return nameMatch && tagMatch;
        }

        private void ApplySceneAction()
        {
            if (activate != null) foreach (var go in activate) if (go != null) go.SetActive(true);
            if (deactivate != null) foreach (var go in deactivate) if (go != null) go.SetActive(false);
            if (lightsOn != null) foreach (var l in lightsOn) if (l != null) l.enabled = true;
            if (lightsOff != null) foreach (var l in lightsOff) if (l != null) l.enabled = false;
        }

        private void PlayTimeline()
        {
            if (director == null)
            {
                Debug.LogWarning($"[CutsceneTrigger] '{name}': Timeline 모드지만 PlayableDirector가 할당되지 않아 스킵함.", this);
                Finish();
                return;
            }

            if (cutscenePlayer != null)
            {
                CutsceneManager.Instance.StartCutscene(cutscenePlayer);
            }
            else
            {
                SetPlayerComponentsEnabled(false);
            }

            Subscribe();
            director.Play();
        }

        private void Subscribe()
        {
            if (subscribed || director == null) return;
            director.stopped += OnDirectorStopped;
            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!subscribed || director == null) return;
            director.stopped -= OnDirectorStopped;
            subscribed = false;
        }

        private void OnDirectorStopped(PlayableDirector pd)
        {
            if (pd != director) return;
            Unsubscribe();
            CleanupAndFinish();
        }

        /// <summary>
        /// 컷씬 종료 공통 처리. 정상 종료/중단 모두에서 호출 가능.
        /// </summary>
        private void CleanupAndFinish()
        {
            if (!isRunning) return; // 이미 정리됨

            // CutsceneManager는 내부에서 null-safe하게 동작한다.
            // cutscenePlayer가 할당되어 있다면 해당 모델의 위치를 플레이어에게 복사한다.
            CutsceneManager.Instance.EndCutscene(cutscenePlayer);

            if (cutscenePlayer == null)
            {
                SetPlayerComponentsEnabled(true);
            }

            Finish();
        }

        private void SetPlayerComponentsEnabled(bool enabled)
        {
            if (disableDuringCutscene == null) return;
            foreach (var c in disableDuringCutscene) if (c != null) c.enabled = enabled;
        }

        private void Finish()
        {
            isRunning = false;
            onFinished?.Invoke();
        }

        private void OnDisable()
        {
            // 구독 해제. 실행 중이었다면 상태 복원까지 처리한다.
            Unsubscribe();
            if (isRunning)
            {
                CleanupAndFinish();
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();
            // OnDestroy 단계에선 플레이어 상태 복원이 오히려 위험(파괴된 참조 가능)
            // Finish만 호출하여 isRunning 플래그 정리.
            if (isRunning) Finish();
        }
    }
}
