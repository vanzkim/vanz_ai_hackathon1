using UnityEngine;
using UnityEngine.Playables;
using VanzAI.Sequences;

namespace VanzAI.Sequences
{
    /// <summary>
    /// Scene Hospital의 시작 시퀀스를 관리하는 컨트롤러.
    /// Death Cutscene -> Intro Timeline -> Player Activation 순서로 실행을 제어한다.
    /// </summary>
    public class HospitalStartController : MonoBehaviour
    {
        [Header("Sequence Directors")]
        [SerializeField] private PlayableDirector deathDirector;
        [SerializeField] private IntroSequenceManager introManager;

        private void Start()
        {
            if (deathDirector != null)
            {
                // Death Cutscene 종료 이벤트를 구독
                deathDirector.stopped += OnDeathCutsceneFinished;
                
                // Death Cutscene 재생 시작
                deathDirector.Play();
                Debug.Log("[HospitalStartController] Starting Death Cutscene.");
            }
            else
            {
                Debug.LogWarning("[HospitalStartController] Death Director is missing. Skipping to Intro.");
                StartIntro();
            }
        }

        private void OnDeathCutsceneFinished(PlayableDirector director)
        {
            if (director == deathDirector)
            {
                deathDirector.stopped -= OnDeathCutsceneFinished;
                Debug.Log("[HospitalStartController] Death Cutscene finished. Starting Intro.");
                StartIntro();
            }
        }

        private void StartIntro()
        {
            if (introManager != null)
            {
                introManager.PlayIntro();
            }
            else
            {
                Debug.LogError("[HospitalStartController] Intro Manager is missing.");
            }
        }
    }
}
