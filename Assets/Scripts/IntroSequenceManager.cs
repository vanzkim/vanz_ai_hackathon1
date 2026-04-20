using UnityEngine;
using UnityEngine.Playables;
using VanzAI.Managers;

namespace VanzAI.Sequences
{
    /// <summary>
    /// 인트로 전용 처리를 위해 CutsceneManager를 활용하는 브릿지 스크립트.
    /// </summary>
    public class IntroSequenceManager : MonoBehaviour
    {
        [Header("Timeline")]
        [SerializeField] private PlayableDirector introDirector;

        [Header("Players")]
        [SerializeField] private GameObject cutscenePlayer;

        private void Awake()
        {
            if (introDirector == null)
                introDirector = GetComponent<PlayableDirector>();
            
            // 시작하자마자 컷씬 모드로 진입 (Gameplay Player 숨김)
            if (introDirector != null && introDirector.playOnAwake)
            {
                CutsceneManager.Instance.StartCutscene(cutscenePlayer);
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
            // 범용 시스템을 통해 조작권 반환 및 위치 동기화 처리
            CutsceneManager.Instance.EndCutscene();
        }
    }
}
