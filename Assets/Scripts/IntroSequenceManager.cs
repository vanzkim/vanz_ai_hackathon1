using UnityEngine;
using UnityEngine.Playables;
using VanzAI.Managers;

namespace VanzAI.Sequences
{
    /// <summary>
    /// 인트로 전용 처리를 위해 CutsceneManager를 활용하는 브릿지 스크립트.
    /// PlayableDirector의 playOnAwake가 스크립트보다 먼저 실행되는 것을 막기 위해
    /// DefaultExecutionOrder로 우선 순위를 올리고, 재생을 코드로 직접 시작한다.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class IntroSequenceManager : MonoBehaviour
    {
        [Header("Timeline")]
        [SerializeField] private PlayableDirector introDirector;

        [Header("Players")]
        [Tooltip("인트로 컷씬 동안 노출할 전용 모델. 없으면 비워둠.")]
        [SerializeField] private GameObject cutscenePlayer;

        [Tooltip("체크 시 Awake에서 자동으로 StartCutscene + Play 수행. " +
                 "해제 시 외부에서 PlayIntro()를 명시적으로 호출해야 한다.")]
        [SerializeField] private bool autoPlayOnAwake = true;

        private bool _intended;

        private void Awake()
        {
            if (introDirector == null)
                introDirector = GetComponent<PlayableDirector>();

            if (introDirector != null)
            {
                // playOnAwake가 true면 스크립트 실행 순서에 따라 Timeline이 먼저 한 프레임 돌 수 있다.
                // 이 타이밍 경합을 차단하기 위해 강제로 false로 설정한 뒤 명시적으로 Play()를 호출한다.
                introDirector.playOnAwake = false;
            }

            if (autoPlayOnAwake)
            {
                PlayIntro();
            }
        }

        /// <summary>
        /// 외부에서 호출 가능한 인트로 재생 진입점.
        /// </summary>
        public void PlayIntro()
        {
            if (_intended) return;
            _intended = true;

            // 모델 스왑을 먼저 적용해 첫 프레임 번쩍임을 방지.
            CutsceneManager.Instance.StartCutscene(cutscenePlayer);

            if (introDirector != null)
            {
                introDirector.Play();
            }
            else
            {
                Debug.LogWarning("[IntroSequenceManager] introDirector가 없어 재생을 건너뜀.");
            }
        }

        private void OnEnable()
        {
            if (introDirector != null)
                introDirector.stopped += OnIntroFinished;
        }

        private void OnDisable()
        {
            if (introDirector != null)
                introDirector.stopped -= OnIntroFinished;
        }

        private void OnIntroFinished(PlayableDirector director)
        {
            if (director != introDirector) return;
            // 범용 시스템을 통해 조작권 반환 및 위치 동기화 처리
            CutsceneManager.Instance.EndCutscene();
        }
    }
}
