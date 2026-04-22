using UnityEngine;
using UnityEngine.Playables;
using VanzAI.Managers;

namespace VanzAI.Audio
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CutsceneMusicTrigger : MonoBehaviour
    {
        [SerializeField] private AudioClip musicClip;
        
        [Tooltip("체크 시 해당 컷씬이 종료되면 몬스터 추격 기능이 활성화됩니다.")]
        [SerializeField] private bool isMeetWhiteCutscene;

        private PlayableDirector _director;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            
            // 하드코딩된 예외 처리: 이름이 Meet_white인 경우 자동으로 플래그 설정
            if (gameObject.name == "Meet_white")
            {
                isMeetWhiteCutscene = true;
            }
        }

        private void OnEnable()
        {
            if (_director != null)
            {
                _director.played += OnPlayableDirectorPlayed;
                _director.stopped += OnPlayableDirectorStopped;
            }
        }

        private void OnDisable()
        {
            if (_director != null)
            {
                _director.played -= OnPlayableDirectorPlayed;
                _director.stopped -= OnPlayableDirectorStopped;
            }
        }

        private void OnPlayableDirectorPlayed(PlayableDirector director)
        {
            CutsceneManager.Instance.RegisterCutscene(true);
            if (musicClip != null)
            {
                AudioManager.Instance.PlayMusic(musicClip);
            }
        }

        private void OnPlayableDirectorStopped(PlayableDirector director)
        {
            CutsceneManager.Instance.RegisterCutscene(false);
            
            // Meet_white 컷씬인 경우 전역 추격 플래그 활성화
            if (isMeetWhiteCutscene)
            {
                CutsceneManager.Instance.CanMonstersChase = true;
                Debug.Log("[CutsceneMusicTrigger] Meet_white finished. Monsters are now allowed to chase.");
            }

            AudioManager.Instance.StopMusic();
        }
    }
}
