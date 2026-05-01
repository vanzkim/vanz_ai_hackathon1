using UnityEngine;
using VanzAI.Managers;

namespace VanzAI.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerBreathing : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] breathingClips;
        
        [Header("Breathing Intervals")]
        [SerializeField] private float minInterval = 3.0f;
        [SerializeField] private float maxInterval = 8.0f;
        [SerializeField] private float speedThreshold = 0.1f;

        private CharacterController _controller;
        private float _breathingTimer;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            ResetTimer();
        }

        private void Update()
        {
            if (_controller == null) return;

            // Check if player is moving
            if (new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude > speedThreshold)
            {
                _breathingTimer -= Time.deltaTime;
                if (_breathingTimer <= 0)
                {
                    PlayBreathing();
                    ResetTimer();
                }
            }
        }

        private void PlayBreathing()
        {
            if (breathingClips == null || breathingClips.Length == 0) return;

            AudioClip clip = breathingClips[Random.Range(0, breathingClips.Length)];
            Debug.Log($"[PlayerBreathing] Playing breath: {clip.name}");
            AudioManager.Instance.PlaySFX(clip, transform.position);
        }

        private void ResetTimer()
        {
            _breathingTimer = Random.Range(minInterval, maxInterval);
        }
    }
}
