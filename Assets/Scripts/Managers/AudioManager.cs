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
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (_musicSource.clip == clip && _musicSource.isPlaying) return;

            StopAllCoroutines();
            StartCoroutine(FadeMusicCoroutine(clip, loop));
        }

        public void StopMusic()
        {
            StopAllCoroutines();
            StartCoroutine(FadeMusicCoroutine(null, false));
        }

        public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
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
