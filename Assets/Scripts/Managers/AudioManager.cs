using UnityEngine;
using System.Collections;

namespace VanzAI.Managers
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindAnyObjectByType<AudioManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        _instance = go.AddComponent<AudioManager>();
                    }
                }
                return _instance;
            }
        }

        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 1.0f;

        [Header("Whisper System")]
        [SerializeField] private AudioClip[] whisperClips;
        [SerializeField] private float whisperMinInterval = 5f;
        [SerializeField] private float whisperMaxInterval = 15f;
        [SerializeField] private float whisperVolume = 0.5f;
        [SerializeField] private bool whispersEnabled = false;

        private Coroutine _whisperCoroutine;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            if (whispersEnabled)
            {
                StartWhispers();
            }
        }

        public void StartWhispers()
        {
            whispersEnabled = true;
            if (_whisperCoroutine != null) StopCoroutine(_whisperCoroutine);
            _whisperCoroutine = StartCoroutine(WhisperRoutine());
            Debug.Log("[AudioManager] Whisper System started.");
        }

        public void StopWhispers()
        {
            whispersEnabled = false;
            if (_whisperCoroutine != null)
            {
                StopCoroutine(_whisperCoroutine);
                _whisperCoroutine = null;
            }
            Debug.Log("[AudioManager] Whisper System stopped.");
        }

        private IEnumerator WhisperRoutine()
        {
            while (whispersEnabled)
            {
                float delay = Random.Range(whisperMinInterval, whisperMaxInterval);
                yield return new WaitForSeconds(delay);

                // Check conditions before playing:
                // 1. Not in a cutscene
                // 2. We have clips to play
                bool isInCutscene = CutsceneManager.Instance != null && CutsceneManager.Instance.IsCutsceneActive;

                if (!isInCutscene && whisperClips != null && whisperClips.Length > 0)
                {
                    AudioClip clip = whisperClips[Random.Range(0, whisperClips.Length)];
                    PlaySFXGlobal(clip, whisperVolume);
                    Debug.Log($"[AudioManager] Playing whisper: {clip.name}");
                }
            }
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (_musicSource.clip == clip && _musicSource.isPlaying) return;

            Debug.Log($"[AudioManager] Playing music: {(clip != null ? clip.name : "null")}");
            StopAllCoroutines();
            StartCoroutine(FadeMusicCoroutine(clip, loop));
        }

        public void StopMusic()
        {
            Debug.Log("[AudioManager] Stopping music.");
            StopAllCoroutines();
            StartCoroutine(FadeMusicCoroutine(null, false));
        }

        public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
            Debug.Log($"[AudioManager] Playing SFX: {clip.name} at {position}");
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        public void PlaySFXGlobal(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, volume);
        }

        private IEnumerator FadeMusicCoroutine(AudioClip newClip, bool loop)
        {
            if (_musicSource.isPlaying && _musicSource.clip != null)
            {
                float startVolume = _musicSource.volume;
                for (float t = 0; t < fadeDuration; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                    yield return null;
                }
                _musicSource.Stop();
            }

            if (newClip != null)
            {
                _musicSource.clip = newClip;
                _musicSource.loop = loop;
                _musicSource.Play();

                float targetVolume = 1.0f; // Default target volume
                for (float t = 0; t < fadeDuration; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
                    yield return null;
                }
                _musicSource.volume = targetVolume;
            }
        }
    }
}
