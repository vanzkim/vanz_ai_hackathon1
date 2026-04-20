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
        [Tooltip("컷씬 동안 조작용 플레이어 대신 사용할 전용 모델 (있을 경우만)")]
        [SerializeField] private GameObject cutscenePlayer;

        [Header("Player Detection")]
        [Tooltip("빈 값이 아니면 루트 혹은 충돌 오브젝트의 이름이 이 값과 일치해야 함. 기본: Player_Model")]
        [SerializeField] private string playerName = "Player_Model";

        [Tooltip("빈 값이 아니면 루트 혹은 충돌 오브젝트의 태그가 이 값과 일치해야 함.")]
        [SerializeField] private string playerTag = "Player";

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

            director.stopped += OnDirectorStopped;
            director.Play();
        }

        private void OnDirectorStopped(PlayableDirector pd)
        {
            if (pd != director) return;
            director.stopped -= OnDirectorStopped;

            if (cutscenePlayer != null)
            {
                CutsceneManager.Instance.EndCutscene();
            }
            else
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
            if (director != null) director.stopped -= OnDirectorStopped;
        }
    }
}
