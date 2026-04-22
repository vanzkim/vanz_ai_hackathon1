using UnityEngine;
using UnityEngine.Playables;
using VanzAI.Managers;

namespace VanzAI.Audio
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CutsceneMusicTrigger : MonoBehaviour
    {
        [SerializeField] private AudioClip musicClip;
        private PlayableDirector _director;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
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
            AudioManager.Instance.StopMusic();
        }
    }
}
